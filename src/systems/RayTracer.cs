
using System.Runtime.InteropServices;
using FrogLib;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class RayTracer : GameSystem {

    public Vector3 CameraRotation { get; private set; }
    public Vector3 CameraPosition { get; private set; }
    public float CameraFocalLength { get; private set; }
    public Stats Stats { get; private set; }
    public long TransformTime { get; private set; }
    public long RayTraceTime { get; private set; }
    public long OutlineTime { get; private set; }
    public TimeSpan BufferTime { get; private set; }

    private float sunAngle;


    private VertexArray vertexArray;


    private UniformBuffer cameraBuffer;
    private UniformBuffer sunBuffer;
    private StorageBuffer sphereBuffer;
    private StorageBuffer modelBuffer;
    private StorageBuffer worldPositionsBuffer;
    private StorageBuffer worldNormalsBuffer;
    private StorageBuffer worldVerticesBuffer;
    private StorageBuffer worldTrianglesBuffer;
    private StorageBuffer boundingBoxBuffer;
    private StorageBuffer stats;



    private List<TraceableSphere> spheres;
    private Vector3 prevCameraPosition;
    private List<TraceableModel> models;


    private Vector2i outputSize;

    private ImGuiController imGuiController;

    private int textureID;


    public Query transformQuery = new Query(QueryTarget.TimeElapsed);
    public Query rayTraceQuery = new Query(QueryTarget.TimeElapsed);
    public Query outlineQuery = new Query(QueryTarget.TimeElapsed);

    private RayTracer() {

        spheres = new List<TraceableSphere>();
        models = new List<TraceableModel>();

        vertexArray = new VertexArray(
            new Layout(VertexAttribPointerType.Float, 2), // position
            new Layout(VertexAttribPointerType.Float, 2)  // uv
        );

        float[] vertices = {
            -1f, -1f, 0f, 0f,
            1f, -1f, 1f, 0f,
            -1f, 1f, 0f, 1f,
            1f, 1f, 1f, 1f,
        };

        int[] indices = {
            0, 1, 2,
            1, 2, 3
        };

        vertexArray.BufferVertexData(vertices, BufferUsageHint.StaticDraw);
        vertexArray.BufferIndices(indices, BufferUsageHint.StaticDraw);


        cameraBuffer = new UniformBuffer(BufferUsageHint.DynamicRead);
        sunBuffer = new UniformBuffer(BufferUsageHint.StreamRead);
        sphereBuffer = new StorageBuffer(1024, BufferUsageHint.StreamRead);
        modelBuffer = new StorageBuffer(1024 * 1024, BufferUsageHint.StreamRead);
        worldPositionsBuffer = new StorageBuffer(1024 * 1024, BufferUsageHint.StreamRead);
        worldNormalsBuffer = new StorageBuffer(1024 * 1024, BufferUsageHint.StreamRead);
        worldVerticesBuffer = new StorageBuffer(1024 * 1024, BufferUsageHint.StreamRead);
        worldTrianglesBuffer = new StorageBuffer(1024 * 1024, BufferUsageHint.StreamRead);
        boundingBoxBuffer = new StorageBuffer(1024 * 1024, BufferUsageHint.StreamRead);


        stats = new StorageBuffer(16, BufferUsageHint.StreamRead);





        imGuiController = new ImGuiController((int)Game.WindowSize.X, (int)Game.WindowSize.Y);

        // outputSize = new Vector2i(
        //     (int)MathF.Ceiling(Game.WindowSize.X / 2f / 8f),
        //     (int)MathF.Ceiling(Game.WindowSize.X / 2f / 4f)
        // );

        outputSize = new Vector2i((int)MathF.Ceiling(Game.WindowSize.X / 2f), (int)MathF.Ceiling(Game.WindowSize.Y / 2f));
    }

    public void SubmitSphere(TraceableSphere sphere) => spheres.Add(sphere);

    public void SubmitModel(TraceableModel model) => models.Add(model);

    public void RotateCamera(Vector3 rotation) {
        CameraRotation = new Vector3(
            MathF.Max(MathF.Min(CameraRotation.X + rotation.X, MathF.PI / 2f), -MathF.PI / 2f),
            Util.Normalize(CameraRotation.Y + rotation.Y),
            Util.Normalize(CameraRotation.Z + rotation.Z)
        );
    }

    public void MoveCamera(Vector3 movement) {
        var rotation = Matrix3.CreateRotationY(-CameraRotation.Y);
        prevCameraPosition = CameraPosition;
        CameraPosition += movement * rotation;
    }

    public void SetImGuiFrameLayout(Action layout) => imGuiController.SetLayout(layout);



    protected override void Startup() {
        var shaderLibrary = Game.Get<ShaderLibrary>();
        var textureLibrary = Game.Get<TextureLibrary>();
        var meshLibrary = Game.Get<MeshLibrary>();



        // shaders
        var screen = new Shader()
        .FromPath(ShaderType.VertexShader, "./res/shaders/screen.vert")
        .FromPath(ShaderType.FragmentShader, "./res/shaders/screen.frag");

        var fragModel = new Shader()
        .FromPath(ShaderType.VertexShader, "./res/shaders/screen.vert")
        .FromPath(ShaderType.FragmentShader, "./res/shaders/ray-trace-models.frag");

        var fragSphere = new Shader()
        .FromPath(ShaderType.VertexShader, "./res/shaders/screen.vert")
        .FromPath(ShaderType.FragmentShader, "./res/shaders/ray-trace-spheres.frag");

        var compModel = new Shader()
        .FromPath(ShaderType.ComputeShader, "./res/shaders/ray-trace-models.comp");

        var outlineCompute = new Shader()
        .FromPath(ShaderType.ComputeShader, "./res/shaders/outline.comp");

        var transformCompute = new Shader()
        .FromPath(ShaderType.ComputeShader, "./res/shaders/transform.comp");




        shaderLibrary.Add("screen", screen);
        shaderLibrary.Add("fragModels", fragModel);
        shaderLibrary.Add("fragSpheres", fragSphere);
        shaderLibrary.Add("compModels", compModel);
        shaderLibrary.Add("outline", outlineCompute);
        shaderLibrary.Add("transform", transformCompute);

        shaderLibrary.Add("test", new Shader()
        .FromPath(ShaderType.ComputeShader, "./res/shaders/test.comp"));



        // textures
        var colorOutput = new Texture(outputSize, SizedInternalFormat.Rgba8);
        colorOutput.SetParam(TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        colorOutput.SetParam(TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        var depthOutput = new Texture(outputSize, SizedInternalFormat.R32f);
        depthOutput.SetParam(TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        depthOutput.SetParam(TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        var objectOutput = new Texture(outputSize, SizedInternalFormat.R32i);
        objectOutput.SetParam(TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        objectOutput.SetParam(TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        textureLibrary.Add("colorOutput", colorOutput);
        textureLibrary.Add("depthOutput", depthOutput);
        textureLibrary.Add("objectOutput", objectOutput);


        // meshes
        meshLibrary.LoadPrimitive("cube", new PrimitiveCubeMesh());
        meshLibrary.LoadPrimitive("tri", new PrimitiveTriangleMesh());

    }

    protected override void Update() {

        imGuiController.WindowResized((int)Game.WindowSize.X, (int)Game.WindowSize.Y);


        imGuiController.NewFrame();

        if (Game.Input.IsKeyPressed(Keys.Tab)) {
            textureID = (textureID + 1) % 3;
        }

    }


    protected override void Render(float alpha) {

        GL.Clear(ClearBufferMask.ColorBufferBit);

        var shaderLibrary = Game.Get<ShaderLibrary>();
        var textureLibrary = Game.Get<TextureLibrary>();
        var meshLibrary = Game.Get<MeshLibrary>();

        var computeSize = new Vector2i(
            (int)MathF.Ceiling(outputSize.X / 8f),
            (int)MathF.Ceiling(outputSize.Y / 4f));

        cameraBuffer.Use(0);
        sunBuffer.Use(1);

        meshLibrary.SourcePositionsBuffer.Use(0);
        meshLibrary.SourceNormalsBuffer.Use(1);
        meshLibrary.SourceUVsBuffer.Use(2);
        meshLibrary.SourceVerticesBuffer.Use(3);
        meshLibrary.SourceTrianglesBuffer.Use(4);
        modelBuffer.Use(5);
        worldPositionsBuffer.Use(6);
        worldNormalsBuffer.Use(7);
        worldVerticesBuffer.Use(8);
        worldTrianglesBuffer.Use(9);
        boundingBoxBuffer.Use(10);

        stats.BufferData(default(Stats), BufferUsageHint.StreamRead);
        stats.Use(11);


        BufferSun();
        BufferCamera(alpha);
        BufferTime = Timer.Time(() => BufferModelsNew(meshLibrary));

        var transformCompute = shaderLibrary.Get("transform");
        //transformCompute.Uniform("modelCount", models.Count);
        transformCompute.Use();

        transformQuery.Begin(() => GL.DispatchCompute(models.Count, 1, 1));
        //GL.DispatchCompute(models.Count, 1, 1);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);


        var rayTraceCompute = shaderLibrary.Get("compModels");
        rayTraceCompute.Uniform("modelCount", models.Count);
        rayTraceCompute.Uniform("resolution", (Vector2)outputSize);


        textureLibrary.UseImage("colorOutput", 0, TextureAccess.ReadWrite);
        textureLibrary.UseImage("depthOutput", 1, TextureAccess.ReadWrite);
        textureLibrary.UseImage("objectOutput", 2, TextureAccess.ReadWrite);

        rayTraceCompute.Use();
        rayTraceQuery.Begin(() => GL.DispatchCompute(computeSize.X, computeSize.Y, 1));
        //GL.DispatchCompute(computeSize.X, computeSize.Y, 1);
        GL.MemoryBarrier(MemoryBarrierFlags.TextureFetchBarrierBit);

        //Stats = stats.GetData<Stats>();



        // outline
        var outlineShader = shaderLibrary.Get("outline");
        outlineShader.Use();
        outlineQuery.Begin(() => GL.DispatchCompute(computeSize.X, computeSize.Y, 1));
        //GL.DispatchCompute(computeSize.X, computeSize.Y, 1);
        GL.MemoryBarrier(MemoryBarrierFlags.TextureFetchBarrierBit);

        // render to screen quad
        textureLibrary.Use("colorOutput", 0);
        textureLibrary.Use("depthOutput", 1);
        textureLibrary.Use("objectOutput", 2);

        shaderLibrary.Use("screen");
        shaderLibrary.Uniform("screen", "textureID", textureID);

        vertexArray.Bind();
        GL.DrawElements(PrimitiveType.Triangles, vertexArray.IndexCount, DrawElementsType.UnsignedInt, 0);

        TransformTime = transformQuery.Get();
        RayTraceTime = rayTraceQuery.Get();
        OutlineTime = outlineQuery.Get();

        // imgui
        imGuiController.Render();

        models.Clear();

        Game.Context.SwapBuffers();

    }

    private void BufferSun() {

        sunAngle = Util.Normalize(sunAngle + Game.Delta * 0.5f);

        var rotation =
            Matrix3.CreateRotationX(-MathF.PI / 3f) *
            Matrix3.CreateRotationY(sunAngle);



        var direction = Vector3.UnitZ * rotation;

        //rotation.Transpose();




        var sun = new Sun(rotation, direction);

        sunBuffer.BufferData(sun);
    }

    private void BufferCamera(float alpha) {

        Matrix4 transform = Matrix4.Identity;
        transform *= Matrix4.CreateRotationZ(CameraRotation.Z);
        transform *= Matrix4.CreateRotationY(CameraRotation.Y);
        transform *= Matrix4.CreateRotationX(CameraRotation.X);

        var position = (CameraPosition - prevCameraPosition) * alpha + prevCameraPosition;

        transform *= Matrix4.CreateTranslation(position.X, position.Y, position.Z);
        transform *= Matrix4.CreateScale(1f);


        var camera = new Camera(transform, 1f);

        cameraBuffer.BufferData(camera);
    }

    private void BufferSpheres() {

        var data = new Sphere[spheres.Count];

        for (int i = 0; i < spheres.Count; i++) {
            var traceableSphere = spheres[i];
            data[i] = new Sphere(traceableSphere.Position, traceableSphere.Color, traceableSphere.Radius);
        }

        sphereBuffer.Clear();
        sphereBuffer.BufferSubData(data);
        spheres.Clear();
    }

    private void BufferModelsNew(MeshLibrary meshLibrary) {

        int vertexSize = Marshal.SizeOf<Vertex>();
        int triangleSize = Marshal.SizeOf<Triangle>();

        int currectPositionOffset = 0;
        int currentNormalOffset = 0;
        int currentVertexOffset = 0;
        int currentTriangleOffset = 0;

        //modelBuffer.Clear();

        var data = new Model[models.Count];

        for (int i = 0; i < models.Count; i++) {
            var traceableModel = models[i];

            var transform = Matrix4.Identity *
                Matrix4.CreateRotationZ(traceableModel.Rotation.Z) *
                Matrix4.CreateRotationY(traceableModel.Rotation.Y) *
                Matrix4.CreateRotationX(traceableModel.Rotation.X) *
                Matrix4.CreateScale(traceableModel.Scale) *
                Matrix4.CreateTranslation(traceableModel.Position);

            transform.Transpose();

            var sourceMesh = meshLibrary.GetMesh(traceableModel.MeshName);

            var worldMesh = new Mesh(
                new BufferSpan(currectPositionOffset, sourceMesh.Positions.Count),
                new BufferSpan(currentNormalOffset, sourceMesh.Normals.Count),
                new BufferSpan(sourceMesh.UVs.Offset, sourceMesh.UVs.Count),
                new BufferSpan(currentVertexOffset, sourceMesh.Vertices.Count),
                new BufferSpan(currentTriangleOffset, sourceMesh.Triangles.Count)
            );

            data[i] = new Model(transform, sourceMesh, worldMesh);

            currectPositionOffset += sourceMesh.Positions.Count;
            currentNormalOffset += sourceMesh.Normals.Count;
            currentVertexOffset += sourceMesh.Vertices.Count;
            currentTriangleOffset += sourceMesh.Triangles.Count;
        }

        modelBuffer.BufferSubData(data);
    }

    // private void BufferModels() {

    //     var meshLibrary = Game.Get<MeshLibrary>();
    //     meshLibrary.Bind();

    //     var data = new BufferSpan[models.Count];
    //     var matrices = new Matrix4[models.Count];

    //     for (int i = 0; i < models.Count; i++) {
    //         var traceableModel = models[i];

    //         var matrix =
    //             Matrix4.CreateRotationZ(traceableModel.Rotation.Z) *
    //             Matrix4.CreateRotationY(traceableModel.Rotation.Y) *
    //             Matrix4.CreateRotationX(traceableModel.Rotation.X) *
    //             Matrix4.CreateScale(traceableModel.Scale) *
    //             Matrix4.CreateTranslation(traceableModel.Position);

    //         matrix.Transpose();

    //         matrices[i] = matrix;

    //         var mesh = meshLibrary.GetMesh(traceableModel.MeshName);

    //         //data[i] = mesh;
    //     }
    //     models.Clear();

    //     modelBuffer.Clear();
    //     modelBuffer.BufferSubData(data);

    //     matrixSSBO.Clear();
    //     matrixSSBO.BufferSubData(matrices);


    // }

    private void DrawSpheres() {

        var shaderLibrary = Game.Get<ShaderLibrary>();


        shaderLibrary.Uniform("fragSpheres", "resolution", Game.WindowSize);
        shaderLibrary.Use("fragSpheres");

        BufferSpheres();

        sphereBuffer.Use(0);


        vertexArray.Bind();

        GL.DrawElements(PrimitiveType.Triangles, vertexArray.IndexCount, DrawElementsType.UnsignedInt, 0);
    }

    // private void DrawModels() {

    //     var shaderLibrary = Game.Get<ShaderLibrary>();

    //     var shader = shaderLibrary.Get("compModels");
    //     shader.Uniform("modelCount", models.Count);
    //     shader.Uniform("resolution", Game.WindowSize / 4f);

    //     BufferModels();

    //     modelBuffer.Use(5);
    //     matrixSSBO.Use(6);

    //     vertexArray.Bind();
    //     //GL.DrawElements(PrimitiveType.Triangles, vertexArray.IndexCount, DrawElementsType.UnsignedInt, 0);

    // }


}




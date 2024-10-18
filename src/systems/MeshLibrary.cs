
using System.Runtime.InteropServices;
using System.Text;
using FrogLib;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class MeshLibrary : GameSystem {

    public StorageBuffer SourcePositionsBuffer { get; }
    public StorageBuffer SourceNormalsBuffer { get; }
    public StorageBuffer SourceUVsBuffer { get; }
    public StorageBuffer SourceVerticesBuffer { get; }
    public StorageBuffer SourceTrianglesBuffer { get; }


    private List<Mesh> meshes;
    private Dictionary<string, int> meshesByName;

    public MeshLibrary() {
        SourcePositionsBuffer = new StorageBuffer(1024 * 1024); // 1024 MB
        SourceNormalsBuffer = new StorageBuffer(1024 * 1024); // 1024 MB
        SourceUVsBuffer = new StorageBuffer(1024 * 512); // 512 MB
        SourceVerticesBuffer = new StorageBuffer(1024 * 768); // 768 MB
        SourceTrianglesBuffer = new StorageBuffer(1024 * 768); // 768 MB

        meshes = new List<Mesh>();
        meshesByName = new Dictionary<string, int>();
    }

    public void Load(string path) {
        using FileStream file = new FileStream(path, FileMode.Open);
        using StreamReader reader = new StreamReader(file, Encoding.UTF8);
    }

    public Mesh GetMesh(string name) {
        if (!meshesByName.TryGetValue(name, out int index)) return default;
        return meshes[index];
    }

    public void LoadPrimitive(string name, PrimitiveMesh mesh) {

        var positions = mesh.Positions;
        var normals = mesh.Normals;
        var uvs = mesh.UVs;
        var vertices = mesh.Vertices;
        var triangles = mesh.Triangles;

        int currentPositionCount = SourcePositionsBuffer.End / Marshal.SizeOf<Vector4>();
        int currentNormalCount = SourceNormalsBuffer.End / Marshal.SizeOf<Vector4>();
        int currentUVCount = SourceUVsBuffer.End / Marshal.SizeOf<Vector2>();
        int currentVertexCount = SourceVerticesBuffer.End / Marshal.SizeOf<Vertex>();
        int currentTriangleCount = SourceTrianglesBuffer.End / Marshal.SizeOf<Vector4i>(); // vector4i instead of triangle since a bool in glsl is 4 bytes

        Log.Info($"Loading mesh: {name} | positions: {positions.Length}, normals: {normals.Length}, uvs: {uvs.Length}, vertices: {vertices.Length}, triangles: {triangles.Length}");

        meshesByName.Add(name, meshes.Count);
        meshes.Add(new Mesh(
            new BufferSpan(currentPositionCount, positions.Length),
            new BufferSpan(currentNormalCount, normals.Length),
            new BufferSpan(currentUVCount, uvs.Length),
            new BufferSpan(currentVertexCount, vertices.Length),
            new BufferSpan(currentTriangleCount, triangles.Length)
        ));

        SourcePositionsBuffer.AppendData(positions);
        SourceNormalsBuffer.AppendData(normals);
        SourceUVsBuffer.AppendData(uvs);
        SourceVerticesBuffer.AppendData(vertices);
        SourceTrianglesBuffer.AppendData(triangles);
    }
}
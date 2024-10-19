using FrogLib;
using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class MainScene : Scene {

    private SphereObject[] spheres;
    private Vector3 movementDirection;

    private bool isCapturingInput = true;


    private Accumulator<double> updateTime = new(200);
    private Accumulator<double> renderTime = new(200);

    public MainScene() {

        var random = new Random();

        spheres = new SphereObject[32];

        for (int i = 0; i < spheres.Length; i++) {
            var position = new Vector3(random.NextSingle() * 2f - 1f, random.NextSingle() * 2f - 1f, random.NextSingle() * 2f - 1f);
            var radius = random.NextSingle() * 0.1f + 0.1f;

            var color = (i % 3) switch {
                0 => new Color4(1f, 0f, 0f, 0.5f),
                1 => new Color4(0f, 1f, 0f, 0.5f),
                2 => new Color4(0f, 0f, 1f, 0.5f),
                _ => new Color4(0f, 0f, 0f, 0.5f),
            };

            spheres[i] = new SphereObject(position, radius, color);
        }

    }

    public override void Startup() {

        var rayTracer = Game.Get<RayTracer>();

        rayTracer.SetImGuiFrameLayout(() => {
            ImGui.Begin("info");
            ImGui.Text($"update: {updateTime.Average}ms");
            ImGui.Text($"render: {renderTime.Average}ms");
            ImGui.Text($"fps: {1000.0 / renderTime.Average}");
            ImGui.Text($"buffer time: {rayTracer.BufferTime.TotalMilliseconds}ms");
            ImGui.Text($"transform time: {rayTracer.TransformTime / 1000000d}ms");
            ImGui.Text($"ray trace time: {rayTracer.RayTraceTime / 1000000d}ms");
            ImGui.Text($"outline time: {rayTracer.OutlineTime / 1000000d}ms");
            //ImGui.Text($"triangle checks: {rayTracer.Stats.NumTriangleChecks}");
            //ImGui.Text($"box checks: {rayTracer.Stats.NumBoxChecks}");
            ImGui.End();
        });
    }


    public override void Update() {

        updateTime.Add(Game.UpdateTime.AsMilliseconds());
        renderTime.Add(Game.RenderTime.AsMilliseconds());


        //if (ImGuiNET.ImGui.IsWindowHovered(ImGuiNET.ImGuiHoveredFlags.AnyWindow)) return;

        var rayTracer = Game.Get<RayTracer>();

        if (Game.Input.IsMouseDown(MouseButton.Right)) {
            Game.CursorState = CursorState.Grabbed;

            var mouseDelta = Game.Input.GetMouseDelta();


            rayTracer.Camera.Rotate(new Vector3(-mouseDelta.Yx) * 0.002f);

        } else {
            Game.CursorState = CursorState.Normal;
        }

        movementDirection = new Vector3(
            (Game.Input.IsKeyDown(Keys.D) ? 1f : 0f) - (Game.Input.IsKeyDown(Keys.A) ? 1f : 0f),
            (Game.Input.IsKeyDown(Keys.Space) ? 1f : 0f) - (Game.Input.IsKeyDown(Keys.LeftControl) ? 1f : 0f),
            (Game.Input.IsKeyDown(Keys.W) ? 1f : 0f) - (Game.Input.IsKeyDown(Keys.S) ? 1f : 0f)
        );

        rayTracer.Camera.Translate(movementDirection * 3f * Game.Delta.AsSeconds());
    }

    public override void Render(float alpha) {

        var rayTracer = Game.Get<RayTracer>();

        // rayTracer.SubmitModel(new TraceableModel("cube") {
        //     Position = new Vector3(1f, MathF.Sin(Game.Time.AsSeconds()), 0f),
        //     Rotation = new Vector3(Game.Time.AsSeconds() * 0.1f, Game.Time.AsSeconds() * 0.2f, Game.Time.AsSeconds() * 0.3f),
        //     Scale = new Vector3(MathF.Sin(Game.Time.AsSeconds()) * 0.25f + 0.75f),
        // });

        // rayTracer.SubmitModel(new TraceableModel("cube") {
        //     Position = new Vector3(-1f, 0f, 0f),
        // });

        for (int z = -4; z <= 4; z++) {
            for (int y = -4; y <= 4; y++) {
                for (int x = -4; x <= 4; x++) {
                    rayTracer.SubmitModel(new TraceableModel("cube") {
                        Position = new Vector3(x * 2, y * 2, z * 2),
                        Rotation = new Vector3(Game.Time.AsSeconds(), Game.Time.AsSeconds(), Game.Time.AsSeconds())
                    });
                }
            }
        }


        // rayTracer.SubmitModel(new TraceableModel("triangle") {
        //     Position = new Vector3(0f, 0f, 0f),
        //     Rotation = new Vector3(0f, Game.Time / 5000000f, 0f)
        // });




        // for (int i = 0; i < spheres.Length; i++) {
        //     spheres[i].Render(alpha);
        // }



    }

}
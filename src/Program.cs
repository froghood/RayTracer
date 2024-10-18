
using FrogLib;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

internal class Program {
    private static void Main(string[] args) {

        Log.Info(Directory.GetCurrentDirectory());

        var settings = new NativeWindowSettings() {
            ClientSize = new Vector2i(1280, 720),
            WindowState = WindowState.Fullscreen
        };
        Game.Init(settings);

        var scenes = Game.Register<SceneStorage>();
        Game.Register<ShaderLibrary>();
        Game.Register<MeshLibrary>();
        Game.Register<TextureLibrary>();
        Game.Register<RayTracer>();

        scenes.Change<MainScene>();





        Game.SetUpdateFrequency(100);
        Game.SetRenderFrequency(int.MaxValue);
        Game.Run(RunType.FixedDecoupled);

    }
}
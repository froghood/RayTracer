using FrogLib;
using OpenTK.Mathematics;

public class SphereObject {
    private Vector3 position;
    private float radius;
    private Color4 color;
    private float startingAngle;
    private float frequency;

    public SphereObject(Vector3 position, float radius, Color4 color) {
        this.position = position;
        this.radius = radius;
        this.color = color;

        var random = new Random();

        startingAngle = Util.Normalize(random.NextSingle() * MathF.Tau);
        frequency = random.NextSingle() * 1f + 1f;


    }

    public void Update() { }

    public void Render(float alpha) {
        // Game.Get<RayTracer>().SubmitSphere(new TraceableSphere() {
        //     Position = position + new Vector3(0f, 0.1f, 0f) * MathF.Sin(Util.Normalize(Game.Time / (1000000f * frequency) + startingAngle)),
        //     Radius = radius,
        //     Color = new Color4(color.R, color.G, color.B, MathF.Sin(Util.Normalize(Game.Time / 1000000f)) * 0.25f + 0.75f)
        // });
    }
}
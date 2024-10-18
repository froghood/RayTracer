using OpenTK.Mathematics;

public struct Sphere {

    public Vector4 PositionRadius { get; } // 16 bytes
    public Color4 Color { get; } // 16 bytes


    public Sphere(Vector3 position, Color4 color, float radius) {
        PositionRadius = new Vector4(position, radius);
        Color = color;
    }

}
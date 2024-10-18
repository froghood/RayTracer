using OpenTK.Mathematics;

public class PrimitiveTriangleMesh : PrimitiveMesh {
    public override Vector4[] Positions => new[] {
        new Vector4(0f, 0.5f, 0f, 1f),
        new Vector4(0.5f, -0.5f, 0f, 1f),
        new Vector4(-0.5f, -0.5f, 0f, 1f),
        new Vector4(0f, 0.5f, 1f, 1f),
        new Vector4(0.5f, -0.5f, 1f, 1f),
        new Vector4(-0.5f, -0.5f, 1f, 1f),
    };

    public override Vector4[] Normals => new[] {
        new Vector4(0f, 0f, -1f, 0f),
    };

    public override Vector2[] UVs => new[] {
        new Vector2(0.5f, 1f),
        new Vector2(1f, 0f),
        new Vector2(0f, 0f),
    };

    public override Vertex[] Vertices => new[] {
        new Vertex(0, 0, 0),
        new Vertex(1, 0, 1),
        new Vertex(2, 0, 2),
        new Vertex(3, 0, 0),
        new Vertex(4, 0, 1),
        new Vertex(5, 0, 2),
    };

    public override Triangle[] Triangles => new[] {
        new Triangle(0, 1, 2, false),
        new Triangle(3, 4, 5, false),
    };
}
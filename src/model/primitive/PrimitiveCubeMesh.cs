using OpenTK.Mathematics;

public class PrimitiveCubeMesh : PrimitiveMesh {
    public override Vector4[] Positions => new[] {
        new Vector4(0.5f, 0.5f, 0.5f, 1f),
        new Vector4(-0.5f, 0.5f, 0.5f, 1f),
        new Vector4(0.5f, -0.5f, 0.5f, 1f),
        new Vector4(-0.5f, -0.5f, 0.5f, 1f),
        new Vector4(0.5f, 0.5f, -0.5f, 1f),
        new Vector4(-0.5f, 0.5f, -0.5f, 1f),
        new Vector4(0.5f, -0.5f, -0.5f, 1f),
        new Vector4(-0.5f, -0.5f, -0.5f, 1f),
    };

    public override Vector4[] Normals => new[] {
        new Vector4(1f, 1f, 1f, 0f).Normalized(),
        new Vector4(-1f, 1f, 1f, 0f).Normalized(),
        new Vector4(1f, -1f, 1f, 0f).Normalized(),
        new Vector4(-1f, -1f, 1f, 0f).Normalized(),
        new Vector4(1f, 1f, -1f, 0f).Normalized(),
        new Vector4(-1f, 1f, -1f, 0f).Normalized(),
        new Vector4(1f, -1f, -1f, 0f).Normalized(),
        new Vector4(-1f, -1f, -1f, 0f).Normalized(),
    };

    public override Vector2[] UVs => new[] {
        new Vector2(0f, 0f),
        new Vector2(1f, 0f),
        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
    };

    public override Vertex[] Vertices => new[] {
            
        // +x face
        new Vertex(4, 4, 2), // 0
        new Vertex(0, 0, 3), // 1
        new Vertex(2, 2, 1), // 2
        // new Vertex(4, 4, 2), // 0
        // new Vertex(2, 2, 1), // 2
        new Vertex(6, 6, 0), // 3

        // -x face
        new Vertex(1, 1, 2), // 4
        new Vertex(5, 5, 3), // 5
        new Vertex(7, 7, 1), // 6
        // new Vertex(1, 1, 2), // 4
        // new Vertex(7, 7, 1), // 6
        new Vertex(3, 3, 0), // 7
        
        // +y face
        // new Vertex(1, 1, 2), // 4
        // new Vertex(0, 0, 3), // 1
        new Vertex(4, 4, 1), // 8
        // new Vertex(1, 1, 2), // 4
        // new Vertex(4, 4, 1), // 8
        new Vertex(5, 5, 0), // 9
        
        // -y face
        new Vertex(7, 7, 2), // 10
        new Vertex(6, 6, 3), // 11
        // new Vertex(2, 2, 1), // 2
        // new Vertex(7, 7, 2), // 10
        // new Vertex(2, 2, 1), // 2
        // new Vertex(3, 3, 0), // 7
        
        // +z
        new Vertex(0, 0, 2), // 12
        new Vertex(1, 1, 3), // 13
        new Vertex(3, 3, 1), // 14
        // new Vertex(0, 0, 2), // 12
        // new Vertex(3, 3, 1), // 14
        new Vertex(2, 2, 0), // 15
        
        // -z
        new Vertex(5, 5, 2), // 16
        new Vertex(4, 4, 3), // 17
        new Vertex(6, 6, 1), // 18
        // new Vertex(5, 5, 2), // 16
        // new Vertex(6, 6, 1), // 18
        new Vertex(7, 7, 0), // 19
    };

    public override Triangle[] Triangles => new[] {
        new Triangle(0, 1, 2, true), // +x
        new Triangle(0, 2, 3, true),
        new Triangle(4, 5, 6, true), // -x
        new Triangle(4, 6, 7, true),
        new Triangle(4, 1, 8, true), // +y
        new Triangle(4, 8, 9, true),
        new Triangle(10, 11, 2, true), // -y
        new Triangle(10, 2, 7, true),
        new Triangle(12, 13, 14, true), // +z
        new Triangle(12, 14, 15, true),
        new Triangle(16, 17, 18, true), // -z
        new Triangle(16, 18, 19, true)
    };
}
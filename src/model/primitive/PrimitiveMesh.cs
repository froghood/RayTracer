using OpenTK.Mathematics;

public abstract class PrimitiveMesh {
    public abstract Vector4[] Positions { get; }
    public abstract Vector4[] Normals { get; }
    public abstract Vector2[] UVs { get; }
    public abstract Vertex[] Vertices { get; }
    public abstract Triangle[] Triangles { get; }
}
public struct Mesh {
    public BufferSpan Positions { get; }
    public BufferSpan Normals { get; }
    public BufferSpan UVs { get; }
    public BufferSpan Vertices { get; }
    public BufferSpan Triangles { get; }

    public Mesh(BufferSpan positions, BufferSpan normals, BufferSpan uvs, BufferSpan vertices, BufferSpan triangles) {
        Positions = positions;
        Normals = normals;
        UVs = uvs;
        Vertices = vertices;
        Triangles = triangles;
    }
}
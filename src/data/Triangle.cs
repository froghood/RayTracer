public struct Triangle {
    public int VertexIndexA { get; }
    public int VertexIndexB { get; }
    public int VertexIndexC { get; }
    public bool CullBackface { get; }
    public Triangle(int indexA, int indexB, int indexC, bool cullBackFace = false) {
        VertexIndexA = indexA;
        VertexIndexB = indexB;
        VertexIndexC = indexC;
        CullBackface = cullBackFace;
    }
}
using OpenTK.Mathematics;

public struct Vertex {
    public int PositionIndex { get; }
    public int NormalIndex { get; }
    public int UVIndex { get; }
    public Vertex(int positionIndex, int normalIndex, int uvIndex) {
        PositionIndex = positionIndex;
        NormalIndex = normalIndex;
        UVIndex = uvIndex;
    }
}
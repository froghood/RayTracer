
public struct BufferSpan {
    public int Offset { get; }
    public int Count { get; }
    public BufferSpan(int index, int count) {
        Offset = index;
        Count = count;
    }
}
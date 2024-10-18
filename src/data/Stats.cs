public struct Stats {
    public int NumTriangleChecks { get; }
    public int NumBoxChecks { get; }

    public Stats(int numTriangleChecks, int numBoxChecks) {
        NumTriangleChecks = numTriangleChecks;
        NumBoxChecks = numBoxChecks;
    }
}
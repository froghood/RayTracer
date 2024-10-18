using OpenTK.Mathematics;

public class TraceableModel {
    public string MeshName { get; }
    public Vector3 Position { get; init; }
    public Vector3 Rotation { get; init; }
    public Vector3 Scale { get; init; } = Vector3.One;
    public TraceableModel(string meshName) {
        MeshName = meshName;
    }
}
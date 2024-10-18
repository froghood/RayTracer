
using OpenTK.Mathematics;

public struct Model {
    public Matrix4 Transform { get; }
    public Mesh SourceMesh { get; }
    public Mesh WorldMesh { get; }


    public Model(Matrix4 transform, Mesh sourceMesh, Mesh worldMesh) {
        Transform = transform;
        SourceMesh = sourceMesh;
        WorldMesh = worldMesh;
    }
}




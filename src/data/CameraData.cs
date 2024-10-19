using OpenTK.Mathematics;

public struct CameraData {
    public Matrix4 Transform { get; }
    public float FocalLength { get; }
    public CameraData(Matrix4 transform, float focalLength) {
        Transform = transform;
        FocalLength = focalLength;
    }
}
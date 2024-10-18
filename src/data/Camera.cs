using OpenTK.Mathematics;

public struct Camera {

    public Matrix4 Transform { get; }
    public float FocalLength { get; }

    public Camera(Matrix4 transform, float focalLength) {
        Transform = transform;
        FocalLength = focalLength;
    }

}
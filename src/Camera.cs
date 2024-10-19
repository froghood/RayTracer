using OpenTK.Mathematics;

public class Camera {

    private Vector3 position;
    private Vector3 prevPosition;
    public Vector3 rotation;
    public float focalLength;



    public Camera(Vector3 position, Vector3 rotation, float focalLength) {
        this.position = position;
        this.rotation = rotation;
        this.focalLength = focalLength;
    }



    public void Translate(Vector3 translation) {
        prevPosition = this.position;
        this.position += translation * Matrix3.CreateRotationY(-this.rotation.Y);
    }

    public void Rotate(Vector3 rotation) {
        this.rotation = new Vector3(
            MathF.Max(MathF.Min(this.rotation.X + rotation.X, MathF.PI / 2f), -MathF.PI / 2f),
            Util.Normalize(this.rotation.Y + rotation.Y),
            Util.Normalize(this.rotation.Z + rotation.Z)
        );
    }



    public CameraData GetData(float alpha) {
        var matrix = Matrix4.Identity *
            Matrix4.CreateRotationZ(rotation.Z) *
            Matrix4.CreateRotationY(rotation.Y) *
            Matrix4.CreateRotationX(rotation.X) *
            Matrix4.CreateTranslation(prevPosition + (position - prevPosition) * alpha);

        return new CameraData(matrix, focalLength);
    }
}
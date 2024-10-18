using OpenTK.Mathematics;

public struct Sun {

    //public Matrix3 Rotation { get; }
    //public Vector4 p0 { get; } // padding
    public Vector3 Direction { get; }

    public Sun(Matrix3 rotation, Vector3 direction) {
        //Rotation = rotation;
        Direction = direction;
    }

}
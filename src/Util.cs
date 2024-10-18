public static class Util {



    public static float Normalize(float angle) {
        return angle + MathF.Ceiling((-angle - MathF.PI) / MathF.Tau) * MathF.Tau;
    }
}
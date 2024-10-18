using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

public class UniformBuffer {


    private int ubo;
    private BufferUsageHint usageHint;



    public UniformBuffer(BufferUsageHint usageHint) {
        this.ubo = GL.GenBuffer();
        this.usageHint = usageHint;
    }



    public void Use(int index) => GL.BindBufferBase(BufferRangeTarget.UniformBuffer, index, ubo);



    public void BufferData<T>(T[] data) where T : struct {

        var size = Marshal.SizeOf<T>();

        GL.BindBuffer(BufferTarget.UniformBuffer, ubo);
        GL.BufferData<T>(BufferTarget.UniformBuffer, data.Length * size, data, usageHint);
    }

    public void BufferData<T>(T data) where T : struct {

        var size = Marshal.SizeOf<T>();

        GL.BindBuffer(BufferTarget.UniformBuffer, ubo);
        GL.BufferData<T>(BufferTarget.UniformBuffer, size, ref data, usageHint);
    }



}
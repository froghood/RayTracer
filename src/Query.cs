using FrogLib;
using OpenTK.Graphics.OpenGL4;

public class Query {

    private Queue<int> freeHandles = new Queue<int>();
    private Queue<int> usedHandles = new Queue<int>();

    private int numQueries = 0;

    private QueryTarget target;

    public Query(QueryTarget target) {
        this.target = target;
    }

    public void Begin(Action action) {
        int handle;

        if (freeHandles.Count == 0) {
            Log.Info($"no free queries, creating new query... total: {++numQueries}");
            handle = GL.GenQuery();
        } else {
            handle = freeHandles.Dequeue();
        }

        usedHandles.Enqueue(handle);

        GL.BeginQuery(target, handle);
        action.Invoke();
        GL.EndQuery(target);
    }


    public unsafe long Get() {
        long result = 0L;

        if (usedHandles.Count == 0) return result;

        int available = 0;
        GL.GetQueryObject(usedHandles.Peek(), GetQueryObjectParam.QueryResultAvailable, &available);

        if (available == 0) return result;

        int handle = usedHandles.Dequeue();
        GL.GetQueryObject(handle, GetQueryObjectParam.QueryResult, &result);

        freeHandles.Enqueue(handle);

        return result;
    }

}
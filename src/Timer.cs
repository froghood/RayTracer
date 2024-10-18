public static class Timer {

    public static TimeSpan Time(Action action) {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        action();
        sw.Stop();
        return sw.Elapsed;
    }
}
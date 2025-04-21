public static class Spinner
{
    private static readonly string[] Sequence = ["|", "/", "-", "\\"];
    private static bool _active;
    private static Task? _spinnerTask;

    public static void Start(string message = "Loading")
    {
        _active = true;
        _spinnerTask = Task.Run(() =>
        {
            int counter = 0;
            while (_active)
            {
                Console.Write($"\r{message} {Sequence[counter++ % Sequence.Length]}");
                Thread.Sleep(100);
            }
        });
    }

    public static void Stop()
    {
        _active = false;
        _spinnerTask?.Wait();
        Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r"); // clear line
    }
}
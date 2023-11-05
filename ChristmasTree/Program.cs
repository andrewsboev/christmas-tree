using System.Diagnostics;

PrepareForChristmas();

var cancellationTokenSource = new CancellationTokenSource();

var christmasTree = CreateChristmasTree(cancellationTokenSource.Token);
await christmasTree.TurnOn();

Console.CancelKeyPress += (obj, args) =>
{
    args.Cancel = true;
    ChristmasIsOver();
};

void PrepareForChristmas()
{
    Console.Clear();
    Console.CursorVisible = false;
}

string RunCommandWithBash(string command)
{
    var psi = new ProcessStartInfo
    {
        FileName = "/bin/bash",
        Arguments = command,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using var process = Process.Start(psi);

    process.WaitForExit();

    var output = process.StandardOutput.ReadToEnd();

    return output;
}

ChristmasTree CreateChristmasTree(CancellationToken cancellationToken)
{
    var commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();
    var height = SafeParse(commandLineArgs.FirstOrDefault(), 25);

    return new ChristmasTree(height, cancellationToken);
}

int SafeParse(string? str, int @default) => 
    int.TryParse(str, out var result) ? result : @default;

void ChristmasIsOver()
{
    Console.WriteLine();
    Console.CursorVisible = true;
}
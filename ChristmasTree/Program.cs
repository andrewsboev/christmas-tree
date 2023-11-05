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
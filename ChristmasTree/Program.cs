using System.Diagnostics;

PrepareForChristmas();

using var cancellationTokenSource = new CancellationTokenSource();

// Properly wire up cancellation
Console.CancelKeyPress += (obj, args) =>
{
    args.Cancel = true;
    cancellationTokenSource.Cancel(); // Actually trigger cancellation
    ChristmasIsOver();
};

var christmasTree = CreateChristmasTree(cancellationTokenSource.Token);
await christmasTree.TurnOn();

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

int SafeParse(string? str, int @default)
{
    if (string.IsNullOrWhiteSpace(str))
        return @default;
    
    if (!int.TryParse(str, out var result))
        return @default;
    
    // Add bounds checking for security
    if (result <= 0 || result > 50)
        return @default;
    
    return result;
}

void ChristmasIsOver()
{
    Console.WriteLine();
    Console.CursorVisible = true;
}
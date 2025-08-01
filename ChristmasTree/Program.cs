using System.Diagnostics;

// Store original console state for restoration
var originalCursorVisible = Console.CursorVisible;
var originalForegroundColor = Console.ForegroundColor;

try
{
    PrepareForChristmas();

    using var cancellationTokenSource = new CancellationTokenSource();

    // Properly wire up cancellation with console state restoration
    Console.CancelKeyPress += (obj, args) =>
    {
        args.Cancel = true;
        cancellationTokenSource.Cancel(); // Actually trigger cancellation
        ChristmasIsOver();
    };

    // Also handle AppDomain unload and process exit for cleanup
    AppDomain.CurrentDomain.ProcessExit += (obj, args) => RestoreConsoleState();
    AppDomain.CurrentDomain.UnhandledException += (obj, args) => RestoreConsoleState();

    var christmasTree = CreateChristmasTree(cancellationTokenSource.Token);
    await christmasTree.TurnOn();
}
catch (InvalidOperationException ex) when (ex.Message.Contains("Console"))
{
    Console.WriteLine($"\nConsole Error: {ex.Message}");
    Console.WriteLine("Please resize your console window and try again.");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"\nInput Error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"\nUnexpected error: {ex.Message}");
}
finally
{
    // Always restore console state
    RestoreConsoleState();
}

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
    
    // Add bounds checking for security - fixed edge case
    if (result <= 0 || result >= 50) // Changed from > to >= for consistency
        return @default;
    
    return result;
}

void ChristmasIsOver()
{
    Console.WriteLine();
    RestoreConsoleState();
}

void RestoreConsoleState()
{
    try
    {
        Console.CursorVisible = originalCursorVisible;
        Console.ForegroundColor = originalForegroundColor;
        Console.ResetColor(); // Ensure colors are fully reset
    }
    catch
    {
        // Ignore errors during cleanup to prevent masking original exceptions
    }
}
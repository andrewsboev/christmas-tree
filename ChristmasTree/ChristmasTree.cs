public class ChristmasTree
{
    private readonly ConsoleColor[] availableColors =
    {
        ConsoleColor.Red,
        ConsoleColor.Green,
        ConsoleColor.Yellow
    };

    private readonly Random random = new();
    private readonly ConsoleColor[,] colors;
    private readonly CancellationToken cancellationToken;
    private readonly int height;

    public async Task TurnOn()
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Blink();
                Draw();

                await Task.Delay(150, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Gracefully handle cancellation
        }
    }

    public ChristmasTree(int height, CancellationToken cancellationToken)
    {
        // Validate input
        if (height <= 0)
            throw new ArgumentException("Height must be positive", nameof(height));
        if (height > 50)
            throw new ArgumentException("Height too large (max 50)", nameof(height));

        this.height = height;
        // Fix: Correct array dimensions - each row needs (2 * row + 1) columns, max is (2 * height - 1)
        colors = new ConsoleColor[height, 2 * height - 1];
        this.cancellationToken = cancellationToken;
    }

    public void Draw()
    {
        Console.SetCursorPosition(0, 0);
        
        for (var row = 0; row < height; ++row)
        {
            // Calculate proper centering offset
            var starsInRow = 2 * row + 1;
            var leftPadding = height - row - 1;
            
            // Move to the correct position for this row
            Console.SetCursorPosition(leftPadding, row);
            
            for (var column = 0; column < starsInRow; ++column)
            {
                Console.ForegroundColor = colors[row, column];
                Console.Write("*");
            }
        }
    }

    public void Blink()
    {
        for (var row = 0; row < height; ++row)
        {
            var starsInRow = 2 * row + 1;
            for (var column = 0; column < starsInRow; ++column)
            {
                colors[row, column] = GenerateRandomColor();
            }
        }
    }

    private ConsoleColor GenerateRandomColor() =>
        availableColors[random.Next(0, availableColors.Length)];
}
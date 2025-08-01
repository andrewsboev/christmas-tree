public class ChristmasTree
{
    private readonly ConsoleColor[] availableColors =
    {
        ConsoleColor.Red,
        ConsoleColor.Green,
        ConsoleColor.Yellow
    };

    private readonly ThreadSafeRandom random = new();
    private readonly ConsoleColor[] colors; // Changed to 1D array for better performance
    private readonly int[] rowStarts; // Lookup table for row starts
    private readonly CancellationToken cancellationToken;
    private readonly int height;
    private readonly int totalStars; // Pre-calculated total stars needed

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
        catch (ArgumentOutOfRangeException ex) when (ex.Message.Contains("cursor"))
        {
            // Handle console dimension issues gracefully
            Console.WriteLine("\nError: Console window is too small for the Christmas tree.");
            Console.WriteLine($"Please resize your console to at least {height} rows and {2 * height - 1} columns.");
        }
    }

    public ChristmasTree(int height, CancellationToken cancellationToken)
    {
        // Validate input
        if (height <= 0)
            throw new ArgumentException("Height must be positive", nameof(height));
        if (height >= 50) // Changed from > to >= for clarity
            throw new ArgumentException("Height too large (max 49)", nameof(height));

        // Validate console dimensions
        if (Console.BufferHeight < height)
            throw new InvalidOperationException($"Console buffer height ({Console.BufferHeight}) is too small for tree height ({height})");
        if (Console.BufferWidth < 2 * height - 1)
            throw new InvalidOperationException($"Console buffer width ({Console.BufferWidth}) is too small for tree width ({2 * height - 1})");

        this.height = height;
        this.cancellationToken = cancellationToken;

        // Calculate total stars needed (triangular number formula)
        totalStars = height * height; // Sum of (2*i+1) from i=0 to height-1

        // Initialize optimized data structures
        colors = new ConsoleColor[totalStars];
        rowStarts = new int[height];
        
        // Pre-calculate row start positions for O(1) lookup
        int currentStart = 0;
        for (int row = 0; row < height; row++)
        {
            rowStarts[row] = currentStart;
            currentStart += 2 * row + 1;
        }
    }

    public void Draw()
    {
        // Validate console dimensions before drawing
        if (!IsConsoleSizeValid())
        {
            Console.Clear();
            Console.WriteLine("Console too small! Please resize and restart.");
            return;
        }

        Console.SetCursorPosition(0, 0);
        
        for (var row = 0; row < height; ++row)
        {
            var starsInRow = 2 * row + 1;
            var leftPadding = height - row - 1;
            
            // Safety check for cursor position
            if (leftPadding >= 0 && row >= 0 && 
                leftPadding < Console.BufferWidth && row < Console.BufferHeight)
            {
                Console.SetCursorPosition(leftPadding, row);
                
                var rowStart = rowStarts[row];
                for (var column = 0; column < starsInRow; ++column)
                {
                    Console.ForegroundColor = colors[rowStart + column];
                    Console.Write("*");
                }
            }
        }
    }

    public void Blink()
    {
        // Use optimized array access with pre-calculated positions
        for (var row = 0; row < height; ++row)
        {
            var starsInRow = 2 * row + 1;
            var rowStart = rowStarts[row];
            
            for (var column = 0; column < starsInRow; ++column)
            {
                colors[rowStart + column] = GenerateRandomColor();
            }
        }
    }

    private bool IsConsoleSizeValid()
    {
        return Console.BufferWidth >= 2 * height - 1 && 
               Console.BufferHeight >= height &&
               Console.WindowWidth > 0 && 
               Console.WindowHeight > 0;
    }

    private ConsoleColor GenerateRandomColor() =>
        availableColors[random.Next(0, availableColors.Length)];
}

// Thread-safe Random wrapper
internal class ThreadSafeRandom
{
    private static readonly ThreadLocal<Random> ThreadLocalRandom = new(() => new Random());
    
    public int Next(int minValue, int maxValue) => ThreadLocalRandom.Value!.Next(minValue, maxValue);
}
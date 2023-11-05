Console.Clear();
Console.CursorVisible = false;

var christmasTree = new ChristmasTree(25);

while (true)
{
    christmasTree.Blink();
    christmasTree.Draw();

    await Task.Delay(150);
}

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

    public ChristmasTree(int height)
    {
        colors = new ConsoleColor[height, 2 * height - 1];
    }

    public void Draw()
    {
        for (var row = 0; row < colors.GetLength(0); ++row)
        {
            for (var column = 0; column < 2 * row + 1; ++column)
            {
                Console.SetCursorPosition(colors.GetLength(0) - row - 1 + column, row);
                Console.ForegroundColor = colors[row, column];
                Console.Write("*");           
            }
        }
    }

    public void Blink()
    {
        for (var row = 0; row < colors.GetLength(0); ++row)
        {
            for (var column = 0; column < 2 * row + 1; ++column)
            {
                colors[row, column] = GenerateRandomColor();
            }
        }       
    }

    private ConsoleColor GenerateRandomColor() => 
        availableColors[random.Next() % availableColors.Length];
}
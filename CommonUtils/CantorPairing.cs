namespace Common.Utils;

public class CantorPairing
{
    //Encoding pairs uniquely for theoretical use 0rder specifix
    public static int Pair(int x, int y)
    {
        return (x + y) * (x + y + 1) / 2 + y;
    }

    // Cantor inverse function to get (x, y) back from z
    public static (int x, int y) Unpair(int z)
    {
        int w = (int)((Math.Sqrt(8 * z + 1) - 1) / 2);
        int t = (w * (w + 1)) / 2;
        int y = z - t;
        int x = w - y;
        return (x, y);
    }

    //static void Main()
    //{
    //    int x = 5, y = 3;
    //    int paired = Pair(x, y);
    //    Console.WriteLine($"Paired value of ({x}, {y}): {paired}");

    //    var (xDecoded, yDecoded) = Unpair(paired);
    //    Console.WriteLine($"Decoded values: ({xDecoded}, {yDecoded})");
    //}
}

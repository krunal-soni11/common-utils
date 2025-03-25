namespace Common.Utils;

public class SzudzikPairing
{
    //for a more space-efficient alternative
    public static int Pair(int x, int y)
    {
        return (x >= y) ? (x * x + y) : (y * y + x);
    }

    // Szudzik inverse function
    public static (int x, int y) Unpair(int z)
    {
        int w = (int)Math.Floor(Math.Sqrt(z));
        int t = z - (w * w);

        return (t < w) ? (t, w) : (w, t);
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

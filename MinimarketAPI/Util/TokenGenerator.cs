namespace Minimarket.Util;

public static class TokenGenerator
{
    public static string GenerateToken()
    {
        // NOTA: Esto se puede mejorar.
        var rand = new Random();
        return string.Concat(DateTime.Now.Ticks.ToString(),
            rand.NextDouble().ToString()!.AsSpan(2));
    }
}
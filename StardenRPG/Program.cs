using System;

namespace StardenRPG
{
    public static class Program
    {
        static void Main()
        {
            using var game = new StardenRPG.Game1();
            game.Run();
        }
    }
}

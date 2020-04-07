using System;

namespace PuyoPuyoProj
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using PuyoPuyoProjGame game = new PuyoPuyoProjGame();
            game.Run();
        }
    }
}

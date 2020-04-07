using PuyoPuyoProj.Core.Interface;
using PuyoPuyoProj.Core.PuyoPuyo;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoPuyoProj.Core
{
    public class Vars
    {
        public static Resources resources;
        public static GameState state;
        public static Board mainBoard;
        public static GameActionSource mainActionSource;
    }
    public enum GameState
    {
        MAIN_MENU
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoPuyoProj.Core.PuyoPuyo
{
    [Flags]
    public enum CardinalDir : byte
    {
        SOUTH = 1,
        NORTH = 2,
        EAST = 4,
        WEST = 8
    }
    public static class CardinalDirExt
    {
        public static CardinalDir[] cardinalDirs = new CardinalDir[] {
            CardinalDir.NORTH,
            CardinalDir.EAST,
            CardinalDir.SOUTH,
            CardinalDir.WEST
        };
        public static (int x, int y) Offset(this CardinalDir cardinal) {
            int x = 0;
            int y = 0;
            if (cardinal.HasFlag(CardinalDir.NORTH)) {
                y += 1;
            }
            if (cardinal.HasFlag(CardinalDir.EAST)) {
                x += 1;
            }
            if (cardinal.HasFlag(CardinalDir.SOUTH)) {
                y -= 1;
            }
            if (cardinal.HasFlag(CardinalDir.WEST)) {
                x -= 1;
            }
            return (x, y);
        }
        public static CardinalDir RotateLeft(this CardinalDir cardinal) {
            return cardinal.Rotate(-1);
        }
        public static CardinalDir RotateRight(this CardinalDir cardinal) {
            return cardinal.Rotate(1);
        }
        private static CardinalDir Rotate(this CardinalDir cardinal, int diff) {
            CardinalDir rotated = 0;
            for (int i = 0; i < cardinalDirs.Length; i++) {
                if (cardinal.HasFlag(cardinalDirs[i])) {
                    int newDir = i + diff;
                    if (newDir >= cardinalDirs.Length) {
                        newDir = 0;
                    }
                    if (newDir < 0) {
                        newDir = cardinalDirs.Length - 1;
                    }
                    rotated |= cardinalDirs[newDir];
                }
            }
            return rotated;
        }
    }
}

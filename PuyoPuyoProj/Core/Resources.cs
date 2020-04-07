using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PuyoPuyoProj.Core.PuyoPuyo;

namespace PuyoPuyoProj.Core
{
    public class Resources
    {
        public const string SPRITE_DIR = "Sprites";
        public const string PUYO_SKIN_DIR = SPRITE_DIR + "/" + "Puyo";
        public Texture2D puyoSkin;
        public const int TILE_SIZE = 32;
        public void Load(ContentManager content) {
            puyoSkin = content.Load<Texture2D>(PUYO_SKIN_DIR + "/Default");
        }
        public void Unload(ContentManager content) {
            puyoSkin = content.Load<Texture2D>(PUYO_SKIN_DIR + "/Default");
        }
        public Rectangle PuyoSrcRect(Puyo puyo, CardinalDir connections) {
            int x = (int)connections;
            int y = (int)puyo - 1;
            if (puyo == Puyo.NUISANCE) {
                x = 6;
                y = 12;
            }
            return new Rectangle(x * TILE_SIZE + 1, y * TILE_SIZE + 1, TILE_SIZE - 1, TILE_SIZE - 1);
        }
    }
}

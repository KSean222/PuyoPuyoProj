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
            content.Unload();
        }
        public Rectangle PuyoSrcRect(Puyo puyo, CardinalDir connections) {
            int x = (int)connections;
            int y = (int)puyo - 1;
            if (puyo == Puyo.NUISANCE) {
                throw new NotImplementedException();
            }
            return new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
        }
        public Rectangle PuyoGhostSrcRect(Puyo puyo) {
            int x = 16;
            int y = (int)puyo - 1;
            return new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
        }
    }
}

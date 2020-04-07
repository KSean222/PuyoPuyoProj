using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PuyoPuyoProj.Core;
using PuyoPuyoProj.Core.Interface;
using PuyoPuyoProj.Core.PuyoPuyo;
using System;

namespace PuyoPuyoProj
{
    public class PuyoPuyoProjGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public PuyoPuyoProjGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize() {
            Vars.resources = new Resources();
            Vars.mainActionSource = new GameActionSource();
            base.Initialize();
        }
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Vars.resources.Load(Content);
            Vars.mainBoard = new Board(new PuyoQueue(0));
        }
        protected override void UnloadContent() {
            Vars.resources.Unload(Content);
        }
        protected override void Update(GameTime gameTime) {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();
            switch (Vars.state) {
                
            }
            Vars.mainBoard.Update((float)gameTime.ElapsedGameTime.TotalSeconds, Vars.mainActionSource);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            switch (Vars.state) {
                
            }
            Vars.mainBoard.Draw(spriteBatch, new Rectangle(0, 0, 6 * 30, 13 * 30));
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoPuyoProj.Core.Interface
{
    public class GameActionSource
    {
        public static GameAction[] actions;
        private readonly Keys[] map;
        private ActionState[] prevState;
        static GameActionSource() {
            actions = (GameAction[])Enum.GetValues(typeof(GameAction));
        }
        public GameActionSource() {
            map = new Keys[actions.Length];
            prevState = new ActionState[actions.Length];
            Bind(Keys.Left, GameAction.SHIFT_LEFT);
            Bind(Keys.Right, GameAction.SHIFT_RIGHT);
            Bind(Keys.Z, GameAction.ROTATE_PUYO_LEFT);
            Bind(Keys.X, GameAction.ROTATE_PUYO_RIGHT);
        }
        public ActionState[] FetchState() {
            KeyboardState keyboardState = Keyboard.GetState();
            ActionState[] state = new ActionState[actions.Length];
            for (int i = 0; i < actions.Length; i++) {
                if (keyboardState.IsKeyDown(map[i])) {
                    state[i] |= ActionState.PRESSED;
                    if (!prevState[i].HasFlag(ActionState.PRESSED)) {
                        state[i] |= ActionState.JUST_PRESSED;
                    }
                } else {
                    if (prevState[i].HasFlag(ActionState.PRESSED)) {
                        state[i] |= ActionState.JUST_RELEASED;
                    }
                }
            }
            prevState = state;
            return state;
        }
        public void Bind(Keys key, GameAction action) {
            map[(int)action] = key;
        }
    }
    public enum GameAction
    {
        SHIFT_LEFT,
        SHIFT_RIGHT,
        ROTATE_PUYO_LEFT,
        ROTATE_PUYO_RIGHT
    }
    public enum ActionState
    {
        NONE = 0,
        JUST_RELEASED = 1,
        PRESSED = 2,
        JUST_PRESSED = 4
    }
}

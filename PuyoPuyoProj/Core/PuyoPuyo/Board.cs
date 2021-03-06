﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PuyoPuyoProj.Core.Interface;
using System;
using System.Collections.Generic;

namespace PuyoPuyoProj.Core.PuyoPuyo
{
    public class Board
    {
        public int width;
        public int height;
        private readonly List<PuyoState>[] field;
        public (Puyo main, Puyo side) current;
        public int pairX;
        public int pairY;
        public CardinalDir pairRot;
        public BoardState state;
        private float fallOffset;
        private float rotOffsetAngle;
        private float shiftOffset;
        private float lockTimer;
        private float dasTimer;
        private bool dasIsLeft;
        private const float ROT_DELAY = 0.5f;
        private const float LOCK_DELAY = 0.5f;
        private const float DAS = 0.5f;
        private const float ARR = 0.1f;
        private const float LOCK_ANIM_DELAY = 0.5f;
        private readonly PuyoQueue queue;
        private List<DropAnimation> dropAnims;
        public Board(PuyoQueue queue, int width = 6, int height = 13) {
            this.width = width;
            this.height = height;
            field = new List<PuyoState>[width];
            for (int i = 0; i < width; i++) {
                field[i] = new List<PuyoState>(height);
            }
            this.queue = queue;
            dropAnims = new List<DropAnimation>();
            NextPuyo();
            field[0].Add(new PuyoState(Puyo.RED));
            field[1].Add(new PuyoState(Puyo.RED));
            field[0].Add(new PuyoState(Puyo.RED));
            field[0].Add(new PuyoState(Puyo.RED));
            field[0].Add(new PuyoState(Puyo.GREEN));
            field[2].Add(new PuyoState(Puyo.GREEN));
            field[2].Add(new PuyoState(Puyo.GREEN));
            field[3].Add(new PuyoState(Puyo.GREEN));
            field[4].Add(new PuyoState(Puyo.GREEN));
            field[1].Add(new PuyoState(Puyo.BLUE));
            field[1].Add(new PuyoState(Puyo.BLUE));
            field[2].Add(new PuyoState(Puyo.BLUE));
            field[3].Add(new PuyoState(Puyo.BLUE));
            field[5].Add(new PuyoState(Puyo.YELLOW));
            field[4].Add(new PuyoState(Puyo.YELLOW));
            field[4].Add(new PuyoState(Puyo.YELLOW));
            Reflow();
        }
        private void NextPuyo() {
            state = BoardState.FALLING;
            current = queue.Take();
            pairRot = CardinalDir.NORTH;
            pairX = 2;
            pairY = height;
        }
        private bool TouchingStack() {
            pairY -= 1;
            bool touching = IsInWall();
            pairY += 1;
            return touching;
        }
        private bool IsInWall() {
            (int offsetX, int offsetY) = pairRot.Offset();
            return IsSolid(pairX, pairY) || IsSolid(pairX + offsetX, pairY + offsetY);
        }
        private bool IsSolid(int x, int y) {
            return x < 0 || x >= width || y < field[x].Count;
        }
        public void Update(float delta, GameActionSource controller) {
            ActionState[] controllerState = controller.FetchState();
            ActionState leftState = controllerState[(int)GameAction.SHIFT_LEFT];
            ActionState rightState = controllerState[(int)GameAction.SHIFT_RIGHT];
            if (leftState.HasFlag(ActionState.PRESSED) != rightState.HasFlag(ActionState.PRESSED)) {
                bool Shift(bool isLeft) {
                    int initX = pairX;
                    pairX += isLeft ? -1 : 1;
                    if (IsInWall()) {
                        pairX = initX;
                        return false;
                    }
                    shiftOffset += isLeft ? 1 : -1;
                    return true;
                }
                bool isLeft = leftState.HasFlag(ActionState.PRESSED);
                if (isLeft != dasIsLeft) {
                    dasIsLeft = isLeft;
                    dasTimer = 0;
                }
                if (dasTimer == 0) {
                    Shift(isLeft);
                }
                dasTimer += delta;
                if (dasTimer >= DAS) {
                    if (shiftOffset == 0) {
                        Shift(isLeft);
                    }
                }
            } else {
                dasTimer = 0;
            }
            void Falling() {
                bool rotLeftJustPressed = controllerState[(int)GameAction.ROTATE_PUYO_LEFT].HasFlag(ActionState.JUST_PRESSED);
                bool rotRightJustPressed = controllerState[(int)GameAction.ROTATE_PUYO_RIGHT].HasFlag(ActionState.JUST_PRESSED);
                if (rotLeftJustPressed != rotRightJustPressed) {
                    float initAngle = rotOffsetAngle;
                    CardinalDir initRot = pairRot;
                    rotOffsetAngle = (float)Math.PI / 2f;
                    if (rotLeftJustPressed) {
                        pairRot = pairRot.RotateLeft();
                        rotOffsetAngle *= -1;
                    } else {
                        pairRot = pairRot.RotateRight();
                    }
                    if (IsInWall()) {
                        (int kickX, int kickY) = pairRot.Offset();
                        pairX -= kickX;
                        pairY -= kickY;
                        if (IsInWall()) {
                            pairX += kickX;
                            pairY += kickY;
                            rotOffsetAngle = initAngle;
                            pairRot = initRot;
                            //TODO handle flipping special case
                        }
                    }
                }
                if (!TouchingStack()) {
                    fallOffset += delta;
                    while (fallOffset > 1) {
                        fallOffset -= 1;
                        if (!TouchingStack()) {
                            pairY -= 1;
                        }
                    }
                }
                if (TouchingStack()) {
                    fallOffset = 0;
                    lockTimer += delta;
                    if (lockTimer > LOCK_DELAY) {
                        lockTimer = 0;
                        rotOffsetAngle = 0;
                        state = BoardState.LOCK_ANIMATION;
                    }
                }
                if (rotOffsetAngle != 0) {
                    bool neg = rotOffsetAngle < 0;
                    rotOffsetAngle += (neg ? delta : -delta) * (float)Math.PI / ROT_DELAY;
                    if (neg != rotOffsetAngle < 0) {
                        rotOffsetAngle = 0;
                    }
                }
                if (shiftOffset != 0) {
                    bool neg = shiftOffset < 0;
                    shiftOffset += (neg ? delta : -delta) / ARR;
                    if (neg != shiftOffset < 0) {
                        shiftOffset = 0;
                    }
                }
            }
            void Locking() {
                lockTimer += delta;
                if (lockTimer > LOCK_ANIM_DELAY) {
                    lockTimer = 0;
                    if (pairRot == CardinalDir.NORTH) {
                        field[pairX].Add(new PuyoState(current.main));
                    }
                    field[pairX + pairRot.Offset().x].Add(new PuyoState(current.side));
                    if (pairRot != CardinalDir.NORTH){
                        field[pairX].Add(new PuyoState(current.main));
                    }
                    dropAnims = dropAnims.FindAll(anim => anim.Update(delta));
                    Reflow();
                    NextPuyo();
                }
            }
            switch (state) {
                case BoardState.FALLING:
                    Falling();
                    break;
                case BoardState.LOCK_ANIMATION:
                    Locking();
                    break;
                case BoardState.POPPING:
                    break;
            }
        }
        
        public void Draw(SpriteBatch spriteBatch, Rectangle dest) {
            int tileWidth = dest.Width / width;
            int tileHeight = dest.Height / height;
            void DrawPuyo(float x, float y, Rectangle src, float scaleX = 1, float scaleY = 1) {
                int rectX = (int)((x + (1 - scaleX) / 2) * tileWidth);
                int rectY = (int)((height - y - 1 + (1 - scaleY) / 2) * tileHeight);
                int rectW = (int)(tileWidth * scaleX);
                int rectH = (int)(tileHeight * scaleY);
                spriteBatch.Draw(
                    Vars.resources.puyoSkin,
                    new Rectangle(rectX, rectY, rectW, rectH),
                    src,
                    Color.White
                );
            }
            //Draw board
            for (int x = 0; x < width; x++) {
                List<PuyoState> column = field[x];
                for (int y = 0; y < column.Count; y++) {
                    PuyoState state = column[y];
                    DrawPuyo(x, y, Vars.resources.PuyoSrcRect(state.puyo, state.connections));
                }
            }
            //Draw ghost
            (int x, int y) cardOffset = pairRot.Offset();
            DrawPuyo(pairX, field[pairX].Count + (pairRot == CardinalDir.SOUTH ? 1 : 0), Vars.resources.PuyoGhostSrcRect(current.main));
            DrawPuyo(pairX + cardOffset.x, field[pairX + cardOffset.x].Count + (pairRot == CardinalDir.NORTH ? 1 : 0), Vars.resources.PuyoGhostSrcRect(current.side));

            //Draw pair
            Vector2 rotOffset = new Vector2(cardOffset.x, cardOffset.y);
            rotOffset = Vector2.Transform(rotOffset, Matrix.CreateRotationZ(rotOffsetAngle));
            DrawPuyo(pairX + shiftOffset, pairY - fallOffset, Vars.resources.PuyoSrcRect(current.main, 0));
            DrawPuyo(pairX + shiftOffset + rotOffset.X, pairY + rotOffset.Y - fallOffset, Vars.resources.PuyoSrcRect(current.side, 0));
        }
        private void Reflow() {
            for (int x = 0; x < width; x++) {
                List<PuyoState> column = field[x];
                for (int y = 0; y < column.Count; y++) {
                    column[y].check = true;
                }
            }
            for (int x = 0; x < width; x++) {
                List<PuyoState> column = field[x];
                for (int y = 0; y < column.Count; y++) {
                    if (field[x][y].check) {
                        int blobSize = 0;
                        List<(int, int)> toBond = new List<(int, int)> {
                            (x, y)
                        };
                        for (int i = 0; i < toBond.Count; i++) {
                            (int x, int y) pos = toBond[i];
                            PuyoState state = field[pos.x][pos.y];
                            state.check = false;
                            state.connections = 0;
                            blobSize += 1;
                            foreach (CardinalDir dir in CardinalDirExt.cardinalDirs) {
                                (int x, int y) offset = dir.Offset();
                                (int x, int y) check = (pos.x + offset.x, pos.y + offset.y);
                                if (PuyoAt(check.x, check.y) == state.puyo) {
                                    if (field[check.x][check.y].check) {
                                        toBond.Add(check);
                                    }
                                    state.connections |= dir;
                                }
                            }
                        }
                        foreach ((int puyoX, int puyoY) in toBond) {
                            field[puyoX][puyoY].blobSize = blobSize;
                        }
                    }
                }
            }
        }
        private Puyo PuyoAt(int x, int y) {
            if (x < 0 || x >= width || y < 0) {
                return Puyo.SOLID;
            }
            List<PuyoState> column = field[x];
            return y < column.Count
                ? column[y].puyo
                : Puyo.EMPTY;
        }
    }
    public enum BoardState
    {
        FALLING,
        LOCK_ANIMATION,
        POPPING
    }
    public class PuyoState
    {
        public CardinalDir connections;
        public Puyo puyo;
        public bool check;
        public int blobSize;
        public PuyoState(Puyo puyo) {
            this.puyo = puyo;
        }
    }
    public enum Puyo
    {
        EMPTY,
        RED,
        GREEN,
        BLUE,
        YELLOW,
        PURPLE,
        NUISANCE,
        SOLID
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoPuyoProj.Core.PuyoPuyo
{
    public class PuyoQueue
    {
        private readonly List<Puyo> queue;
        private readonly Queue<Puyo> history;
        private readonly Random rng;
        private readonly Puyo[] puyos;
        public PuyoQueue(int seed) {
            rng = new Random(seed);//Ignores sign bit so it's really a 31 bit seed :P
            puyos = new Puyo[] {
                Puyo.RED,
                Puyo.GREEN,
                Puyo.BLUE,
                Puyo.YELLOW,
                Puyo.PURPLE
            };
            int n = puyos.Length;
            while (n > 1) {
                int k = rng.Next(n--);
                Puyo temp = puyos[n];
                puyos[n] = puyos[k];
                puyos[k] = temp;
            }
            queue = new List<Puyo>(4);
            history = new Queue<Puyo>();
            for (int i = 0; i < 4; i++) {
                Enqueue(RandPuyo(3));
            }
        }
        private Puyo RandPuyo(int colors) {
            return puyos[rng.Next(colors)];
        }
        private void Enqueue(Puyo puyo) {
            queue.Add(puyo);
            if (history.Count == 2) {
                history.Dequeue();
            }
            history.Enqueue(puyo);
        }
        public (Puyo, Puyo) Take() {
            (Puyo, Puyo) pair = (queue[0], queue[1]);
            queue.RemoveRange(0, 2);
            for (int i = 0; i < 2; i++) {
                Puyo puyo = RandPuyo(4);
                foreach (Puyo prev in history) {
                    if (prev == puyo) {
                        puyo = RandPuyo(4);
                        break;
                    }
                }
                Enqueue(puyo);
            }
            return pair;
        }
        public ((Puyo, Puyo), (Puyo, Puyo)) Preview() {
            return ((queue[0], queue[1]), (queue[2], queue[3]));
        }
    }
}

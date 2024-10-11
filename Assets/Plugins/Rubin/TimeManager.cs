using System;
using Rubin.LetterBattle.Utility;
using UnityEngine;

namespace Rubin
{
    ///Assumes you dont modify timescale directly
    public static class TimeManager
    {
        private static readonly QueueValue<float> queue = new QueueValue<float>(1, (x, y) =>
        {
            if (x == 1) return y;
            if (y == 1) return x;

            return Math.Min(x, y);
        });

        static TimeManager()
        {
            queue.OnValueChanged += OnValueChanged;
        }

        private static void OnValueChanged(QueueValue<float> obj)
        {
            Time.timeScale = obj.Value;
        }

        public static void Register(object x,float v)
        {
            queue.Register(x,v);
        }
        public static bool Unregister(object x,float v)
        {
            return queue.Unregister(x);
        }
    }
}
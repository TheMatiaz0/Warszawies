using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Rubin
{

    public class Randomer
    {
        public int? Seed { get; }
        private readonly Random random;
        public static readonly Randomer Base = new Randomer();

        public Randomer(int seed)
        {
            Seed = seed;
            random = new Random(seed);
        }

        public Randomer()
        {
            Seed = null;
            random = new Random();
        }

        public int NextInt(int min = int.MinValue, int max = int.MaxValue)
        {
            return random.Next(min, max);
        }

        public double NextDouble(double min = double.MinValue, double max = double.MaxValue)
        {
            return random.NextDouble() * (max - min) + min;

        }

        public float NextFloat(float min = float.MinValue, float max = float.MaxValue)
        {
            return (float) (NextDouble(min, max));
        }

        public Vector2 NextVector2(Vector2 min, Vector2 max)
        {
            float x = NextFloat(min.x, max.x);
            float y = NextFloat(min.y, max.y);
            return new Vector2(x, y);
        }

        public Vector2 GetRandomDirection()
        {
            float radAngle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            return new(Mathf.Cos(radAngle), MathF.Sin(radAngle));
        }

        public T NextRandomElement<T>(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            var ar = (collection as IList<T>) ?? collection.ToArray();
            if (ar.Count == 0)
            {
                throw new ArgumentException("collection was empty");
            }

            return ar[NextInt(0, ar.Count)];

        }

        public void ShuffleInPlace<T>(T[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int r = this.NextInt(i + 1, array.Length);
                (array[i], array[r]) = (array[r], array[i]);
            }
        }

        public T NextRandomElement<T>(IList<T> collection)
        {
            return collection[NextInt(0, collection.Count)];
        }
    }
}
#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Rubin
{
    public static class RHelper
    {

        public static Vector3 WithZ(this Vector2 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector2 GetRotation(this Vector2 vec, float angle)
        {
            float rad = Mathf.Deg2Rad * angle;
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);
            float x = vec.x;
            float y = vec.y;
            float newX = x * cos - y * sin;
            float newY = x * sin + y * cos;
            return new Vector2(newX, newY);
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static bool Flag<T>(this T val, T other)
            where T : Enum
        {
            return val.HasFlag(other);
        }
        
        
        public static float CapMaxAbs(float val, float max)
        {
            return Mathf.Sign(val) * Mathf.Min(Mathf.Abs(val), (max));
        }
     
        public static Vector2 Mx(this Vector2 vec, float x)
        {
            return new Vector2(x, vec.y);
        }
   
        public static Vector3 Mx(this Vector3 vec, float x)
        {
            return new Vector3(x, vec.y, vec.y);
        }
   
        public static Vector2 My(this Vector2 vec, float y)
        {
            return new Vector2(vec.x, y);
        }
   
        public static Vector3 My(this Vector3 vec, float y)
        {
            return new Vector3(vec.x, y, vec.y);
        }

        ///% doesnt work like mathematical modul for negative values 
        public static int Wrap(int value, int size)
        {
            if (value < 0)
            {
                return size - ((-value) % size);
            }
            else return value % size;
        }
    
        ///% doesnt work like mathematical modul for negative values 
        public static float Wrap(float value, float size)
        {
            if (value < 0)
            {
                return size - ((-value) % size);
            }
            else return value % size;
        }
      

        public static float Atan3InDegrees(this Vector2 v)
        {
            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }
        
        public static T2 GetOrAdd<T1, T2>(this IDictionary<T1, T2> dictioniary, T1 key, T2 ifEmpty)
        {
            if (dictioniary is null)
            {
                throw new ArgumentNullException(nameof(dictioniary));
            }
            if (dictioniary.TryGetValue(key, out T2 val))
            {
                return val;
            }
            else
            {
                dictioniary[key] = ifEmpty;
                return ifEmpty;
            }

        }
        public static Dictionary<TKey, IGrouping<TKey, TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groups)
        {
            return groups.ToDictionary(item => item.Key, item => item);
        } 
        
         /// <summary>
        /// Generate more readable version for dictionaries and collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="maxDeepLv"></param>
        /// <returns></returns>
        public static string ToDebugString<T>(this T obj, int maxDeepLv = 10)
        {
            return ToDebugString(obj, 0, maxDeepLv);
        }
        private static string ToDebugString<T>(this T obj, uint deep, int maxDeeplv)
        {

            string basic = obj?.ToString() ?? ">null<";
            if (deep > maxDeeplv)
            {
                return basic;
            }
            uint next = deep + 1;
            if ((basic == (obj?.GetType().ToString() ?? string.Empty)))
            {

                StringBuilder elementBullider = new StringBuilder();
                bool first = true;
                string SaveToDebugString(object val)
                {
                    if (System.Object.ReferenceEquals(val, obj))
                    {
                        return "this";
                    }
                    else
                        return ToDebugString(val, next, maxDeeplv);
                }
                void CheckFirst()
                {
                    if (first == false)
                    {
                        elementBullider.Append(", ");

                    }

                    first = false;
                }
                switch (obj)
                {
                    case IDictionary dict:

                        foreach (var key in dict.Keys)
                        {
                            CheckFirst();
                            elementBullider.Append($"{{{SaveToDebugString(key)}}}={{{SaveToDebugString(dict[key])}}}");
                        }
                        return $"{{ {elementBullider} }}";
                    case IEnumerable collection:


                        foreach (var element in collection)
                        {
                            CheckFirst();
                            elementBullider.Append(SaveToDebugString(element));
                        }
                        return $"{{ {elementBullider} }}";
                }
            }
            return basic;
        }
        /// <summary>
        /// Converts the value from basic range to a value from needed range.
        /// </summary>
        /// <param name="basic">Basic range.</param>
        /// <param name="needed">Needed range.</param>
        /// <param name="value">Value from basic range.</param>
        /// <returns></returns>
        public static float ReCalculateRange((float min, float max) basic, (float min, float max) needed, float value)
        {
            return (value - basic.min) / (basic.max - basic.min) * (needed.max - needed.min) + needed.min;
        }

        public static void Foreach<T>(IEnumerable<T> enu, Action<T> a)
        {
            foreach (var el in enu)
            {
                a(el);
            }
        }
        public static void Foreach<T>(IEnumerable<T> enu, Action<T,int> a)
        {
            int index = 0;
            foreach (var el in enu)
            {
                a(el,index);
                index++;
            }
        }
        
    }
}
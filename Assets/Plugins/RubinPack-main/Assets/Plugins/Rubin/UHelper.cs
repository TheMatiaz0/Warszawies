#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rubin
{
    public static class UHelper
    {
        
        public static IEnumerator CMoveTo(Transform ob,Vector2 to,float time)
        {
          
            if (time <= 0)
            {
                throw new ArgumentException("time has to be >0");
            }
            float startTime = Time.time;
            Vector2 startPos = ob.localPosition;
            Vector2 dif = to - startPos;
            while (true)
            {
                float passed =Mathf.Clamp(  Time.time - startTime,0,time);

                ob.localPosition = startPos+ dif * (passed / time);
                if (passed >= time)
                {
                    break;
                }
                yield return null;
            }
        }
        public static bool IfHasComponent<T>(this GameObject obj, Action<T> action)
            where T : UnityEngine.Object
        {
            T comp = obj.GetComponent<T>();
            if (comp != null) action(comp);
            return comp != null;
        }
        public static void SetYVelocity(this Rigidbody2D rigi, float val)
        {
            rigi.velocity = rigi.velocity.My(val);

        }

        public static void AddXVelocity(this Rigidbody2D rigi, float val)
        {
            rigi.SetXVelocity(rigi.velocity.x + val);
        }

        public static void AddYVelocity(this Rigidbody2D rigi, float val)
        {
            rigi.SetYVelocity(rigi.velocity.y + val);
        }
   
        public static void SetXVelocity(this Rigidbody2D rigi, float val)
        {
            rigi.velocity = rigi.velocity.Mx(val);
        } 
        
        public static void SetRawFlip(this Transform transform, float val)
        {
            var scale = transform.localScale;
            scale = new Vector3(Mathf.Abs(scale.x) * Mathf.Sign(val), scale.y,
                scale.z);
            transform.localScale = scale;
        }
         
        public static T GetOrAdd<T>(this GameObject gameObject, Action<T>? ifAdd = null)
            where T : Component
        {
            T comp = gameObject.GetComponent<T>();
            if (comp == null)
            {
                comp = gameObject.AddComponent<T>();
                ifAdd?.Invoke(comp);
            }
            return comp;
        } 
        
        public static void AddEvent(this EventTrigger trigger, EventTriggerType type, Action action)
        {
            EventTrigger.TriggerEvent ev = new ();
            ev.AddListener(_=>action());
            trigger.triggers.Add( new EventTrigger.Entry(){ callback = ev, eventID= type});
        } 
       
        public static Vector2 Get2Pos(this Transform tr)
        {
            return (Vector2) tr.position; ;
        }
        public static Vector2 Get2LocalScale(this Transform transform)
        {
            return (Vector2)transform.localScale;
        }
    

        public static void SetAllGraphicsAlpha(this GameObject gameObject, float value)
        {
            foreach (var component in gameObject.GetComponentsInChildren<Graphic>())
            {
                component.color = new Clr(component.color, value);
            }
        }
        public static IEnumerable<Transform> GetChildren(this Transform transform)
        {
            return transform.OfType<Transform>();
        }
        


    }
}
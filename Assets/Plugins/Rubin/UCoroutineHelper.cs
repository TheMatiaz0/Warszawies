using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Rubin
{
    public static class UCoroutineHelper
    {
        public static object NextFrame { get; } = null;
        public static WaitForFixedUpdate FixedUpdate { get; } = new WaitForFixedUpdate();
        public static WaitForEndOfFrame EndOfFrame { get; } = new WaitForEndOfFrame();
        public static WaitForSeconds Wait(TimeSpan time)
            => new WaitForSeconds((float)time.TotalSeconds);
        public static WaitUntil Wait(Task task)
        {
            return new WaitUntil(()=>task.IsCompleted);
        }
        public static WaitForSeconds Wait(float seconds) => new WaitForSeconds(seconds);

        public static WaitForSecondsRealtime WaitUnscaled(float seconds)
            => new WaitForSecondsRealtime(seconds);
        public static WaitForSecondsRealtime WaitUnscaled(TimeSpan time)
            => new WaitForSecondsRealtime((float)time.TotalSeconds);
        
        public static object Wait(float seconds, bool realTime)
            => (realTime) ? (object)WaitUnscaled(seconds) : (object)Wait(seconds);
        
        
        public static WaitUntil Until(Func<bool> predicate)
            => new WaitUntil(predicate);
        public static WaitWhile While(Func<bool> predicate) 
            => new WaitWhile(predicate);
    }}
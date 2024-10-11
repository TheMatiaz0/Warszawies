namespace Rubin
{
 using System;
using System.Collections.Generic;
using System.Linq;
namespace LetterBattle.Utility
{
    public class QueueBoolBuilder
    {
        public static QueueValue<bool> Build()
        {
            return new QueueValue<bool>(false, (a, b) => a || b);
        }
    }
    
    
    /// <summary>
    /// Slow Priority Queue with seperate representation and value. It also has events. 
    /// Made for situation, such that when new objects "enters" it register its value and when it "exits" it removes it.
    /// We constantly peek at the max value, which is the strongest of ones that are registered (alive).
    /// <br/><br/>
    /// For example:, a queuevalue for timescale (freezing purpose) is used instead of directly editing Time.timeScale. 
    /// When any object requests 0 time scale its zero untill no object wants it.
    /// <br/><br/>
    /// 
    /// <br/>
    /// Deciding function should follow given properties <br/>
    /// -  F(a,b) = a or b <br/>
    /// -  Associativity/Commutativity <br/>
    /// - F(a,identity) == a && F(identity,a) <br/>
    /// <br/>
    /// examples of such functions: <br/>
    /// Max (numbers) <br/>
    /// Min (numbers) <br/>
    /// Logic any (bools)  --- for this kind you can use use QueueBoolBuilder.Build() <br/>
    /// logic any false (bools) <br/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueValue<T>
    {
        public delegate T DeciderCallback(T a, T b);
        public readonly T Identity;
        
        public T Value => cacheValue;
        public int RegisteredAmount => registered.Count;
        public DeciderCallback DeciderFunction { get; }
        
        private Dictionary<object, T> registered = new Dictionary<object, T>();
        public event Action<QueueValue<T>> OnValueChanged = delegate { };
        
        private T cacheValue;
        
        public QueueValue(T identity,  DeciderCallback decider)
        {
            this.Identity = identity;
            this.DeciderFunction = decider ?? throw new ArgumentNullException(nameof(decider));
            this.cacheValue = identity;

        }
        public void Register(object obj,T value)
        {
            if (value.Equals(Identity)|| registered.ContainsKey(obj))
            {
                Unregister(obj);
            }
            if (value.Equals(Identity)) return;
            
            registered[obj] = value;
            T newValue = DeciderFunction(cacheValue, value);// the cache value has the most priority, if the new value beats, it beats everything
            TryChangeValue(newValue);
        }
        public bool Unregister(object obj)
        {
            if (!registered.ContainsKey(obj))
                return false;
            T value = registered[obj];
            registered.Remove(obj);
            if (value.Equals(cacheValue))//only case in which everything should be calculated from the beginning
            {
                T nwValue= InternalGetValue();
                TryChangeValue(nwValue);
            }
            return true;
            // if the cache value is different than value that means that the "cache value" has more priority over "value", thus removing value
            //will not change anything
        }
        
        //calculate the value from beginning
        private T InternalGetValue()
        {
            return registered.Aggregate(Identity, (current, element) => DeciderFunction(current, element.Value));
        }
       
        private void TryChangeValue(T value)
        {

            if (!value.Equals(cacheValue))
            {
                cacheValue = value;
                OnValueChanged(this);
            }
        }
      


    }
}
}
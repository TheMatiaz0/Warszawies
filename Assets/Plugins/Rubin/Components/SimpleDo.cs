using System;
using System.Collections;
using System.Collections.Generic;
using Honey;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Rubin.Components
{
    public class SimpleDo : MonoBehaviour
    {
        public interface ISimpleDoCommandLogic
        {
            void DoAwake(SimpleDo g,UnityEvent ev){}
            void DoStart(SimpleDo g,UnityEvent ev){}
        }
        
        [Serializable]

        public class WaitOnce : ISimpleDoCommandLogic
        {
            [FormerlySerializedAs("time")] [HoneyRun][HMin(0)]
            public float delay;
            public void DoAwake(SimpleDo g,UnityEvent ev)
            {
                g.StartCoroutine(C(ev));
            }

            private IEnumerator C(UnityEvent ev)
            {
                yield return new WaitForSeconds(delay);
                ev.Invoke();

            }
        }

        [Serializable]
        public enum SignalKind
        {
            Awake,
            Start,
            Update,
            OnDestroy,
            
        }
        public class OnSignal : ISimpleDoCommandLogic
        {
             public SignalKind Signal;
            public void DoAwake(SimpleDo g, UnityEvent ev)
            {
                if (Signal == SignalKind.Awake)
                    ev.Invoke();
            }

            public void DoStart(SimpleDo g, UnityEvent ev)
            {
                switch (Signal)
                {
                    case SignalKind.Start:
                        ev.Invoke();
                        break;
                    
                    case SignalKind.Update:
                        IEnumerator CUpdate()
                        {
                            while (true)
                            {
                                yield return null;
                                ev.Invoke();
                            }
                        }

                        g.StartCoroutine(CUpdate());
                        break;
                    
                    case SignalKind.OnDestroy:
                        g.destroyCancellationToken.Register(ev.Invoke);
                        break;
                }
            }
        }

        [Serializable]
        public struct SimpleDoCommand
        {
             [SerializeReference][SerializeReferenceHelper]
            public ISimpleDoCommandLogic Logic;
             public UnityEvent Ev;
        }

        [SerializeField] private SimpleDoCommand command;

        private void Awake()
        {
            if (command.Logic == null)
                return;
            try
            {
                command.Logic.DoAwake(this, command.Ev);
            }
            catch 
            {
                Destroy(this.gameObject);
                throw;
            }
        }
        private void Start()
        {
            if (command.Logic != null)
                command.Logic.DoStart(this, command.Ev);
        }
    }
}
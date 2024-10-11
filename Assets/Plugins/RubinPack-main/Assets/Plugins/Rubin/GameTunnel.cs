using System;
using UnityEngine;

namespace Rubin
{
    [CreateAssetMenu(menuName = "Tunnel")]
    public class GameTunnel : ScriptableObject
    {
        public event Action OnGameFinish = delegate { };

        public void CallOnGameFinisha()
        {
            OnGameFinish();
        }
        
        
    }
}
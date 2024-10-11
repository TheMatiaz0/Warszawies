using UnityEngine;

namespace Rubin.Components
{
    public class UnityEventMProvider : MonoBehaviour
    {
        public void Print(string message)
        {
            Debug.Log(message);
        }

        public void DoDestroy(UnityEngine.Object g)
        {
            UnityEngine.Object.Destroy(g);
        }
        
    }
}
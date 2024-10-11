using System.Collections.Generic;
using UnityEditor;

namespace Rubin.Editor 
{
    public class EditorStacksHelper
    {
        public static Stack<int> dumpedIndentStack = new Stack<int>();

        public static void DumpIndent()
        {
            dumpedIndentStack.Push(EditorGUI.indentLevel);
            EditorGUI.indentLevel = 0;
        }

        public static void RestoreIndent()
        {
            EditorGUI.indentLevel=dumpedIndentStack.Pop();
            
        }
        
    }
}
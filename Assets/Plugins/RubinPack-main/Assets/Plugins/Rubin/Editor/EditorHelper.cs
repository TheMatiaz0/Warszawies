#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Rubin.Editor
{
    public class EditorHelper
    {
        
        public class TypeSearchProvider : ScriptableObject, ISearchWindowProvider
        {
            public IEnumerable<Type> Types { get; set; } = Enumerable.Empty<Type>();
            public Action<Type>? Action { get; set; }
            public bool ShowNamespaces { get; set; }
            public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
            {
                List<SearchTreeEntry> list = new();
                list.Add(new SearchTreeGroupEntry(new GUIContent("Types")));
                SearchTreeEntry nullEntry = new SearchTreeEntry(new GUIContent("null"));
                nullEntry.level = 1;
                list.Add(nullEntry);
                nullEntry.userData = null;
                foreach (var type in Types)
                {
                    string text=(ShowNamespaces )?$"{type.Name} from ({type.Namespace})": type.Name;
                    var entry = new SearchTreeEntry(new GUIContent(text))
                    {
                        level = 1,
                        userData = type,
                    };
                    list.Add(entry );
                }
                return list;
            }

            public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
            {
                Action?.Invoke((searchTreeEntry.userData as Type)!);
                return true;
            }
        } 
    }
}
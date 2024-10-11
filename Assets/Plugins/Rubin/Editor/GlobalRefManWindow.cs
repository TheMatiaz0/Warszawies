#nullable enable
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Codice.CM.Common.Matcher;
using Rubin.Editor;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}


namespace Rubin.Editor{
    public class GlobalRefManWindow : EditorWindow
    {

        //todo: filter for field should be able to handle any component incuding stuff like camera
        //probably needs a type picker for that
        //todo: handle recusive objects
    
        public record GameObjectNode(List<GameObjectNode> Children, List<MonoRepr> MonoReprs, GameObject? Itself);
        public record MonoRepr(SerializedObject SerializedObject,  List<SerializedProperty> Props, MonoBehaviour Mono);

        private static Lazy<GUIStyle> richStyle = new Lazy<GUIStyle>(() =>
        {
            return new GUIStyle(GUI.skin.label) {richText = true};
        });

        private UnityEngine.Object? bulkObject;   
        private GameObjectNode cache = new GameObjectNode(new  List<GameObjectNode>(),new(), null);
        private readonly HashSet<(SerializedProperty, MonoBehaviour)> toggled = new HashSet<(SerializedProperty, MonoBehaviour)>();



        private IGlobalManBaseFilter[] filters = new[]
        {
            new GlobalManFilter<Type>(new GlobalManFilterFieldRule(), new GlobalManFilterMonoScriptDrawer("By Field Type")) as IGlobalManBaseFilter,
            new GlobalManFilter<Type>(new GlobalManFilterMonoRule(), new GlobalManFilterMonoScriptDrawer("By Script type")) as IGlobalManBaseFilter,
        };
    
    
    
        [MenuItem("Rubin/GlobalRefMan")]
        public static void Open()
        {
            GlobalRefManWindow wnd = GetWindow<GlobalRefManWindow>();
            wnd.titleContent = new GUIContent("Global Ref Man");
        }
    
        private  IEnumerable<MonoRepr> SearchForFieldInScene()
        {

            MonoBehaviour[] scripts = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var script in scripts)
            {

                FieldInfo[] fields = script.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(item => item.GetCustomAttributes<GloballyManageable>().Any()).ToArray();

                if (fields.Length == 0)
                {
                    continue;
                }
                var serialized= new SerializedObject(script);
                var properties = new List<SerializedProperty>();
                foreach (var field in fields)
                {
                    properties.Add( serialized.FindProperty(field.Name));
                }

                yield return new MonoRepr(serialized,properties,script);
            }
        
        
        }

        private GameObjectNode PackFieldsInContext(GameObject? context, Dictionary<MonoBehaviour,MonoRepr> dict)
        {
            MonoRepr[] reprs = new MonoRepr[] { };

            if (context != null)
            {
                MonoBehaviour[] monos = context.gameObject.GetComponents<MonoBehaviour>();
                reprs= monos.Select(item => dict!.GetValueOrDefault(item, null)).Where(item=>item!=null).Select(item=>(MonoRepr)item!).ToArray(); 

            }

            GameObject[] children;

            if (context == null)
                children = FindObjectsOfType<GameObject>().Where(item => item.transform.parent == null).ToArray();
            else
                children = context.transform.Cast<Transform>().Select(item=>item.gameObject).ToArray();


            var childrenNodes=children.Select(item => PackFieldsInContext(item,dict)).ToArray();

            return new GameObjectNode(childrenNodes.ToList(), reprs.ToList(), context);

        }

        private void RemoveUp(GameObjectNode node, GameObjectNode? parent)
        {

            if (parent == null)
            {
                return;
            }
        
            int index=parent.Children.Select((a, i) => (a, i)).Where(item => item.a == node).Select(tuple => tuple.i)
                .First();
            parent.Children.RemoveAt(index);
        }
    
        private void CutDeadParts(GameObjectNode node, GameObjectNode? parent, ICollection<IGlobalManBaseFilter> localFilters)
        {

            foreach (GameObjectNode child in node.Children.ToArray())
            {
                CutDeadParts(child, node,localFilters);
            }

            foreach (var monoRepr in node.MonoReprs.ToArray())
            {
                foreach (SerializedProperty prop in monoRepr.Props.ToArray())
                {
                    if (localFilters.Any(filter => !filter.Check(prop, monoRepr.Mono)))
                    {
                        monoRepr.Props.Remove(prop);
                    }
                }

                if (monoRepr.Props.Count == 0)
                {
                    node.MonoReprs.Remove(monoRepr);
                }
            }
        
            if ((node.MonoReprs.Count == 0 && node.Children.Count == 0)
               )
            {
                RemoveUp(node,parent);
            }
        
        }
        private  GameObjectNode BuildTree()
        {

            var dict=SearchForFieldInScene().ToDictionary(item=>item.Mono, item=>item);
            var root = PackFieldsInContext(null, dict);
            CutDeadParts(root,null,filters);
            return root;
        }


        private void DisposeTree(GameObjectNode node)
        {
            foreach (var child in node.Children)
            {
                DisposeTree(child);
            }
            foreach (var repr in node.MonoReprs)
            {
                repr.SerializedObject.Dispose();
            }
        }
    
        private void RefreshTree()
        {
            DisposeTree(cache);
            cache=BuildTree();

        }

        private void DumpIndent()
        {
           
        }


        private void DrawTree(GameObjectNode node, int indent)
        {
            if (node.Itself != null)
            {

                EditorGUILayout.LabelField($"<b>{node.Itself.name}</b>",richStyle.Value);
                ;
            }
        
            EditorGUI.indentLevel++;
            foreach (var repr in node.MonoReprs)
            {

                if (repr.Props.Count==0)
                {
                    return;
                }

            
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField($"||{repr.Mono.GetType().Name}");
            
            
                EditorGUI.indentLevel++;
                foreach (SerializedProperty prop in repr.Props)
                {

                
                    Type type = repr.Mono.GetType().GetField(prop.name,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.FieldType;


                    EditorGUILayout.BeginHorizontal();
                    EditorStacksHelper.DumpIndent();
                
                    var toggled = this.toggled.Contains((prop,repr.Mono));
                    var colorString = (toggled) ? "<color=red>" : "";
                    var colorStringEnd = (toggled) ? "</color>" : "";
                    if (GUILayout.Button($" {colorString}<i>{type.Name}</i> {prop.name} {colorStringEnd}",richStyle.Value))
                    {
                        if( !toggled)
                        {
                            this.toggled.Add((prop,repr.Mono));
                        }
                        else
                        {
                            this.toggled.Remove((prop,repr.Mono));
                        }
                    }
                
                
                    EditorGUILayout.PropertyField(prop, new GUIContent(""));

                    EditorStacksHelper.RestoreIndent();
                    EditorGUILayout.EndHorizontal();
                }
                
                if (EditorGUI.EndChangeCheck())
                {
                    repr.SerializedObject.ApplyModifiedProperties();

                }

                EditorGUI.indentLevel--;


            }

            foreach (var child in node.Children)
            {
                DrawTree(child,indent );
            }
        
            EditorGUI.indentLevel--;
        
        }

        private void OnEnable()
        {
            foreach (IGlobalManBaseFilter baseFilter in filters)
            {
                baseFilter.OnValueChanged += () => RefreshTree();
            }

            RefreshTree();
        }
    
        private void DrawBulkMenu()
        {
            EditorGUILayout.BeginHorizontal();
            bulkObject = EditorGUILayout.ObjectField(bulkObject, typeof(UnityEngine.Object),true);
        
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                bulkObject = null;
            }
            if (GUILayout.Button("Apply"))
            {
                foreach (var (item,mono) in toggled)
                {
                    item.objectReferenceValue = bulkObject;
                    item.serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUILayout.EndHorizontal();
        
        }

        private void DrawFilterMenu()
        {

            foreach (var filter in filters)
            {
                EditorGUILayout.BeginHorizontal();
                bool prev = filter.Enabled;
                filter.Enabled= EditorGUILayout.Toggle(filter.Enabled, GUILayout.Width(20));
                if (filter.Enabled != prev)
                {
                    RefreshTree();
                }
                filter.Draw();
                EditorGUILayout.EndHorizontal();
            }

        }

        private void OnGUI()
        {

            DrawBulkMenu();
            DrawFilterMenu();
            if (GUILayout.Button("refresh",GUILayout.Width(100)))
            {
                toggled.Clear();
                RefreshTree();;
            }
            DrawTree(cache,0);
        
        
        }
    }
#endif
}
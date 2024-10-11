#nullable  enable
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;


//todo: filter change should reset toggles
//either handle checkbox outside or response to an event
namespace Rubin.Editor
{

    using System;
    using UnityEditor;
    using UnityEngine;

    internal interface IGlobalManBaseFilter
    {
        public event Action OnValueChanged;
        public bool Enabled { get; set; }
        bool Check(SerializedProperty prop, MonoBehaviour mono);
        void Draw();
    }

    [Serializable]
    internal class GlobalManFilter<T> : IGlobalManBaseFilter
        where T : class
    {

        private readonly IGlobalManFilterRule<T> rule;
        private readonly IGlobalManFilterDrawer<T> drawer;

        public event Action OnValueChanged = delegate { };

        private T? _Value;
        public T? Value
        {
            get => _Value;
            set
            {
                if (value == _Value)
                {
                    return;
                }

                _Value = value;
                OnValueChanged();
            }
        }

        public bool Enabled { get; set; }

        public GlobalManFilter(IGlobalManFilterRule<T> rule, IGlobalManFilterDrawer<T> drawer)
        {
            this.rule = rule;
            this.drawer = drawer;
        }

        public bool Check(SerializedProperty prop, MonoBehaviour mono)
        {
            T? v = this.Value;
            var result = !Enabled || rule.Filter(prop, mono, ref v);
            this.Value = v;
            return result;


        }

        public void Draw()
        {
            drawer.Draw(this);
        }
    }

    internal interface IGlobalManFilterRule<T>
    where T : class
    {
        bool Filter(SerializedProperty prop, MonoBehaviour mono, ref T? value);
    }

    internal interface IGlobalManFilterDrawer<T>
    where T : class
    {
        void Draw(GlobalManFilter<T> filter);
    }



    internal class MonoScripHelper
    {
        private static readonly Dictionary<Type, MonoScript> monoScriptBank = new();

        //i couldn't find a way to grab them all at the start
        //i never need Type -> MonoScript before MonoScript -> Type anyway
        public static void Register(MonoScript sc)
        {
            monoScriptBank[sc.GetClass()] = sc;
        }
        public static MonoScript Get(Type tp)
        {
            return monoScriptBank[tp];
        }
    }

    internal class GlobalManFilterFieldRule : IGlobalManFilterRule<Type>
    {
        public bool Filter(SerializedProperty prop, MonoBehaviour mono, ref Type? value)
        {
            if (value == null)
                return true;
            Type fieldType = mono.GetType().GetField(prop.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.FieldType;
            return value.IsAssignableFrom(fieldType);

        }
    }
    internal class GlobalManFilterMonoRule : IGlobalManFilterRule<Type>
    {
        public bool Filter(SerializedProperty prop, MonoBehaviour mono, ref Type? value)
        {
            if (value == null)
            {
                return true;
            }
            return value.IsAssignableFrom(mono.GetType());

        }
    }


    internal class GlobalManFilterMonoScriptDrawer : IGlobalManFilterDrawer<Type>
    {

        public readonly string Name;
        private SearchProvider provider;


        public void Draw(GlobalManFilter<Type> filter)
        {
            MonoScript? monoScript = (filter.Value != null) ? MonoScripHelper.Get(filter.Value) : null;
            EditorGUILayout.LabelField(Name, GUILayout.Width(100));




            monoScript = (MonoScript)EditorGUILayout.ObjectField("", monoScript, typeof(MonoScript), false, GUILayout.MaxWidth(300));

            if (GUILayout.Button(new GUIContent("X"), GUILayout.Width(20)))
            {
                monoScript = null;
            }
            if (monoScript != null)
                MonoScripHelper.Register(monoScript);
            filter.Value = monoScript?.GetClass();
        }

        public GlobalManFilterMonoScriptDrawer(string name)
        {
            Name = name;
        }
    }
}

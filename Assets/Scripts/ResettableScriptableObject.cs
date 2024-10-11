#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public abstract class ResettableScriptableObject : ScriptableObject
{
#if UNITY_EDITOR
    private string _initialJson = string.Empty;
#endif

    private void OnEnable()
    {
        SaveAsset();
    }

    private void SaveAsset()
    {
#if UNITY_EDITOR
        if (Application.isPlaying) return;
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR

        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

#endif
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredPlayMode:
                _initialJson = EditorJsonUtility.ToJson(this);
                Debug.Log(_initialJson);
                break;

            case PlayModeStateChange.ExitingPlayMode:
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                EditorJsonUtility.FromJsonOverwrite(_initialJson, this);
                Debug.Log(_initialJson);
                break;
        }
    }
#endif
}

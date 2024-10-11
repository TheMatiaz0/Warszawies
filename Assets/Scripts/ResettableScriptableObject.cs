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

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.ExitingEditMode:
                if (Application.isPlaying) return;
                _initialJson = EditorJsonUtility.ToJson(this);
                break;

            case PlayModeStateChange.ExitingPlayMode:
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                EditorJsonUtility.FromJsonOverwrite(_initialJson, this);
                break;
        }
    }
#endif
}

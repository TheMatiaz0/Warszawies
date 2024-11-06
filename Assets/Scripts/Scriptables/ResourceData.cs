using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "CRPK/Resource", order = 1)]
public class ResourceData : ResettableScriptableObject
{
    public string Name;
    public Sprite Icon;
    public AudioClip OnZeroResourceClip;
}

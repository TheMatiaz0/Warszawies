using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "CRPK/Resource", order = 1)]
public class ResourceData : ScriptableObject
{
    public string name;
    public Sprite icon;
    public int count;
}

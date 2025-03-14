using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event", menuName = "CRPK/Event", order = 1)]
public class EventData : ScriptableObject
{
    public string Title;
    [TextArea]
    public string Description;

    public Sprite Illustration;
    public Sprite SmallThumbnail;

    public List<CountableResource> Requirement;

    public List<CountableResource> WinCondition;
    public List<CountableResource> FailCondition;

    public bool AbleToCancel = true;
    public bool AbleToAccept = true;
    public bool AbleToRestart = false;

    public AudioClip OnEventStartClip;
}


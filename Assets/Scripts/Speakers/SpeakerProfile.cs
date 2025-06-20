using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeakerProfile", menuName = "Dialogue/Speaker Profile")]
public class SpeakerProfile : ScriptableObject
{
    [Header("Identity")]
    public string speakerName;
    [TextArea]
    public string speakerBackStory;
    [TextArea]
    public string speakerPersonality;
    [TextArea]
    public string speakerAction;

    [Header("Color Settings")]
    public Color nameColor = Color.white; // Color for the name
    public Color textColor = Color.white; // Color for the message text
}
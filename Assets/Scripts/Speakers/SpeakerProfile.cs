using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeakerProfile", menuName = "Dialogue/Speaker Profile")]
public class SpeakerProfile : ScriptableObject
{
    [Header("Identity")]
    public string speakerName;
    public string speakerBackStory;
    public string speakerPersonality;
    public string speakerAcknowledge;

    [Header("Color Settings")]
    public Color nameColor = Color.white; // Color for the name
    public Color textColor = Color.white; // Color for the message text
}
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] SpeakerProfile playerProfile;
    public string DefaultFileName = "/chatLogs.json";

    private void Update()
    {
        MessageManager.Instance.RecieveMessageFromInputField(playerProfile);
    }
}

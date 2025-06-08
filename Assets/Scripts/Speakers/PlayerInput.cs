using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] SpeakerProfile playerProfile;


    private void Update()
    {
        MessageManager.Instance.RecieveMessageFromInputField(playerProfile);
    }
}

using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class MessageSaveData
{
    public List<SimpleMessage> messages;
}

[System.Serializable]
public class SimpleMessage
{
    public string speakerName;
    public string text;
}

public class SaveMessageToJson : MonoBehaviour
{

    public string DefaultFileName = "/chatLogs.json";
    public void SaveMessage()
    {
        MessageManager.Instance.SaveMessagesToJson(Application.persistentDataPath + DefaultFileName);
    }
    
}


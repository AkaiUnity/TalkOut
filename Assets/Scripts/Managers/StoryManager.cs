using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    [TextArea][SerializeField] string firstDay;
    [TextArea][SerializeField] string playerBacklog;
    [TextArea][SerializeField] string residents;
    [SerializeField] int currentDay;
    [SerializeField] List<DailyLog> logList = new();

    // 🏞️ PUBLIC GETTERS (to access from other scripts)
    public string FirstDay => firstDay;
    public string PlayerBacklog => playerBacklog;
    public string Residents => residents;
    public int CurrentDay => currentDay;
    public List<DailyLog> LogList => logList;
}

// Keep the DailyLog class as is
[System.Serializable]
public class DailyLog
{
    public string Day;
    public string Context;
}

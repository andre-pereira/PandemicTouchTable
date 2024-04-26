using System;
using System.IO;
using UnityEngine;

public abstract class EventLogger
{
    public string GetEventLog(TimelineEvent timelineEvent)
    {
        string commonLog = $@"
                    ""timestamp"" : ""{Time.time - MainMenu.startTimestamp}"",
                    ""eventType"" : ""{timelineEvent.GetType()}"",
                    {(Game.theGame.CurrentPlayer != null ? (
                        $@"""currentPlayer"" : {{
                            ""role"" : ""{Game.theGame.CurrentPlayer.Role}"",
                            ""name"" : ""{Game.theGame.CurrentPlayer.Name}""
                        }},") : null )} 
                ";
        
        
        string eventLog = timelineEvent.GetLogInfo();
        
        return 
            $@"{{ 
                    {commonLog}
                    {eventLog} 
            }},";
    }

    public abstract void BroadcastLogs(TimelineEvent timelineEvent);
    
}

public class FileLogger : EventLogger
{
    private static string fileName = $"log_{DateTime.Now.ToString("ddMMyyyy_HHmmss")}.txt";
    private string filePath;

    public FileLogger()
    {
        string logsFolderPath = Path.Combine(Application.dataPath, "Logs");

        if (!Directory.Exists(logsFolderPath))
        {
            Directory.CreateDirectory(logsFolderPath);
        }
        filePath = Path.Combine(logsFolderPath, fileName);
    }

    public override void BroadcastLogs(TimelineEvent timelineEvent)
    {
        string logs = GetEventLog(timelineEvent);
        try
        {
            
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(logs);
            }
            
        }
        catch (Exception ex)
        {
            Debug.LogError($"LOG: Error writing to log file: {ex.Message}");
        }
        //Debug.Log(Time.time + " - Logging event : " + timelineEvent.GetType() + " Current player : " + Game.theGame.CurrentPlayer.Role);
    }
}

public class NetworkLogger : EventLogger
{
    public override void BroadcastLogs(TimelineEvent timelineEvent)
    {
        //TODO: to be implemented
    }
}


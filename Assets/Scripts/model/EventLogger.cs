using System;
using System.IO;
using UnityEngine;

public abstract class EventLogger
{
    public string GetEventLog(TimelineEvent timelineEvent)
    {
        string currentPlayerLog = Game.theGame.CurrentPlayer != null
            ? $@"                   ""currentPlayer"" : {{
                            ""role"" : ""{Game.theGame.CurrentPlayer.Role}"",
                            ""name"" : ""{Game.theGame.CurrentPlayer.Name}""
                        }}"
            : null;
        
        string commonLog = 
            $@"""timestamp"" : ""{Time.time - MainMenu.startTimestamp}"",
                    ""eventType"" : ""{timelineEvent.GetType()}""{(currentPlayerLog != null ? $", \n {currentPlayerLog}" : "")}";
        
        
        string eventLog = timelineEvent.GetLogInfo();
        
        return
            $@"{{
                    {commonLog}{(eventLog != null ? $",\n\t\t\t\t   {eventLog}" : "")}
            }},";
    }

    public abstract void BroadcastLogs(TimelineEvent timelineEvent);
    
}

public class FileLogger : EventLogger
{
    private static string fileName = $"log_{DateTime.Now.ToString("ddMMyyyy_HHmmss")}.txt";
    private static string filePath = "";
    
    public FileLogger()
    {
        if (filePath == "")
        {
            string logsFolderPath;

            if (Application.isEditor) // Dev mode
                logsFolderPath = Path.Combine(Application.dataPath, "Logs");
            else // Prod (build) mode
                logsFolderPath = Path.Combine(Application.persistentDataPath, "Logs");
        
            if (!Directory.Exists(logsFolderPath))
            {
                Directory.CreateDirectory(logsFolderPath);
            }
            filePath = Path.Combine(logsFolderPath, fileName);
        }
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


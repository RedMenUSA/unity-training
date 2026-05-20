using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole Instance;

    private bool showConsole;
    private string input = "";
    private Vector2 scroll;

    private Dictionary<string, Action<string[]>> commands;

    private List<string> logs = new List<string>();

    void Awake()
    {
        Instance = this;
        commands = new Dictionary<string, Action<string[]>>();

        RegisterCommands();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote)) // ` key
        {
            showConsole = !showConsole;
        }
    }

    void OnGUI()
    {
        if (!showConsole) return;

        float y = 10;

        GUI.Box(new Rect(10, y, Screen.width - 20, Screen.height / 2), "Debug Console");

        GUILayout.BeginArea(new Rect(20, y + 30, Screen.width - 40, Screen.height / 2 - 40));

        scroll = GUILayout.BeginScrollView(scroll);

        foreach (var log in logs)
            GUILayout.Label(log);

        GUILayout.EndScrollView();

        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(20, Screen.height / 2 + 10, Screen.width - 40, 40));

        GUI.SetNextControlName("ConsoleInput");
        input = GUILayout.TextField(input);

        GUI.FocusControl("ConsoleInput");

        if (Event.current.keyCode == KeyCode.Return)
        {
            ExecuteCommand(input);
            input = "";
        }

        GUILayout.EndArea();
    }

    void RegisterCommands()
    {
        // HELP
        commands["help"] = args =>
        {
            Log("Available commands:");
            foreach (var cmd in commands.Keys)
                Log("- " + cmd);
        };

        // LOAD SCENE
        commands["load"] = args =>
        {
            if (args.Length > 0)
            {
                SceneManager.LoadScene(args[0]);
            }
        };

        // TIME SCALE
        commands["timescale"] = args =>
        {
            if (args.Length > 0 && float.TryParse(args[0], out float t))
            {
                Time.timeScale = t;
                Log("TimeScale set to " + t);
            }
        };

        // CLEAR LOG
        commands["clear"] = args =>
        {
            logs.Clear();
        };
    }

    void ExecuteCommand(string commandLine)
    {
        Log("> " + commandLine);

        string[] parts = commandLine.Split(' ');
        string cmd = parts[0].ToLower();

        string[] args = new string[Math.Max(0, parts.Length - 1)];
        Array.Copy(parts, 1, args, 0, args.Length);

        if (commands.TryGetValue(cmd, out var action))
        {
            action.Invoke(args);
        }
        else
        {
            Log("Unknown command: " + cmd);
        }
    }

    public void Log(string message)
    {
        logs.Add(message);
    }
}
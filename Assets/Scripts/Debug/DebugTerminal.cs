using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class DebugTerminal : MonoBehaviour
{
    // TODO: Does the terminal be globably accessible? If yes, make it accessible through the GameManager or implement a proper Singleton pattern.
    public static DebugTerminal Instance;

    [Header("Console")]
    public KeyCode toggleKey = KeyCode.BackQuote;
    public int maxLogs = 100;

    private bool isOpen;
    private string input = "";

    private Vector2 scrollPos;

    private readonly List<string> logs = new();
    private readonly List<string> history = new();

    private int historyIndex = -1;

    private readonly Dictionary<string, Action<string[]>> commands = new();

    private GUIStyle logStyle;
    private GUIStyle inputStyle;
    private GUIStyle boxStyle;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        RegisterCommands();

        Application.logMessageReceived += HandleUnityLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleUnityLog;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOpen = !isOpen;

            if (isOpen)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        if (!isOpen)
            return;

        // Command history navigation
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (history.Count > 0)
            {
                historyIndex--;
                historyIndex = Mathf.Clamp(historyIndex, 0, history.Count - 1);
                input = history[historyIndex];
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (history.Count > 0)
            {
                historyIndex++;
                historyIndex = Mathf.Clamp(historyIndex, 0, history.Count - 1);
                input = history[historyIndex];
            }
        }
    }

    private void InitializeStyles()
    {
        if (logStyle != null)
            return;

        logStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            richText = true
        };

        inputStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = 18
        };

        boxStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = 18
        };
    }

    private void OnGUI()
    {
        InitializeStyles();

        if (!isOpen)
            return;

        var height = Screen.height * 0.5f;

        GUI.Box(
            new Rect(0, 0, Screen.width, height),
            "",
            boxStyle
        );

        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, height - 20));

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        foreach (var log in logs) 
            GUILayout.Label(log, logStyle);
        
        GUILayout.EndScrollView();

        GUI.SetNextControlName("ConsoleInput");

        input = GUILayout.TextField(input, inputStyle);

        GUI.FocusControl("ConsoleInput");

        var e = Event.current;

        if (e.isKey && e.keyCode == KeyCode.Return) 
            SubmitCommand();
        
        GUILayout.EndArea();
    }
    
    private void SubmitCommand()
    {
        if (string.IsNullOrWhiteSpace(input))
            return;

        Log($"> {input}");

        // TODO: Encapsulate this logic into a method that handles the recording of all submitted inputs.
        history.Add(input);
        historyIndex = history.Count;

        ExecuteCommand(input);

        input = "";
    }

    private void ExecuteCommand(string commandLine)
    {
        var split = commandLine.Split(' ');
        var command = split[0].ToLower();
        var args = new string[Math.Max(0, split.Length - 1)];

        Array.Copy(split, 1, args, 0, args.Length);

        if (commands.TryGetValue(command, out var action))
        {
            action.Invoke(args);
        }
        else
        {
            Log($"<color=red>Unknown command:</color> {command}");
        }
    }

    private void RegisterCommands()
    {
        commands["help"] = args =>
        {
            Log("<color=yellow>Commands:</color>");

            foreach (var cmd in commands.Keys)
            {
                Log("- " + cmd);
            }
        };

        commands["clear"] = args =>
        {
            logs.Clear();
        };

        commands["timescale"] = args =>
        {
            if (args.Length == 0)
            {
                Log("Usage: timescale 1");
                return;
            }

            if (float.TryParse(args[0], out float value))
            {
                Time.timeScale = value;
                Log("TimeScale set to " + value);
            }
        };

        commands["load"] = args =>
        {
            if (args.Length == 0)
            {
                Log("Usage: load SceneName");
                return;
            }

            SceneManager.LoadScene(args[0]);
        };

        commands["reload"] = args =>
        {
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.name);
        };

        commands["quit"] = args =>
        {
            Log("Quitting game...");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        };

        commands["fps"] = args =>
        {
            float fps = 1f / Time.unscaledDeltaTime;
            Log($"FPS: {Mathf.RoundToInt(fps)}");
        };
    }

    private void HandleUnityLog(string condition, string stackTrace, LogType type)
    {
        var color = "white";

        switch (type)
        {
            case LogType.Warning:
                color = "yellow";
                break;
            case LogType.Error:
            case LogType.Exception:
                color = "red";
                break;
        }

        Log($"<color={color}>{condition}</color>");
    }

    private void Log(string message)
    {
        logs.Add(message);

        if (logs.Count > maxLogs)
        {
            logs.RemoveAt(0);
        }

        scrollPos.y = Mathf.Infinity;
    }
}
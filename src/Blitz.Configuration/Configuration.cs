﻿using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using Blitz.Goto;
using Blitz.AvaloniaEdit.Models;

// ReSharper disable once CheckNamespace
namespace Blitz;

public class Configuration
{
    private static Configuration? _instance;
    
    public GotoEditor GotoEditor { get; set; } = new GotoEditor();

    public bool FromFile { get; set; }
    
    private static string _getStoreFile(string fileName)
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var specificFolder = Path.Combine(folder, "NathanSilvers");
        Directory.CreateDirectory(specificFolder);
        return Path.Combine(specificFolder,fileName);
    }

    public string GetStoreFile(string filename) => _getStoreFile(filename);

    [JsonIgnore]
    public static Configuration Instance => _instance ??= LoadInstanceOrDefault();

    public static readonly Configuration DefaultSetting = new Configuration();
    public WindowState WindowState { get; set; }

    public double BlitzPosX { get; set; } = 40;
    public double BlitzPosY { get; set; } = 40;
    public double BlitzPosWidth { get; set; } = 1600;
    public double BlitzPosHeight { get; set; } = 1000;
    
    public bool IsWelcomed { get; set; } 

    public string SelectedThemePremium { get; set; } = "DarkPlus";
    public bool SelectedThemeIsDark { get; set; } = true;
    
    public List<string> LiteralSearchTextHistory { get; set; } = [];
    public List<string> RegexSearchTextHistory { get; set; } = [];
    public List<string> SearchTextHistory { get; set; } = [];
    public List<string> SearchFileNameTextHistory { get; set; } = [];
    public List<string> PathFolderHistory { get; set; } = [];
    public List<string> ReplaceHistory { get; set; } = [];
    public List<string> ReplaceWithHistory { get; set; } = [];

    public BlitzEditorConfig EditorConfig { get; set; } = new BlitzEditorConfig();
    public List<ScopeConfig> ScopeConfigs { get; set; } = [];
    
    [JsonIgnore]
    public BlitzTheme CurrentTheme { get; set; } = BlitzTheme.Dark;

    public bool ShowOnTaskBar { get; set; } = true;
    public bool SplitPane { get; set; } = true;

    private const string ConfigurationFileName = "BlitzConfig.Json";

    private static Configuration LoadInstanceOrDefault()
    {
        try
        {
            var fileName = _getStoreFile(ConfigurationFileName);
            if (File.Exists(fileName))
            {
                var configFromFile = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(fileName),
                    JsonContext.Default.Configuration);
                if (configFromFile != null)
                {
                    configFromFile.FromFile = true;
                    return configFromFile;
                }
            }
        }
        catch (Exception e)
        {
            //Todo: expose to user via message.
            Console.WriteLine(e);
        }
        return new Configuration();
    }

    public RobotDetectionSettings RobotDetectionSettings { get; set; } = new();
    

    [DefaultValue(false)]
    public bool IsLiteralCaseSensitive { get; set; } = false;
    
    [DefaultValue(false)]
    public bool IsReplaceCaseSensitive { get; set; } = false;
    
    [DefaultValue(false)]
    public bool IsRegexCaseSensitive { get; set; } = false;
    
    [DefaultValue(true)]
    public bool CaseInSensitive { get; set; } = true;
    
    [DefaultValue(false)]
    public bool CaseSmart { get; set; } = false;

    [DefaultValue(true)]
    public bool ShowStatusBar { get; set; } = false;


    [DefaultValue("Words Query")]
    public string ReplaceMode { get; set; } = "Words Query";



    public bool ShowTotalSearchTime { get; set; } = false;
    public string SelectedScope { get; set; }


    [JsonIgnore] public bool _cleanSlateShutdown = false;

    public void CleanSlateShutdown()
    {
        var fileName = _getStoreFile(ConfigurationFileName);
        if (File.Exists(fileName))
        {
            System.IO.File.Delete(fileName);
        }
        _cleanSlateShutdown = true;
    }
    
    public void SaveConfig()
    {
        if (_cleanSlateShutdown)
        {
            return;
        }
        try
        {
            var fileName = _getStoreFile(ConfigurationFileName);
            var fileContents =  JsonSerializer.Serialize(this, JsonContext.Default.Configuration);
            File.WriteAllText(fileName, fileContents);
        }
        catch (Exception e)
        {
            //Todo Message box this:
            Console.WriteLine(e);
        }
    }
}
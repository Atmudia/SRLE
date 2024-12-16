using System;
using System.IO;
using System.Text.Json;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace SRLE.Components;

[RegisterTypeInIl2Cpp]
public class SettingsUI : MonoBehaviour
{
    private GameObject m_GameObject;
    private Text m_RenderText;
    private Text m_HighlightText;

    public static SettingsUI Instance;

    [Serializable]
    public class Settings
    {
        public bool EnableFog = false;
        public int RenderDistance = 1000;
        public byte HighlightStrength = 50;
        public ObjectHighlight.HighlightType HighlightMethod;
    }

    private void Start()
    {
        Instance = this;
        m_GameObject = transform.Find("Settings").gameObject;

        Button closeButton = GetSettingsButton("Close");
        Toggle fogToggle = transform.Find("Settings/Panel/FogToggle").GetComponent<Toggle>();
        m_RenderText = transform.Find("Settings/Panel/RenderText").GetComponent<Text>();
        Slider renderSlider = transform.Find("Settings/Panel/RenderSlider").GetComponent<Slider>();
        Dropdown hightlightDropdown = transform.Find("Settings/Panel/HighlightDropdown").GetComponent<Dropdown>();
        m_HighlightText = transform.Find("Settings/Panel/HighlightText").GetComponent<Text>();
        Slider hightlightSlider = transform.Find("Settings/Panel/HighlightSlider").GetComponent<Slider>();

        closeButton.onClick.AddListener(new System.Action(OnClose));
        fogToggle.onValueChanged.AddListener(new System.Action<bool>(OnFogChanged));
        renderSlider.onValueChanged.AddListener(new System.Action<float>(OnRenderDistanceChanged));
        hightlightDropdown.onValueChanged.AddListener(new System.Action<int>(OnHighlightMethodChanged));
        hightlightSlider.onValueChanged.AddListener(new System.Action<float>(OnHightlightStrengthChanged));

        if(File.Exists(Path.Combine(SaveManager.DataPath, "settings.txt")))
        {
            SaveManager.Settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(Path.Combine(SaveManager.DataPath, "settings.txt")));
        }
        else
        {
            SaveManager.Settings = new Settings()
            {
                EnableFog = true,
                HighlightMethod = ObjectHighlight.HighlightType.Wireframe,
                HighlightStrength = 10,
                RenderDistance = 1000
            };

        }

        renderSlider.value = SaveManager.Settings.RenderDistance;
        OnRenderDistanceChanged(SaveManager.Settings.RenderDistance);
        fogToggle.isOn = SaveManager.Settings.EnableFog;
        OnFogChanged(SaveManager.Settings.EnableFog);
        hightlightSlider.value = SaveManager.Settings.HighlightStrength;
        OnHightlightStrengthChanged(SaveManager.Settings.HighlightStrength);

        OnClose();
    }

    private void OnHightlightStrengthChanged(float arg0)
    {
        m_HighlightText.text = $"Highlight Strength: {arg0}";
        SaveManager.Settings.HighlightStrength = (byte)arg0;
        
        SaveSettings();
    }

    private void OnHighlightMethodChanged(int arg0)
    {
        SaveManager.Settings.HighlightMethod = (ObjectHighlight.HighlightType)arg0;
        SaveSettings();

        foreach (var highlight in FindObjectsOfType<ObjectHighlight>())
        {
            highlight.Awake();
        }
    }

    private void OnRenderDistanceChanged(float arg0)
    {
        m_RenderText.text = $"Render Distance: {arg0} Meters";
        SaveManager.Settings.RenderDistance = (int)arg0;

        SaveSettings();
    }

    private void OnFogChanged(bool arg0)
    {
        RenderSettings.fog = arg0;
        SaveManager.Settings.EnableFog = arg0;
        SaveSettings();
    }

    private void OnClose()
    {
        m_GameObject.SetActive(false);
    }

    public void Open()
    {
        m_GameObject.SetActive(true);
    }

    private Button GetSettingsButton(string btnName)
    {
        return transform.Find($"Settings/Panel/{btnName}Button")?.GetComponent<Button>();
    }

    private static void SaveSettings()
    {
        File.WriteAllText(Path.Combine(SaveManager.DataPath, "settings.txt"), JsonSerializer.Serialize(SaveManager.Settings));
    }
}
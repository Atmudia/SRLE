using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Il2CppMonomiPark.SlimeRancher.UI;
using Newtonsoft.Json;
using SRLE;

public class SettingsUI : BaseUI
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

        if(File.Exists(Path.Combine(SRLESaveManager.DataPath, "settings.txt")))
        {
            SRLESaveManager.Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Path.Combine(SRLESaveManager.DataPath, "settings.txt")));
        }
        else
        {
            
        }

        renderSlider.value = SRLESaveManager.Settings.RenderDistance;
        OnRenderDistanceChanged(SRLESaveManager.Settings.RenderDistance);
        fogToggle.isOn = SRLESaveManager.Settings.EnableFog;
        OnFogChanged(SRLESaveManager.Settings.EnableFog);
        //hightlightDropdown.value = (int)SRLESaveManager.Settings.HighlightMethod;
        //OnHighlightMethodChanged((int)SRLESaveManager.Settings.HighlightMethod);
        hightlightSlider.value = SRLESaveManager.Settings.HighlightStrength;
        OnHightlightStrengthChanged(SRLESaveManager.Settings.HighlightStrength);

        OnClose();
    }

    private void OnHightlightStrengthChanged(float arg0)
    {
        m_HighlightText.text = $"Highlight Strength: {arg0}";
        SRLESaveManager.Settings.HighlightStrength = (byte)arg0;

        // Globals.HighlightMaterial.color = new Color(SRLESaveManager.Settings.HighlightMaterial.color.r, Globals.HighlightMaterial.color.g, Globals.HighlightMaterial.color.b, arg0 / 255f);
        // Globals.WireframeMaterial.color = new Color(SRLESaveManager.Settings.WireframeMaterial.color.r, Globals.WireframeMaterial.color.g, Globals.WireframeMaterial.color.b, arg0 / 255f);
        SaveSettings();
    }

    private void OnHighlightMethodChanged(int arg0)
    {
        //Globals.Settings.HighlightMethod = (ObjectHighlight.HighlightType)arg0;
        SaveSettings();

        // foreach (var highlight in FindObjectsOfType<ObjectHighlight>())
        // {
        //     highlight.Awake();
        // }
    }

    private void OnRenderDistanceChanged(float arg0)
    {
        m_RenderText.text = $"Render Distance: {arg0} Meters";
        SRLESaveManager.Settings.RenderDistance = (int)arg0;

        SaveSettings();
    }

    private void OnFogChanged(bool arg0)
    {
        RenderSettings.fog = arg0;
        SRLESaveManager.Settings.EnableFog = arg0;
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

    private Button GetSettingsButton(string name)
    {
        return transform.Find($"Settings/Panel/{name}Button")?.GetComponent<Button>();
    }

    private void SaveSettings()
    {
        File.WriteAllText(Path.Combine(SRLESaveManager.DataPath, "settings.txt"), JsonConvert.SerializeObject(SRLESaveManager.Settings));
    }
}

using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace SRLE.Components
{
    public class SettingsUI : MonoBehaviour
    {
        private GameObject m_GameObject;
        private Text m_RenderText;
        private Text m_HighlightText;

        public static SettingsUI Instance;

        [System.Serializable]
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
            Dropdown highlightDropdown = transform.Find("Settings/Panel/HighlightDropdown").GetComponent<Dropdown>();
            m_HighlightText = transform.Find("Settings/Panel/HighlightText").GetComponent<Text>();
            Slider highlightSlider = transform.Find("Settings/Panel/HighlightSlider").GetComponent<Slider>();

            // Set initial values before adding listeners so no callbacks fire during init
            renderSlider.value = SaveManager.Settings.RenderDistance;
            fogToggle.isOn = SaveManager.Settings.EnableFog;
            highlightSlider.value = SaveManager.Settings.HighlightStrength;
            highlightDropdown.value = (int)SaveManager.Settings.HighlightMethod;

            closeButton.onClick.AddListener(OnClose);
            fogToggle.onValueChanged.AddListener(OnFogChanged);
            renderSlider.onValueChanged.AddListener(OnRenderDistanceChanged);
            highlightDropdown.onValueChanged.AddListener(OnHighlightMethodChanged);
            highlightSlider.onValueChanged.AddListener(OnHighlightStrengthChanged);

            OnClose();
        }

        private void OnHighlightStrengthChanged(float value)
        {
            m_HighlightText.text = $"Highlight Strength: {value}";
            SaveManager.Settings.HighlightStrength = (byte)value;
            SaveSettings();
        }

        private void OnHighlightMethodChanged(int value)
        {
            SaveManager.Settings.HighlightMethod = (ObjectHighlight.HighlightType)value;
            SaveSettings();

            foreach (var highlight in FindObjectsOfType<ObjectHighlight>())
            {
                highlight.Awake();
            }
        }

        private void OnRenderDistanceChanged(float value)
        {
            m_RenderText.text = $"Render Distance: {value} Meters";
            EntryPoint.ConsoleInstance.Log("Render Distance: " + value);
            SaveManager.Settings.RenderDistance = (int)value;
            SaveSettings();
        }

        private void OnFogChanged(bool value)
        {
            RenderSettings.fog = value;
            SaveManager.Settings.EnableFog = value;
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
            File.WriteAllText(Path.Combine(SaveManager.DataPath, "settings.txt"), JsonConvert.SerializeObject(SaveManager.Settings));
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SRLE.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SRLE.Components.MainMenuUIs
{
    public class SRLENewLevelUI : BaseUI
    {
        public SRToggle SeaToggle;
        public SRToggle DesertToggle;
        public SRToggle VoidToggle;
        public SRToggle StandardToggle;

        public TMP_Text worldModeDescription;
        public InputField levelNameField;

        public GameObject gameIconPrefab;
        public ToggleGroup iconGroup;
        public Button leftIconButton;
        public Button rightIconButton;
        public TabByMenuKeys iconTabByMenuKeys;

        private Toggle[] iconToggles;
        private int selectedIconIndex = 0;
        private bool isSettingToggleStates;

        public WorldType SelectedWorldType { get; private set; }

        internal static List<Sprite> AllSprites { get; set; } = new List<Sprite>();

        private void Start()
        {
            LoadAvailableSprites();
            InitializeIconToggles();
            SetDefaultLevelName();
            SetupWorldToggleEvents();
            SetWorldType(WorldType.STANDARD);
        }

        #region Initialization

        private void LoadAvailableSprites()
        {
            iconToggles = new Toggle[AllSprites.Count];
        }

        private void InitializeIconToggles()
        {
            bool firstToggle = true;

            for (int i = 0; i < AllSprites.Count; i++)
            {
                GameObject iconObject = Instantiate(gameIconPrefab, iconGroup.transform, false);
                Toggle toggle = iconObject.GetComponent<Toggle>();
                iconObject.transform.Find("GameIcon").GetComponent<Image>().sprite = AllSprites[i];
                toggle.group = iconGroup;
                iconToggles[i] = toggle;

                int index = i;
                toggle.onValueChanged.AddListener(isOn =>
                {
                    if (!isOn || isSettingToggleStates)
                        return;
                    SetSelectedIcon(index);
                });

                OnSelectDelegator.Create(toggle.gameObject, () => toggle.isOn = true);

                if (firstToggle)
                {
                    firstToggle = false;
                    toggle.isOn = true;
                }
            }
        }

        private void SetDefaultLevelName()
        {
            string[] existingNames = Directory.GetFiles(SaveManager.LevelsPath, "*.srle")
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();

            for (int i = 1; i < 1000; i++)
            {
                string defaultName = uiBundle.Get("m.srle.default_level_name", i);
                if (!existingNames.Contains(defaultName))
                {
                    levelNameField.text = defaultName;
                    break;
                }
            }
        }

        private void SetupWorldToggleEvents()
        {
            SeaToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) SetWorldType(WorldType.SEA);
            });

            DesertToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) SetWorldType(WorldType.DESERT);
            });

            VoidToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) SetWorldType(WorldType.VOID);
            });

            StandardToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) SetWorldType(WorldType.STANDARD);
            });
        }

        #endregion

        #region World Type

        public void SetWorldType(WorldType worldType)
        {
            SelectedWorldType = worldType;
            string descKey = $"m.srle.desc.worldType.{worldType.ToString().ToLowerInvariant()}";
            worldModeDescription.text = uiBundle.Get(descKey);
        }

        #endregion

        #region Icon Navigation

        public void SelectNextIcon()
        {
            if (selectedIconIndex < AllSprites.Count - 1)
                SetSelectedIcon(selectedIconIndex + 1);
        }

        public void SelectPreviousIcon()
        {
            if (selectedIconIndex > 0)
                SetSelectedIcon(selectedIconIndex - 1);
        }

        private void SetSelectedIcon(int index)
        {
            selectedIconIndex = index;

            try
            {
                isSettingToggleStates = true;
                iconToggles[index].isOn = true;
                leftIconButton.interactable = index > 0;
                rightIconButton.interactable = index < AllSprites.Count - 1;
                iconTabByMenuKeys.RecalcSelected();
            }
            finally
            {
                isSettingToggleStates = false;
            }
        }

        #endregion

        // Uncomment this method when you're ready to enable level creation
        /*
        internal void CreateNewLevel(Button createButton)
        {
            string levelFileName = $"{levelNameField.text}.srle";
            string fullPath = Path.Combine(SRLEManager.Worlds.FullName, levelFileName);
            FileInfo fileInfo = new FileInfo(fullPath);

            if (fileInfo.Exists)
            {
                SRSingleton<GameContext>.Instance.UITemplates.CreateErrorDialog("e.srsle.level_name_exists");
                return;
            }

            var newLevel = SRLEName.Create(levelNameField.text, SelectedWorldType);
            newLevel.spriteType = selectedIconIndex;

            using var fileStream = fileInfo.Create();
            newLevel.Write(fileStream);

            SRLEManager.currentData = newLevel;
            SRLEManager.isSRLELevel = true;

            createButton.interactable = false;
            gameObject.SetActive(false);

            SRCallbacksUtils.AddSRCallbacksAndDeleteAfterLoading(_ =>
            {
                GameObject cam = new GameObject("SRLECamera", typeof(Camera));
                cam.AddComponent<SRLECamera>().controller = cam.AddComponent<RuntimeGizmos.TransformGizmo>();

                Instantiate(EntryPoint.srle.LoadAsset<GameObject>("CreatorUI"))
                    .AddComponent<SRLECreatorUI>();
            });

            SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame("", Identifiable.Id.HEN, PlayerState.GameMode.CASUAL, () =>
            {
                createButton.interactable = true;
                gameObject.SetActive(true);
            });
        }
        */
    }
}

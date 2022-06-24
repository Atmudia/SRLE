using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DebuggingMod.Extensions;
using SRLE.SaveSystem;
using SRML.SR;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Console = SRML.Console.Console;

namespace SRLE.Components
{
    public class SRLENewLevelUI : BaseUI
    {
        public SRToggle SeaToggle;
        public SRToggle DesertToggle;
        public SRToggle VoidToggle;
        public SRToggle StandardToggle;
        public TMP_Text worldModeDescription;
        public InputField levelNameField;

        public WorldType selWorldType;

        public Toggle[] toggles;


        public GameObject gameIconPrefab;
        public ToggleGroup iconGroup;
        
        
        public TabByMenuKeys iconTabByMenuKeys;
        public Button leftIconButton;
        public Button rightIconButton;
        private int selIconIdIdx = 0;
        private bool settingToggleStates;

        internal static List<Sprite> allSprites = new List<Sprite>();



        public void Start()
        {
            
            

            
            
            toggles = new Toggle[allSprites.Count];

            
            bool flag = true;
            for (var index = 0; index < allSprites.Count; index++)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.gameIconPrefab, this.iconGroup.transform, false);
                Toggle toggle = gameObject.GetComponent<Toggle>();
                gameObject.transform.Find("GameIcon").GetComponent<Image>().sprite = allSprites[index];
                toggle.group = this.iconGroup;
                this.toggles[index] = toggle;
                int idxToSet = index;
                toggle.onValueChanged.AddListener((UnityAction<bool>) (isOn =>
                {
                    if (!isOn || this.settingToggleStates)
                        return;
                    SetIconIdIdx(idxToSet);
                }));
                OnSelectDelegator.Create(toggle.gameObject, (UnityAction) (() =>
                {
                    toggle.isOn = true;
                }));
                if (flag)
                {
                    flag = false;
                    toggle.isOn = true;
                }
            }


            var fileInfos = SRLEManager.Worlds.GetFiles().ToList().Select(x => x.Name.Replace(".srle", String.Empty)).ToList();
            for (int i = 1; i < 1000; i++)
            {
                string str = uiBundle.Get("m.srle.default_level_name", (object) i);
                if (!fileInfos.Contains(str))
                {
                    levelNameField.text = str;
                    break;
                }

            }
            this.SeaToggle.onValueChanged.AddListener(isOn =>
            {
                if (!isOn)
                    return;
                this.SetWorldType(WorldType.SEA);
            });
            this.DesertToggle.onValueChanged.AddListener(isOn =>
            {
                if (!isOn)
                    return;
                this.SetWorldType(WorldType.DESERT);
            });
            this.VoidToggle.onValueChanged.AddListener(isOn =>
            {
                if (!isOn)
                    return;
                this.SetWorldType(WorldType.VOID);
            });
            this.StandardToggle.onValueChanged.AddListener(isOn =>
            {
                if (!isOn)
                    return;
                this.SetWorldType(WorldType.STANDARD);
            });
            SetWorldType(WorldType.STANDARD);

        }

        public void SetWorldType(WorldType worldType)
        {
            
            this.selWorldType = worldType;
            string key = $"m.srle.desc.worldType.{(object) worldType.ToString().ToLowerInvariant()}";
            worldModeDescription.text = uiBundle.Get(key);
        }
        public void SelectNextIcon() => this.SetIconIdIdx(Math.Min(allSprites.Count - 1, this.selIconIdIdx + 1));
        public void SelectPrevIcon() => this.SetIconIdIdx(Math.Max(0, this.selIconIdIdx - 1));


        private void SetIconIdIdx(int idx)
        {
            this.selIconIdIdx = idx;
            try
            {
                this.settingToggleStates = true;
                this.toggles[idx].isOn = true;
                this.leftIconButton.interactable = idx > 0;
                this.rightIconButton.interactable = idx < allSprites.Count - 1;
                this.iconTabByMenuKeys.RecalcSelected();
            }
            finally
            {
                this.settingToggleStates = false;
            }        
        }

        internal void CreateNewLevel(Button button)
        {
            var text = levelNameField.text + ".srle";
            var combine = Path.Combine(SRLEManager.Worlds.FullName, text);

            var fileInfo = new FileInfo(combine);
            
            
            if (fileInfo.Exists)
                SRSingleton<GameContext>.Instance.UITemplates.CreateErrorDialog("e.srsle.level_name_exists");
            else
            {
                
                var srleName = SRLEName.Create(levelNameField.text, selWorldType);
                srleName.spriteType = selIconIdIdx;
                using var fileStream = fileInfo.Open(FileMode.Create);
                srleName.Write(fileStream);
            
                SRLEManager.currentData = srleName;
                SRLEManager.isSRLELevel = true;
            
                button.interactable = false;
                this.gameObject.SetActive(false);

                SRLEUIMenu.playing = true;


                SRCallbacks.PreSaveGameLoad += context =>
                {
                    switch (selWorldType)
                    {
                        case WorldType.STANDARD:
                            return;
                        case WorldType.VOID:
                        {
                            FindObjectOfType<ZoneDirector>().gameObject.SetActive(false);
                            break;
                        }
                    }
                };
                SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame("", Identifiable.Id.HEN, PlayerState.GameMode.CASUAL,
                    () =>
                    {
                        button.interactable = true;
                        this.gameObject.SetActive(true);
                        SRLEUIMenu.playing = false;
                    });
            }
           
        }




    }
}
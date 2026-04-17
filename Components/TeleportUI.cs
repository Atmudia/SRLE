using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonomiPark.SlimeRancher.Regions;
using Newtonsoft.Json;
using SRLE.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SRLE.Components
{
    public class TeleportUI : MonoBehaviour
    {
        private GameObject m_GameObject;
        private List<TeleportModel> m_TeleportModels;
        private ScrollRect m_CategoryScroll;
        private InputField m_NameInput;

        public bool IsOpen => m_GameObject.activeSelf;

        public static TeleportUI Instance;

        private void Start()
        {
            Instance = this;
            m_GameObject = transform.Find("Teleports").gameObject;
            m_CategoryScroll = transform.Find("Teleports/CategoryScroll").GetComponent<ScrollRect>();
            m_NameInput = transform.Find("Teleports/NameInput").GetComponent<InputField>();
            var addButton = transform.Find("Teleports/AddButton").GetComponent<Button>();

            LoadTeleportModels();
            InitializeCategoryButtons();

            addButton.onClick.AddListener(OnAddTeleport);
            Close();
        }

        private void LoadTeleportModels()
        {
            string filePath = Path.Combine(SaveManager.DataPath, "tp.txt");

            if (File.Exists(filePath))
            {
                m_TeleportModels = JsonConvert.DeserializeObject<List<TeleportModel>>(File.ReadAllText(filePath));
                if (m_TeleportModels.Any(x => x.Position == null))
                {
                    InitializeDefaultTeleportModels();
                }
            }
            else
            {
                InitializeDefaultTeleportModels();
            }
        }

        private void InitializeDefaultTeleportModels()
        {
            m_TeleportModels = new List<TeleportModel>
            {
                new()
                {
                    Name = "Ranch Home",
                    Position = new Vector3(52.8f, 16.3f, 132.7f),
                    Rotation = new Vector3(0.0f, 102.6f, 0.0f),
                    RegionSet = RegionRegistry.RegionSetId.HOME
                },
                new()
                {
                    Name = "Desert Temple",
                    Position = new Vector3(119.9f, 1077.4f, 917.5f),
                    Rotation = new Vector3(0.0f, 0.0f, 0.0f),
                    RegionSet = RegionRegistry.RegionSetId.DESERT
                },
                new()
                {
                    Name = "Slimulations",
                    Position = new Vector3(1052.1f, 14.4f, 824.0f),
                    Rotation = new Vector3(0.0f, 315.0f, 0.0f),
                    RegionSet = RegionRegistry.RegionSetId.SLIMULATIONS
                },
                new()
                {
                    Name = "Nimble 1",
                    Position = new Vector3(-768.5f, 7.5f, -841.3f),
                    Rotation = new Vector3(0.0f, 44.9f, 0.0f),
                    RegionSet = RegionRegistry.RegionSetId.VALLEY
                },
                new()
                {
                    Name = "Nimble 2",
                    Position = new Vector3(-189.8f, 11.1f, -1008.4f),
                    Rotation = new Vector3(0.0f, 273.6f, 0.0f),
                    RegionSet = RegionRegistry.RegionSetId.VALLEY
                }
            };
            
            File.WriteAllText(Path.Combine(SaveManager.DataPath, "tp.txt"), JsonConvert.SerializeObject(m_TeleportModels, Formatting.Indented));
        }

        private void InitializeCategoryButtons()
        {
            foreach (var model in m_TeleportModels)
            {
                CreateCategoryButton(model);
            }
        }

        private void CreateCategoryButton(TeleportModel model)
        {
            GameObject categoryObj = Instantiate(AssetManager.CategoryButtonPrefab, m_CategoryScroll.content, false);
            string categoryName = model.Name;
            categoryObj.GetComponentInChildren<Button>().onClick.AddListener(new UnityAction(() =>  TeleportTo(model.RegionSet, (Vector3)model.Position, (Vector3)model.Rotation)));
            categoryObj.GetComponentInChildren<Text>().text = categoryName;
        }

        private void OnAddTeleport()
        {
            string name = string.IsNullOrEmpty(m_NameInput.text) ? SRSingleton<SceneContext>.Instance.RegionRegistry.GetCurrentRegionSetId() + " Teleport" : m_NameInput.text;

            var model = new TeleportModel
            {
                Name = name,
                Position = SRLECamera.Instance.transform.position,
                Rotation =  SRLECamera.Instance.transform.eulerAngles,
                RegionSet = SRSingleton<SceneContext>.Instance.RegionRegistry.GetCurrentRegionSetId()
            };

            CreateCategoryButton(model);

            m_NameInput.text = "";

            m_TeleportModels.Add(model);
            File.WriteAllText(Path.Combine(SaveManager.DataPath, "tp.txt"), JsonConvert.SerializeObject(m_TeleportModels, Formatting.Indented));
        }

        private static void TeleportTo(RegionRegistry.RegionSetId regionSetId, Vector3 position, Vector3 rotation)
        {
            SRLECamera.Instance.transform.position = position;
            SRLECamera.Instance.transform.eulerAngles = new Vector3(SRLECamera.Instance.transform.eulerAngles.x, rotation.y, SRLECamera.Instance.transform.eulerAngles.z);
            SRSingleton<SceneContext>.Instance.PlayerState.model.SetCurrRegionSet(regionSetId);
            
        }

        public void Open() => m_GameObject.SetActive(true);
        public void Close() => m_GameObject.SetActive(false);
    }
}

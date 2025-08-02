using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonomiPark.SlimeRancher.Regions;
using Newtonsoft.Json;
using SRLE.Models;
using TMPro;
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
                new TeleportModel
                {
                    Name = "Ranch Home",
                    PositionX = 52.8f,
                    PositionY = 16.3f,
                    PositionZ = -132.7f,
                    RotationX = 0.0f,
                    RotationY = 102.6f,
                    RotationZ = 0.0f,
                    RegionSet = RegionRegistry.RegionSetId.HOME
                },
                new TeleportModel
                {
                    Name = "Desert Temple",
                    PositionX = 119.9f,
                    PositionY = 1077.4f,
                    PositionZ = 917.5f,
                    RotationX = 0.0f,
                    RotationY = 0.0f,
                    RotationZ = 0.0f,
                    RegionSet = RegionRegistry.RegionSetId.DESERT
                },
                new TeleportModel
                {
                    Name = "Slimulations",
                    PositionX = 1052.1f,
                    PositionY = 14.4f,
                    PositionZ = 824.0f,
                    RotationX = 0.0f,
                    RotationY = 315.0f,
                    RotationZ = 0.0f,
                    RegionSet = RegionRegistry.RegionSetId.SLIMULATIONS
                },
                new TeleportModel
                {
                    Name = "Nimble 1",
                    PositionX = -768.5f,
                    PositionY = 7.5f,
                    PositionZ = -841.3f,
                    RotationX = 0.0f,
                    RotationY = 44.9f,
                    RotationZ = 0.0f,
                    RegionSet = RegionRegistry.RegionSetId.VALLEY
                },
                new TeleportModel
                {
                    Name = "Nimble 2",
                    PositionX = -189.8f,
                    PositionY = 11.1f,
                    PositionZ = -1008.4f,
                    RotationX = 0.0f,
                    RotationY = 273.6f,
                    RotationZ = 0.0f,
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
            categoryObj.GetComponentInChildren<Button>().onClick.AddListener(new UnityAction(() =>  TeleportTo(model.RegionSet, new Vector3(model.PositionX, model.PositionY, model.PositionZ), new Vector3(model.RotationX, model.RotationY, model.RotationZ))));
            categoryObj.GetComponentInChildren<Text>().text = categoryName;
        }

        private void OnAddTeleport()
        {
            string name = string.IsNullOrEmpty(m_NameInput.text) ? SRSingleton<SceneContext>.Instance.RegionRegistry.GetCurrentRegionSetId() + " Teleport" : m_NameInput.text;

            EntryPoint.ConsoleInstance.Log(SRLECamera.Instance.transform.position);
            var model = new TeleportModel
            {
                Name = name,
                PositionX = SRLECamera.Instance.transform.position.x,
                PositionY = SRLECamera.Instance.transform.position.y,
                PositionZ = SRLECamera.Instance.transform.position.z,
                RotationX = SRLECamera.Instance.transform.eulerAngles.x,
                RotationY = SRLECamera.Instance.transform.eulerAngles.y,
                RotationZ = SRLECamera.Instance.transform.eulerAngles.z,
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

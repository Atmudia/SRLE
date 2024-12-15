using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppTMPro;
using MelonLoader;
using SRLE.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SRLE.Components
{
    [RegisterTypeInIl2Cpp]
    public class TeleportUI : MonoBehaviour
    {
        private GameObject m_GameObject;
        private List<TeleportModel> m_TeleportModels;
        private ScrollRect m_CategoryScroll;
        private TMP_InputField m_NameInput;

        public bool IsOpen => m_GameObject.activeSelf;

        public static TeleportUI Instance;

        private void Start()
        {
            Instance = this;
            m_GameObject = transform.Find("Teleports").gameObject;
            m_CategoryScroll = transform.Find("Teleports/CategoryScroll").GetComponent<ScrollRect>();
            m_NameInput = transform.Find("Teleports/NameInput").GetComponent<TMP_InputField>();
            var addButton = transform.Find("Teleports/AddButton").GetComponent<Button>();

            LoadTeleportModels();
            InitializeCategoryButtons();

            addButton.onClick.AddListener(new Action(OnAddTeleport));
            Close();
        }

        private void LoadTeleportModels()
        {
            string filePath = Path.Combine(SaveManager.DataPath, "tp.txt");

            if (File.Exists(filePath))
            {
                m_TeleportModels = JsonSerializer.Deserialize<List<TeleportModel>>(File.ReadAllText(filePath));
            }
            else
            {
                InitializeDefaultTeleportModels();
            }
        }

        private void InitializeDefaultTeleportModels()
        {
            string jsonData = """
                              [
                                              {"Name":"Ranch","PositionX":541.9353,"PositionY":20.62804,"PositionZ":349.61053,"RotationX":2.5,"RotationY":231.5,"RotationZ":0.0,"Region":"SceneGroup.ConservatoryFields"},
                                              {"Name":"Starlight Strand","PositionX":-4.2704897,"PositionY":15.532708,"PositionZ":-122.46755,"RotationX":1.0000001,"RotationY":155.00003,"RotationZ":-2.6684488E-08,"Region":"SceneGroup.LuminousStrand"},
                                              {"Name":"Ember Valley","PositionX":-212.91,"PositionY":19.0,"PositionZ":468.7,"RotationX":0.0,"RotationY":-135.0,"RotationZ":0.0,"Region":"SceneGroup.RumblingGorge"},
                                              {"Name":"Powderfall Bluffs","PositionX":-710.1747,"PositionY":6.5834,"PositionZ":1357.909,"RotationX":342.048,"RotationY":30.624,"RotationZ":0.0,"Region":"SceneGroup.PowderfallBluffs"},
                                              {"Name":"Labyrinth","PositionX":1432.718,"PositionY":79.9886,"PositionZ":-1185.905,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"Region":"SceneGroup.Labyrinth"}
                                          ]
                              """;

            m_TeleportModels = JsonSerializer.Deserialize<List<TeleportModel>>(jsonData);
            File.WriteAllText(Path.Combine(SaveManager.DataPath, "tp.txt"), JsonSerializer.Serialize(m_TeleportModels.ToArray(), new JsonSerializerOptions()
            {
                WriteIndented = true
            }));
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


            var sceneGroups = SRSingleton<SystemContext>.Instance.SceneLoader.SceneGroupList.GameplaySceneGroups.Cast<Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<SceneGroup>>();

            foreach (var group in sceneGroups)
            {
                if (group.ReferenceId == model.Region)
                {
                    categoryObj.GetComponentInChildren<Button>().onClick.AddListener(new Action(() =>  TeleportTo(group, new Vector3(model.PositionX, model.PositionY, model.PositionZ), new Vector3(model.RotationX, model.RotationY, model.RotationZ))));
                    categoryObj.GetComponentInChildren<Text>().text = categoryName;
                    return;
                }
            }

            Debug.LogError($"Scene group not found for teleport model {model.Name}");
            Destroy(categoryObj);
        }

        private void OnAddTeleport()
        {
            string name = string.IsNullOrEmpty(m_NameInput.text) ? SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup.name + " Teleport" : m_NameInput.text;

            var model = new TeleportModel
            {
                Name = name,
                PositionX = SRLECamera.Instance.transform.position.x,
                PositionY = SRLECamera.Instance.transform.position.y,
                PositionZ = SRLECamera.Instance.transform.position.z,
                RotationX = SRLECamera.Instance.transform.eulerAngles.x,
                RotationY = SRLECamera.Instance.transform.eulerAngles.y,
                RotationZ = SRLECamera.Instance.transform.eulerAngles.z,
                Region = SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup.ReferenceId
            };

            CreateCategoryButton(model);

            m_NameInput.text = "";

            m_TeleportModels.Add(model);
            File.WriteAllText(Path.Combine(SaveManager.DataPath, "tp.txt"), JsonSerializer.Serialize(m_TeleportModels, new JsonSerializerOptions()
            {
                WriteIndented = true
            }));
        }

        private void TeleportTo(SceneGroup sceneGroup, Vector3 position, Vector3 rotation)
        {
            SRLECamera.Instance.transform.position = position;
            SRLECamera.Instance.transform.eulerAngles = new Vector3(SRLECamera.Instance.transform.eulerAngles.x, rotation.y, SRLECamera.Instance.transform.eulerAngles.z);
            SRSingleton<SystemContext>.Instance.SceneLoader.LoadSceneGroup(sceneGroup, new SceneLoadingParameters { TeleportPlayer = true });
        }

        public void Open() => m_GameObject.SetActive(true);
        public void Close() => m_GameObject.SetActive(false);
    }
}

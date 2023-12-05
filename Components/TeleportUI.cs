using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppSystem.Linq;
using Il2CppTMPro;
using SRLE;
using SRLE.Components;

public class TeleportUI : BaseUI
{
    private GameObject m_GameObject;
    private List<TeleportModel> m_TeleportModels;

    private ScrollRect m_CategoryScroll;
    private TMP_InputField m_NameInput;

    public bool IsOpen { get { return m_GameObject.activeSelf; } }

    public static TeleportUI Instance;

    private void Start()
    {
        Instance = this;
        m_GameObject = transform.Find("Teleports").gameObject;

        m_CategoryScroll = transform.Find("Teleports/CategoryScroll").GetComponent<ScrollRect>();
        m_NameInput = transform.Find("Teleports/NameInput").GetComponent<TMP_InputField>();
        var addButton = transform.Find("Teleports/AddButton").GetComponent<Button>();

        if (File.Exists(Path.Combine(SRLESaveManager.DataPath, "tp.txt")))
        {
            m_TeleportModels = JsonConvert.DeserializeObject<TeleportModel[]>(File.ReadAllText(Path.Combine(SRLESaveManager.DataPath, "tp.txt"))).ToList();
        }
        else
        {
            string data = "[{\"Name\":\"Ranch Home\",\"PositionX\":52.8,\"PositionY\":16.3,\"PositionZ\":-132.7,\"RotationX\":0.0,\"RotationY\":102.6,\"RotationZ\":0.0,\"Region\":0},{\"Name\":\"Desert Temple\",\"PositionX\":119.9,\"PositionY\":1077.4,\"PositionZ\":917.5,\"RotationX\":0.0,\"RotationY\":0.0,\"RotationZ\":0.0,\"Region\":1},{\"Name\":\"Slimulations\",\"PositionX\":1052.1,\"PositionY\":14.4,\"PositionZ\":824.0,\"RotationX\":0.0,\"RotationY\":315.0,\"RotationZ\":0.0,\"Region\":4},{\"Name\":\"Nimble 1\",\"PositionX\":-768.5,\"PositionY\":7.5,\"PositionZ\":-841.3,\"RotationX\":0.0,\"RotationY\":44.9,\"RotationZ\":0.0,\"Region\":2},{\"Name\":\"Nimble 2\",\"PositionX\":-189.8,\"PositionY\":11.1,\"PositionZ\":-1008.4,\"RotationX\":0.0,\"RotationY\":273.6,\"RotationZ\":0.0,\"Region\":2}]";
            m_TeleportModels = JsonConvert.DeserializeObject<TeleportModel[]>(data).ToList();

            File.WriteAllText(Path.Combine(SRLESaveManager.DataPath, "tp.txt"), JsonConvert.SerializeObject(m_TeleportModels.ToArray(), Formatting.Indented));
        }

        foreach (var model in m_TeleportModels)
        {
            GameObject categoryObj = Instantiate(SRLEAssetManager.CategoryButtonPrefab, m_CategoryScroll.content, false);

            string categoryName = model.Name;
            var sceneGroup = SRSingleton<SystemContext>.Instance.SceneLoader.SceneGroupList.GameplaySceneGroups.FirstOrDefault(new System.Func<SceneGroup, bool>(
                group =>
                {
                    if (group.ReferenceId == model.Region)
                        return true;
                    return false;
                }));
            categoryObj.GetComponentInChildren<Button>().onClick.AddListener( new Action(() => TeleportTo(sceneGroup, new Vector3(model.PositionX, model.PositionY, model.PositionZ), new Vector3(model.RotationX, model.RotationY, model.RotationZ))));
            categoryObj.GetComponentInChildren<Text>().text = categoryName;
        }

        addButton.onClick.AddListener(new Action(OnAddTeleport));

        Close();
    }

    private void OnAddTeleport()
    {
        var name = m_NameInput.text;
        if(string.IsNullOrEmpty(name))
        {
            name = SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup.name + " Teleport";
        }

        var model = new TeleportModel()
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
        GameObject categoryObj = Instantiate(SRLEAssetManager.CategoryButtonPrefab, m_CategoryScroll.content, false);

        string categoryName = model.Name;
        var sceneGroup = SRSingleton<SystemContext>.Instance.SceneLoader.SceneGroupList.GameplaySceneGroups.FirstOrDefault(new System.Func<SceneGroup, bool>(
            group =>
            {
                if (group.ReferenceId == model.Region)
                    return true;
                return false;
            }));
        categoryObj.GetComponentInChildren<Button>().onClick.AddListener( new Action(() => TeleportTo(sceneGroup, new Vector3(model.PositionX, model.PositionY, model.PositionZ), new Vector3(model.RotationX, model.RotationY, model.RotationZ))));
        categoryObj.GetComponentInChildren<Text>().text = categoryName;

        m_NameInput.text = "";

        m_TeleportModels.Add(model);
        File.WriteAllText(Path.Combine(SRLESaveManager.DataPath, "tp.txt"), JsonConvert.SerializeObject(m_TeleportModels.ToArray(), Formatting.Indented));
    }

    private void TeleportTo(SceneGroup sceneGroup, Vector3 position, Vector3 rotation)
    {
        SRLECamera.Instance.transform.position = position;
        SRLECamera.Instance.transform.eulerAngles = new Vector3(SRLECamera.Instance.transform.eulerAngles.x, rotation.y, SRLECamera.Instance.transform.eulerAngles.z);
        SRSingleton<SystemContext>.Instance.SceneLoader.LoadSceneGroup(sceneGroup, new SceneLoadingParameters()
        {
            TeleportPlayer = true,
            
        });
    }

    public void Open()
    {
        m_GameObject.SetActive(true);
    }

    public void Close()
    {
        m_GameObject.SetActive(false);
    }
}

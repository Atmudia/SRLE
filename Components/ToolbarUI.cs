using System;
using System.Linq;
using MelonLoader;
using SRLE.RuntimeGizmo;
using SRLE.RuntimeGizmo.Objects;
using SRLE.RuntimeGizmo.UndoRedo;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SRLE.Components;

[RegisterTypeInIl2Cpp]
public class ToolbarUI : MonoBehaviour
{
    public static ToolbarUI Instance;
    private GameObject infoGameObject;
    private Text StatusText;

    public void Awake()
    {
        Instance = this;
        var saveButton = GetToolbarButton("Save");
        saveButton.onClick.AddListener(new Action(SaveManager.SaveLevel));
        var undoButton = GetToolbarButton("Undo");
        undoButton.onClick.AddListener(new Action(UndoRedoManager.Undo));
        var redoButton = GetToolbarButton("Redo");
        redoButton.onClick.AddListener(new Action(UndoRedoManager.Redo));
        var testButton = GetToolbarButton("Test");
        testButton.onClick.AddListener(new Action(() => SRLECamera.Instance.SetActive(false)));
        var copyButton = GetToolbarButton("Copy");
        var transformGizmo = SRLECamera.Instance.transformGizmo;
        copyButton.onClick.AddListener(new Action(() =>
        {
            if (transformGizmo.mainTargetRoot != null)
            {
                CopyPasteManager.Paste();
                Instance.UpdateStatus();
            }
        }));
        var deleteButton = GetToolbarButton("Delete");
        deleteButton.onClick.AddListener(new Action(() =>
        {
            if (transformGizmo.mainTargetRoot != null)
            {
                
                ObjectManager.RemoveObject(transformGizmo.mainTargetRoot.gameObject);
                Instance.UpdateStatus();
            }
        }));
        var moveButton = GetToolbarButton("Move");
        moveButton.onClick.AddListener(new Action(() =>
        {
            transformGizmo.transformType = TransformType.Move;
            Melon<EntryPoint>.Logger.Msg("Move tool");
        }));
        var rotateButton = GetToolbarButton("Rotate");
        rotateButton.onClick.AddListener(new Action(() =>
        {
            transformGizmo.transformType = TransformType.Rotate;
            Melon<EntryPoint>.Logger.Msg("Rotate tool");
        }));
        var scaleButton = GetToolbarButton("Scale");
        scaleButton.onClick.AddListener(new Action(() =>
        {
            transformGizmo.transformType = TransformType.Scale;
            Melon<EntryPoint>.Logger.Msg("Scale tool");
        }));
        var settingsButton = GetToolbarButton("Settings");
        settingsButton.onClick.AddListener(new Action(() =>
        {
            SettingsUI.Instance.Open();
        }));
        // infoGameObject = this.transform.Find("Hierarchy/Info").gameObject;
        // Button closeButton = infoGameObject.transform.Find($"Panel/CloseButton")?.GetComponent<Button>();
        // closeButton.onClick.AddListener(new Action(() =>
        // {
        //     infoGameObject.SetActive(false);
        // }));
        // infoGameObject.SetActive(false);

        var infoButton = GetToolbarButton("Info");
        infoButton.onClick.AddListener(new Action(() =>
        {
            // infoGameObject.SetActive(true);
        }));
        var exitButton = GetToolbarButton("Exit");
        exitButton.onClick.AddListener(new Action(() =>
        {
            //SRLESaveManager.SaveLevel();
            SRSingleton<SceneContext>.Instance.PauseMenuDirector.Quit();
            LevelManager.SetMode(LevelManager.Mode.NONE);
        }));

        var teleportButton = GetToolbarButton("Teleport");
        teleportButton.onClick.AddListener(new Action(() =>
        {
            if(TeleportUI.Instance.IsOpen)
            {
                TeleportUI.Instance.Close();
            }
            else
            {
                TeleportUI.Instance.Open();
            }
        }));
        StatusText = GetToolbarText("Status");




    }

    public UnityEngine.UI.Button GetToolbarButton(string nameButton)
    {
        return this.transform.Find("Toolbar").Find(nameButton + "Button").GetComponent<UnityEngine.UI.Button>();
    }
    public UnityEngine.UI.Text GetToolbarText(string nameText)
    {
        return this.transform.Find("Toolbar").Find(nameText + "Text").GetComponent<Text>();
    }

    private static int GetPercentageOfLoadedObjects(int totalGameObjectCount)
    {
        if (totalGameObjectCount != 0)
            return (int)Math.Round((double)ObjectManager.World.transform.GetChildCount() / totalGameObjectCount * 100);
        return 100;
    }
    
    public void UpdateStatus()
    {
        if (StatusText == null) return;
        
        int totalGameObjectCount = ObjectManager.BuildObjects.Sum(keyValuePair => keyValuePair.Value.Count);
        var percentageOfLoadedObjects = GetPercentageOfLoadedObjects(totalGameObjectCount);
        StatusText.text = percentageOfLoadedObjects != 100 ? $"World is loading...{percentageOfLoadedObjects}%" : $"Loaded Objects: {totalGameObjectCount}";
    }
}
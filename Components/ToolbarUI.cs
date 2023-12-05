using System;
using Il2CppMonomiPark.SlimeRancher.UI;
using MelonLoader;
using SRLE.RuntimeGizmo;
using SRLE.RuntimeGizmo.Objects;
using SRLE.RuntimeGizmo.UndoRedo;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace SRLE.Components;

public class ToolbarUI : BaseUI
{
    public static ToolbarUI Instance;

    public GameObject infoGameObject;
    public ToolbarUI(IntPtr value) : base(value)
    {
    }

    public override void Awake()
    {
        
        Instance = this;
        var saveButton = GetToolbarButton("Save");
        saveButton.onClick.AddListener(new Action(SRLESaveManager.SaveLevel));
        var undoButton = GetToolbarButton("Undo");
        undoButton.onClick.AddListener(new Action(UndoRedoManager.Undo));
        var redoButton = GetToolbarButton("Redo");
        redoButton.onClick.AddListener(new Action(UndoRedoManager.Redo));
        var testButton = GetToolbarButton("Test");
        testButton.onClick.AddListener(new Action(() => SRLECamera.Instance.SetActive(false)));
        var copyButton = GetToolbarButton("Copy");
        copyButton.onClick.AddListener(new Action(() =>
        {
            CopyPasteManager.Copy();
            CopyPasteManager.Paste();

        }));
        var deleteButton = GetToolbarButton("Delete");
        deleteButton.onClick.AddListener(new Action(() =>
        {
            Object.Destroy(SRLECamera.Instance.transformGizmo.mainTargetRoot);
        }));
        var moveButton = GetToolbarButton("Move");
        moveButton.onClick.AddListener(new Action(() =>
        {
            SRLECamera.Instance.transformGizmo.transformType = TransformType.Move;
        }));
        var rotateButton = GetToolbarButton("Rotate");
        rotateButton.onClick.AddListener(new Action(() =>
        {
            SRLECamera.Instance.transformGizmo.transformType = TransformType.Rotate;
        }));
        var scaleButton = GetToolbarButton("Scale");
        scaleButton.onClick.AddListener(new Action(() =>
        {
            SRLECamera.Instance.transformGizmo.transformType = TransformType.Scale;
        }));
        var settingsButton = GetToolbarButton("Settings");
        settingsButton.onClick.AddListener(new Action(() =>
        {
            SettingsUI.Instance.Open();
        }));
        infoGameObject = HierarchyUI.Instance.transform.Find("Info").gameObject;
        Button closeButton = infoGameObject.transform.Find($"Panel/CloseButton")?.GetComponent<Button>();
        closeButton.onClick.AddListener(new Action(() =>
        {
            infoGameObject.SetActive(false);
        }));
        infoGameObject.SetActive(false);

        var infoButton = GetToolbarButton("Info");
        infoButton.onClick.AddListener(new Action(() =>
        {
            infoGameObject.SetActive(true);
        }));
        var exitButton = GetToolbarButton("Exit");
        exitButton.onClick.AddListener(new Action(() =>
        {
            SRLESaveManager.SaveLevel();
            SRSingleton<SceneContext>.Instance.PauseMenuDirector.Quit();
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



    }

    private void Update()
    {
        
    }

    public UnityEngine.UI.Button GetToolbarButton(string nameButton)
    {
        return this.transform.Find("Toolbar").Find(nameButton + "Button").GetComponent<UnityEngine.UI.Button>();
    }
}
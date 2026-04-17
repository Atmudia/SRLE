using System;
using System.Linq;
using SRLE.RuntimeGizmo;
using SRLE.RuntimeGizmo.Objects;
using SRLE.RuntimeGizmo.UndoRedo;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SRLE.Components
{
    public class ToolbarUI : MonoBehaviour
    {
        public static ToolbarUI Instance;
        private GameObject infoGameObject;
        private Text StatusText;

        public void Awake()
        {
            Instance = this;
            var saveButton = GetToolbarButton("Save");
            saveButton.onClick.AddListener(SaveManager.SaveLevel);
            var undoButton = GetToolbarButton("Undo");
            undoButton.onClick.AddListener(new UnityAction(UndoRedoManager.Undo));
            var redoButton = GetToolbarButton("Redo");
            redoButton.onClick.AddListener(new UnityAction(UndoRedoManager.Redo));
            var testButton = GetToolbarButton("Test");
            testButton.onClick.AddListener(new UnityAction(() => SRLECamera.Instance.SetActive(false)));
            var copyButton = GetToolbarButton("Copy");
            var transformGizmo = SRLECamera.Instance.transformGizmo;
            copyButton.onClick.AddListener(new UnityAction(() =>
            {
                CopyPasteManager.Duplicate();
                Instance.UpdateStatus();
            }));
            var deleteButton = GetToolbarButton("Delete");
            deleteButton.onClick.AddListener(new UnityAction(() =>
            {
                if (transformGizmo.mainTargetRoot)
                {
                    ObjectManager.RemoveObject(transformGizmo.mainTargetRoot.gameObject);
                    Instance.UpdateStatus();
                }
            }));
            var moveButton = GetToolbarButton("Move");
            moveButton.onClick.AddListener(new UnityAction(() =>
            {
                transformGizmo.transformType = TransformType.Move;
            }));
            //TODO add move tool
            var rotateButton = GetToolbarButton("Rotate");
            rotateButton.onClick.AddListener(new UnityAction(() =>
            {
                transformGizmo.transformType = TransformType.Rotate;
            }));
            //TODO add rotate tool
            var scaleButton = GetToolbarButton("Scale");
            scaleButton.onClick.AddListener(new UnityAction(() =>
            {
                transformGizmo.transformType = TransformType.Scale;
                EntryPoint.ConsoleInstance.Log("Scale tool");
            }));
            //TODO add scale tool
            var settingsButton = GetToolbarButton("Settings");
            settingsButton.onClick.AddListener(() =>
            {
                SettingsUI.Instance.Open();
            });
            // infoGameObject = this.transform.Find("Hierarchy/Info").gameObject;
            // Button closeButton = infoGameObject.transform.Find($"Panel/CloseButton")?.GetComponent<Button>();
            // closeButton.onClick.AddListener(new Action(() =>
            // {
            //     infoGameObject.SetActive(false);
            // }));
            // infoGameObject.SetActive(false);

            var infoButton = GetToolbarButton("Info");
            infoButton.onClick.AddListener(new UnityAction(() =>
            {
                // infoGameObject.SetActive(true);
            }));
            var exitButton = GetToolbarButton("Exit");
            exitButton.onClick.AddListener(new UnityAction(() =>
            {
                PauseMenu.Instance.Quit();
                LevelManager.SetMode(LevelManager.Mode.NONE);
            }));

            var teleportButton = GetToolbarButton("Teleport");
            teleportButton.onClick.AddListener(new UnityAction(() =>
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
                return (int)Math.Round((double)ObjectManager.World.transform.childCount / totalGameObjectCount * 100);
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
}
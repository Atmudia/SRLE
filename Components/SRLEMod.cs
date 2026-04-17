using System.IO;
using SRLE.Models;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SRLE.Components
{
    public class SRLEMod : MonoBehaviour
    {
        public static SRLEMod Instance;
        private string currentLevel;

        protected void Awake()
        {
            if (Instance == null) Instance = this;
            else Object.Destroy(this);
        }

        private WorldType m_SelectedWorldType = WorldType.STANDARD;
        private static readonly string[] s_WorldTypeNames = { "Standard", "Sea", "Desert", "Void" };
        private static readonly WorldType[] s_WorldTypes = { WorldType.STANDARD, WorldType.SEA, WorldType.DESERT, WorldType.VOID };

        protected void OnGUI()
        {
            if (!Levels.isMainMenu())
                return;

            GUI.Label(new Rect(15f, 125f, 150f, 25f), "SRLE v" + EntryPoint.Version);

            var files = Directory.GetFiles(SaveManager.LevelsPath, "*.srle");
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var levelName = Path.GetFileNameWithoutExtension(file);
                if (GUI.Button(new Rect(15f, 150f + 35f * i, 150f, 25f), "Load " + levelName))
                {
                    LevelManager.IsLoading = true;
                    SaveManager.LoadLevel(file);
                }
            }

            float y = 150f + 35f * files.Length;
            GUI.Box(new Rect(15f, y, 350f, 130f), "");
            GUI.Label(new Rect(25f, y + 10f, 100f, 25f), "Level Name:");
            currentLevel = GUI.TextField(new Rect(125f, y + 10f, 225f, 25f), currentLevel);

            GUI.Label(new Rect(25f, y + 45f, 100f, 25f), "World Type:");
            for (int i = 0; i < s_WorldTypes.Length; i++)
            {
                bool selected = m_SelectedWorldType == s_WorldTypes[i];
                if (GUI.Toggle(new Rect(125f + 80f * i, y + 45f, 75f, 25f), selected, s_WorldTypeNames[i]) && !selected)
                    m_SelectedWorldType = s_WorldTypes[i];
            }

            if (GUI.Button(new Rect(25f, y + 85f, 150f, 25f), "Create new Level"))
            {
                var isValid = !string.IsNullOrEmpty(currentLevel) &&
                              currentLevel.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 &&
                              !File.Exists(Path.Combine(SaveManager.LevelsPath, currentLevel + ".srle"));
                if (isValid)
                {
                    LevelManager.IsLoading = true;
                    SaveManager.CreateLevel(currentLevel, m_SelectedWorldType);
                }
            }
        }

        private void Update()
        {
            // if (UIInitializer.IsInitialized)
            //     ObjectManager.UpdateRequests();

            if (!LevelManager.IsActive) return;

            Vector3 pos;
            if (SRLECamera.Instance != null && SRLECamera.Instance.gameObject.activeSelf)
                pos = SRLECamera.Instance.transform.position;
            else if (SRLECamera.Instance?.playerController != null)
                pos = SRLECamera.Instance.playerController.transform.position;
            else
                return;

            ChunkManager.UpdateActiveChunks(pos);
        }
    }
}
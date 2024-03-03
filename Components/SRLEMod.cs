using System.IO;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SRLE.Components;

[RegisterTypeInIl2Cpp]
public class SRLEMod : MonoBehaviour
{
    public static SRLEMod Instance;
    private string currentLevel;

    protected void Awake()
    {
        if (Instance == null) Instance = this;
        else Object.Destroy(this);
    }

    protected void OnGUI()
    {
        if (!SceneManager.GetActiveScene().name.Equals("MainMenuEnvironment"))
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
        currentLevel = GUI.TextField(new Rect(125f, 170f + 35f * files.Length, 200f, 25f), currentLevel);

        GUI.Box(new Rect(15f, 150f + 35f * files.Length, 350f, 100f), "");
        GUI.Label(new Rect(25f, 170f + 35f * files.Length, 150f, 25f), "Level Name:");
        if (GUI.Button(new Rect(25f, 210f + 35f * files.Length, 150f, 25f), "Create new Level"))
        {
            var fileName = currentLevel + ".srle";
            var isValid = !string.IsNullOrEmpty(fileName) &&
                          fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 &&
                          !File.Exists(Path.Combine(currentLevel, fileName));
            if (isValid)
            {
                LevelManager.IsLoading = true;
                SaveManager.CreateLevel(currentLevel);
            }
            else
            {
                //error = "Invalid Levelname or already exists";
            }
        }
    }

    private void Update()
    {
        if (UIInitializer.IsInitialized)
            ObjectManager.UpdateRequests();
    }
}
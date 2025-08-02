using System.IO;
using SRLE.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SRLE.Components.MainMenuUIs
{
    public class LevelSummaryEntry : MonoBehaviour
    {
        public TMP_Text gameNameText;
        public Image gameIcon;
        public string nameOfFile;

        public void Init(LevelData levelSummaryEntry)
        {
            nameOfFile = Path.GetFileName(levelSummaryEntry.Path);
            gameNameText.text = levelSummaryEntry.LevelName;
            gameIcon.sprite = SRLENewLevelUI.AllSprites[0];
        }
    }
}
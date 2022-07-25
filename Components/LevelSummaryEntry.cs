﻿using SRLE.SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SRLE.Components
{
    public class LevelSummaryEntry : MonoBehaviour
    {
        public TMP_Text gameNameText;
        public Image gameIcon;
        public string nameOfFile;

        public void Init(SRLEName levelSummaryEntry)
        {
            nameOfFile = levelSummaryEntry.nameOfFile;
            gameNameText.text = levelSummaryEntry.nameOfLevel;
            gameIcon.sprite = SRLENewLevelUI.allSprites[levelSummaryEntry.spriteType];
        }
    }
}
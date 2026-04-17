using System.IO;
using System.Linq;
using SRLE.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SRLE.Components.MainMenuUIs
{
    public class LevelSummaryPanel : MonoBehaviour
    {
        public Image levelIcon;
        public TMP_Text levelNameText;
        public TMP_Text modeText;
        public TMP_Text modeDescText;
        public TMP_Text objectsAmountText;
        public TMP_Text fileSizeText;


        public void Init(LevelData srleName)
        {

            var bundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
            this.levelNameText.text = Path.GetFileNameWithoutExtension(srleName.Path);
            
            // levelIcon.sprite = SRLENewLevelUI.AllSprites[srleName.spriteType];
            string lowerInvariant = srleName.WorldType.ToString().ToLowerInvariant();
            
            
            this.modeText.text = bundle.Xlate("l.srle.world_type." + lowerInvariant); 
            this.modeDescText.text = bundle.Xlate("m.srle.desc.worldType." + lowerInvariant);


            int num = srleName.BuildObjects.Values.Sum(list => list.Count);
            
            this.objectsAmountText.text = bundle.Xlate(MessageUtil.Tcompose("l.srle.object_count", (object) num));

            EntryPoint.ConsoleInstance.Log("Hello: " + srleName.Path);
           
            this.fileSizeText.text =  bundle.Xlate(MessageUtil.Tcompose("l.srle.filesize", (object)  new FileInfo(srleName.Path).Length.ToPrettySize()));

            
        }
    }
}
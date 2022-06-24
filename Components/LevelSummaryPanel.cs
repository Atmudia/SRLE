using System.IO;
using SRLE.SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SRLE.Components
{
    public class LevelSummaryPanel : MonoBehaviour
    {
        public Image levelIcon;
        public TMP_Text levelNameText;
        public TMP_Text modeText;
        public TMP_Text modeDescText;
        public TMP_Text objectsAmountText;
        public TMP_Text fileSizeText;

        public void Init(SRLEName srleName)
        {
            
            
            var bundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
            this.levelNameText.text = srleName.nameOfLevel;
            
            levelIcon.sprite = SRLENewLevelUI.allSprites[srleName.spriteType];
            string lowerInvariant = srleName.worldType.ToString().ToLowerInvariant();
            
            
            this.modeText.text = bundle.Xlate("l.srle.world_type_" + lowerInvariant); 
            this.modeDescText.text = bundle.Xlate("m.srle.desc.worldType." + lowerInvariant);


            int num = 0;
            foreach (var VARIABLE in srleName.objects)
            {
                VARIABLE.Value.ForEach(_ => num++);
            }
            
            this.objectsAmountText.text = bundle.Xlate(MessageUtil.Tcompose("l.srle.object_count", (object) num));

            var combine = Path.Combine(SRLEManager.Worlds.FullName, srleName.nameOfLevel + ".srle");
           
            this.fileSizeText.text =  bundle.Xlate(MessageUtil.Tcompose("l.srle.filesize", (object)  new FileInfo(combine).Length.ToPrettySize()));

            
        }
    }
}
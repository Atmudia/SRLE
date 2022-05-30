using SRML;
using SRML.Console;
using SRML.SR;
using SRML.SR.UI.Utils;
using SRML.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace SRLE
{
    public class EntryPoint : ModEntryPoint
    {
        public override void PreLoad()
        {
            
            SRCallbacks.OnMainMenuLoaded += menu =>
            {
                
                var c = PrefabUtils.CopyPrefab(SRObjects.Get<ExpoGameSelectUI>().gameObject);
                var addMainMenuButton = MainMenuUtils.AddMainMenuButton(menu, "SRLE", () =>
                {
                    /*Destroyer.Destroy(Object.FindObjectOfType<MainMenuUI>().gameObject, "SRLE.Button");
                    menu.InstantiateAndWaitForDestroy(c);
                    */
                });
                addMainMenuButton.GetComponent<Button>().onClick = menu.transform.Find("ExpoModePanel/PlayButton").GetComponent<Button>().onClick;
                addMainMenuButton.name = "SRLEButton";
                addMainMenuButton.transform.Find("Text").GetComponent<XlateText>().SetKey("b.srle");
                addMainMenuButton.transform.SetSiblingIndex(4);
                

            };
        }
    }
}
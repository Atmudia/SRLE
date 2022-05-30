using SRML;
using SRML.SR;
using SRML.SR.UI.Utils;

namespace SRLE
{
    public class EntryPoint : ModEntryPoint
    {
        public override void PreLoad()
        {
            SRCallbacks.OnMainMenuLoaded += menu =>
            {
                var addMainMenuButton = MainMenuUtils.AddMainMenuButton(menu, "SRLE", () =>
                {

                });
                addMainMenuButton.name = "SRLEButton";
                addMainMenuButton.transform.Find("Text").GetComponent<XlateText>().SetKey("b.srle");
                addMainMenuButton.transform.SetSiblingIndex(4);
            };
        }
    }
}
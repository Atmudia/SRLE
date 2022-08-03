using UnityEngine;

namespace SRLE
{
    public class LoaderAssets
    {
        public static NewGameUI NewLevelUI;

        public static void CreateNewLevelUI()
        {
            NewLevelUI = Object.Instantiate(EntryPoint.menu.newGameUI).GetComponent<NewGameUI>();
            var panel = NewLevelUI.transform.Find("Panel").gameObject;
            panel.transform.Find("Title").GetComponent<XlateText>().SetKey("l.srle.create_a_level");
            panel = panel.transform.Find("InfoPanel").gameObject;
            panel.transform.Find("GameNameLabel").GetComponent<XlateText>().SetKey("l.srle.level_name");
        }
    }
}
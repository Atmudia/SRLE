using SRLE.Components;
using UnityEngine;

namespace SRLE
{
    
    public static class SRLEUIManager
    {
        // private static AssetBundle UIData = AssetBundle.LoadFromFile(@"E:\SlimeRancherModding\Unity\SRLE\Assets\AssetBundles\srledata");

        public static void LoadUIData()
        {
            /*var Toolbar = Object.Instantiate(UIData.LoadAsset<GameObject>("Toolbar"));
            Toolbar.name = "Toolbar";
            var Panel = Toolbar.transform.Find("Panel").gameObject;
            Panel.transform.Find("ExitLevelButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (SRSingleton<PauseMenu>.Instance)
                    SRSingleton<PauseMenu>.Instance.Quit();
            });
            Panel.transform.Find("SaveLevelButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                SRLEManager.SaveLevel();
            });
            Panel.transform.Find("PlayModeButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (SRSingleton<SRLECamera>.Instance)
                    SRSingleton<SRLECamera>.Instance.gameObject.SetActive(false);
                
            });
            */
                       
            

            var srleCamera = new GameObject("SRLECamera", new []
            {
                typeof(Camera), typeof(SRLECamera)
            }).GetComponent<SRLECamera>();
            /*
            srleCamera.gameObject.SetActive(false);

            srleCamera.listOfUIs.Add(Toolbar);
            //srleCamera.listOfUIs.Add(hierarchy);
            srleCamera.gameObject.SetActive(true);
            */


            
 


        }

    }
}
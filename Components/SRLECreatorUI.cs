using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
namespace SRLE
{
    public class SRLECreatorUI : BaseUI
    {
        List<Category> categories = new List<Category>();

        // These are all set in unity
        public GameObject playButton;
        public GameObject exportButton;
        public GameObject saveButton;
        public GameObject quitButton;
        public Transform categoryContent;
        public Transform objectContent;
        // Prefabs from unity that we will use
        public GameObject categoryPrefab;
        public GameObject objectPrefab;
        public override void Awake()
        {
            base.Awake();
            categories = JsonConvert.DeserializeObject<List<Category>>(
                Encoding.Default.GetString(Assembly.GetExecutingAssembly().GetManifestResourceStream("buildobjects.txt").ReadAllBytes()));
            foreach (var category in categories)
            {
                var categoryButton = Object.Instantiate(categoryPrefab, categoryContent);
                categoryButton.GetComponentInChildren<Text>().text = category.CategoryName;
                categoryButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    foreach (IdClass o in category.Objects)
                    {
                        
                    }
                });
            }
        }
    }
}

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
using SRLE.SaveSystem;
using RuntimeGizmos;

namespace SRLE.Components
{
	public class SRLECreatorUI : BaseUI
	{
		List<Category> categories = new List<Category>();


		// These are all set in unity
		public GameObject playButton;
		public GameObject exportButton;
		public GameObject saveButton;
		public GameObject quitButton;
		public GameObject undoButton;
		public GameObject redoButton;
		public Transform categoryContent;
		public Transform objectContent;
		// Prefabs from unity that we will use
		public GameObject categoryPrefab;
		public GameObject objectPrefab;
		public override void Awake()
		{
			base.Awake();
			Transform t = gameObject.transform;
			categoryContent = t.Find("Object Panel/Categories/Viewport/Content");
			categoryPrefab = null; // add this when prefab completed
			exportButton = t.Find("Toolbar/ExportButton").gameObject;
			objectContent = t.Find("Object Panel/Objects/Viewport/Content");
			objectPrefab = null; // add this when prefab completed
			playButton = t.Find("Toolbar/Play").gameObject;
			saveButton = t.Find("Toolbar.Save").gameObject;
			quitButton = t.Find("Toolbar/Save and Quit").gameObject;
			undoButton = t.Find("Toolbar/Undo").gameObject;
			redoButton = t.Find("Toolbar/Redo").gameObject;
			categories = JsonConvert.DeserializeObject<List<Category>>(
				Encoding.Default.GetString(Assembly.GetExecutingAssembly().GetManifestResourceStream("buildobjects.txt").ReadAllBytes()));
			/* foreach (var category in categories)
			{
				var categoryButton = Object.Instantiate(categoryPrefab, categoryContent);
				categoryButton.GetComponentInChildren<Text>().text = category.CategoryName;
				categoryButton.GetComponent<Button>().onClick.AddListener(() =>
				{
					foreach (IdClass o in category.Objects)
					{
						
					}
				});
			} */
			saveButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				SRLEManager.SaveLevel();
			});
			quitButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				SRLEManager.SaveLevel();
				SRLEManager.currentData = null;
				SRLEManager.isSRLELevel = false;
				SRSingleton<GameContext>.Instance.AutoSaveDirector.RevertToMainMenu(() => {});
				Destroyer.Destroy(gameObject, "quitButton.onClick");
			});
			playButton.GetComponent<Button>().onClick.AddListener(() => 
			{
				gameObject.GetComponent<Canvas>().enabled = false;
				SRSingleton<SRLECamera>.Instance.enabled = false;
			});
			exportButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				TransformGizmo gizmo = SRSingleton<SRLECamera>.Instance.controller;
				if (!gizmo.mainTargetRoot)
                {
					"Can't export as custom object without anything selected. Select an object first.".LogError();
					return;
                }
				List<SRLESave> saves = new List<SRLESave>();
				Transform mainTransform = gizmo.mainTargetRoot;
				saves.Add(mainTransform.ToSRLESave());
				foreach (Transform t in gizmo.targetRootsOrdered)
                {
					if (t == mainTransform) continue;
					Vector3 pos = t.position;
					// to get relative pos
					t.position = mainTransform.position - t.position;
					saves.Add(t.ToSRLESave());
					t.position = pos;
                }
				SRLEManager.customObjects.Add((uint)SRLEManager.customObjects.Count, saves);
			});
		}
	}
}

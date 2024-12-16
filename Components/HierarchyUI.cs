using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppSystem;
using Il2CppTMPro;
using MelonLoader;
using SRLE.Models;
using SRLE.Utils;
using UnityEngine;
using UnityEngine.UI;
using Action = System.Action;
using IntPtr = System.IntPtr;

namespace SRLE.Components
{
    [RegisterTypeInIl2Cpp]
    public class HierarchyUI : MonoBehaviour
    {
        public static HierarchyUI Instance;

        private GameObject Hierarchy;
        private ScrollRect CategoryScroll;
        private ScrollRect ObjectsScroll;
        internal InputField SearchInput;
        private Dictionary<uint, Texture2D> BuildObjectsPreview;

        private bool currentlyProcessingPreviews = false;
        private object currPreviewEnumerator = null;
        private Queue<(uint, GameObject)> previewQueue = new Queue<(uint, GameObject)>();

        public HierarchyUI(IntPtr value) : base(value) { }

        public void Awake()
        {
            Instance = this;
            InitializeUIElements();
            PopulateCategoryButtons();
        }

        private void InitializeUIElements()
        {
            if (!Directory.Exists(Path.Combine(SaveManager.DataPath, "Textures")))
                Directory.CreateDirectory(Path.Combine(SaveManager.DataPath, "Textures"));

            Hierarchy = transform.Find("Hierarchy").gameObject;
            CategoryScroll = transform.Find("Hierarchy/CategoryScroll").GetComponent<ScrollRect>();
            ObjectsScroll = transform.Find("Hierarchy/ObjectsScroll").GetComponent<ScrollRect>();
            SearchInput = transform.Find("Hierarchy/SearchInput").GetComponent<InputField>();
            
            BuildObjectsPreview = new Dictionary<uint, Texture2D>();

            ClearChildObjects(CategoryScroll.content);
            ClearChildObjects(ObjectsScroll.content);

            SearchInput.onValueChanged.AddListener(new System.Action<string>(Search));
        }

        private void ClearChildObjects(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        private void PopulateCategoryButtons()
        {
            CreateCategoryButton("Favorites", () => SelectCategory("Favorites"));
            foreach (var categoryName in ObjectManager.BuildCategories.Keys.Where(categoryName => !categoryName.Equals("Favorites")))
            {
                CreateCategoryButton(categoryName, () => SelectCategory(categoryName));
            }
            SelectCategory("Favorites");
        }

        private void CreateCategoryButton(string categoryName, Action onClickAction)
        {
            GameObject categoryButton = Instantiate(AssetManager.CategoryButtonPrefab, CategoryScroll.content, false);
            categoryButton.GetComponentInChildren<Button>().onClick.AddListener(onClickAction);
            categoryButton.GetComponentInChildren<Text>().text = categoryName;
        }

        private void Search(string term)
        {
            ClearChildObjects(ObjectsScroll.content);
            KillCurrentPreviewQueue();

            if (term.Length < 2) return;

            foreach (var buildObject in ObjectManager.BuildObjectsData.Values.Where(buildObject => buildObject.Name.ToLower().Contains(term.ToLower())))
            {
                CreateObjectButton(buildObject);
            }
        }

        private void CreateObjectButton(IdClass buildObject)
        {
            uint objectID = buildObject.Id;
            string objectName = buildObject.Name;

            GameObject buildObj = Instantiate(AssetManager.ObjectButtonPrefab, ObjectsScroll.content, false);
            buildObj.GetComponentInChildren<Button>().onClick.AddListener(new Action(() => SpawnObject(objectID)));
            buildObj.GetComponentInChildren<Text>().text = objectName;

            CreateFavoriteButton(buildObj, objectID);

            LoadOrUpdateObjectPreview(objectID, buildObj);
        }

        private void CreateFavoriteButton(GameObject buildObj, uint objectID)
        {
            var favorite = buildObj.transform.Find("Favorite");
            favorite.GetComponent<Image>().color = ObjectManager.BuildCategories["Favorites"].Contains(objectID) ? Color.yellow : Color.gray;
            favorite.GetComponent<Button>().onClick.AddListener(new Action(() => ToggleFavorite(objectID, favorite)));

            void ToggleFavorite(uint id, Transform favoriteTransform)
            {
                var favorites = ObjectManager.BuildCategories["Favorites"];

                if (!favorites.Contains(id))
                {
                    favorites.Add(id);
                    favoriteTransform.GetComponent<Image>().color = Color.yellow;
                }
                else
                {
                    favorites.Remove(id);
                    favoriteTransform.GetComponent<Image>().color = Color.gray;
                }

                File.WriteAllText(Path.Combine(SaveManager.DataPath, "favorites.txt"), JsonSerializer.Serialize(favorites));
            }
        }

        private void LoadOrUpdateObjectPreview(uint objectID, GameObject buildObj)
        {
            if (!BuildObjectsPreview.ContainsKey(objectID))
            {
                previewQueue.Enqueue((objectID, buildObj));
                if (!currentlyProcessingPreviews)
                    currPreviewEnumerator = MelonCoroutines.Start(LoadPreviewAsync());
            }
            else
            {
                buildObj.GetComponentInChildren<RawImage>().texture = BuildObjectsPreview[objectID];
            }
        }
        

        private void LoadObjectPreviewFromDisk(uint objectID, GameObject buildObj)
        {
            string filePath = Path.Combine(SaveManager.DataPath, "Textures", objectID + ".jpg");

            if (File.Exists(filePath))
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                Texture2D result = new Texture2D(64, 64, TextureFormat.RGB24, false);
                result.LoadImage(bytes, false);
                result.Apply(false, false);

                buildObj.GetComponentInChildren<RawImage>().texture = result;
                BuildObjectsPreview.Add(objectID, result);
            }
            else
            {
                RequestAndSaveObjectPreview(objectID, buildObj);
            }
        }

        private void RequestAndSaveObjectPreview(uint objectID, GameObject buildObj)
        {
            ObjectManager.RequestObject(objectID, (previewObj) =>
            {
                if (previewObj == null) return;
                if (buildObj == null) return;

                Texture2D texture = RuntimePreviewGenerator.GenerateModelPreview(previewObj.transform);
                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(SaveManager.DataPath, "Textures", objectID + ".jpg"), bytes);

                buildObj.GetComponentInChildren<RawImage>().texture = texture;
                BuildObjectsPreview.Add(objectID, texture);
            });
        }

        private void SpawnObject(uint id)
        {
            ObjectManager.RequestObject(id, (buildObject) =>
            {
                if (buildObject == null) return;

                GameObject obj = Instantiate(buildObject, SRLECamera.Instance.transform.position + (SRLECamera.Instance.transform.forward * 10), Quaternion.identity, ObjectManager.World.transform);
                obj.SetActive(true);
                var addComponent = obj.AddComponent<BuildObject>();
                addComponent.ID = ObjectManager.BuildObjectsData[id];
                ObjectManager.AddObject(id, obj);
                
                SRLECamera.Instance.transformGizmo.ClearAndAddTarget(obj.transform);

                
                // UndoManager.RegisterStates(new IUndo[] { new UndoSelection(), new UndoInstantiate(objectID, obj) }, "Create new Object");
                // ObjectSelection.Instance.SetSelection(obj);
            });
            //ObjectManager.SpawnObject(id);
        }

        private void SelectCategory(string categoryName)
        {
            if (ObjectManager.BuildCategories.TryGetValue(categoryName, out List<uint> categoryObjects))
            {
                ClearChildObjects(ObjectsScroll.content);
                KillCurrentPreviewQueue();

                foreach (var objectID in categoryObjects.Where(ObjectManager.BuildObjectsData.ContainsKey))
                {
                    CreateObjectButton(ObjectManager.BuildObjectsData[objectID]);
                }
            }
        }

        private System.Collections.IEnumerator LoadPreviewAsync()
        {
            currentlyProcessingPreviews = true;

            (uint, GameObject) previewToLoad = previewQueue.Dequeue();
            uint objectID = previewToLoad.Item1;
            GameObject buildObj = previewToLoad.Item2;

            string filePath = Path.Combine(SaveManager.DataPath, "Textures", objectID + ".png");

            if (File.Exists(filePath))
            {
                Task<byte[]> asyncBytes = File.ReadAllBytesAsync(filePath);
                while (!asyncBytes.IsCompleted)
                    yield return null;

                Texture2D result = new Texture2D(64, 64, TextureFormat.RGB24, false);
                result.LoadImage(asyncBytes.Result, false);
                result.Apply(false, false);

                BuildObjectsPreview.Add(objectID, result);

                if (buildObj != null)
                    buildObj.GetComponentInChildren<RawImage>().texture = result;
            }
            else
            {
                GameObject previewObj = null;
                ObjectManager.RequestObject(objectID, x => previewObj = x);

                if (previewObj != null)
                {
                    Texture2D texture = RuntimePreviewGenerator.GenerateModelPreview(previewObj.transform);
                    byte[] bytes = texture.EncodeToPNG();
                    if (bytes != null)
                    {
                        Task asyncBytes = File.WriteAllBytesAsync(filePath, bytes);
                        while (!asyncBytes.IsCompleted)
                            yield return null;
                        BuildObjectsPreview.Add(objectID, texture);

                        if (buildObj != null)
                            buildObj.GetComponentInChildren<RawImage>().texture = texture;
                    }
                    else Melon<EntryPoint>.Logger.Warning($"Generated texture for Id {objectID} is null");
                }
                else Melon<EntryPoint>.Logger.Warning("Attempting model generation of non-existent object! Skipping ...");
            }

            if (previewQueue.Count == 0)
            {
                currentlyProcessingPreviews = false;
                currPreviewEnumerator = null;
                yield break;
            }
            currPreviewEnumerator = MelonCoroutines.Start(LoadPreviewAsync());
            yield return null;
        }

        public void KillCurrentPreviewQueue()
        {
            if (currPreviewEnumerator != null)
                MelonCoroutines.Stop(currPreviewEnumerator);
            previewQueue.Clear();
            currentlyProcessingPreviews = false;
        }
    }
}

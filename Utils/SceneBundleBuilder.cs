using System;
using System.Collections.Generic;
using System.IO;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace SRLE.Utils
{
    public static class SceneBundleBuilder
    {
        /// <summary>
        /// Creates an assetbundle that can be used to load one of the game's scenes like a prefab
        /// <br/><br/>
        /// Note: Each of the scene's root gameobjects will be a seperate asset in the bundle
        /// </summary>
        /// <param name="sceneName">The "scene path" of the scene to build the loader for</param>
        /// <returns>The generated "loader" bundle as a byte array. Can be written to a file and/or loaded directly</returns>
        public static byte[] CreateSceneLoaderBundle(string sceneName)
        {
            var index = SceneUtility.GetBuildIndexByScenePath(sceneName);
            if (index == -1)
                throw new ArgumentOutOfRangeException(nameof(sceneName), "There is no scene with the name " + sceneName);
            return CreateSceneLoaderBundle(index);
        }
        /// <summary>
        /// Creates an assetbundle that can be used to load one of the game's scenes like a prefab
        /// <br/><br/>
        /// Note: Each of the scene's root gameobjects will be a seperate asset in the bundle
        /// </summary>
        /// <param name="sceneInd">The build index of the scene to build the loader for</param>
        /// <returns>The generated "loader" bundle as a byte array. Can be written to a file and/or loaded directly</returns>
        public static byte[] CreateSceneLoaderBundle(int sceneInd)
        {
            if (sceneInd < 0 || sceneInd >= SceneManager.sceneCountInBuildSettings)
                throw new ArgumentOutOfRangeException(nameof(sceneInd), "There is no scene with the index " + sceneInd);

            var manager = new AssetsManager();

            // IMPORTANT: class package is required, only difference is speed and memory usage. https://github.com/AssetRipper/Tpk/blob/master/README.md
            // uncompressed = fastest, most memory usage
            // lz4 = ok speed, ok memory usage
            // lzma = slowest, least memory usage

            using (Stream tpkStream = typeof(EntryPoint).Assembly.GetManifestResourceStream("SRLE.lz4.tpk"))
                manager.LoadClassPackage(tpkStream);
            
            
            List<long> rootPathIDs = new List<long>();
            using (var sceneFile = File.OpenRead(Path.Combine(Application.dataPath, "level" + sceneInd)))
            {
                var assetsFile = manager.LoadAssetsFile(sceneFile, false);

                // Must be run once for the manager before attempting to read any assets
                manager.LoadClassDatabaseFromPackage(assetsFile.file.Metadata.UnityVersion);

                foreach (var transform in assetsFile.file.GetAssetsOfType(AssetClassID.Transform))
                {
                    var transformFields = manager.GetBaseField(assetsFile, transform, AssetReadFlags.SkipMonoBehaviourFields);
                    
                    if (transformFields["m_Father.m_PathID"].AsLong == 0) // If the transform's parent is null
                    {
                        var goPathID = transformFields["m_GameObject.m_PathID"].AsLong;
                        var goInfo = assetsFile.file.GetAssetInfo(goPathID);
                        var goFields = manager.GetBaseField(assetsFile, goInfo, AssetReadFlags.SkipMonoBehaviourFields);

                        if (goFields["m_Name"].AsString.StartsWith("zone"))
                            rootPathIDs.Add(goPathID);
                    }
                }
            }

            // generate a "loader" bundle using a template bundle so we don't need to generate everything from scratch
            using (Stream bStream = typeof(EntryPoint).Assembly.GetManifestResourceStream("SRLE.templatebundle"))
            {
                // The name can be anything, but you cannot load more than 1 bundle with the same name
                var newBundleName = $"level{sceneInd}loader-{Random.Range(0, ushort.MaxValue + 1):X4}";

                var bundleFile = manager.LoadBundleFile(bStream, newBundleName);

                bundleFile.file.BlockAndDirInfo.DirectoryInfos[0].Name = newBundleName;

                var assetsFile = manager.LoadAssetsFileFromBundle(bundleFile, 0, false);

                // Update assetsfile dependencies to target the game's scene assetsfile
                assetsFile.file.Metadata.Externals.Add(new AssetsFileExternal() { Type = AssetsFileExternalType.Normal, VirtualAssetPathName = "level" + sceneInd, OriginalPathName = "level" + sceneInd, PathName = "level" + sceneInd });

                var assetbundleAsset = assetsFile.file.GetAssetsOfType(AssetClassID.AssetBundle)[0];
                var assetbundleFields = manager.GetBaseField(assetsFile, assetbundleAsset, AssetReadFlags.SkipMonoBehaviourFields);

                // Change assetbundle name to our new name
                assetbundleFields["m_AssetBundleName"].AsString = assetbundleFields["m_Name"].AsString = newBundleName;

                // Add each of the root transforms to the preload list and containers
                for (int i = 0; i < rootPathIDs.Count; i++)
                {
                    var containerItem = ValueBuilder.DefaultValueFieldFromArrayTemplate(assetbundleFields["m_Container.Array"]);
                    assetbundleFields["m_Container.Array"].Children.Add(containerItem);
                    containerItem["first"].AsString = "root" + i;
                    containerItem["second.preloadIndex"].AsInt = i;
                    containerItem["second.preloadSize"].AsInt = 1;
                    containerItem["second.asset.m_FileID"].AsInt = 1;
                    containerItem["second.asset.m_PathID"].AsLong = rootPathIDs[i];

                    var preloadItem = ValueBuilder.DefaultValueFieldFromArrayTemplate(assetbundleFields["m_PreloadTable.Array"]);
                    assetbundleFields["m_PreloadTable.Array"].Children.Add(preloadItem);
                    preloadItem["m_FileID"].AsInt = 1;
                    preloadItem["m_PathID"].AsLong = rootPathIDs[i];
                }

                // Store modified data to assetsfile
                assetbundleAsset.SetNewData(assetbundleFields);

                // Store modified assetsfile to assetbundle
                bundleFile.file.BlockAndDirInfo.DirectoryInfos[0].SetNewData(assetsFile.file);

                // Store assetbundle to file
                using (var file = new MemoryStream())
                using (var writer = new AssetsFileWriter(file))
                {
                    bundleFile.file.Write(writer);
                    return file.ToArray();
                }
            }
        }
    }
}
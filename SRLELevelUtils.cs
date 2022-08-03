using System.IO;
using System.Linq;
using MonomiPark.SlimeRancher.Regions;
using SRLE.Components;
using SRLE.SaveSystem;
using SRML.Console;
using SRML.Utils;
using UnityEngine;

namespace SRLE
{
    public static class SRLELevelUtils
    {
        public static void LoadLevel(Stream stream)
        {
            SRLEName levelSummary = new SRLEName();
            levelSummary.Load(stream);
            switch (levelSummary.worldType)
                {
                    case WorldType.STANDARD:
                        break;
                    case WorldType.SEA:
                    {
                        Object.FindObjectsOfType<Region>().ToList().ForEach(x => x.gameObject.SetActive(false));
                        break;
                    }
                    case WorldType.VOID:
                    {
                        Object.FindObjectsOfType<Region>().ToList().ForEach(x => x.gameObject.SetActive(false));
                        foreach (Transform o in GameObject.Find("zoneSEA").transform)
                        {
                            o.gameObject.SetActive(false);
                        }

                        break;
                    }
                    case WorldType.DESERT:
                    {
                        Object.FindObjectsOfType<Region>().ToList().ForEach(x => x.gameObject.SetActive(false));
                        foreach (Transform o in GameObject.Find("zoneSEA").transform)
                        {
                            o.gameObject.SetActive(false);
                        }

                        foreach (Transform o in GameObject.Find("zoneDESERT").transform)
                        {
                            if (o.gameObject.name == "SandSea")
                            {
                                o.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
                                o.gameObject.SetActive(true);
                                o.GetComponent<ManageWithRegionSet>().setId = RegionRegistry.RegionSetId.HOME;
                            }
                        }

                        break;
                    }
                }

                foreach (var levelSummaryObject in levelSummary.objects)
                {
                    EntryPoint.SRLEConsoleInstance.Log(levelSummaryObject.Key);
                    IdClass idClass = null;
                    foreach (var keyValuePair in SRLEManager.BuildObjects)
                    {
                        if (keyValuePair.Value.TryGetValue(@levelSummaryObject.Key, out var idClass1))
                        {
                            idClass = idClass1;
                            break;
                        }
                    }

                    foreach (var srleSave in levelSummaryObject.Value)
                    {
                        var original = SRSingleton<ContainersOfObject>.Instance.GetObject(idClass.Id);
                        if (original is null) continue;


                        var instantiateInactive = GameObjectUtils.InstantiateInactive(original).transform;
                        foreach (var VARIABLE in srleSave.dictionaryWithProperties)
                        {
                            switch (VARIABLE.Key)
                            {
                                case "JournalText":
                                    instantiateInactive.GetComponent<JournalEntry>().entryKey = "srle." + VARIABLE.Value.property;
                                    break;
                                case "TeleportDestination":
                                    instantiateInactive.GetComponentInChildren<TeleportDestination>().teleportDestinationName = VARIABLE.Value.property;
                                    break;
                                case "TeleportSource":
                                    instantiateInactive.GetComponentInChildren<TeleportSource>().destinationSetName = VARIABLE.Value.property;
                                    break;
                            }
                        }

                        instantiateInactive.localPosition = srleSave.position.value;
                        instantiateInactive.localEulerAngles = srleSave.rotation.value;
                        instantiateInactive.transform.localScale = srleSave.scale.value;
                        var objectAddedBySrle = instantiateInactive.gameObject.AddComponent<ObjectAddedBySRLE>();
                        objectAddedBySrle.id = idClass.Id;
                        objectAddedBySrle.srleSave = srleSave;
                        instantiateInactive.gameObject.SetActive(true);
                    }
            }
        }
        public  static void LoadLevel(SRLEName levelSummary)
        {

            switch (levelSummary.worldType)
                {
                    case WorldType.STANDARD:
                        break;
                    case WorldType.SEA:
                    {
                        Object.FindObjectsOfType<Region>().ToList().ForEach(x => x.gameObject.SetActive(false));
                        break;
                    }
                    case WorldType.VOID:
                    {
                        
                        
                        foreach (var region in Object.FindObjectsOfType<Region>())
                        {
                            //region.cellDir.zoneDirector.regionSetId =
                        }
                        /*foreach (Transform o in GameObject.Find("zoneSEA").transform)
                        {
                            o.gameObject.SetActive(false);
                        }
                        */

                        break;
                    }
                    case WorldType.DESERT:
                    {
                        Object.FindObjectsOfType<Region>().ToList().ForEach(x => x.gameObject.SetActive(false));
                        foreach (Transform o in GameObject.Find("zoneSEA").transform)
                        {
                            o.gameObject.SetActive(false);
                        }

                        foreach (Transform o in GameObject.Find("zoneDESERT").transform)
                        {
                            if (o.gameObject.name == "SandSea")
                            {
                                o.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
                                o.gameObject.SetActive(true);
                                o.GetComponent<ManageWithRegionSet>().setId = RegionRegistry.RegionSetId.HOME;
                            }
                        }

                        break;
                    }
                }

                foreach (var levelSummaryObject in levelSummary.objects)
                {
                    IdClass idClass = null;
                    foreach (var keyValuePair in SRLEManager.BuildObjects)
                    {
                        if (keyValuePair.Value.TryGetValue(@levelSummaryObject.Key, out var idClass1))
                        {
                            idClass = idClass1;
                            break;
                        }
                    }

                    foreach (var srleSave in levelSummaryObject.Value)
                    {
                        var original = SRSingleton<ContainersOfObject>.Instance.GetObject(idClass.Id);
                        if (original is null) continue;


                        var instantiateInactive = GameObjectUtils.InstantiateInactive(original).transform;
                        foreach (var VARIABLE in srleSave.dictionaryWithProperties)
                        {
                            switch (VARIABLE.Key)
                            {
                                case "JournalText":
                                    instantiateInactive.GetComponent<JournalEntry>().entryKey = "srle." + VARIABLE.Value.property;
                                    break;
                                case "TeleportDestination":
                                    instantiateInactive.GetComponentInChildren<TeleportDestination>().teleportDestinationName = VARIABLE.Value.property;
                                    break;
                                case "TeleportSource":
                                    instantiateInactive.GetComponentInChildren<TeleportSource>().destinationSetName = VARIABLE.Value.property;
                                    break;
                            }
                        }

                        instantiateInactive.localPosition = srleSave.position.value;
                        instantiateInactive.localEulerAngles = srleSave.rotation.value;
                        instantiateInactive.transform.localScale = srleSave.scale.value;
                        var objectAddedBySrle = instantiateInactive.gameObject.AddComponent<ObjectAddedBySRLE>();
                        objectAddedBySrle.id = idClass.Id;
                        objectAddedBySrle.srleSave = srleSave;
                        instantiateInactive.gameObject.SetActive(true);
                    }
            }
        }


    }
}
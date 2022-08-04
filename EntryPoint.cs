using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets.Script.Util.Extensions;
using HarmonyLib;
using MonomiPark.SlimeRancher.Persist;
using MonomiPark.SlimeRancher.Regions;
using Newtonsoft.Json;
using RuntimeGizmos;
using SRLE.Components;
using SRLE.SaveSystem;
using SRML;
using SRML.SR;
using SRML.SR.Patches;
using SRML.SR.UI.Utils;
using SRML.Utils.Enum;
using UnityEngine;
using Console = SRML.Console.Console;
using Object = UnityEngine.Object;

namespace SRLE
{
	
	//[EnumHolder]
	public class ConsoleInstance
	{
		internal object ConsoleInstanceSRML = null;
		internal MethodInfo LogMethod = null;
		internal MethodInfo LogWarningMethod = null;
		internal MethodInfo LogErrorMethod = null;

		internal ConsoleInstance(string modName)
		{
			foreach (var type in AccessTools.GetTypesFromAssembly(typeof(SRML.Console.Console).Assembly))
			{
				if (type.Name == "ConsoleInstance")
				{
					ConsoleInstanceSRML = type.GetConstructor(new[]
					{
						typeof(string)
					})?.Invoke(new object[] {modName});
					var type1 = ConsoleInstanceSRML.GetType();
					LogMethod = type1.GetMethod("Log");
					LogWarningMethod = type1.GetMethod("LogWarning");
					LogErrorMethod = type1.GetMethod("LogError");
					break;
				}
			}
		}

		internal void Log(object str, bool savetofile = true)
		{
			if (ConsoleInstanceSRML is not null)
			{
				LogMethod.Invoke(ConsoleInstanceSRML, new[]
				{
					str, savetofile
				});
				return;
			}
#pragma warning disable 618
			Console.Log("[SRLE] " + str.ToString(), savetofile);
		}

		internal void LogWarning(object str, bool savetofile = true)
		{
			if (ConsoleInstanceSRML is not null)
			{
				LogWarningMethod.Invoke(ConsoleInstanceSRML, new[]
				{
					str, savetofile
				});
				return;
			}
			Console.Log("[SRLE] " + str.ToString(), savetofile);		
		}

		internal void LogError(object str, bool savetofile = true)
		{
			if (ConsoleInstanceSRML is not null)
			{
				LogErrorMethod.Invoke(ConsoleInstanceSRML, new[]
				{
					str, savetofile
				});
				return;
			}
			Console.Log("[SRLE] " + str.ToString(), savetofile);
		}
#pragma warning restore 618


	}
	public class EntryPoint : ModEntryPoint
	{
		public static ConsoleInstance SRLEConsoleInstance = new("SRLE");

		public static Assembly execAssembly = Assembly.GetExecutingAssembly();
		public static AssetBundle srleDate;
		public static AssetBundle srlegizmo;
		public static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();



		
		
		public override void PreLoad()
		{

			srleDate = AssetBundle.LoadFromStream(EntryPoint.execAssembly.GetManifestResourceStream(typeof(EntryPoint), "srledata"));
			srlegizmo = AssetBundle.LoadFromFile(@"E:\SlimeRancherModding\Unity\GIzmoHandle\Assets\AssetBundles\srlegizmo");
			


			HarmonyInstance.PatchAll();
			HarmonyInstance.GetPatchedMethods().ToList().ForEach(x => x.Log());
			

			TranslationPatcher.AddUITranslation("l.srle.window_title", "SRLE - Slime Rancher Level Editor");
			TranslationPatcher.AddUITranslation("l.srle.load_a_level", "Load a Level");
			TranslationPatcher.AddUITranslation("l.srle.create_a_level", "Create a Level");
			TranslationPatcher.AddUITranslation("b.srle", "SRLE");



			TranslationPatcher.AddUITranslation("l.srle.level_name", "Level Name");
			TranslationPatcher.AddUITranslation("m.srle.default_level_name", "Level {0}");
			TranslationPatcher.AddUITranslation("l.srle.object_count", "Objects: {0}");
			TranslationPatcher.AddUITranslation("l.srle.filesize", "Size: {0}");
			

			TranslationPatcher.AddUITranslation("l.srle.choose_level_icon", "Choose a Level Icon");
			TranslationPatcher.AddUITranslation("l.srle.select_world_type", "Select a World Type");

			TranslationPatcher.AddUITranslation("l.srle.world_type.sea", "Slime Sea");
			TranslationPatcher.AddUITranslation("l.srle.world_type.desert", "Glass Desert Sea");
			TranslationPatcher.AddUITranslation("l.srle.world_type.void", "Complete Void");
			TranslationPatcher.AddUITranslation("l.srle.world_type.standard", "Standard World");

			TranslationPatcher.AddUITranslation("m.srle.desc.worldType.sea",
				"A blank world with the slime sea for you to build your own ranch.");
			TranslationPatcher.AddUITranslation("m.srle.desc.worldType.desert",
				"A blank world with the Glass Desert's slime sea for you to make your own desert.");
			TranslationPatcher.AddUITranslation("m.srle.desc.worldType.standard",
				"The normal slime rancher map, prebuilt with everything the normal map has.");
			TranslationPatcher.AddUITranslation("m.srle.desc.worldType.void",
				"A completely blank world, no slime sea, no sea bed, no objects, build whatever you want here.");

			TranslationPatcher.AddUITranslation("m.srle.no_saved_levels", "No Saved Levels Available");
			TranslationPatcher.AddUITranslation("m.srle.confirm_delete", "Are you sure you wish to permanently delete this level?");
			
			TranslationPatcher.AddTranslationKey("global","l.srle.presence", "SRLE: {0}");
			
			TranslationPatcher.AddTranslationKey("global","l.srle.richpresence.testing", "Testing the map: {0}");
			TranslationPatcher.AddTranslationKey("global", "l.srle.richpresence.editing", "Editing the map: {0}");


			









			Console.RegisterCommand(new SRLE_CreateLevelCommand());
			Console.RegisterCommand(new SRLE_PlaceObjectCommand());
			Console.RegisterCommand(new SRLE_CreatePropertyForObjectCommand());
			Console.RegisterCommand(new SRLECommand());
			//Console.RegisterCommand(new SRLE_SelectControllerModeCommand());
			Console.RegisterCommandCatcher((cmd, args) =>
			{
				if (cmd != "noclip") return true;
				if (SRSingleton<SRLECamera>.Instance?.isActiveAndEnabled == true)
				{
					"The camera of SRLE is enabled, you can't disable noclip".LogWarning();
					return false;
				}
				return true;
			});

			//Console.RegisterCommand(new SRLE_LoadLevelCommand());

			SRLEManager.LoadObjectsFromBuildObjects();
			//RegionSetRegistry.RegisterRegion(RegionSet.VOID, new BoundsQuadtree<Region>(2000f, Vector3.up * 1000f, 250f, 1.2f));
			IntermodCommunication.RegisterIntermodMethod("AddModdedObject", new Action<GameObject>(SRLEManager.AddModdedObject));

			//RuntimeGizmos.TransformGizmo.SetMaterial 

			//Resources.FindObjectsOfTypeAll<Shader>().ForEach(x => x.name.Log());
			
			foreach (var shader in EntryPoint.srlegizmo.LoadAllAssets<Shader>())
			{
				shader.name.Log();
				shaders.Add(shader.name, shader);
			}
			SRCallbacks.OnMainMenuLoaded += menu =>
			{
				//Discord.RichPresenceHandlerImpl
				
				IntermodCommunication.CallIMCMethod("srle", "AddModdedObject", GameObject.Find("Art/BeatrixMainMenu"));

				//RuntimeGizmos.TransformGizmo.

				/*var srleName = SRLEName.Create(name, WorldType.STANDARD);
					srleName.objects = objects;
					*/
				


					SRLENewLevelUI.allSprites = ZoneDirector.zonePediaIdLookup.Values.Distinct().Select((x, y) => x.FindPediaIcon()).ToList();

					var addMainMenuButton = MainMenuUtils.AddMainMenuButton(menu, "SRLE", () =>
					{
						var instantiateAndWaitForDestroy = UnityEngine.Object.Instantiate(menu.expoSelectGameUI);
						instantiateAndWaitForDestroy.name = "SRLE_UI";
						var expoGameSelectUI = instantiateAndWaitForDestroy.GetComponent<ExpoGameSelectUI>();

						expoGameSelectUI.mainMenuUIPrefab = menu.gameObject;
						expoGameSelectUI.onDestroy = null;
						expoGameSelectUI.onDestroy = () =>
						{
							{
								if (SRLEUIMenu.returnToMenu)
									menu?.gameObject.SetActive(true);
								SRLEUIMenu.returnToMenu = true;
							}
								
						};
						menu.gameObject.SetActive(false);
						
						SRLEUIMenu.SetAllButtonsEXPO(instantiateAndWaitForDestroy, menu);

					});
					
					addMainMenuButton.name = "SRLEButton";
					addMainMenuButton.transform.Find("Text").GetComponent<XlateText>().SetKey("b.srle");
					addMainMenuButton.transform.SetSiblingIndex(4);
				
				
				
			};
			SRCallbacks.OnMainMenuLoaded += menu =>
			{

				SRLEManager.currentData = null;
				SRLEManager.isSRLELevel = false;
				
				


			};
			SRCallbacks.PreSaveGameLoad += context =>
			{
				/*var gameObject = new GameObject("SRLECamera", new []
				{
					typeof(Camera)
				});
				gameObject.AddComponent<SRLECamera>().controller = gameObject.AddComponent<TransformGizmo>();
				*/

				;
				//SRLEManager.isSRLELevel = true;
				//var camera = Camera.main.gameObject;
				//camera.AddComponent<TransformGizmo>();

			};
		}

		public override void Load()
		{
			
			//PediaRegistry.RegisterIdEntry(RegionSet.SOMETHING1, null);
		}

		public override void PostLoad()
		{
		
		}
	}
}
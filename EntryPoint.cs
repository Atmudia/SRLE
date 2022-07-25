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
using Newtonsoft.Json;
using SRLE.Components;
using SRLE.SaveSystem;
using SRML;
using SRML.SR;
using SRML.SR.UI.Utils;
using UnityEngine;
using Console = SRML.Console.Console;

namespace SRLE
{
	public class EntryPoint : ModEntryPoint
	{


		public static Assembly execAssembly = Assembly.GetExecutingAssembly();

		public override void PreLoad()
		{



			

			

			HarmonyInstance.PatchAll();
			TranslationPatcher.AddUITranslation("l.srle.window_title", "SRLE - Slime Rancher Level Editor");
			TranslationPatcher.AddUITranslation("l.srle.load_a_level", "Load a Level");
			TranslationPatcher.AddUITranslation("l.srle.create_a_level", "Create a Level");
			TranslationPatcher.AddUITranslation("b.srle", "SRLE");



			TranslationPatcher.AddUITranslation("l.srle.level_name", "Level Name");
			TranslationPatcher.AddUITranslation("m.srle.default_level_name", "Level{0}");
			TranslationPatcher.AddUITranslation("l.srle.object_count", "Objects: {0}");
			TranslationPatcher.AddUITranslation("l.srle.filesize", "Size: {0}");
			

			TranslationPatcher.AddUITranslation("l.srle.choose_level_icon", "Choose a Level Icon");
			TranslationPatcher.AddUITranslation("l.srle.select_world_type", "Select a World Type");

			TranslationPatcher.AddUITranslation("l.srle.world_type_sea", "Slime Sea");
			TranslationPatcher.AddUITranslation("l.srle.world_type_desert", "Glass Desert Sea");
			TranslationPatcher.AddUITranslation("l.srle.world_type_void", "Complete Void");
			TranslationPatcher.AddUITranslation("l.srle.world_type_standard", "Standard World");

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
			









			Console.RegisterCommand(new SRLE_CreateLevelCommand());
			Console.RegisterCommand(new SRLE_PlaceObjectCommand());
			Console.RegisterCommand(new SRLECommand());
			//Console.RegisterCommand(new SRLE_LoadLevelCommand());

			SRLEManager.LoadObjectsFromBuildObjects();


			SRCallbacks.OnMainMenuLoaded += menu =>
			{
				//SRLEManager.AddModdedObject(GameObject.Find("Art/BeatrixMainMenu"));
				
				
				
				
				


				/*var srleName = SRLEName.Create(name, WorldType.STANDARD);
					srleName.objects = objects;
					*/
				


					SRLENewLevelUI.allSprites = ZoneDirector.zonePediaIdLookup.Values.Distinct().Select(new Func<PediaDirector.Id, int, Sprite>((x, y) => x.FindPediaIcon())).ToList();

					var addMainMenuButton = MainMenuUtils.AddMainMenuButton(menu, "SRLE", () =>
					{
						var instantiateAndWaitForDestroy = UnityEngine.Object.Instantiate(menu.expoSelectGameUI);
						instantiateAndWaitForDestroy.name = "SRLE_UI";
						var expoGameSelectUI = instantiateAndWaitForDestroy.GetComponent<ExpoGameSelectUI>();

						expoGameSelectUI.mainMenuUIPrefab = menu.gameObject;
						expoGameSelectUI.onDestroy = null;
						expoGameSelectUI.onDestroy = () =>
						{
							Console.Log("Need to make that");
							{
								if (menu)
									menu.gameObject.SetActive(true);
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
			SRCallbacks.PreSaveGameLoad += context => { };
		}
	}
}
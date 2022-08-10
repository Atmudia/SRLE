using System.Collections.Generic;
using System.Linq;
using SRML.Console;
using UnityEngine;
using SRML;
namespace SRLE.Components
{
	public class ContainersOfObject : SRSingleton<ContainersOfObject>
	{
		public GameObject GetObject(string id)
		{
			var objectById = SRLEManager.GetObjectById(id);
			if (objectById is null) return null;
			if (transform.Find(objectById.Name) is null)
			{
				if (SRLEManager.currentData.objects.TryGetValue(objectById.Id, out var saves) && saves.First().modid != "none"
					&& !SRModLoader.IsModPresent(saves.First().modid))
				{
					$"Modded object present without mod loaded, please install mod {saves.First().modid}".LogError();
					return null;
				}
				var nameOfZones = Object.FindObjectsOfType<ZoneDirector>().Select(x => x.gameObject.name).ToList();
				var strings = objectById.Path.Split('/').ToList();
				string nameOfZone = strings.FirstOrDefault(x => nameOfZones.Contains(x));
				strings.Remove(nameOfZone);

				var partOfTransform = strings.Aggregate("", (x, y) => x + "/" + y).Remove(0, 1);
				if ( GameObject.Find(nameOfZone).transform.Find(partOfTransform) is null)
				{
					EntryPoint.SRLEConsoleInstance.Log($"Please contact with the SRLE team to resolve this. {objectById.Id}");
					return null;

				}
				else
				{
					return GameObject.Find(nameOfZone).transform.Find(partOfTransform).gameObject;
				}
			}
			
			return transform.Find(objectById.Name).gameObject;
		}
	}
}
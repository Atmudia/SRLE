using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;
using SRLE.SaveSystem;
using Console = SRML.Console.Console;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(ZoneDirector), nameof(ZoneDirector.GetRegionSetId))]
    public class Patch_ZoneDirector_GetRegionSetId
    {
        public static bool Prefix(ZoneDirector.Zone zone, ref RegionRegistry.RegionSetId __result)
        {
            /*if (!SRLEManager.isSRLELevel) return true;
            if (SRLEManager.currentData.worldType == WorldType.VOID)
            {
                //if (listofZones.Contains(zone.ToString())) __result = RegionSet.VOID;
                return true;
            }
            return true;
            */
            return true;
        }

        public static List<string> listofZones = "RANCH|REEF|QUARRY|MOSS|DESERT|SEA|RUINS|RUINS_TRANSITION|WILDS|OGDEN_RANCH|VALLEY|MOCHI_RANCH|SLIMULATIONS|VIKTOR_LAB|NONE".Split('|').ToList();
    }
}
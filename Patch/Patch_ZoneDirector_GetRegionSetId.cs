using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;
using SRLE.SaveSystem;
using SRML.Utils.Enum;
using Console = SRML.Console.Console;

namespace SRLE.Patch
{
    [EnumHolder]
    public class RegionSetEnum
    {
        public static readonly RegionRegistry.RegionSetId VOID;
    }
    [HarmonyPatch(typeof(ZoneDirector), nameof(ZoneDirector.GetRegionSetId))]
    public class Patch_ZoneDirector_GetRegionSetId
    {
        public static bool Prefix(ZoneDirector.Zone zone, ref RegionRegistry.RegionSetId __result)
        {
            RegionSetEnum.VOID.Log();
            if (!SRLEManager.isSRLELevel) return true;
            if (SRLEManager.currentData.worldType == WorldType.VOID)
            {
                if (listofZones.Contains(zone)) __result = RegionSetEnum.VOID;
                return false;
            }
            return true;
        }
        public static List<ZoneDirector.Zone> listofZones = "RANCH|REEF|QUARRY|MOSS|DESERT|SEA|RUINS|RUINS_TRANSITION|WILDS|OGDEN_RANCH|VALLEY|MOCHI_RANCH|SLIMULATIONS|VIKTOR_LAB|NONE".Split('|').ToList().ToEnumList<ZoneDirector.Zone>();


    }
}
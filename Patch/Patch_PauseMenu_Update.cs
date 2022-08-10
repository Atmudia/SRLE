using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
namespace SRLE.Patch
{
    [HarmonyPatch(typeof(PauseMenu), nameof(PauseMenu.Update))]
    public class Patch_PauseMenu_Update
    {
        public static bool Prefix(PauseMenu __instance)
        {
            if (SRLEManager.isSRLELevel) return false;
            return true;
        }
    }
}

using System;
using HarmonyLib;
using RichPresence;
using SRLE.Components;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(Discord.RichPresenceHandlerImpl), nameof(Discord.RichPresenceHandlerImpl.SetRichPresence), typeof(InZoneData))]
    public static class Patch_RichPresenceHandlerImpl_SetRichPresence
    {
        public static bool Prefix() => true;
        
        
        /*{
            if (!SRLEManager.isSRLELevel) return true;
            if (SRLEManager.currentData is null) return true;

            var replace = SRLEManager.currentData.nameOfFile.Replace(".srle", string.Empty);
            MessageDirector messageDirector = SRSingleton<GameContext>.Instance.MessageDirector;

            string isTestingOrEditing = "";
            if (SRLECamera.Instance?.isActiveAndEnabled == false)
            {
                isTestingOrEditing = "l.srle.richpresence.testing";
            }
            else
            {
                isTestingOrEditing =  "l.srle.richpresence.editing";
            }
            DiscordRpc.UpdatePresence(new DiscordRpc.RichPresence()
            {
                details = messageDirector.Get("global", isTestingOrEditing, replace),//string.Format("l.srle.presence")),
                largeImageKey = "zone-ranch-large",
                state = "SRLE: " + messageDirector.Get("ui", $"l.srle.world_type.{SRLEManager.currentData.worldType.ToString().ToLower()}")
                
            });
            
            return false;
        }
        */
    }
}
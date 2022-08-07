using MonomiPark.SlimeRancher.Persist;
using SRLE.SaveSystem;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Console = SRML.Console.Console;

namespace SRLE
{
    internal static class Extensions
    {
        public static SRLESave ToSRLESave(this Transform t)
        {
            SRLESave save = new SRLESave();
            var srleSave = new SRLESave();
            (srleSave.position = new Vector3V02()).value = t.position;
            (srleSave.rotation = new Vector3V02()).value = t.rotation.eulerAngles;
            (srleSave.scale = new Vector3V02()).value = t.transform.localScale;
            return save;
        }

        public static Button RemoveAllListeners(this Button button)
        {
            button.onClick.RemoveAllListeners();
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                if (button.onClick.GetPersistentMethodName(i) != "PlayClick")
                    button.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
            return button;

        }

        public static Sprite FindPediaIcon(this PediaDirector.Id id)
        {
            return SRSingleton<SceneContext>.Instance.PediaDirector.entries.FirstOrDefault((PediaDirector.IdEntry x) => x.id == id)?.icon;
        }

        public static Transform GetChild(this GameObject gObject, int childInt)
        {
            Transform transfromTemp = null;
            try
            {
                transfromTemp = gObject.transform.GetChild(childInt);
            }
            catch (UnityException e)
            {
                EntryPoint.SRLEConsoleInstance.Log(e.Message);
                return null;
            }

            return transfromTemp;
        }

        public static Texture2D LoadPNG(string filePath)
        {

            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            return tex;
        }

        public static void ClearChildren(this Transform t)
        {
            foreach (Transform child in t)
            {
                Destroyer.Destroy(child, "ClearChildren");
            }
        }
        private const long OneKb = 1024;
        private const long OneMb = OneKb * 1024;
        private const long OneGb = OneMb * 1024;
        private const long OneTb = OneGb * 1024;

        public static string ToPrettySize(this int value, int decimalPlaces = 0)
        {
            return ((long)value).ToPrettySize(decimalPlaces);
        }

        public static string ToPrettySize(this long value, int decimalPlaces = 0)
        {
            var asTb = Math.Round((double)value / OneTb, decimalPlaces);
            var asGb = Math.Round((double)value / OneGb, decimalPlaces);
            var asMb = Math.Round((double)value / OneMb, decimalPlaces);
            var asKb = Math.Round((double)value / OneKb, decimalPlaces);
            string chosenValue = asTb > 1 ? $"{asTb}Tb"
                : asGb > 1 ? $"{asGb}Gb"
                : asMb > 1 ? $"{asMb}Mb"
                : asKb > 1 ? $"{asKb}Kb"
                : $"{Math.Round((double) value, decimalPlaces)}B";
            return chosenValue;
        }
        public static void Log(this object str)
        {
            EntryPoint.SRLEConsoleInstance.Log(str.ToString());
        }
        public static void LogWarning(this object str)
        {
            EntryPoint.SRLEConsoleInstance.LogWarning(str.ToString());
        }
        public static void LogError(this object str)
        {
            EntryPoint.SRLEConsoleInstance.LogError(str.ToString());
        }
    }
}
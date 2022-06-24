using System;
using System.Linq;
using DebuggingMod.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Console = SRML.Console.Console;

namespace SRLE
{
    internal static class Extensions
    {

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
                Console.Log(e.Message);
                gObject.PrintComponents();

                return null;
            }

            return transfromTemp;
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
        public static void Log(this string str)
        {
            Console.Log(str);
        }
        public static void LogWarning(this string str)
        {
            Console.LogWarning(str);
        }
        public static void LogError(this string str)
        {
            Console.LogError(str);
        }
    }
}
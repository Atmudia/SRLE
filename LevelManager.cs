using UnityEngine;

namespace SRLE
{
    public static class LevelManager
    {
        public enum Mode
        {
            NONE,
            BUILD
        }

        public static Mode CurrentMode { get; private set; } = Mode.NONE;
        public static bool IsLoading = false;
        public static GameObject SRLEGameObject;

        public static void SetMode(Mode mode)
        {
            CurrentMode = mode; 
        }
    }
}
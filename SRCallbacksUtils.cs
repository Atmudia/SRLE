using System;
using SRML.SR;

namespace SRLE
{
    internal static class SRCallbacksUtils
    {
        private static Action<SceneContext> OnInvoke = null;
        public static void AddSRCallbacksAndDeleteAfterLoading(Action<SceneContext> ctx)
        {
            OnInvoke = ctx;
            if (OnInvoke is not null)
                SRCallbacks.OnSaveGameLoaded += ONSceneLoaded;
        }

        private static void ONSceneLoaded(SceneContext ctx)
        {
            OnInvoke.Invoke(ctx);
            SRCallbacks.OnSaveGameLoaded -= ONSceneLoaded;
        }
    }
}
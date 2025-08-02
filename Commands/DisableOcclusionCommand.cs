using SRML.Console;
using UnityEngine;

namespace SRLE.Commands
{
    public class DisableOcclusionCommand : ConsoleCommand
    {
        public override bool Execute(string[] args)
        {
            foreach (var renderer in Camera.allCameras)
            {
                renderer.useOcclusionCulling = false;
            }

            return true;
        }

        public override string ID => "disable_occlusion";
        public override string Usage => ID;
        public override string Description => ID;
    }
}
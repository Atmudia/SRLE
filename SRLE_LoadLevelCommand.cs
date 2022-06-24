using System.IO;
using SRLE.SaveSystem;
using SRML.Console;
using UnityEngine;
using static SRLE.SRLEManager;

namespace SRLE
{
    public class SRLE_LoadLevelCommand : ConsoleCommand
    {
        public override bool Execute(string[] args)
        {
           
            isSRLELevel = true;
            FileInfo fileInfo = new FileInfo(Worlds.FullName + "\\" + args[0] + ".srle");
            var fileStream = fileInfo.Open(FileMode.Open);
            SRLEName srleName = new SRLEName();
            srleName.Load(fileStream);
            currentData = srleName;
            isSRLELevel = true;
            srleName.Write(fileStream);
            fileStream.Dispose();
            SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame("", Identifiable.Id.HEN, PlayerState.GameMode.CASUAL, () => {});
            return true;
        }

        public override string ID => nameof(SRLE_LoadLevelCommand).Replace("Command", string.Empty).ToLower();
        public override string Usage => ID;
        public override string Description => ID;
    }
}
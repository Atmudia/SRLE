using System.IO;
using SRLE.SaveSystem;
using SRML.Console;
using static SRLE.SRLEManager;

namespace SRLE
{
    public class SRLE_CreateLevelCommand : ConsoleCommand
    {
        public override bool Execute(string[] args)
        {
            SRLEName srleName = SRLEName.Create(args[0], WorldType.STANDARD);
            currentData = srleName;
            isSRLELevel = true;
            FileInfo fileInfo = new FileInfo(Worlds.FullName + "\\" + args[0] + ".srle");
            var fileStream = fileInfo.Open(FileMode.Create);
            srleName.Write(fileStream);
            fileStream.Dispose();
            SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame("", Identifiable.Id.HEN, PlayerState.GameMode.CASUAL, () => {});
            return true;
            
        }

        public override string ID => nameof(SRLE_CreateLevelCommand).Replace("Command", string.Empty).ToLower();
        public override string Usage => ID;
        public override string Description => ID;
    }
}
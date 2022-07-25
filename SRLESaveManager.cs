using System.Collections.Generic;
using System.IO;
using SRLE.SaveSystem;

namespace SRLE
{
    public class SRLESaveManager
    {

        public static Dictionary<string, SRLEName> AvailableGames()
        {
            Dictionary<string, SRLEName> srleNames = new Dictionary<string, SRLEName>();
            var fileInfos = SRLEManager.Worlds.GetFiles();
            foreach (var fileInfo in fileInfos)
            {
                if (fileInfo.Extension != ".srle") continue;
                SRLEName srleName = new SRLEName();
                using var fileStream = fileInfo.Open(FileMode.Open);
                srleName.Load(fileStream);
                srleName.nameOfFile = fileInfo.Name;
                srleNames.Add(srleName.nameOfLevel, srleName);
            }

            return srleNames;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SRLE.SaveSystem;
using SRML.Utils;

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

        public static System.Tuple<bool, List<string>> GetRequirementOfLevel(SRLEName srleName)
        {
            if (srleName.isUsingModdedObjects)
            {
                List<string> listOfModID = new List<string>();
                List<string> listOfModID1 = new List<string>();

                foreach (var VARIABLE1 in from VARIABLE in srleName.objects.Values from VARIABLE1 in VARIABLE where !VARIABLE1.modid.IsNullOrEmpty() where !listOfModID.Contains(VARIABLE1.modid) select VARIABLE1)
                {
                    listOfModID.Add(VARIABLE1.modid);
                    listOfModID1.Add(VARIABLE1.modid);
                }

                foreach (var VARIABLE in from VARIABLE in listOfModID let check = SRML.SRModLoader.IsModPresent(VARIABLE) where SRML.SRModLoader.IsModPresent(VARIABLE) select VARIABLE)
                {
                    listOfModID1.Remove(VARIABLE);
                }

                if (listOfModID1.Count == 0)
                {
                    return new System.Tuple<bool, List<string>>(false, new List<string>());
                }
                return new System.Tuple<bool, List<string>>(true, listOfModID1);
            }
            return null;
        }
    }
}
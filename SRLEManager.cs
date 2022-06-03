using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SRLE
{
    public static class SRLEManager
    {
        public static Dictionary<ulong, IdClass> BuildObjects = new Dictionary<ulong, IdClass>();

        public static void LoadObjectsFromBuildObjects()
        {
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(File.ReadAllText(@"E:\SteamLibrary\steamapps\common\Slime Rancher\SRLE\BuildObjects\buildobjects.txt"));
            foreach (var category in categories)
            {
                foreach (var idClass in category.Objects)
                {
                    BuildObjects.Add(idClass.Id, idClass);
                }
            }
        }
        
    }
}
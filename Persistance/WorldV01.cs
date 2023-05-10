using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher.Persist;
using SRLE.Collections;
using BinaryReader = Il2CppSystem.IO.BinaryReader;
using BinaryWriter = Il2CppSystem.IO.BinaryWriter;

namespace SRLE.Persistance
{
    //[RegisterInIL2CPP]
    public class WorldV01 : PersistedDataSet
    {
        public string name;
        public Dictionary<uint, List<BuildObjectV04>> buildObjects;

        public override string Identifier => "BBW";

        public override uint Version
        {
            get
            {
                return 1;
            }
        }

        public WorldV01() { }

        public override void LoadData(BinaryReader reader)
        {
            name = reader.ReadString();
            
            buildObjects = LoadDictionary<uint, List<BuildObjectV04>>(reader, new System.Func<BinaryReader, uint>(binaryReader => binaryReader.ReadUInt32()), new System.Func<BinaryReader, List<BuildObjectV04>>(binaryReader => LoadList<BuildObjectV04>(binaryReader).ToMonoList())).ToMonoDictionary(); }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write(name);

            
            var il2cppData = new Il2CppSystem.Collections.Generic.Dictionary<uint, Il2CppSystem.Collections.Generic.List<BuildObjectV04>>();
            foreach (var buildObject in buildObjects)
            {
                il2cppData.Add(buildObject.Key, buildObject.Value.WrapToIl2Cpp().Cast<Il2CppSystem.Collections.Generic.List<BuildObjectV04>>());
            }
                
            WriteDictionary(
                writer, 
                il2cppData,
                new System.Action<BinaryWriter, uint>((binaryWriter, u) => binaryWriter.Write(u)), 
                new System.Action<BinaryWriter, Il2CppSystem.Collections.Generic.List<BuildObjectV04>>((binaryWriter, v04s) =>
                {
                    v04s.GetType().Log();
                    WriteList<BuildObjectV04>(binaryWriter, null);
                }));
            
            /*
             
         

            //WriteDictionary(writer, buildObjects, (w, k) => w.Write(k), (w, v) => WriteList(w, v));
            */
        }
        
    }
}

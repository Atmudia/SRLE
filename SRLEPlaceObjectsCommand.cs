using System;
using System.Collections.Generic;
using System.IO;
using MonomiPark.SlimeRancher.Persist;
using SRLE.SaveSystem;
using SRML.Console;
using SRML.Console.Commands;
using SRML.Utils;
using UnityEngine;

namespace SRLE
{
    public class SRLEPlaceObjectsCommand : ConsoleCommand
    {
        public override bool Execute(string[] args)
        {
            var @ulong = ulong.Parse(args[0]);
            
            
            DirectoryInfo Worlds = new DirectoryInfo(Environment.CurrentDirectory + "/SRLE/Worlds");
            SRLEName srleName = new SRLEName();
            var fileStream = new FileInfo(Worlds.FullName + @"\Testing.srle");
            var stream = fileStream.Open(FileMode.OpenOrCreate);
            srleName.Load(stream);
            
            var srleSaves = new List<SRLESave>();
            
            SRLEManager.BuildObjects.TryGetValue(@ulong, out IdClass idClass);
            var instantiateInactive = GameObjectUtils.InstantiateInactive(GameObject.Find(idClass.Path));
            var transform = SRSingleton<SceneContext>.Instance.Player.transform;
            instantiateInactive.transform.position = transform.position;
            var srleSave = new SRLESave();
            (srleSave.position = new Vector3V02()).value = transform.position;
            (srleSave.rotation = new Vector3V02()).value = instantiateInactive.transform.rotation.eulerAngles;
            (srleSave.scale = new Vector3V02()).value = instantiateInactive.transform.localScale;
            srleSaves.Add(srleSave); 
            instantiateInactive.SetActive(true);
            SRLEId o = new SRLEId {id = @ulong};
            srleName.objects.Add(o, srleSaves);            
            stream.Dispose();

            srleName.Write(new FileInfo(Worlds.FullName + @"\Testing.srle").Open(FileMode.OpenOrCreate));
            
            

            return true;
        }

        public override string ID => "place_objects";
        public override string Usage => ID;
        public override string Description => ID;
    }
}
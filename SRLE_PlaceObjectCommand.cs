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
    /*public class SRLE_PlaceObjectCommand : ConsoleCommand
    {
        public override bool Execute(string[] args)
        {
            var @ulong = ulong.Parse(args[0]);


            var currentData = SRLEManager.currentData;
            DirectoryInfo Worlds = new DirectoryInfo(Environment.CurrentDirectory + "/SRLE/Worlds");

            
            SRLEManager.BuildObjects.TryGetValue(@ulong, out IdClass idClass);
            var instantiateInactive = GameObjectUtils.InstantiateInactive(GameObject.Find(idClass.Path));
            var transform = SRSingleton<SceneContext>.Instance.Player.transform;
            instantiateInactive.transform.position = transform.position;
            var srleSave = new SRLESave();
            (srleSave.position = new Vector3V02()).value = transform.position;
            (srleSave.rotation = new Vector3V02()).value = instantiateInactive.transform.rotation.eulerAngles;
            (srleSave.scale = new Vector3V02()).value = instantiateInactive.transform.localScale;


            if (currentData.objects.ContainsKey(@ulong))
                currentData.objects[@ulong].Add(srleSave);
            else
            {
                currentData.objects.Add(@ulong, new List<SRLESave> {srleSave});
            }
                
            instantiateInactive.SetActive(true);

            currentData.Write(new FileInfo(Worlds.FullName + @"\Testing.srle").Open(FileMode.OpenOrCreate));
            
            

            return true;
        }
        

        public override string ID => nameof(SRLE_PlaceObjectCommand).Replace("Command", string.Empty).ToLower();
        public override string Usage => ID;
        public override string Description => ID;
    }
    */
}
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonomiPark.SlimeRancher.Persist;
using SRLE.SaveSystem;
using SRML.Console;
using SRML.Console.Commands;
using SRML.Utils;
using UnityEngine;
using Console = SRML.Console.Console;

namespace SRLE
{
    public class SRLE_PlaceObjectCommand : ConsoleCommand
    {
        public override bool Execute(string[] args)
        {
            var @ulong = ulong.Parse(args[0]);


            var currentData = SRLEManager.currentData;



            IdClass idClass = null;
            foreach (var keyValuePair in SRLEManager.BuildObjects)
            {
                if (keyValuePair.Value.TryGetValue(@ulong, out var idClass1))
                {
                    idClass = idClass1;
                    Console.Log(idClass.Path);
                    break;
                }
                
            }
            Console.Log(idClass.Path);
            var instantiateInactive = GameObjectUtils.InstantiateInactive(GameObject.Find(idClass.Path));
            Console.Log("TEST");

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

            currentData.Write(new FileInfo(SRLEManager.Worlds.FullName + currentData.nameOfLevel + ".srle").Open(FileMode.OpenOrCreate));
            
            

            return true;
        }
        

        public override string ID => nameof(SRLE_PlaceObjectCommand).Replace("Command", string.Empty).ToLower();
        public override string Usage => ID;
        public override string Description => ID;
    }
    
}
using System.Collections.Generic;
using System.Linq;
using SRLE.Components;
using SRML.Console;
using SRML.Console.Commands;
using UnityEngine;

namespace SRLE
{
    public class SRLE_CreatePropertyForObjectCommand : ConsoleCommand 
    {
        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex == 0)
            {
                return  "HobsonNote|TeleportSource|TeleportDestination".Split('|').ToList();
            }

            return new List<string>();
        }

        public override bool Execute(string[] args)
        {
           
            Object.Instantiate(EntryPoint.srleDate.LoadAsset<GameObject>("Canvas"));

            
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hitInfo))
            {
                var hitInfoCollider = hitInfo.collider;
                var objectAddedBySrle = hitInfoCollider.GetComponent<ObjectAddedBySRLE>();
                switch (args[0])
                {
                    case "HobsonNote":
                    {
                        /*if (hitInfoCollider.GetComponent<JournalEntry>() is null)
                        {
                            Console.Log("The object is not Hobson Note");
                            return false;

                        }
                        hitInfoCollider.GetComponent<JournalEntry>().entryKey = $"srle.{args[1]}";
                        */
                        //Object.Instantiate(EntryPoint.srleData.LoadAsset<GameObject>("Canvas"));
                        break;
                    }
                }
                
                
                
            }


            return true;
        }
        

        public override string ID => nameof(SRLE_CreatePropertyForObjectCommand).Replace("Command", string.Empty).ToLower();
        public override string Usage => ID;
        public override string Description => ID;
    }
}
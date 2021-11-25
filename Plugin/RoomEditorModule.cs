using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using ThunderRoad;

namespace DungeonConfigurator
{
    public class RoomEditorModule : MonoBehaviour
    {
        public int additional_npc_count = 0;

        public void apply_changes()
        {
            Catalog.gameData.platformParameters.maxRoomNpc = int.MaxValue;
            foreach (Room room in Level.current.dungeon.rooms)
            {
                Logger.Detailed("Starting creature spawning coroutine for room {0}", room.name);
                this.StartCoroutine(spawn_until_full(room));
            }
        }

        private IEnumerator spawn_until_full(Room room)
        {
            System.Random rand = new System.Random();
            room.spawnerMaxNPC += additional_npc_count;
            var spawners = room.GetComponentsInChildren<CreatureSpawner>();
            if(spawners != null && spawners.Length > 0)
            {
                while (room.spawnerNPCCount != room.spawnerMaxNPC)
                {
                    CreatureSpawner spawner = spawners[rand.Next(spawners.Length)];
                    if (!spawner.spawning)
                    {
                        spawner.Spawn();
                        if (spawner.spawning)
                        {
                            room.spawnerNPCCount++;
                            Logger.Detailed("Spawn CR: spawning creature for room {0}", room.name);
                        }
                    }
                    else
                    {
                        Logger.Detailed("Spawn CR: Waiting for end of frame for room {0}", room.name);
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            Logger.Detailed("Spawn CR: Finished for room {0}", room.name);
        }

    }
}

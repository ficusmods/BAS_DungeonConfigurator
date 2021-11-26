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
        public int additional_room_npc_count = 0;
        public int additional_wave_npc_count = 0;
        public int additional_wave_alive_npc_count = 0;

        public void apply_changes()
        {
            Catalog.gameData.platformParameters.maxRoomNpc = int.MaxValue;
            if (additional_room_npc_count > 0)
            {
                foreach (Room room in Level.current.dungeon.rooms)
                {
                    room.spawnerMaxNPC += additional_room_npc_count;
                    Logger.Detailed("Starting creature spawning coroutine for room {0}", room.name);
                    this.StartCoroutine(spawn_until_full(room));
                }
            }
            if (additional_wave_npc_count > 0)
            {
                System.Random rand = new System.Random();
                foreach (WaveSpawner spawner in WaveSpawner.instances)
                {
                    var groups = spawner.waveData.groups;
                    if (groups.Count > 0)
                    {
                        for (int i = 0; i < additional_wave_npc_count; i++)
                        {
                            groups.Add(groups[rand.Next(groups.Count)]);
                        }
                    }
                }
            }
            if (additional_wave_alive_npc_count > 0)
            {
                foreach (WaveSpawner spawner in WaveSpawner.instances)
                {
                    spawner.waveData.maxAlive += additional_wave_alive_npc_count;
                }
            }
        }

        private IEnumerator spawn_until_full(Room room)
        {
            System.Random rand = new System.Random();
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

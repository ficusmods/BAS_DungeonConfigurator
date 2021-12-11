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

        List<string> waveAlreadyAddedNPCs = new List<string>();

        public void apply_changes()
        {
            Catalog.gameData.platformParameters.maxRoomNpc = int.MaxValue;
            Catalog.gameData.platformParameters.maxWaveAlive = int.MaxValue;

            if (Level.current.dungeon != null)
            {
                foreach (Room room in Level.current.dungeon.rooms)
                {
                    if (additional_room_npc_count > 0)
                    {
                        room.spawnerMaxNPC += additional_room_npc_count;
                        Logger.Detailed("Starting creature spawning coroutine for room {0}", room.name);
                        this.StartCoroutine(spawn_until_full(room));
                    }
                    if (additional_wave_npc_count > 0)
                    {
                        System.Random rand = new System.Random();
                        foreach (WaveSpawner spawner in room.GetComponentsInChildren<WaveSpawner>(true))
                        {
                            if (!waveAlreadyAddedNPCs.Contains(spawner.startWaveId))
                            {
                                WaveData wData = Catalog.GetData<WaveData>(spawner.startWaveId);
                                if (wData != null)
                                {
                                    Logger.Detailed("Adding {0} additional groups to {1} in room {2}", additional_wave_npc_count, spawner.name, room.name);
                                    if (wData.groups.Count > 0)
                                    {
                                        for (int i = 0; i < additional_wave_npc_count; i++)
                                        {
                                            wData.groups.Add(wData.groups[rand.Next(wData.groups.Count)]);
                                        }
                                        waveAlreadyAddedNPCs.Add(spawner.startWaveId);
                                    }
                                }
                            }
                        }
                    }
                    if (additional_wave_alive_npc_count > 0)
                    {
                        foreach (WaveSpawner spawner in room.GetComponentsInChildren<WaveSpawner>(true))
                        {
                            spawner.waveData.maxAlive += additional_wave_alive_npc_count;
                            if (additional_room_npc_count == 0)
                            {
                                room.spawnerMaxNPC += additional_wave_alive_npc_count;
                            }
                            Logger.Detailed("Alive count for wave {0} in room {1} set to {2}", spawner.name, room.name, spawner.waveData.maxAlive);
                        }
                    }
                }
            }
        }

        private IEnumerator spawn_until_full(Room room)
        {
            Logger.Detailed("Spawn CR: Creatures in room {0}: {1}/{2}", room.name, room.spawnerNPCCount, room.spawnerMaxNPC);
            var spawners = room.GetComponentsInChildren<CreatureSpawner>(true);
            
            if (spawners != null && spawners.Length > 0)
            {
                while (room.spawnerNPCCount <= room.spawnerMaxNPC)
                {
                    CreatureSpawner spawner = spawners[UnityEngine.Random.Range(0, spawners.Length-1)];
                    if (!spawner.spawning)
                    {
                        spawner.Spawn();
                        if (spawner.spawning)
                        {
                            room.spawnerNPCCount++;
                            Logger.Detailed("Spawn CR: Spawning creautre via {0} in room {1} {2}/{3}", spawner.name, room.name, room.spawnerNPCCount, room.spawnerMaxNPC);
                            yield return new WaitForSeconds(1);
                        }
                    }
                    else
                    {
                        yield return new WaitForSeconds(1);
                    }
                }
            }

            Logger.Detailed("Spawn CR: Finished for room {0}", room.name);
        }
    }
}

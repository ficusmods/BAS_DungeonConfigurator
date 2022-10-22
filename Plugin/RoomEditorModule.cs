
using ThunderRoad;
using UnityEngine;

namespace DungeonConfigurator
{
    public class RoomEditorModule : MonoBehaviour
    {
        public int additional_room_npc_count = 0;
        public int additional_wave_npc_count = 0;
        public int additional_wave_alive_npc_count = 0;

        private void alter_level()
        {
            if (additional_wave_npc_count > 0)
            {
                System.Random rand = new System.Random();

                WaveData wData = Catalog.GetData<WaveData>("DungeonConfiguratorGeneral");
                if (wData != null)
                {
                    Logger.Detailed("Adding {0} additional groups to {1}", additional_wave_npc_count, wData.id);
                    if (wData.groups.Count > 0)
                    {
                        int oriCount = wData.groups.Count;
                        for (int i = 0; i < additional_wave_npc_count; i++)
                        {
                            WaveData.Group currGroup = wData.groups[rand.Next(oriCount)].CloneJson();
                            currGroup.minMaxCount = new Vector2Int(1, 1);
                            wData.groups.Add(currGroup);
                        }
                    }
                }

                wData = Catalog.GetData<WaveData>("DungeonConfiguratorDuel");
                if (wData != null)
                {
                    Logger.Detailed("Adding {0} additional groups to {1}", additional_wave_npc_count, wData.id);
                    if (wData.groups.Count > 0)
                    {
                        int oriCount = wData.groups.Count;
                        for (int i = 0; i < additional_wave_npc_count; i++)
                        {
                            WaveData.Group currGroup = wData.groups[rand.Next(oriCount)].CloneJson();
                            currGroup.minMaxCount = new Vector2Int(1, 1);
                            wData.groups.Add(currGroup);
                        }
                    }
                }
            }
            if (additional_wave_alive_npc_count > 0)
            {
                WaveData wData = Catalog.GetData<WaveData>("DungeonConfiguratorGeneral");
                if (wData != null)
                {
                    wData.totalMaxAlive = 4 + additional_wave_alive_npc_count;
                    foreach (WaveData.WaveFaction factionData in wData.factions)
                    {
                        factionData.factionMaxAlive = wData.totalMaxAlive;
                    }
                    Logger.Detailed("Alive count for wave {0} set to {1}", wData.id, wData.totalMaxAlive);
                }

                wData = Catalog.GetData<WaveData>("DungeonConfiguratorDuel");
                if (wData != null)
                {
                    wData.totalMaxAlive = 1 + additional_wave_alive_npc_count;
                    foreach (WaveData.WaveFaction factionData in wData.factions)
                    {
                        factionData.factionMaxAlive = wData.totalMaxAlive;
                    }
                    Logger.Detailed("Alive count for wave {0} set to {1}", wData.id, wData.totalMaxAlive);
                }
            }
        }
        private void alter_dungeon()
        {
            foreach (Room room in Level.current.dungeon.rooms)
            {
                if (additional_room_npc_count > 0)
                {
                    room.spawnerMaxNPC += additional_room_npc_count;
                    Logger.Detailed("Adding module creature spawner module to room {0}", room.name);
                    add_spawner_module(room);
                }

                if (additional_wave_npc_count > 0)
                {
                    System.Random rand = new System.Random();
                    WaveSpawner[] spawners = room.GetComponentsInChildren<WaveSpawner>(true);
                    if (spawners.Length > 0)
                    {
                        int addToEach = Mathf.CeilToInt((float)additional_wave_npc_count / (float)spawners.Length);
                        foreach (WaveSpawner spawner in spawners)
                        {
                            if (Catalog.GetData<WaveData>(spawner.startWaveId) != null)
                            {
                                WaveData wData = Catalog.GetData<WaveData>(spawner.startWaveId).CloneJson();
                                Logger.Detailed("Adding {0} additional groups to {1} in room {2}", additional_wave_npc_count, spawner.name, room.name);
                                if (wData.groups.Count > 0)
                                {
                                    int oriCount = wData.groups.Count;
                                    for (int i = 0; i < addToEach; i++)
                                    {
                                        WaveData.Group currGroup = wData.groups[rand.Next(oriCount)].CloneJson();
                                        currGroup.minMaxCount = new Vector2Int(1, 1);
                                        wData.groups.Add(currGroup);
                                    }
                                }
                                wData.OnCatalogRefresh();
                                spawner.waveData = wData;
                            }
                        }
                    }

                    if (additional_wave_alive_npc_count > 0)
                    {
                        foreach (WaveSpawner spawner in room.GetComponentsInChildren<WaveSpawner>(true))
                        {
                            if (spawner.waveData == null) continue;
                            spawner.waveData.totalMaxAlive = 4 + additional_wave_alive_npc_count;
                            room.spawnerMaxNPC = Mathf.Max(spawner.waveData.totalMaxAlive, room.spawnerMaxNPC);
                            foreach (WaveData.WaveFaction factionData in spawner.waveData.factions)
                            {
                                factionData.factionMaxAlive = room.spawnerMaxNPC;
                            }
                            Logger.Detailed("Alive count for wave in room {0} set to {1}", room.name, room.spawnerMaxNPC);
                        }
                    }
                }
            }
        }

        public void apply_changes()
        {
            if (Level.current.dungeon != null)
            {
                alter_dungeon();
            }
            else
            {
                alter_level();
            }
        }

        private void add_spawner_module(Room room)
        {
            room.gameObject.AddComponent<RoomSpawnerModule>();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AI;

using ThunderRoad;

namespace DungeonConfigurator
{
    public class RoomSpawnerModule : MonoBehaviour
    {
        Room room;
        List<CreatureSpawner> all_spawners;

        int reserved_spawn_count = 0;

        private void Awake()
        {
            room = GetComponentInParent<Room>();
            all_spawners = room.GetComponentsInChildren<CreatureSpawner>(true).ToList();
            if (all_spawners.Count == 0)
            {
                GameObject.Destroy(this);
            }
            else
            {
                StartCoroutine(SpawnRoutine());
            }
        }

        private IEnumerator SpawnRoutine()
        {
            bool toSpawn = true;
            while (toSpawn)
            {
                if (!room.isCulled)
                {
                    if (room.spawnerNPCCount + reserved_spawn_count <= room.spawnerMaxNPC)
                    {
                        CreatureSpawner spawner = all_spawners[UnityEngine.Random.Range(0, all_spawners.Count)];
                        if (spawner.CurrentState != CreatureSpawner.State.Spawning)
                        {
                            Logger.Detailed("Trying to spawn creature in room {0} {1}/{2} via {3}", room.name, room.spawnerNPCCount + reserved_spawn_count + 1, room.spawnerMaxNPC, spawner.name);
                            if (SpawnNear(spawner))
                            {
                                reserved_spawn_count++;
                            }
                        }
                    }
                    else
                    {
                        if (reserved_spawn_count == 0)
                        {
                            toSpawn = false;
                        }
                    }
                }
                yield return new WaitForSeconds(1.0f);
            }
            Logger.Detailed("Spawner in room {0} finished.", room.name);
            GameObject.Destroy(this);
        }

        private bool SpawnNear(CreatureSpawner spawner)
        {
            if (FieldAccess.WriteProperty(spawner, "CurrentState", CreatureSpawner.State.Init))
            {
                if (FindFreeSpotNear(spawner.transform.position, out Vector3 spot))
                {
                    if (NavMesh.SamplePosition(spot, out NavMeshHit navHit, Config.cfg_spawner_navsampling_distance, -1))
                    {
                        Logger.Detailed("Successful sampling for spawner {0} in room {1}", spawner.name, room.name);
                        spawner.Spawn(delegate
                        {
                            Logger.Detailed("Spawned creature in room {0} via {1}", room.name, spawner.name);
                            room.spawnerNPCCount++;
                            if (FieldAccess.Read<List<CreatureSpawner.SpawnedCreature>>(spawner, "spawnedCreatures", out List<CreatureSpawner.SpawnedCreature> spawnedCreatures))
                            {
                                Creature spawned = spawnedCreatures.Last().creature;
                                room.creatures.Add(spawned);
                                spawned.Teleport(spot, spawned.transform.rotation);
                            }
                            reserved_spawn_count--;
                        });
                        return true;
                    }
                }
            }
            Logger.Detailed("Failed sampling for spawner {0} in room {1}", spawner.name, room.name);
            return false;
        }

        private bool IsFreeSpot(Vector3 pos)
        {
            return room.creatures.All(creature => Vector3.Distance(creature.transform.position, pos) >= Config.cfg_spawner_sampling_block_distance);
        }

        private bool FindFreeSpotNear(Vector3 pos, out Vector3 spot)
        {
            return FindFreeSpotNear_Impl(pos, out spot, 0, new HashSet<Vector3Int>());
        }

        private bool FindFreeSpotNear_Impl(Vector3 point, out Vector3 spot, int retryCount, HashSet<Vector3Int> checkedSpots)
        {
            float angle_d = 360.0f / Config.cfg_spawner_sampling_angle_count;
            List<Vector3Int> positions = new List<Vector3Int>();
            for (int i = 0; i < Config.cfg_spawner_sampling_angle_count; i++)
            {
                float angle = i * angle_d;
                Vector3 target = point + Quaternion.AngleAxis(angle, Vector3.up) * Vector3.right * (Config.cfg_spawner_sampling_block_distance + 0.5f);
                Vector3Int gridTarget = Vector3Int.CeilToInt(target);
                if (!checkedSpots.Contains(gridTarget))
                {
                    positions.Add(gridTarget);
                    checkedSpots.Add(gridTarget);
                }
            }

            foreach (var pos in positions.Shuffle())
            {
                if (IsFreeSpot(pos))
                {
                    spot = pos;
                    return true;
                }
            }

            if (retryCount <= 4)
            {
                foreach (var pos in positions)
                {
                    if(FindFreeSpotNear_Impl(pos, out spot, ++retryCount, checkedSpots))
                    {
                        return true;
                    }
                }
            }

            spot = Vector3.zero;
            return false;
        }

    }
}

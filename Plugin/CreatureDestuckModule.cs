using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using UnityEngine;
using ThunderRoad;

namespace DungeonConfigurator
{
    public class CreatureDestuckModule : MonoBehaviour
    {
        Creature creature;
        float retryWait = 3.0f;

        private void Awake()
        {
            creature = this.gameObject.GetComponent<Creature>();
        }

        private void LateUpdate()
        {
            if (retryWait <= 0)
            {
                if (creature.locomotion.isGrounded == false)
                {
                    if (creature.currentRoom)
                    {
                        Logger.Detailed("Try unstuck {0} in {1}", creature.name, creature.currentRoom.name);
                    }
                    creature.locomotion.SphereCastGround(creature.fallAliveDestabilizeHeight, out RaycastHit hit, out _);
                    Vector3 tpPoint = hit.point;
                    creature.Teleport(tpPoint, creature.transform.rotation);
                    retryWait = 5.0f;
                }
                else
                {
                    GameObject.Destroy(this);
                }
            }

            retryWait -= Time.deltaTime;
        }
    }
}

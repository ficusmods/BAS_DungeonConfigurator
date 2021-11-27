using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using ThunderRoad;

namespace DungeonConfigurator
{
    public class CreatureDestuckModule : MonoBehaviour
    {
        float wait_time = 3;
        Creature creature;
        bool stuck_fixed = false;

        private void Awake()
        {
            creature = this.gameObject.GetComponent<Creature>();
        }

        private void LateUpdate()
        {
            if(creature.fallState == Creature.FallState.None)
            {
                stuck_fixed = true;
            }

            if(wait_time > 0)
            {
                if (!stuck_fixed)
                {
                    creature.locomotion.SphereCastGround(creature.fallAliveDestabilizeHeight, out RaycastHit hit, out _);
                    Vector3 tpPoint = hit.point + Vector3.up * 0.2f;
                    creature.Teleport(tpPoint, creature.transform.rotation);
                    wait_time = 3;
                }

                wait_time -= Time.deltaTime;
            }
        }
    }
}

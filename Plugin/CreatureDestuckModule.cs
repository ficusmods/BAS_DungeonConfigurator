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
        const float retryWait = 3.0f;

        private void Awake()
        {
            creature = this.gameObject.GetComponent<Creature>();
            StartCoroutine("DestuckRoutine");
        }

        private IEnumerable DestuckRoutine()
        {
            bool stuck_fixed = false;
            yield return new WaitForSeconds(retryWait);

            while (!stuck_fixed)
            {
                if (creature.fallState == Creature.FallState.None)
                {
                    stuck_fixed = true;
                }
                else
                {
                    creature.locomotion.SphereCastGround(creature.fallAliveDestabilizeHeight, out RaycastHit hit, out _);
                    Vector3 tpPoint = hit.point + Vector3.up * 0.2f;
                    creature.Teleport(tpPoint, creature.transform.rotation);
                    yield return new WaitForSeconds(retryWait);
                }
            }
        }
    }
}

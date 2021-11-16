using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using ThunderRoad;

namespace DungeonConfigurator
{
    class PointDebug : MonoBehaviour
    {

        private void Update()
        {
            if (PointerInputModule.currentPoint != null)
            {
                Logger.Detailed("Current pointer at object {0}", PointerInputModule.currentPoint.name);
            }
        }
    }
}

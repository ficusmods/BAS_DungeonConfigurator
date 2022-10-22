using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonConfigurator
{
    public class Config
    {
        public static HashSet<string> cfg_randomExcludedItems;
        public static float cfg_patrol_maxradius = 2.0f; /* Changes the maxradius of patrol in the brain settings to avoid npcs trying to take over each other's spot*/
        public static int cfg_spawner_sampling_angle_count = 4; /* performs sampling for free postitions in angles (360/<this value>) */
        public static float cfg_spawner_sampling_block_distance = 2.0f; /* Expected minimum distance from other creatures during sampling */
        public static int cfg_spawner_sampling_block_retry_count = 4; /* If the location was blocked how many times should we retry the process */
        public static float cfg_spawner_navsampling_distance = 1.0f; /* Distance to use to find a Nav position after sampling for a free spot. */
    }
}

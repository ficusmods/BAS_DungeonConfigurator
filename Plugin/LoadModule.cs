using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using ThunderRoad;
using ThunderRoad.AI;
using UnityEngine;
using HarmonyLib;

namespace DungeonConfigurator
{
    public class LoadModule : LevelModule
    {
        public string mod_version = "0.0";
        public string mod_name = "UnnamedMod";
        public string logger_level = "Basic";

        public IList<string> random_excluded_items;

        public float cfg_patrol_maxradius
        {
            get => Config.cfg_patrol_maxradius;
            set
            {
                if (value < 0.1f) Config.cfg_patrol_maxradius = 0.1f;
                else Config.cfg_patrol_maxradius = value;
            }
        }

        public int cfg_spawner_sampling_angle_count
        {
            get => Config.cfg_spawner_sampling_angle_count;
            set
            {
                if (value < 1) Config.cfg_spawner_sampling_angle_count = 1;
                else Config.cfg_spawner_sampling_angle_count = value;
            }
        }

        public float cfg_spawner_sampling_block_distance
        {
            get => Config.cfg_spawner_sampling_block_distance;
            set
            {
                if (value < 1.0f) Config.cfg_spawner_sampling_block_distance = 1.0f;
                else Config.cfg_spawner_sampling_block_distance = value;
            }
        }

        public int cfg_spawner_sampling_block_retry_count
        {
            get => Config.cfg_spawner_sampling_block_retry_count;
            set
            {
                if (value < 1) Config.cfg_spawner_sampling_block_retry_count = 1;
                else Config.cfg_spawner_sampling_block_retry_count = value;
            }
        }

        public float cfg_spawner_navsampling_distance
        {
            get => Config.cfg_spawner_navsampling_distance;
            set
            {
                if (value < 1.0f) Config.cfg_spawner_navsampling_distance = 1.0f;
                else Config.cfg_spawner_navsampling_distance = value;
            }
        }


        public override IEnumerator OnLoadCoroutine()
        {

            Logger.init(mod_name, mod_version, logger_level);
            Logger.Basic("Loading {0}", mod_name);

            Config.cfg_randomExcludedItems = random_excluded_items.ToHashSet();
            foreach(string id in random_excluded_items)
            {
                Logger.Basic("{0} is excluded from the random pool", id);
            }

            return base.OnLoadCoroutine();
        }
    }
}

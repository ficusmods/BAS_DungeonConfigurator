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

        public static bool spawning = false;

        public IList<string> random_excluded_items;

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

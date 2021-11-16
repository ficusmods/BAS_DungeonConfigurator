﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace DungeonConfigurator
{
    public class Utils
    {
        public static GameObject get_child(GameObject obj, string name)
        {
            Logger.Detailed("Get child of {0} with name {1}", obj.name, name);
            return obj.transform.Find(name).gameObject;
        }

        public static GameObject get_child_from_path(GameObject obj, string path)
        {
            var components = path.Split('/');
            GameObject curr = obj;
            foreach (var comp in components)
            {
                curr = get_child(curr, comp);
            }
            return curr;
        }
    }
}

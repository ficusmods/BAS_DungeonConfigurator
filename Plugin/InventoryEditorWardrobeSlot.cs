﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using ThunderRoad;

namespace DungeonConfigurator
{
    public class InventoryEditorWardrobeSlot : InventoryEditorSlot
    {
        public InventoryEditorWardrobeSlot(GameObject obj, string _name) : base(obj, _name)
        { }

        public override List<ItemData> get_possible_coices()
        {
            List<ItemData> ret = new List<ItemData>();
            foreach (var data in Catalog.GetDataList(Catalog.Category.Item))
            {
                ItemData itemData = data as ItemData;
                if (itemData.type == ItemData.Type.Wardrobe)
                {
                    ret.Add(itemData);
                }
            }
            return ret;
        }
    }
}

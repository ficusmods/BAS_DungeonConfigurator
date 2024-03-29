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
    public class InventoryEditorSpellSlot : InventoryEditorSlot
    {
        public InventoryEditorSpellSlot(GameObject obj, string _name) : base(obj, _name)
        { }

        public override List<ItemData> get_possible_coices()
        {
            List<ItemData> ret = new List<ItemData>();
            foreach (var data in Catalog.GetDataList(Catalog.Category.Item))
            {
                ItemData itemData = data as ItemData;
                if (itemData.type == ItemData.Type.Spell)
                {
                    if (itemData.iconAddress == null)
                    {
                        itemData.iconAddress = "Bas.Icon.pentacle";
                    }

                    ItemModuleSpell module = itemData.GetModule<ItemModuleSpell>();
                    CatalogData sdata = Catalog.GetData(Catalog.Category.Spell, module.spellId);
                    if (sdata is SpellCastData)
                    {
                        ret.Add(itemData);
                    }
                }
            }
            return ret;
        }

        public override void refresh_image()
        {
            if (this._item != null && _item.iconAddress == null)
            {
                _item.iconAddress = "Bas.Icon.pentacle";
            }
            base.refresh_image();
        }
    }
}

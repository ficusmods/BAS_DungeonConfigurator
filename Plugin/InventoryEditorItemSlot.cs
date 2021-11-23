using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using ThunderRoad;

namespace DungeonConfigurator
{
    public class InventoryEditorItemSlot : InventoryEditorSlot
    {
        public Holder holder;

        public InventoryEditorItemSlot(GameObject obj, string _name, Holder _holder = null) : base(obj, _name)
        {
            holder = _holder;
        }

        public override ContainerData.Content as_content()
        {
            ContainerData.Content ret = base.as_content();
            if (holder != null)
            {
                Item.SavedValue cval = new Item.SavedValue("Holder", holder.name);
                ret.customValues.Add(cval);
            }
            return ret;
        }

        public override List<ItemData> get_possible_coices()
        {
            List<ItemData> ret = new List<ItemData>();
            foreach (var data in Catalog.GetDataList(Catalog.Category.Item))
            {
                ItemData itemData = data as ItemData;
                if (itemData.categoryPath.Length > 0)
                {
                    ret.Add(itemData);
                }
            }
            return ret;
        }
    }
}

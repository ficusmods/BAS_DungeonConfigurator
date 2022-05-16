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
        public Holder.DrawSlot drawSlot;

        public static HashSet<ItemData.Type> itemTypes = new HashSet<ItemData.Type>(){
                ItemData.Type.Weapon,
                ItemData.Type.Tool,
                ItemData.Type.Body,
                ItemData.Type.Food,
                ItemData.Type.Potion,
                ItemData.Type.Quiver,
                ItemData.Type.Shield
        };

        public InventoryEditorItemSlot(GameObject obj, string _name, Holder.DrawSlot _drawSlot = Holder.DrawSlot.None) : base(obj, _name)
        {
            drawSlot = _drawSlot;
        }

        public override ContainerData.Content as_content()
        {
            ContainerData.Content ret = base.as_content();
            if (drawSlot != Holder.DrawSlot.None)
            {
                Holder holder = Player.local.creature.equipment.GetHolder(drawSlot);
                ret.state = new ContentStateHolder(holder.name);
            }
            return ret;
        }

        public override List<ItemData> get_possible_coices()
        {
            List<ItemData> ret = new List<ItemData>();
            foreach (var data in Catalog.GetDataList(Catalog.Category.Item))
            {
                ItemData itemData = data as ItemData;
                if (itemTypes.Contains(itemData.type))
                {
                    if(itemData.iconAddress == null)
                    {
                        itemData.iconAddress = "Bas.Icon.sword";
                    }
                    ret.Add(itemData);
                }
            }
            return ret;
        }

        public override void refresh_image()
        {
            if (this._item != null && _item.iconAddress == null)
            {
                _item.iconAddress = "Bas.Icon.sword";
            }
            base.refresh_image();
        }
    }
}

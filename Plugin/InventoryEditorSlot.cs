using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThunderRoad;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DungeonConfigurator
{
    public class InventoryEditorSlot
    {
        public GameObject objSlot;
        public string name;

        public Button button;
        public RawImage image;

        protected ItemData _item;
        public ItemData item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                this.refresh_image();
            }
        }

        public delegate void EventClick();
        public event EventClick onClick;
        public delegate void EventHoverStart();
        public event EventHoverStart onHoverStart;
        public delegate void EventHoverEnd();
        public event EventHoverEnd onHoverEnd;

        public InventoryEditorSlot(GameObject obj, string _name)
        {
            objSlot = obj;
            name = _name;
            button = objSlot.GetComponentInChildren<Button>();
            image = objSlot.GetComponentInChildren<RawImage>();

            button.onClick.AddListener(delegate { onClick.Invoke(); });

            var buttonEventTrigger = button.gameObject.GetComponent<EventTrigger>();

            // Init HoverStart event
            EventTrigger.Entry entryHoverStart = new EventTrigger.Entry();
            entryHoverStart.eventID = EventTriggerType.PointerEnter;
            entryHoverStart.callback.AddListener(delegate { onHoverStart.Invoke(); });
            
            // Init HoverEnd event
            EventTrigger.Entry entryHoverEnd = new EventTrigger.Entry();
            entryHoverEnd.eventID = EventTriggerType.PointerExit;
            entryHoverEnd.callback.AddListener((data) => { onHoverEnd.Invoke(); });
            
            buttonEventTrigger.triggers.Add(entryHoverEnd);
            buttonEventTrigger.triggers.Add(entryHoverStart);
        }

        public virtual ContainerData.Content as_content()
        {
            ContainerData.Content ret = new ContainerData.Content();
            ret.reference = ContainerData.Content.Reference.Item;
            ret.referenceID = this._item.id;
            ret.itemData = Catalog.GetData<ItemData>(ret.referenceID);
            return ret;
        }

        public virtual List<ItemData> get_possible_coices() { return null; }

        public virtual void refresh_image()
        {
            if (this._item != null)
            {
                Catalog.LoadAssetAsync<Texture>(this._item.iconAddress, (Texture t) => { this.image.texture = t; }, "Item");
                this.image.gameObject.SetActive(true);
            }
            else
            {
                this.image.gameObject.SetActive(false);
                this.image.texture = null;
            }
        }
    }
}

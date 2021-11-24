using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using ThunderRoad;

namespace DungeonConfigurator
{
    public class InventoryEditor
    {
        GameObject inventoryEditor;
        GameObject viewSlots;
        GameObject viewItemSelect;
        GameObject viewSubItemSelect;

        GameObject itemRowsContentTemplate;
        GameObject itemRowsEntryListTemplate;
        GameObject itemRowsEntryTemplate;
        GameObject slotHint;
        Text slotHintText;

        const int inventorySelectorEntryPerRow = 4;
        string player_container_id = "DungeonConfiguratorPlayerContainer";

        Dictionary<string, InventoryEditorSlot> slots = new Dictionary<string, InventoryEditorSlot>();

        public InventoryEditor(GameObject obj)
        {
            Logger.Detailed("Initializing inventory editor");
            inventoryEditor = obj;

            viewSlots = Utils.get_child(inventoryEditor, "SlotsView");
            viewItemSelect = Utils.get_child(inventoryEditor, "ItemSelectView");
            viewSubItemSelect = Utils.get_child(viewItemSelect, "Item/ItemView");

            slotHint = Utils.get_child(viewSlots, "SlotHint");
            slotHintText = slotHint.GetComponent<Text>();

            itemRowsContentTemplate = Utils.get_child(viewItemSelect, "ItemRowsTemplate");
            itemRowsEntryListTemplate = Utils.get_child(viewItemSelect, "ItemListTemplate");
            itemRowsEntryTemplate = Utils.get_child(viewItemSelect, "ItemListEntryTemplate");

            init_slot_map();
            init_button_callbacks();

            viewItemSelect.SetActive(false);
            itemRowsContentTemplate.SetActive(false);
            itemRowsEntryListTemplate.SetActive(false);
            itemRowsEntryTemplate.SetActive(false);
        }

        private void init_slot_map()
        {
            Logger.Detailed("Initializing slots");
            slots["HipsRight"] = new InventoryEditorItemSlot(
                Utils.get_child(viewSlots, "Items/HipRight"), "HipsRight",
                Player.local.creature.equipment.GetHolder(Holder.DrawSlot.HipsRight)
                );
            slots["HipsLeft"] = new InventoryEditorItemSlot(
                Utils.get_child(viewSlots, "Items/HipLeft"), "HipsLeft",
                Player.local.creature.equipment.GetHolder(Holder.DrawSlot.HipsLeft)
                );
            slots["BackRight"] = new InventoryEditorItemSlot(
                Utils.get_child(viewSlots, "Items/BackRight"), "BackRight",
                Player.local.creature.equipment.GetHolder(Holder.DrawSlot.BackRight)
                );
            slots["BackLeft"] = new InventoryEditorItemSlot(
                Utils.get_child(viewSlots, "Items/BackLeft"), "BackLeft",
                Player.local.creature.equipment.GetHolder(Holder.DrawSlot.BackLeft)
                );
            
            slots["ItemsExtra1"] = new InventoryEditorItemSlot(Utils.get_child(viewSlots, "Items/Extra1"), "ItemsExtra1");
            slots["ItemsExtra2"] = new InventoryEditorItemSlot(Utils.get_child(viewSlots, "Items/Extra2"), "ItemsExtra2");
            slots["ItemsExtra3"] = new InventoryEditorItemSlot(Utils.get_child(viewSlots, "Items/Extra3"), "ItemsExtra3");
            slots["ItemsExtra4"] = new InventoryEditorItemSlot(Utils.get_child(viewSlots, "Items/Extra4"), "ItemsExtra4");
            
            slots["ArmorChest"] = new InventoryEditorWardrobeSlot(Utils.get_child(viewSlots     , "Armor/Chest"     ), "ArmorChest");
            slots["ArmorHelmet"] = new InventoryEditorWardrobeSlot(Utils.get_child(viewSlots    , "Armor/Helmet"    ), "ArmorHelmet");
            slots["ArmorHandLeft"] = new InventoryEditorWardrobeSlot(Utils.get_child(viewSlots  , "Armor/HandLeft"  ), "ArmorHandLeft");
            slots["ArmorHandRight"] = new InventoryEditorWardrobeSlot(Utils.get_child(viewSlots , "Armor/HandRight" ), "ArmorHandRight");
            slots["ArmorLegs"] = new InventoryEditorWardrobeSlot(Utils.get_child(viewSlots      , "Armor/Legs"      ), "ArmorLegs");
            slots["ArmorBootsRight"] = new InventoryEditorWardrobeSlot(Utils.get_child(viewSlots, "Armor/BootsRight"), "ArmorBootsRight");
            slots["ArmorBootsLeft"] = new InventoryEditorWardrobeSlot(Utils.get_child(viewSlots , "Armor/BootsLeft" ), "ArmorBootsLeft");
            slots["ArmorCosmetics1"] = new InventoryEditorWardrobeSlot(Utils.get_child(viewSlots, "Armor/Cosmetics1"), "ArmorCosmetics1");
            slots["ArmorCosmetics2"] = new InventoryEditorWardrobeSlot(Utils.get_child(viewSlots, "Armor/Cosmetics2"), "ArmorCosmetics2");
            slots["ArmorCosmetics3"] = new InventoryEditorWardrobeSlot(Utils.get_child(viewSlots, "Armor/Cosmetics3"), "ArmorCosmetics3");
            slots["ArmorCosmetics4"] = new InventoryEditorWardrobeSlot(Utils.get_child(viewSlots, "Armor/Cosmetics4"), "ArmorCosmetics4");

            slots["Spell1"] = new InventoryEditorSpellSlot(Utils.get_child(viewSlots, "Spells/Spell1"), "Spell1");
            slots["Spell2"] = new InventoryEditorSpellSlot(Utils.get_child(viewSlots, "Spells/Spell2"), "Spell2");
            slots["Spell3"] = new InventoryEditorSpellSlot(Utils.get_child(viewSlots, "Spells/Spell3"), "Spell3");
            slots["Spell4"] = new InventoryEditorSpellSlot(Utils.get_child(viewSlots, "Spells/Spell4"), "Spell4");
            slots["Spell5"] = new InventoryEditorSpellSlot(Utils.get_child(viewSlots, "Spells/Spell5"), "Spell5");
        }
        public virtual void setHidden(bool hidden)
        {
            inventoryEditor.SetActive(!hidden);
        }

        public virtual void apply_changes()
        {
            EventManager.onLevelLoad += apply_changes_impl;
        }

        private void apply_changes_impl(LevelData levelData, EventTime eventTime)
        {
            Logger.Basic("Applying inventory changes");
            Container playerContainer = Player.local.creature.container;
            playerContainer.loadContent = Container.LoadContent.ContainerID;
            playerContainer.containerID = player_container_id;

            ContainerData containterData = Catalog.GetData(Catalog.Category.Container, player_container_id) as ContainerData;
            foreach(var entry in slots)
            {
                if(entry.Value.item != null)
                {
                    ContainerData.Content content = entry.Value.as_content();
                    containterData.contents.Add(content);
                }
            }
            playerContainer.Load();
            Player.local.creature.mana.Load();
            Player.local.creature.equipment.UnequipWeapons();
            Player.local.creature.equipment.UnequipAllWardrobes();
            Player.local.creature.equipment.EquipAllWardrobes(false, false);
            Player.local.creature.equipment.EquipWeapons();
            EventManager.onLevelLoad -= apply_changes_impl;
        }

        private void init_button_callbacks()
        {
            foreach(var entry in slots)
            {
                entry.Value.onClick += delegate {
                    Logger.Detailed("Click on slot {0}", entry.Value.name);
                    handle_slot_click(entry.Value); };
                entry.Value.onHoverStart += delegate {
                    Logger.Detailed("Showing hint for slot {0}", entry.Value.name);
                    show_hint_for_slot(entry.Value); };
                entry.Value.onHoverEnd += delegate { hide_slot_hint(); };
            }
        }

        private void handle_slot_click(InventoryEditorSlot slot)
        {
            slot.item = null;
            fill_items(slot);
        }

        private void show_hint_for_slot(InventoryEditorSlot slot)
        {
            slotHint.SetActive(true);
            if(slot.item == null)
            {
                slotHintText.text = slot.name;
            }
            else
            {
                slotHintText.text = slot.item.id;
            }
            slotHint.transform.position = slot.objSlot.transform.position;
            slotHint.transform.LookAt(Camera.main.transform.position, Camera.main.transform.up);   
        }

        private void hide_slot_hint()
        {
            slotHint.SetActive(false);
        }

        private void fill_items(InventoryEditorSlot slot)
        {
            Logger.Detailed("Filling item list based on {0}", slot.name);
            foreach (Transform child in viewSubItemSelect.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            viewItemSelect.SetActive(true);

            List<ItemData> items = slot.get_possible_coices();
            GameObject rows = GameObject.Instantiate(itemRowsContentTemplate, viewSubItemSelect.transform);
            GameObject column = GameObject.Instantiate(itemRowsEntryListTemplate, rows.transform);
            foreach(var item in items)
            {
                if(column.transform.childCount > inventorySelectorEntryPerRow)
                {
                    column = GameObject.Instantiate(itemRowsEntryListTemplate, rows.transform);
                }
                
                GameObject currEntry = GameObject.Instantiate(itemRowsEntryTemplate, column.transform);
                RawImage currImg = currEntry.GetComponentInChildren<RawImage>();
                Catalog.LoadAssetAsync<Texture>(item.iconAddress, (Texture t) => {
                    currImg.texture = t;
                }, "Item");
                Button currButton = currEntry.GetComponent<Button>();
                currButton.onClick.AddListener(delegate {
                    slot.item = item;
                    viewItemSelect.SetActive(false);
                });
            }
            viewSubItemSelect.GetComponent<ScrollRect>().content = rows.GetComponent<RectTransform>();
        }
    }
}

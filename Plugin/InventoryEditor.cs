﻿using System;
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
        GameObject viewSubCategorySelect;
        GameObject viewSubItemSelect;
        Button buttonHipRight;
        Button buttonHipLeft;
        Button buttonBackRight;
        Button buttonBackLeft;
        Button buttonSpellTop;
        Button buttonSpellLeft;
        Button buttonSpellRight;
        Button buttonSpellBottom;
        RawImage imgHipRight;
        RawImage imgHipLeft;
        RawImage imgBackRight;
        RawImage imgBackLeft;
        RawImage imgSpellTop;
        RawImage imgSpellLeft;
        RawImage imgSpellRight;
        RawImage imgSpellBottom;

        GameObject itemRowsContentTemplate;
        GameObject itemRowsEntryListTemplate;
        GameObject itemRowsEntryTemplate;
        GameObject categoryRowsContent;
        GameObject categoryRowsEntryListTemplate;
        GameObject categoryRowsEntryTemplate;

        GameObject slotHint;
        Text slotHintText;

        const int inventorySelectorEntryPerRow = 4;
        string player_container_id = "DungeonConfiguratorPlayerContainer";
        public enum SlotType {
            Hand,
            Hip,
            Back,
            Spell
        }

        Dictionary<string, ItemData> equippedItems = new Dictionary<string, ItemData>();
        Dictionary<string, RawImage> equippedItemImages = new Dictionary<string, RawImage>();
        string currSelectedSlot = "";

        List<string> non_holder_list = new List<string>{"HandRight", "HandLeft", "SpellTop", "SpellLeft", "SpellRight", "SpellBottom"};

        Dictionary<string, GameObject> item_page_map = new Dictionary<string, GameObject>();
        GameObject spellPage;

        public InventoryEditor(GameObject obj)
        {
            inventoryEditor = obj;

            viewSlots = Utils.get_child(inventoryEditor, "SlotsView");
            viewItemSelect = Utils.get_child(inventoryEditor, "ItemSelectView");
            viewSubCategorySelect = Utils.get_child(viewItemSelect, "Category/CategoryView");
            viewSubItemSelect = Utils.get_child(viewItemSelect, "Item/ItemView");

            slotHint = Utils.get_child(viewSlots, "SlotHint");
            slotHintText = slotHint.GetComponent<Text>();

            buttonHipRight    = Utils.get_child(viewSlots, "Items/HipRight"   ).GetComponentInChildren<Button>();
            buttonHipLeft     = Utils.get_child(viewSlots, "Items/HipLeft"    ).GetComponentInChildren<Button>();
            buttonBackRight   = Utils.get_child(viewSlots, "Items/BackRight"  ).GetComponentInChildren<Button>();
            buttonBackLeft    = Utils.get_child(viewSlots, "Items/BackLeft"   ).GetComponentInChildren<Button>();
            buttonSpellTop    = Utils.get_child(viewSlots, "Items/TopSpell"   ).GetComponentInChildren<Button>();
            buttonSpellLeft   = Utils.get_child(viewSlots, "Items/LeftSpell"  ).GetComponentInChildren<Button>();
            buttonSpellRight  = Utils.get_child(viewSlots, "Items/RightSpell" ).GetComponentInChildren<Button>();
            buttonSpellBottom = Utils.get_child(viewSlots, "Items/BottomSpell").GetComponentInChildren<Button>();
            imgHipRight       = Utils.get_child(viewSlots, "Items/HipRight"   ).GetComponentInChildren<RawImage>();
            imgHipLeft        = Utils.get_child(viewSlots, "Items/HipLeft"    ).GetComponentInChildren<RawImage>();
            imgBackRight      = Utils.get_child(viewSlots, "Items/BackRight"  ).GetComponentInChildren<RawImage>();
            imgBackLeft       = Utils.get_child(viewSlots, "Items/BackLeft"   ).GetComponentInChildren<RawImage>();
            imgSpellTop       = Utils.get_child(viewSlots, "Items/TopSpell"   ).GetComponentInChildren<RawImage>();
            imgSpellLeft      = Utils.get_child(viewSlots, "Items/LeftSpell"  ).GetComponentInChildren<RawImage>();
            imgSpellRight     = Utils.get_child(viewSlots, "Items/RightSpell" ).GetComponentInChildren<RawImage>();
            imgSpellBottom    = Utils.get_child(viewSlots, "Items/BottomSpell").GetComponentInChildren<RawImage>();

            itemRowsContentTemplate = Utils.get_child(viewSubItemSelect, "ItemRowsTemplate");
            itemRowsEntryListTemplate = Utils.get_child(viewSubItemSelect, "ItemListTemplate");
            itemRowsEntryTemplate = Utils.get_child(viewSubItemSelect, "ItemListEntryTemplate");
            categoryRowsContent = Utils.get_child(viewSubCategorySelect, "CategoryRows");
            categoryRowsEntryListTemplate = Utils.get_child(viewSubCategorySelect, "CategoryListTemplate");
            categoryRowsEntryTemplate = Utils.get_child(viewSubCategorySelect, "CategoryListEntryTemplate");

            imgHipRight.gameObject.SetActive(false);
            imgHipLeft.gameObject.SetActive(false);
            imgBackRight.gameObject.SetActive(false);
            imgBackLeft.gameObject.SetActive(false);
            imgSpellTop.gameObject.SetActive(false);
            imgSpellLeft.gameObject.SetActive(false);
            imgSpellRight.gameObject.SetActive(false);
            imgSpellBottom.gameObject.SetActive(false);

            init_button_callbacks();
            init_categories();
            init_item_list();
            init_spell_list();
            init_equipped_image_map();

            viewItemSelect.SetActive(false);
            categoryRowsEntryListTemplate.SetActive(false);
            categoryRowsEntryTemplate.SetActive(false);
            itemRowsContentTemplate.SetActive(false);
            itemRowsEntryListTemplate.SetActive(false);
            itemRowsEntryTemplate.SetActive(false);
        }
        public virtual void setHidden(bool hidden)
        {
            inventoryEditor.SetActive(!hidden);
        }

        public virtual void apply_changes()
        {
            EventManager.onLevelLoad += (LevelData levelData, EventTime eventTime) => {
                Logger.Detailed("Applying inventory changes");
                Container playerContainer = Player.local.creature.container;
                playerContainer.loadContent = Container.LoadContent.ContainerID;
                playerContainer.containerID = player_container_id;

                ContainerData containterData = Catalog.GetData(Catalog.Category.Container, player_container_id) as ContainerData;
                foreach(var entry in equippedItems)
                {
                    ContainerData.Content content = new ContainerData.Content();
                    content.reference = ContainerData.Content.Reference.Item;
                    content.referenceID = entry.Value.id;
                    content.itemData = Catalog.GetData<ItemData>(content.referenceID);
                    if(!non_holder_list.Contains(entry.Key))
                    {
                        Item.SavedValue cval = new Item.SavedValue("Holder", entry.Key);
                        content.customValues.Add(cval);
                    }
                    containterData.contents.Add(content);
                }
                playerContainer.Load();
                Player.local.creature.mana.Load();
                Player.local.creature.equipment.UnequipWeapons();
                Player.local.creature.equipment.UnequipAllWardrobes();
                Player.local.creature.equipment.EquipAllWardrobes(false, false);
                Player.local.creature.equipment.EquipWeapons();
            };
        }

        private void tryRemoveItemFromSlot(string slot)
        {
            if(equippedItems.ContainsKey(slot))
            {
                equippedItems.Remove(slot);
                equippedItemImages[slot].texture = null;
            }
        }
        private void init_button_callbacks(){
            buttonHipRight.onClick.AddListener(delegate    { handle_slot_click("HipsRight"  ); });
            buttonHipLeft.onClick.AddListener(delegate     { handle_slot_click("HipsLeft"   ); });
            buttonBackRight.onClick.AddListener(delegate   { handle_slot_click("BackRight"  ); });
            buttonBackLeft.onClick.AddListener(delegate    { handle_slot_click("BackLeft"   ); });
            buttonSpellTop.onClick.AddListener(delegate    { handle_slot_click("SpellTop"   ); });
            buttonSpellLeft.onClick.AddListener(delegate   { handle_slot_click("SpellLeft"  ); });
            buttonSpellRight.onClick.AddListener(delegate  { handle_slot_click("SpellRight" ); });
            buttonSpellBottom.onClick.AddListener(delegate { handle_slot_click("SpellBottom"); });

            add_pointer_enter_to_slotbutton(buttonHipRight   , "HipsRight"  );
            add_pointer_enter_to_slotbutton(buttonHipLeft    , "HipsLeft"   );
            add_pointer_enter_to_slotbutton(buttonBackRight  , "BackRight"  );
            add_pointer_enter_to_slotbutton(buttonBackLeft   , "BackLeft"   );
            add_pointer_enter_to_slotbutton(buttonSpellTop   , "SpellTop"   );
            add_pointer_enter_to_slotbutton(buttonSpellLeft  , "SpellLeft"  );
            add_pointer_enter_to_slotbutton(buttonSpellRight , "SpellRight" );
            add_pointer_enter_to_slotbutton(buttonSpellBottom, "SpellBottom");
            add_pointer_exit_to_slotbutton(buttonHipRight   , "HipsRight"  );
            add_pointer_exit_to_slotbutton(buttonHipLeft    , "HipsLeft"   );
            add_pointer_exit_to_slotbutton(buttonBackRight  , "BackRight"  );
            add_pointer_exit_to_slotbutton(buttonBackLeft   , "BackLeft"   );
            add_pointer_exit_to_slotbutton(buttonSpellTop   , "SpellTop"   );
            add_pointer_exit_to_slotbutton(buttonSpellLeft  , "SpellLeft"  );
            add_pointer_exit_to_slotbutton(buttonSpellRight , "SpellRight" );
            add_pointer_exit_to_slotbutton(buttonSpellBottom, "SpellBottom");
        }

        private void add_pointer_enter_to_slotbutton(Button slotbutton, string slot)
        {
            var trigger = slotbutton.gameObject.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { showHintForSlot(slot); });
            trigger.triggers.Add(entry);
        }

        private void add_pointer_exit_to_slotbutton(Button slotbutton, string slot)
        {
            var trigger = slotbutton.gameObject.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;
            entry.callback.AddListener((data) => { hideSlotHint();});
            trigger.triggers.Add(entry);
        }

        private void handle_slot_click(string slot)
        {
            tryRemoveItemFromSlot(slot);
            currSelectedSlot = slot;
            switch_to_category_view();
        }

        private void showHintForSlot(string slot)
        {
            if(equippedItems.ContainsKey(slot))
            {
                slotHint.SetActive(true);
                slotHintText.text = equippedItems[slot].id;
                slotHint.transform.position = equippedItemImages[slot].transform.position;
                slotHint.transform.LookAt(Camera.main.transform.position, Camera.main.transform.up);
            }
        }

        private void hideSlotHint()
        {
            slotHint.SetActive(false);
        }

        private void init_equipped_image_map()
        {
            equippedItemImages["HipsRight"] = imgHipRight;
            equippedItemImages["HipsLeft"] = imgHipLeft;
            equippedItemImages["BackRight"] = imgBackRight;
            equippedItemImages["BackLeft"] = imgBackLeft;
            equippedItemImages["SpellTop"] = imgSpellRight;
            equippedItemImages["SpellLeft"] = imgSpellLeft;
            equippedItemImages["SpellRight"] = imgSpellRight;
            equippedItemImages["SpellBottom"] = imgSpellBottom;
        }
        private void init_categories()
        {
            GameObject currList = GameObject.Instantiate(categoryRowsEntryListTemplate, categoryRowsContent.transform);
            var categories = Catalog.gameData.categories;
            foreach(GameData.Category category in categories)
            {
                if(currList.transform.childCount > inventorySelectorEntryPerRow)
                {
                    currList = GameObject.Instantiate(categoryRowsEntryListTemplate, categoryRowsContent.transform);
                }
                
                GameObject currEntry = GameObject.Instantiate(categoryRowsEntryTemplate, currList.transform);
                RawImage currImg = currEntry.GetComponentInChildren<RawImage>();
                Catalog.LoadAssetAsync<Texture>(category.iconAddress, (Texture t) => {
                    currImg.texture = t;
                }, "Inventory");
                Button currButton = currEntry.GetComponent<Button>();
                currButton.onClick.AddListener(delegate {
                    fill_items_with_category(category);
                });
            }

            if(currList.transform.childCount > inventorySelectorEntryPerRow)
            {
                currList = GameObject.Instantiate(categoryRowsEntryListTemplate, categoryRowsContent.transform);
            }
            GameObject spellEntry = GameObject.Instantiate(categoryRowsEntryTemplate, currList.transform);
            RawImage spellImg = spellEntry.GetComponentInChildren<RawImage>();
            Catalog.LoadAssetAsync<Texture>("Bas.Icon.pentacle", (Texture t) => {
                spellImg.texture = t;
            }, "Inventory");
            Button spellButton = spellEntry.GetComponent<Button>();
            spellButton.onClick.AddListener(delegate {
                fill_items_with_spells();
            });
        }

        private void init_item_list()
        {
            var categories = Catalog.gameData.categories;

            Dictionary<string, List<ItemData>> item_map = new Dictionary<string, List<ItemData>>();
            foreach(var data in Catalog.GetDataList(Catalog.Category.Item))
            {
                ItemData itemData = data as ItemData;
                foreach (string category in itemData.categoryPath){
                    List<ItemData> itemList;
                    if(!item_map.ContainsKey(category))
                    {
                        item_map[category] = new List<ItemData>();
                    }
                    itemList = item_map[category];
                    itemList.Add(itemData);
                }
            }

            foreach(GameData.Category category in categories)
            {
                if(item_map.ContainsKey(category.name))
                {
                    var itemList = item_map[category.name];
                    GameObject rows = GameObject.Instantiate(itemRowsContentTemplate, viewSubItemSelect.transform);
                    item_page_map.Add(category.name, rows);
                    GameObject currList = GameObject.Instantiate(itemRowsEntryListTemplate, rows.transform);
                    
                    foreach(var item in itemList)
                    {
                        if(currList.transform.childCount > inventorySelectorEntryPerRow)
                        {
                            currList = GameObject.Instantiate(itemRowsEntryListTemplate, rows.transform);
                        }
                        
                        GameObject currEntry = GameObject.Instantiate(itemRowsEntryTemplate, currList.transform);
                        RawImage currImg = currEntry.GetComponentInChildren<RawImage>();
                        Catalog.LoadAssetAsync<Texture>(item.iconAddress, (Texture t) => {
                            currImg.texture = t;
                        }, "Item");
                        Button currButton = currEntry.GetComponent<Button>();
                        currButton.onClick.AddListener(delegate {
                            equipItem(item);
                            viewItemSelect.SetActive(false);
                        });
                    }
                }
                else
                {
                    Logger.Detailed("Missing category in item map: {0}", category.name);
                }
            }
        }

        private void init_spell_list()
        {
            Dictionary<string, List<ItemData>> spell_map = new Dictionary<string, List<ItemData>>();
            spellPage = GameObject.Instantiate(itemRowsContentTemplate, viewSubItemSelect.transform);
            GameObject currList = GameObject.Instantiate(itemRowsEntryListTemplate, spellPage.transform);
            foreach(var data in Catalog.GetDataList(Catalog.Category.Item))
            {
                ItemData itemData = data as ItemData;
                if (itemData.type == ItemData.Type.Spell)
                {
                    if (currList.transform.childCount > inventorySelectorEntryPerRow)
                    {
                        currList = GameObject.Instantiate(itemRowsEntryListTemplate, spellPage.transform);
                    }
                    GameObject currEntry = GameObject.Instantiate(itemRowsEntryTemplate, currList.transform);
                    Button currButton = currEntry.GetComponent<Button>();
                    Text currText = currEntry.GetComponent<Text>();
                    currText.text = itemData.id;
                    RawImage currImg = currEntry.GetComponentInChildren<RawImage>();
                    currImg.gameObject.SetActive(false);
                    currButton.onClick.AddListener(delegate
                    {
                        equipItem(itemData);
                        viewItemSelect.SetActive(false);
                    });
                }
            }
        }

        private void equipItem(ItemData item)
        {
            if(currSelectedSlot != "")
            {
                equippedItems[currSelectedSlot] = item;
                Catalog.LoadAssetAsync<Texture>(item.iconAddress, (Texture t) => {
                    equippedItemImages[currSelectedSlot].texture = t;
                    equippedItemImages[currSelectedSlot].gameObject.SetActive(true);
                }, "Item");
            }
        }
        private void fill_items_with_category(GameData.Category category)
        {
            viewItemSelect.SetActive(true);
            viewSubItemSelect.SetActive(true);
            viewSubCategorySelect.SetActive(false);
            spellPage.SetActive(false);
            foreach(var entry in item_page_map){
                entry.Value.SetActive(false);
            }
            item_page_map[category.name].SetActive(true);
            viewSubItemSelect.GetComponent<ScrollRect>().content = item_page_map[category.name].GetComponent<RectTransform>();
        }

        private void fill_items_with_spells()
        {
            viewItemSelect.SetActive(true);
            viewSubItemSelect.SetActive(true);
            viewSubCategorySelect.SetActive(false);
            foreach(var entry in item_page_map){
                entry.Value.SetActive(false);
            }
            spellPage.SetActive(true);
            viewSubItemSelect.GetComponent<ScrollRect>().content = spellPage.GetComponent<RectTransform>();
        }

        private void switch_to_category_view()
        {
            viewItemSelect.SetActive(true);
            viewSubItemSelect.SetActive(false);
            viewSubCategorySelect.SetActive(true);
        }
    }
}

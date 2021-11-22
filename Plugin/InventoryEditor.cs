using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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
        Button buttonHandRight;
        Button buttonHandLeft;
        Button buttonSpellTop;
        Button buttonSpellLeft;
        Button buttonSpellRight;
        Button buttonSpellBottom;
        RawImage imgHipRight;
        RawImage imgHipLeft;
        RawImage imgBackRight;
        RawImage imgBackLeft;
        RawImage imgHandRight;
        RawImage imgHandLeft;
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

            buttonHipRight    = Utils.get_child(viewSlots, "HipRightButton").GetComponent<Button>();
            buttonHipLeft     = Utils.get_child(viewSlots, "HipLeftButton").GetComponent<Button>();
            buttonBackRight   = Utils.get_child(viewSlots, "BackRightButton").GetComponent<Button>();
            buttonBackLeft    = Utils.get_child(viewSlots, "BackLeftButton").GetComponent<Button>();
            buttonHandRight   = Utils.get_child(viewSlots, "HandRightButton").GetComponent<Button>();
            buttonHandLeft    = Utils.get_child(viewSlots, "HandLeftButton").GetComponent<Button>();
            buttonSpellTop    = Utils.get_child(viewSlots, "TopSpellButton").GetComponent<Button>();
            buttonSpellLeft   = Utils.get_child(viewSlots, "LeftSpellButton").GetComponent<Button>();
            buttonSpellRight  = Utils.get_child(viewSlots, "RightSpellButton").GetComponent<Button>();
            buttonSpellBottom = Utils.get_child(viewSlots, "BottomSpellButton").GetComponent<Button>();
            imgHipRight       = Utils.get_child(viewSlots, "HipRightImage").GetComponent<RawImage>();
            imgHipLeft        = Utils.get_child(viewSlots, "HipLeftImage").GetComponent<RawImage>();
            imgBackRight      = Utils.get_child(viewSlots, "BackRightImage").GetComponent<RawImage>();
            imgBackLeft       = Utils.get_child(viewSlots, "BackLeftImage").GetComponent<RawImage>();
            imgHandRight      = Utils.get_child(viewSlots, "HandRightImage").GetComponent<RawImage>();
            imgHandLeft       = Utils.get_child(viewSlots, "HandLeftImage").GetComponent<RawImage>();
            imgSpellTop       = Utils.get_child(viewSlots, "TopSpellImage").GetComponent<RawImage>();
            imgSpellLeft      = Utils.get_child(viewSlots, "LeftSpellImage").GetComponent<RawImage>();
            imgSpellRight     = Utils.get_child(viewSlots, "RightSpellImage").GetComponent<RawImage>();
            imgSpellBottom    = Utils.get_child(viewSlots, "BottomSpellImage").GetComponent<RawImage>();

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
            imgHandRight.gameObject.SetActive(false);
            imgHandLeft.gameObject.SetActive(false);
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

        private void init_button_callbacks(){
            buttonHipRight.onClick.AddListener(delegate {
                currSelectedSlot = "HipsRight";
                switch_to_category_view();
            });
            buttonHipLeft.onClick.AddListener(delegate {
                currSelectedSlot = "HipsLeft";
                switch_to_category_view();
            });
            buttonBackRight.onClick.AddListener(delegate {
                currSelectedSlot = "BackRight";
                switch_to_category_view();
            });
            buttonBackLeft.onClick.AddListener(delegate {
                currSelectedSlot = "BackLeft";
                switch_to_category_view();
            });
            buttonHandRight.onClick.AddListener(delegate {
                currSelectedSlot = "HandRight";
                switch_to_category_view();
            });
            buttonHandLeft.onClick.AddListener(delegate {
                currSelectedSlot = "HandLeft";
                switch_to_category_view();
            });
            buttonSpellTop.onClick.AddListener(delegate {
                currSelectedSlot = "SpellTop";
                switch_to_category_view();
            });
            buttonSpellLeft.onClick.AddListener(delegate {
                currSelectedSlot = "SpellLeft";
                switch_to_category_view();
            });
            buttonSpellRight.onClick.AddListener(delegate {
                currSelectedSlot = "SpellRight";
                switch_to_category_view();
            });
            buttonSpellBottom.onClick.AddListener(delegate {
                currSelectedSlot = "SpellBottom";
                switch_to_category_view();
            });
        }

        private void init_equipped_image_map()
        {
            equippedItemImages["HipsRight"] = imgHipRight;
            equippedItemImages["HipsLeft"] = imgHipLeft;
            equippedItemImages["BackRight"] = imgBackRight;
            equippedItemImages["BackLeft"] = imgBackLeft;
            equippedItemImages["HandRight"] = imgHandRight;
            equippedItemImages["HandLeft"] = imgHandLeft;
            equippedItemImages["SpellTop"] = imgSpellRight;
            equippedItemImages["SpellLeft"] = imgSpellLeft;
            equippedItemImages["SpellRight"] = imgSpellRight;
            equippedItemImages["SpellBottom"] = imgSpellBottom;
        }
        private void init_categories()
        {
            GameObject currList = GameObject.Instantiate(categoryRowsEntryListTemplate, categoryRowsContent.transform);
            var categories = Catalog.gameData.categories;
            foreach(GameData.Category category in categories){
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
        }

        private void switch_to_category_view()
        {
            viewItemSelect.SetActive(true);
            viewSubItemSelect.SetActive(false);
            viewSubCategorySelect.SetActive(true);
        }
    }
}

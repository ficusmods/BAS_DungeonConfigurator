using System;
using System.Collections.Generic;
using System.Collections;
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
        Scrollbar scrollBarItem;
        GameObject slotHint;
        GameObject itemHint;
        Text slotHintText;
        Text itemHintText;
        Button buttonRandomize;

        bool changes = false;

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
            itemHint = Utils.get_child(viewItemSelect, "Item/ItemHint");
            itemHintText = itemHint.GetComponent<Text>();

            itemRowsContentTemplate = Utils.get_child(viewItemSelect, "Item/ItemRowsTemplate");
            itemRowsEntryListTemplate = Utils.get_child(viewItemSelect, "Item/ItemListTemplate");
            itemRowsEntryTemplate = Utils.get_child(viewItemSelect, "Item/ItemListEntryTemplate");
            scrollBarItem = Utils.get_child(viewItemSelect, "Item/ItemScrollbar").GetComponent<Scrollbar>();

            buttonRandomize = Utils.get_child(inventoryEditor, "RandomizeButton").GetComponent<Button>();

            init_slot_map();
            init_button_callbacks();
            init_random_loot_tables();

            equip_default();

            viewItemSelect.SetActive(false);
            slotHint.SetActive(false);
            itemHint.SetActive(false);
        }

        private void init_random_loot_tables()
        {
            LootTable ltAnyItem = Catalog.GetData<LootTable>("AnyItemRandom");
            LootTable ltAnyWeapon = Catalog.GetData<LootTable>("WeaponRandom");
            LootTable ltAnyApparel = Catalog.GetData<LootTable>("ApparelRandom");
            LootTable ltAnySpell = Catalog.GetData<LootTable>("SpellRandom");
            LootTable ltAnyBow = Catalog.GetData<LootTable>("BowRandom");
            LootTable ltAnyQuiver = Catalog.GetData<LootTable>("QuiverRandom");
            LootTable ltAnyMeleeAI = Catalog.GetData<LootTable>("WeaponRandomMeleeAI");
            LootTable ltAnyBowAI = Catalog.GetData<LootTable>("WeaponRandomBowAI");
            LootTable ltAnyShieldAI = Catalog.GetData<LootTable>("WeaponRandomShieldAI");
            LootTable ltAnyWandAI = Catalog.GetData<LootTable>("WeaponRandomWandAI");
            LootTable ltAnySpellAI = Catalog.GetData<LootTable>("SpellRandomAI");

            var allItems = Catalog.GetDataList(Catalog.Category.Item);

            foreach(var item in allItems)
            {
                ItemData idata = item as ItemData;
                if (!LoadModule.random_excluded_items.Contains(idata.id))
                {
                    LootTable.Drop idrop = new LootTable.Drop();
                    idrop.reference = LootTable.Drop.Reference.Item;
                    idrop.referenceID = idata.id;
                    idrop.probabilityWeight = 1.0f;
                    ltAnyItem.drops.Add(idrop);

                    if (idata.type == ItemData.Type.Weapon
                        || idata.type == ItemData.Type.Quiver
                        || idata.type == ItemData.Type.Shield
                        || idata.type == ItemData.Type.Potion)
                    {
                        ltAnyWeapon.drops.Add(idrop);
                        if (idata.slot == "Bow")
                        {
                            ltAnyBow.drops.Add(idrop);
                        }
                        else if (idata.slot == "Quiver")
                        {
                            ltAnyQuiver.drops.Add(idrop);
                        }

                        if (idata.moduleAI != null)
                        {
                            if (idata.moduleAI.weaponClass == ItemModuleAI.WeaponClass.Bow)
                            {
                                ltAnyBowAI.drops.Add(idrop);
                            }
                            else if (idata.moduleAI.weaponClass == ItemModuleAI.WeaponClass.Shield)
                            {
                                ltAnyShieldAI.drops.Add(idrop);
                            }
                            else if (idata.moduleAI.weaponClass == ItemModuleAI.WeaponClass.Wand)
                            {
                                ltAnyWandAI.drops.Add(idrop);
                            }
                            else if (idata.moduleAI.weaponClass == ItemModuleAI.WeaponClass.Melee && idata.moduleAI.weaponHandling == ItemModuleAI.WeaponHandling.OneHanded)
                            {
                                ltAnyMeleeAI.drops.Add(idrop);
                            }
                        }

                    }
                    else if (idata.type == ItemData.Type.Wardrobe)
                    {

                        ltAnyApparel.drops.Add(idrop);
                    }
                    else if (idata.type == ItemData.Type.Spell)
                    {
                        ItemModuleSpell module = idata.GetModule<ItemModuleSpell>();
                        CatalogData sdata = Catalog.GetData(Catalog.Category.Spell, module.spellId);
                        if (sdata is SpellCastData)
                        {
                            SpellCastData scdata = sdata as SpellCastData;
                            ltAnySpell.drops.Add(idrop);
                            if (scdata.aiCastMaxDistance != float.PositiveInfinity && scdata.aiCastMaxDistance != float.MaxValue)
                            {
                                ltAnySpellAI.drops.Add(idrop);
                            }
                        }
                    }
                }
            }

            ltAnyItem.OnCatalogRefresh();
            ltAnyWeapon.OnCatalogRefresh();
            ltAnyApparel.OnCatalogRefresh();
            ltAnySpell.OnCatalogRefresh();
            ltAnyBow.OnCatalogRefresh();
            ltAnyQuiver.OnCatalogRefresh();
            ltAnyMeleeAI.OnCatalogRefresh();
            ltAnyBowAI.OnCatalogRefresh();
            ltAnyShieldAI.OnCatalogRefresh();
            ltAnyWandAI.OnCatalogRefresh();
            ltAnySpellAI.OnCatalogRefresh();
        }

        private void equip_default()
        {
            ItemData chestData = Catalog.GetData(Catalog.Category.Item, "ApparelCivilianChest") as ItemData;
            ItemData legsData = Catalog.GetData(Catalog.Category.Item, "ApparelCivilianLegs") as ItemData;
            ItemData bootsData = Catalog.GetData(Catalog.Category.Item, "ApparelCivilianBoots") as ItemData;
            slots["ArmorChest"].item = chestData;
            slots["ArmorLegs"].item = legsData;
            slots["ArmorBoots"].item = bootsData;
        }

        private void unequip_all_slots()
        {
            foreach(var entry in slots)
            {
                entry.Value.item = null;
            }
        }

        private void init_slot_map()
        {
            Logger.Detailed("Initializing slots");
            slots["HipsRight"] = new InventoryEditorItemSlot(
                Utils.get_child(viewSlots, "Items/HipRight"), "HipsRight",
                Holder.DrawSlot.HipsRight
                );
            slots["HipsLeft"] = new InventoryEditorItemSlot(
                Utils.get_child(viewSlots, "Items/HipLeft"), "HipsLeft",
                Holder.DrawSlot.HipsLeft
                );
            slots["BackRight"] = new InventoryEditorItemSlot(
                Utils.get_child(viewSlots, "Items/BackRight"), "BackRight",
                Holder.DrawSlot.BackRight
                );
            slots["BackLeft"] = new InventoryEditorItemSlot(
                Utils.get_child(viewSlots, "Items/BackLeft"), "BackLeft",
                Holder.DrawSlot.BackLeft
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
            slots["ArmorBoots"] = new InventoryEditorWardrobeSlot(Utils.get_child(viewSlots, "Armor/Boots"), "ArmorBoots");
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
            if (changes)
            {
                EventManager.onLevelLoad -= apply_changes_impl;
                EventManager.onLevelLoad += apply_changes_impl;
            }
        }

        private void apply_changes_impl(LevelData levelData, EventTime eventTime)
        {
            if (levelData.id.ToLower() != "master" && levelData.id.ToLower() != "home")
            {
                if (eventTime == EventTime.OnEnd)
                {
                    Logger.Basic("Applying inventory changes");
                    GameManager.local.StartCoroutine(change_equipment());
                }
            }
            else
            {
                EventManager.onLevelLoad -= apply_changes_impl;
            }
        }

        private IEnumerator change_equipment()
        {
            while (Player.local.creature.equipment.spawningItemCount > 0)
                yield return new WaitForSeconds(1.0f);

            Player.local.creature.equipment.UnequipWeapons();
            Player.local.creature.equipment.UnequipAllWardrobes();

            Container playerContainer = Player.local.creature.container;
            playerContainer.loadContent = Container.LoadContent.ContainerID;
            playerContainer.containerID = player_container_id;

            ContainerData containterData = Catalog.GetData<ContainerData>(player_container_id);
            containterData.contents = new List<ContainerData.Content>();
            foreach (var entry in slots)
            {
                if (entry.Value.item != null)
                {
                    ContainerData.Content content = entry.Value.as_content();
                    containterData.contents.Add(content);
                    Logger.Detailed("Equipping {0} from slot {1}", entry.Value.item.id, entry.Value.name);
                }
            }

            playerContainer.Load();
            Player.local.creature.mana.Load();
            Player.local.creature.equipment.EquipAllWardrobes(false, false);
            Player.local.creature.equipment.EquipWeapons();
        }

        private void equip_random(InventoryEditorSlot slot, LootTable table, List<ItemData> equipped)
        {
            var pick = table.Pick();
            if(!equipped.Contains(pick))
            {
                equipped.Add(pick);
                slot.item = pick;
            }
        }

        private void randomize_slots()
        {
            unequip_all_slots();
            changes = true;
            List<ItemData> equipped = new List<ItemData>();

            LootTable ltAnyWeapon = Catalog.GetData<LootTable>("WeaponRandom");
            LootTable ltAnyApparel = Catalog.GetData<LootTable>("ApparelRandom");
            LootTable ltAnySpell = Catalog.GetData<LootTable>("SpellRandom");

            equip_random(slots["HipsRight"], ltAnyWeapon, equipped);
            equip_random(slots["HipsLeft"], ltAnyWeapon, equipped);
            equip_random(slots["BackRight"], ltAnyWeapon, equipped);
            equip_random(slots["BackLeft"], ltAnyWeapon, equipped);

            equip_random(slots["ArmorChest"], ltAnyApparel, equipped);
            equip_random(slots["ArmorHelmet"], ltAnyApparel, equipped);
            equip_random(slots["ArmorHandLeft"], ltAnyApparel, equipped);
            equip_random(slots["ArmorHandRight"], ltAnyApparel, equipped);
            equip_random(slots["ArmorLegs"], ltAnyApparel, equipped);
            equip_random(slots["ArmorBoots"], ltAnyApparel, equipped);
            equip_random(slots["ArmorCosmetics1"], ltAnyApparel, equipped);
            equip_random(slots["ArmorCosmetics2"], ltAnyApparel, equipped);
            equip_random(slots["ArmorCosmetics3"], ltAnyApparel, equipped);
            equip_random(slots["ArmorCosmetics4"], ltAnyApparel, equipped);

            equip_random(slots["Spell1"], ltAnySpell, equipped);
            equip_random(slots["Spell2"], ltAnySpell, equipped);
            equip_random(slots["Spell3"], ltAnySpell, equipped);
            equip_random(slots["Spell4"], ltAnySpell, equipped);
            equip_random(slots["Spell5"], ltAnySpell, equipped);
        }

        private void init_button_callbacks()
        {
            buttonRandomize.onClick.AddListener(delegate { randomize_slots(); });

            foreach (var entry in slots)
            {
                entry.Value.onClick += delegate { handle_slot_click(entry.Value); };
                entry.Value.onHoverStart += delegate { show_hint_for_slot(entry.Value); };
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
            slotHint.transform.rotation = slot.objSlot.transform.rotation;
            slotHint.transform.position -= slotHint.transform.forward.normalized * 0.05f;
        }
        private void hide_slot_hint()
        {
            slotHint.SetActive(false);
        }

        private void show_hint_for_item(GameObject entry, ItemData item)
        {
            itemHint.SetActive(true);
            itemHintText.text = item.id;
            itemHint.transform.position = entry.transform.position;
            itemHint.transform.rotation = entry.transform.rotation;
            itemHint.transform.position -= itemHint.transform.forward.normalized * 0.05f;
        }

        private void hide_item_hint()
        {
            itemHint.SetActive(false);
        }

        private void fill_items(InventoryEditorSlot slot)
        {
            Logger.Detailed("Filling item list based on {0}", slot.name);
            foreach (Transform child in viewSubItemSelect.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            viewItemSelect.SetActive(true);
            viewSubItemSelect.SetActive(true);
            itemRowsContentTemplate.SetActive(true);
            itemRowsEntryListTemplate.SetActive(true);
            itemRowsEntryTemplate.SetActive(true);
            
            List<ItemData> items = slot.get_possible_coices();
            GameObject rows = GameObject.Instantiate(itemRowsContentTemplate, viewSubItemSelect.transform);
            rows.SetActive(true);
            GameObject column = GameObject.Instantiate(itemRowsEntryListTemplate, rows.transform);
            column.SetActive(true);
            foreach(var item in items)
            {
                if(column.transform.childCount > inventorySelectorEntryPerRow)
                {
                    column = GameObject.Instantiate(itemRowsEntryListTemplate, rows.transform);
                }
                
                GameObject currEntry = GameObject.Instantiate(itemRowsEntryTemplate, column.transform);
                currEntry.SetActive(true);
                RawImage currImg = currEntry.GetComponentInChildren<RawImage>();
                Catalog.LoadAssetAsync<Texture>(item.iconAddress, (Texture t) => {
                    currImg.texture = t;
                }, "Item");
                Button currButton = currEntry.GetComponent<Button>();
                currButton.onClick.AddListener(delegate {
                    slot.item = item;
                    changes = true;
                    viewItemSelect.SetActive(false);
                });

                var buttonEventTrigger = currButton.gameObject.GetComponent<EventTrigger>();

                EventTrigger.Entry entryHoverStart = new EventTrigger.Entry();
                entryHoverStart.eventID = EventTriggerType.PointerEnter;
                entryHoverStart.callback.AddListener(delegate { show_hint_for_item(currEntry, item); });
                
                EventTrigger.Entry entryHoverEnd = new EventTrigger.Entry();
                entryHoverEnd.eventID = EventTriggerType.PointerExit;
                entryHoverEnd.callback.AddListener(delegate { hide_item_hint(); });

                buttonEventTrigger.triggers.Add(entryHoverEnd);
                buttonEventTrigger.triggers.Add(entryHoverStart);
            }
            ScrollRect scrollRect = viewSubItemSelect.GetComponent<ScrollRect>();
            scrollRect.content = rows.GetComponent<RectTransform>();
            scrollRect.verticalNormalizedPosition = 0;
            scrollBarItem.value = 0;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rows.GetComponent<RectTransform>());
            
            itemRowsContentTemplate.SetActive(false);
            itemRowsEntryListTemplate.SetActive(false);
            itemRowsEntryTemplate.SetActive(false);
        }
    }
}

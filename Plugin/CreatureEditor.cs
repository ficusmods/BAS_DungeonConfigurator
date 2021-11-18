using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThunderRoad;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonConfigurator
{
    public class CreatureEditor
    {

        protected GameObject creatureEditor;

        Toggle toggleCreatureReference;
        Toggle toggleTableReference;
        ToggleGroup tgroupReference;
        Toggle toggleNoneContainer;
        Toggle toggleOverrideContainer;
        ToggleGroup tgroupContainer;
        Toggle toggleNoneBrain;
        Toggle toggleOverrideBrain;
        ToggleGroup tgroupBrain;

        GameObject selectorAreaContent;
        GameObject toggleSelectorTemplate;
        GameObject tgroupSelector;

        CreatureTable originalDungeonTable_DungeonHumansHardMelee;
        CreatureTable originalDungeonTable_DungeonHumansMix;
        CreatureTable originalDungeonTable_DungeonHumansNormalMelee;
        CreatureTable originalDungeonTable_DungeonHumansRange;
        CreatureTable originalDungeonTable_DungeonHumansRareMelee;

        string idCreature = "";
        string idTable = "";
        string idContainer = "";
        string idBrain = "";

        public CreatureEditor(GameObject obj)
        {
            creatureEditor = obj;

            toggleCreatureReference =   Utils.get_child(creatureEditor, "Reference/ToggleCreatureReference").GetComponent<Toggle>();
            toggleTableReference =      Utils.get_child(creatureEditor, "Reference/ToggleTableReference").GetComponent<Toggle>();
            tgroupReference =           Utils.get_child(creatureEditor, "Reference/ToggleGroupReference").GetComponent<ToggleGroup>();
            toggleNoneContainer =       Utils.get_child(creatureEditor, "Container/ToggleNoneContainer").GetComponent<Toggle>();
            toggleOverrideContainer =   Utils.get_child(creatureEditor, "Container/ToggleOverrideContainer").GetComponent<Toggle>();
            tgroupContainer =           Utils.get_child(creatureEditor, "Container/ToggleGroupContainer").GetComponent<ToggleGroup>();
            toggleNoneBrain =           Utils.get_child(creatureEditor, "Brain/ToggleNoneBrain").GetComponent<Toggle>();
            toggleOverrideBrain =       Utils.get_child(creatureEditor, "Brain/ToggleOverrideBrain").GetComponent<Toggle>();
            tgroupBrain =               Utils.get_child(creatureEditor, "Brain/ToggleGroupBrain").GetComponent<ToggleGroup>();

            selectorAreaContent = Utils.get_child(creatureEditor, "SelectorArea/ScrollView/SelectorContent");
            toggleSelectorTemplate = Utils.get_child(creatureEditor, "SelectorArea/ScrollView/ToggleTemplate");
            tgroupSelector = Utils.get_child(creatureEditor, "SelectorArea/ScrollView/ToggleGroupSelector");

            toggleSelectorTemplate.SetActive(false);

            toggleCreatureReference.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    fillSelectorWithCreature();
                }
                else
                {
                    Logger.Detailed("Cleared Creature id");
                    idCreature = "";
                }
            });
            toggleTableReference.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    fillSelectorWithTable();
                } 
                else
                {
                    Logger.Detailed("Cleared Table id");
                    idTable = "";
                }
            });
            toggleNoneContainer.onValueChanged.AddListener((bool active) =>{
                if (active)
                {
                    clearSelector();
                    Logger.Detailed("Cleared Container id");
                    idContainer = "";
                }
            });
            toggleOverrideContainer.onValueChanged.AddListener((bool active) =>
            {
                if (active)
                {
                    fillSelectorWithContainer();
                }
                else
                {
                    Logger.Detailed("Cleared Container id");
                    idContainer = "";
                }
            });
            toggleNoneBrain.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    clearSelector();
                    Logger.Detailed("Cleared Brain id");
                    idBrain = "";
                }
            });
            toggleOverrideBrain.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    fillSelectorWithBrain();
                }
                else
                {
                    idBrain = "";
                }
            });

            originalDungeonTable_DungeonHumansHardMelee = Catalog.GetData<CreatureTable>("DungeonHumansHardMelee").CloneJson();
            originalDungeonTable_DungeonHumansMix = Catalog.GetData<CreatureTable>("DungeonHumansMix").CloneJson();
            originalDungeonTable_DungeonHumansNormalMelee = Catalog.GetData<CreatureTable>("DungeonHumansNormalMelee").CloneJson();
            originalDungeonTable_DungeonHumansRange = Catalog.GetData<CreatureTable>("DungeonHumansRange").CloneJson();
            originalDungeonTable_DungeonHumansRareMelee = Catalog.GetData<CreatureTable>("DungeonHumansRareMelee").CloneJson();

        }

        public virtual void setHidden(bool hidden)
        {
            if (hidden)
            {
                creatureEditor.SetActive(false);
            }
            else
            {
                creatureEditor.SetActive(true);
            }
        }
        public virtual void apply_changes()
        {
            Logger.Basic("Applying changes to the creature tables");
            CreatureTable.Drop newdrop = new CreatureTable.Drop();

            if(idTable != "")
            {
                Logger.Detailed("Changing drop to Table with id: {0}", idTable);
                newdrop.reference = CreatureTable.Drop.Reference.Table;
                newdrop.referenceID = idTable;

            }
            else if(idCreature != "")
            {
                Logger.Detailed("Changing drop to Creature with id: {0}", idCreature);
                newdrop.reference = CreatureTable.Drop.Reference.Creature;
                newdrop.referenceID = idCreature;
            }
            else
            {
                Logger.Detailed("No reference to creatures set");
                return;
            }

            newdrop.overrideContainer = false;
            newdrop.overrideBrain = false;

            if(idBrain != "")
            {
                Logger.Detailed("Changing drop to override Brain with id: {0}", idBrain);
                newdrop.overrideBrain = true;
                newdrop.overrideBrainID = idBrain;
            }

            if (idContainer != "")
            {
                Logger.Detailed("Changing drop to override Container with id: {0}", idContainer);
                newdrop.overrideContainer = true;
                newdrop.overrideContainerID = idContainer;
            }

            newdrop.factionID = 0;
            newdrop.overrideFaction = false;
            
            newdrop.probabilityWeights[0] = 1;
            newdrop.probabilityWeights[1] = 1;
            newdrop.probabilityWeights[2] = 1;
            newdrop.probabilityWeights[3] = 1;
            newdrop.probabilityWeights[4] = 1;

            CreatureTable newtable = Catalog.GetData<CreatureTable>("DungeonHumansHardMelee").CloneJson();
            newtable.drops = new List<CreatureTable.Drop>();
            newtable.drops.Add(newdrop);
            alter_table("DungeonHumansHardMelee", newtable);
            alter_table("DungeonHumansMix", newtable);
            alter_table("DungeonHumansNormalMelee", newtable);
            alter_table("DungeonHumansRange", newtable);
            alter_table("DungeonHumansRareMelee", newtable);
        }

        public virtual void revert_changes()
        {
            Logger.Basic("Reverting changes made to the creature tables");
            alter_table("DungeonHumansHardMelee", originalDungeonTable_DungeonHumansHardMelee);
            alter_table("DungeonHumansMix", originalDungeonTable_DungeonHumansMix);
            alter_table("DungeonHumansNormalMelee", originalDungeonTable_DungeonHumansNormalMelee);
            alter_table("DungeonHumansRange", originalDungeonTable_DungeonHumansRange);
            alter_table("DungeonHumansRareMelee", originalDungeonTable_DungeonHumansRareMelee);
        }

        private void alter_table(string tableid, CreatureTable change)
        {
            CreatureTable data = Catalog.GetData<CreatureTable>(tableid);
            data.description = change.description;
            data.drops = change.drops;
            data.generationBrain = change.generationBrain;
            data.generationContainer = change.generationContainer;
            data.generationFaction = change.generationFaction;
            data.minMaxDifficulty = change.minMaxDifficulty;
        }

        public virtual void fillSelectorWithCreature()
        {
            clearSelector();
            var creaturesData = Catalog.GetDataList(Catalog.Category.Creature);
            foreach(CatalogData data in creaturesData)
            {
                CreatureData cdata = data as CreatureData;
                GameObject entry = GameObject.Instantiate(toggleSelectorTemplate, selectorAreaContent.transform);
                Text label =  entry.GetComponentInChildren<Text>(true);
                Toggle toggle = entry.GetComponentInChildren<Toggle>(true);
                toggle.onValueChanged.AddListener((bool active) => {
                    if (active)
                    {
                        Logger.Detailed("Selected Creature {0}", cdata.id);
                        idCreature = cdata.id;
                    }
                });
                label.text = cdata.id;
                entry.SetActive(true);
            }

        }
        public virtual void fillSelectorWithTable()
        {
            clearSelector();
            var tablesData = Catalog.GetDataList(Catalog.Category.CreatureTable);
            foreach (CatalogData data in tablesData)
            {
                CreatureTable ctdata = data as CreatureTable;
                GameObject entry = GameObject.Instantiate(toggleSelectorTemplate, selectorAreaContent.transform);
                Text label = entry.GetComponentInChildren<Text>(true);
                Toggle toggle = entry.GetComponentInChildren<Toggle>(true);
                toggle.onValueChanged.AddListener((bool active) => {
                    if (active)
                    {
                        Logger.Detailed("Selected CreatureTable {0}", ctdata.id);
                        idTable = ctdata.id;
                    }
                });
                label.text = ctdata.id;
                entry.SetActive(true);
            }
        }
        public virtual void fillSelectorWithContainer()
        {
            clearSelector();
            var containersData = Catalog.GetDataList(Catalog.Category.Container);
            foreach (CatalogData data in containersData)
            {
                ContainerData cdata = data as ContainerData;
                GameObject entry = GameObject.Instantiate(toggleSelectorTemplate, selectorAreaContent.transform);
                Text label = entry.GetComponentInChildren<Text>(true);
                Toggle toggle = entry.GetComponentInChildren<Toggle>(true);
                toggle.onValueChanged.AddListener((bool active) => {
                    if (active)
                    {
                        Logger.Detailed("Selected Container {0}", cdata.id);
                        idContainer = cdata.id;
                    }
                });
                label.text = cdata.id;
                entry.SetActive(true);
            }
        }
        public virtual void fillSelectorWithBrain()
        {
            clearSelector();
            var brainsData = Catalog.GetDataList(Catalog.Category.Brain);
            foreach (CatalogData data in brainsData)
            {
                BrainData bdata = data as BrainData;
                GameObject entry = GameObject.Instantiate(toggleSelectorTemplate, selectorAreaContent.transform);
                Text label = entry.GetComponentInChildren<Text>(true);
                Toggle toggle = entry.GetComponentInChildren<Toggle>(true);
                toggle.onValueChanged.AddListener((bool active) => {
                    if (active)
                    {
                        Logger.Detailed("Selected Brain {0}", bdata.id);
                        idBrain = bdata.id;
                    }
                });
                label.text = bdata.id;
                entry.SetActive(true);
            }
        }
        public virtual void clearSelector()
        {
            foreach (Transform child in selectorAreaContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}

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

        CreatureTable ctDungeonConfiguratorGeneral;
        CreatureTable ctDungeonConfiguratorRanged;
        CreatureTable ctDungeonConfiguratorSpecial;
        CreatureTable ctDungeonConfiguratorEmpty;

        LinkedList<string> idCreatures = new LinkedList<string>();
        LinkedList<string> idTables = new LinkedList<string>();
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
                    Logger.Detailed("Switched to creature selector");
                    idCreatures.Clear();
                    fillSelectorWithCreature();
                }
            });
            toggleTableReference.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    Logger.Detailed("Switched to creature table selector");
                    idTables.Clear();
                    fillSelectorWithTable();
                } 
            });
            toggleNoneContainer.onValueChanged.AddListener((bool active) =>{
                if (active)
                {
                    Logger.Detailed("Container set to None");
                    clearSelector();
                    idContainer = "";
                }
            });
            toggleOverrideContainer.onValueChanged.AddListener((bool active) =>
            {
                if (active)
                {
                    Logger.Detailed("Container set to Override");
                    idContainer = "";
                    fillSelectorWithContainer();
                }
            });
            toggleNoneBrain.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    Logger.Detailed("Brain set to None");
                    clearSelector();
                    idBrain = "";
                }
            });
            toggleOverrideBrain.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    Logger.Detailed("Brain set to Override");
                    idBrain = "";
                    fillSelectorWithBrain();
                }
            });

            ctDungeonConfiguratorGeneral = Catalog.GetData<CreatureTable>("DungeonConfiguratorGeneral");
            ctDungeonConfiguratorRanged = Catalog.GetData<CreatureTable>("DungeonConfiguratorRanged");
            ctDungeonConfiguratorSpecial = Catalog.GetData<CreatureTable>("DungeonConfiguratorSpecial");
            ctDungeonConfiguratorEmpty = Catalog.GetData<CreatureTable>("DungeonConfiguratorGeneral").CloneJson();
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

        private List<CreatureTable.Drop> getCreatureDrops()
        {
            List<CreatureTable.Drop> ret = new List<CreatureTable.Drop>();
            foreach(string id in idCreatures)
            {
                Logger.Detailed("Adding drop via creature id: {0}", id);
                CreatureTable.Drop currDrop = new CreatureTable.Drop();
                currDrop.reference = CreatureTable.Drop.Reference.Table;
                currDrop.referenceID = id;
                currDrop.overrideContainer = false;
                currDrop.overrideBrain = false;
                if(idBrain != "")
                {
                    Logger.Detailed("Changing drop {0} to override Brain with id: {1}", id, idBrain);
                    currDrop.overrideBrain = true;
                    currDrop.overrideBrainID = idBrain;
                }
                if (idContainer != "")
                {
                    Logger.Detailed("Changing drop {0} to override Container with id: {1}", id, idContainer);
                    currDrop.overrideContainer = true;
                    currDrop.overrideContainerID = idContainer;
                }
                currDrop.factionID = 0;
                currDrop.overrideFaction = false;
                
                currDrop.probabilityWeights[0] = 1;
                currDrop.probabilityWeights[1] = 1;
                currDrop.probabilityWeights[2] = 1;
                currDrop.probabilityWeights[3] = 1;
                currDrop.probabilityWeights[4] = 1;
            }
            return ret;
        }

        private List<CreatureTable.Drop> getTableDrops()
        {
            List<CreatureTable.Drop> ret = new List<CreatureTable.Drop>();
            foreach(string id in idTables)
            {
                Logger.Detailed("Adding drop via CreatureTable id: {0}", id);
                CreatureTable.Drop currDrop = new CreatureTable.Drop();
                currDrop.reference = CreatureTable.Drop.Reference.Table;
                currDrop.referenceID = id;
                currDrop.overrideContainer = false;
                currDrop.overrideBrain = false;
                if(idBrain != "")
                {
                    Logger.Detailed("Changing drop {0} to override Brain with id: {1}", id, idBrain);
                    currDrop.overrideBrain = true;
                    currDrop.overrideBrainID = idBrain;
                }
                if (idContainer != "")
                {
                    Logger.Detailed("Changing drop {0} to override Container with id: {1}", id, idContainer);
                    currDrop.overrideContainer = true;
                    currDrop.overrideContainerID = idContainer;
                }
                currDrop.factionID = 0;
                currDrop.overrideFaction = false;
                
                currDrop.probabilityWeights[0] = 1;
                currDrop.probabilityWeights[1] = 1;
                currDrop.probabilityWeights[2] = 1;
                currDrop.probabilityWeights[3] = 1;
                currDrop.probabilityWeights[4] = 1;
            }
            return ret;
        }

        public virtual void apply_changes()
        {
            Logger.Basic("Applying creature editor changes");
            CreatureTable newtable = ctDungeonConfiguratorEmpty.CloneJson();
            var creatureDrops = getCreatureDrops();
            var tableDrops = getTableDrops();
            newtable.drops = creatureDrops.Concat(tableDrops).ToList();

            if(newtable.drops.Count > 0)
            {
                alter_table("DungeonConfiguratorGeneral", newtable);
                Level.current.OnLevelEvent += HandleLevelLoad;
            }
        }

        private void HandleLevelLoad()
        {
            foreach(Room room in Level.current.dungeon.rooms)
            {
                var spawners = room.GetComponentsInChildren<CreatureSpawner>();
                foreach(CreatureSpawner spawner in spawners)
                {
                    spawner.creatureTableID = "DungeonConfiguratorGeneral";
                }
            } 
            Level.current.OnLevelEvent -= HandleLevelLoad;
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
                        idCreatures.AddLast(cdata.id);
                    }
                    else
                    {
                        idCreatures.Remove(cdata.id);
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
                        idTables.AddLast(ctdata.id);
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

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

        CreatureTable ctDungeonConfiguratorEmpty;
        WaveData wdDungeonConfiguratorGeneral;
        WaveData wdDungeonConfiguratorDuel;

        LinkedList<string> idCreatures = new LinkedList<string>();
        LinkedList<string> idTables = new LinkedList<string>();
        LinkedList<string> idContainers = new LinkedList<string>();
        string idBrain = "";

        int waveGroupCount = 8;

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
                    idContainers.Clear();
                }
            });
            toggleOverrideContainer.onValueChanged.AddListener((bool active) =>
            {
                if (active)
                {
                    Logger.Detailed("Container set to Override");
                    idContainers.Clear();
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

            ctDungeonConfiguratorEmpty = Catalog.GetData<CreatureTable>("DungeonConfiguratorGeneral").CloneJson();
            wdDungeonConfiguratorGeneral = Catalog.GetData<WaveData>("DungeonConfiguratorGeneral");
            wdDungeonConfiguratorDuel = Catalog.GetData<WaveData>("DungeonConfiguratorDuel");
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

            foreach (string id in idCreatures)
            {
                Logger.Detailed("Adding drop via creature id: {0}", id);
                CreatureTable.Drop currDrop = new CreatureTable.Drop();
                currDrop.reference = CreatureTable.Drop.Reference.Creature;
                currDrop.referenceID = id;
                currDrop.overrideContainer = false;
                currDrop.overrideBrain = false;
                if(idBrain != "")
                {
                    Logger.Detailed("Changing drop {0} to override Brain with id: {1}", id, idBrain);
                    currDrop.overrideBrain = true;
                    currDrop.overrideBrainID = idBrain;
                }

                if (idContainers.First != null)
                {
                    currDrop.overrideContainer = true;
                    currDrop.overrideContainerID = "DCCurrent";
                }

                currDrop.factionID = 0;
                currDrop.overrideFaction = false;
                
                currDrop.probabilityWeights[0] = 1;
                currDrop.probabilityWeights[1] = 1;
                currDrop.probabilityWeights[2] = 1;
                currDrop.probabilityWeights[3] = 1;
                currDrop.probabilityWeights[4] = 1;
                ret.Add(currDrop);
            }
            return ret;
        }

        private List<CreatureTable.Drop> getTableDrops()
        {
            List<CreatureTable.Drop> ret = new List<CreatureTable.Drop>();

            foreach (string id in idTables)
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

                if (idContainers.First != null)
                {
                    currDrop.overrideContainer = true;
                    currDrop.overrideContainerID = "DCCurrent";
                }

                currDrop.factionID = 0;
                currDrop.overrideFaction = false;
                
                currDrop.probabilityWeights[0] = 1;
                currDrop.probabilityWeights[1] = 1;
                currDrop.probabilityWeights[2] = 1;
                currDrop.probabilityWeights[3] = 1;
                currDrop.probabilityWeights[4] = 1;
                ret.Add(currDrop);
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

            ContainerData mergedContainer = Catalog.GetData<ContainerData>("DCCurrent");
            mergedContainer.contents.Clear();
            for (var node = idContainers.First; node != null; node = node.Next)
            {
                Logger.Detailed("Changing drop override Container with id: {0}", node.Value);
                ContainerData currContainer = Catalog.GetData<ContainerData>(node.Value);
                mergedContainer.contents.AddRange(currContainer.contents);
            }
            mergedContainer.OnCatalogRefresh();

            if (newtable.drops.Count > 0)
            {
                alter_table("DungeonConfiguratorGeneral", newtable);
                fill_wave_data(wdDungeonConfiguratorGeneral);
                fill_wave_data(wdDungeonConfiguratorDuel);
                EventManager.onLevelLoad -= HandleLevelLoad;
                EventManager.onLevelLoad += HandleLevelLoad;
            }
        }

        private void fill_wave_data(WaveData waveData)
        {
            waveData.groups = new List<WaveData.Group>();
            WaveData.Group group = new WaveData.Group();
            group.factionID = 0;
            group.overrideFaction = false;
            group.reference = WaveData.Group.Reference.Table;
            group.referenceID = "DungeonConfiguratorGeneral";
            group.minMaxCount = new Vector2Int(1, 1);
            group.overrideContainer = false;
            group.overrideBrain = false;
            group.overrideMaxMelee = false;
            group.spawnPointIndex = -1;
            group.prereqGroupIndex = -1;
            group.prereqGroup = null;
            group.prereqMaxRemainingAlive = 0;
            for (int i = 0; i < waveGroupCount; i++)
            {
                waveData.groups.Add(group.CloneJson());
            }
            waveData.OnCatalogRefresh();
        }

        private void HandleLevelLoad(LevelData levelData, EventTime eventTime)
        {
            if (levelData.id.ToLower() != "master" && levelData.id.ToLower() != "home")
            {
                if (eventTime == EventTime.OnEnd)
                {
                    if (Level.current.dungeon != null)
                    {
                        Logger.Detailed("Replacing creature spawner ids");
                        foreach (Room room in Level.current.dungeon.rooms)
                        {
                            Logger.Detailed("Replacing creatures in room {0}", room.name);
                            foreach (Creature c in new List<Creature>(room.creatures))
                            {
                                c.Despawn();
                            }

                            room.spawnerNPCCount = 0;

                            var spawners = room.GetComponentsInChildren<CreatureSpawner>(true).Shuffle<CreatureSpawner>();
                            foreach (CreatureSpawner spawner in spawners)
                            {
                                spawner.creatureTableID = "DungeonConfiguratorGeneral";
                                Logger.Detailed("Respawning creatures in {0} using {1} (NPC: {2}/{3})", room.name, spawner.name, room.spawnerNPCCount, room.spawnerMaxNPC);
                                if (spawner.ignoreRoomMaxNPC)
                                {
                                    if (FieldAccess.WriteProperty(spawner, "CurrentState", CreatureSpawner.State.Init))
                                    {
                                        spawner.Spawn();
                                        if (spawner.CurrentState == CreatureSpawner.State.Spawning)
                                            ++room.spawnerNPCCount;
                                    }
                                }

                                else if (room.spawnerNPCCount < Mathf.Min(Catalog.gameData.platformParameters.maxRoomNpc, room.spawnerMaxNPC))
                                {
                                    if (FieldAccess.WriteProperty(spawner, "CurrentState", CreatureSpawner.State.Init))
                                    {
                                        spawner.Spawn();
                                        if (spawner.CurrentState == CreatureSpawner.State.Spawning)
                                            ++room.spawnerNPCCount;
                                    }
                                }
                            }

                            Logger.Detailed("Replacing wave spawner ids in room {0}", room.name);
                            foreach (WaveSpawner spawner in room.GetComponentsInChildren<WaveSpawner>(true).ToList())
                            {
                                Logger.Detailed("Replacing WaveSpawner creatures in {0}", spawner.name);
                                spawner.startWaveId = "DungeonConfiguratorGeneral";
                                spawner.waveData = wdDungeonConfiguratorGeneral;
                            }
                        }
                    }
                }
            }
            else
            {
                EventManager.onLevelLoad -= HandleLevelLoad;
            }
        }

        private void alter_table(string tableid, CreatureTable change)
        {
            Logger.Detailed("Altering CreatureTable {0} with {1}", tableid, change.id);
            CreatureTable data = Catalog.GetData<CreatureTable>(tableid);
            data.description = change.description;
            data.drops = change.drops;
            data.generationBrain = change.generationBrain;
            data.generationContainer = change.generationContainer;
            data.generationFaction = change.generationFaction;
            data.minMaxDifficulty = change.minMaxDifficulty;
            data.OnCatalogRefresh();
        }

        public virtual void fillSelectorWithCreature()
        {
            clearSelector();
            var creaturesData = Catalog.GetDataList(Catalog.Category.Creature);
            creaturesData.Sort(delegate (CatalogData data1, CatalogData data2) {
                return String.Compare(data1.id, data2.id, StringComparison.Ordinal);
            });
            foreach (CatalogData data in creaturesData)
            {
                CreatureData cdata = data as CreatureData;
                GameObject entry = GameObject.Instantiate(toggleSelectorTemplate, selectorAreaContent.transform);
                Text label =  entry.GetComponentInChildren<Text>(true);
                Toggle toggle = entry.GetComponentInChildren<Toggle>(true);
                toggle.group = null;
                toggle.onValueChanged.AddListener((bool active) => {
                    if (active)
                    {
                        Logger.Detailed("Selected Creature {0}", cdata.id);
                        idCreatures.AddLast(cdata.id);
                    }
                    else
                    {
                        idCreatures.Remove(cdata.id);
                        Logger.Detailed("Removed Creature {0}", cdata.id);
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
            tablesData.Sort(delegate (CatalogData data1, CatalogData data2) {
                return String.Compare(data1.id, data2.id, StringComparison.Ordinal);
            });
            foreach (CatalogData data in tablesData)
            {
                CreatureTable ctdata = data as CreatureTable;
                GameObject entry = GameObject.Instantiate(toggleSelectorTemplate, selectorAreaContent.transform);
                Text label = entry.GetComponentInChildren<Text>(true);
                Toggle toggle = entry.GetComponentInChildren<Toggle>(true);
                toggle.group = null;
                toggle.onValueChanged.AddListener((bool active) => {
                    if (active)
                    {
                        Logger.Detailed("Selected CreatureTable {0}", ctdata.id);
                        idTables.AddLast(ctdata.id);
                    }
                    else
                    {
                        idTables.Remove(ctdata.id);
                        Logger.Detailed("Removed CreatureTable {0}", ctdata.id);
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
            containersData.Sort(delegate (CatalogData data1, CatalogData data2) {
                return String.Compare(data1.id, data2.id, StringComparison.Ordinal);
            });
            foreach (CatalogData data in containersData)
            {
                ContainerData cdata = data as ContainerData;
                GameObject entry = GameObject.Instantiate(toggleSelectorTemplate, selectorAreaContent.transform);
                Text label = entry.GetComponentInChildren<Text>(true);
                Toggle toggle = entry.GetComponentInChildren<Toggle>(true);
                toggle.group = null;
                toggle.onValueChanged.AddListener((bool active) => {
                    if (active)
                    {
                        Logger.Detailed("Selected Container {0}", cdata.id);
                        idContainers.AddLast(cdata.id);
                    }
                    else
                    {
                        idContainers.Remove(cdata.id);
                        Logger.Detailed("Removed Container {0}", cdata.id);
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
            brainsData.Sort(delegate (CatalogData data1, CatalogData data2) {
                return String.Compare(data1.id, data2.id, StringComparison.Ordinal);
            });
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

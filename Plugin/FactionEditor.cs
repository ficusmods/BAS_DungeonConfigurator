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
    public class FactionEditor
    {

        enum FactionType
        {
            AllAgainst,
            Default,
            Three,
            AlliesEnemies,
            Peaceful,
            FFA
        }

        protected GameObject factionEditor;

        Toggle toggleAllAgainst;
        Toggle toggleDefault;
        Toggle toggleThreeCompeting;
        Toggle toggleAlliesEnemies;
        Toggle togglePeaceful;
        Toggle toggleFFA;

        FactionType prevSelected = FactionType.AllAgainst;
        FactionType selected = FactionType.AllAgainst;

        public FactionEditor(GameObject obj)
        {
            factionEditor = obj;

            toggleAllAgainst = Utils.get_child(factionEditor, "Toggles/ToggleAllAgainst").GetComponent<Toggle>();
            toggleDefault = Utils.get_child(factionEditor, "Toggles/ToggleDefault").GetComponent<Toggle>();
            toggleThreeCompeting = Utils.get_child(factionEditor, "Toggles/ToggleThree").GetComponent<Toggle>();
            toggleAlliesEnemies = Utils.get_child(factionEditor, "Toggles/ToggleAllies").GetComponent<Toggle>();
            togglePeaceful = Utils.get_child(factionEditor, "Toggles/TogglePeaceful").GetComponent<Toggle>();
            toggleFFA = Utils.get_child(factionEditor, "Toggles/ToggleFFA").GetComponent<Toggle>();

            toggleAllAgainst.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    selected = FactionType.AllAgainst;
                    Logger.Detailed("Switched to faction type {0}", selected);
                }
            });

            toggleDefault.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    selected = FactionType.Default;
                    Logger.Detailed("Switched to faction type {0}", selected);
                }
            });

            toggleThreeCompeting.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    selected = FactionType.Three;
                    Logger.Detailed("Switched to faction type {0}", selected);
                }
            });

            toggleAlliesEnemies.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    selected = FactionType.AlliesEnemies;
                    Logger.Detailed("Switched to faction type {0}", selected);
                }
            });

            togglePeaceful.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    selected = FactionType.Peaceful;
                    Logger.Detailed("Switched to faction type {0}", selected);
                }
            });

            toggleFFA.onValueChanged.AddListener((bool active) => {
                if (active)
                {
                    selected = FactionType.FFA;
                    Logger.Detailed("Switched to faction type {0}", selected);
                }
            });

            toggleAllAgainst.isOn = true;
        }

        private void EventManager_onCreatureSpawn(Creature creature)
        {
            if (!creature.isPlayer)
            {
                int faction;
                switch (prevSelected)
                {
                    case FactionType.AllAgainst:
                        creature.SetFaction(3);
                        break;
                    case FactionType.Default:
                        break;
                    case FactionType.FFA:
                        creature.SetFaction(0);
                        break;
                    case FactionType.Peaceful:
                        creature.SetFaction(2);
                        break;
                    case FactionType.AlliesEnemies:
                        faction = UnityEngine.Random.Range(2, 4);
                        creature.SetFaction(faction);
                        break;
                    case FactionType.Three:
                        faction = UnityEngine.Random.Range(3, 6);
                        creature.SetFaction(faction);
                        break;
                    default:
                        break;
                }
            }
        }

        public virtual void setHidden(bool hidden)
        {
            if (hidden)
            {
                factionEditor.SetActive(false);
            }
            else
            {
                factionEditor.SetActive(true);
            }
        }

        public virtual void apply_changes()
        {
            prevSelected = selected;
            EventManager.onCreatureSpawn -= EventManager_onCreatureSpawn;
            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
            EventManager.onLevelLoad -= EventManager_onLevelLoad;
            EventManager.onLevelLoad += EventManager_onLevelLoad;
        }

        private void EventManager_onLevelLoad(LevelData levelData, EventTime eventTime)
        {
            if (levelData.id.ToLower() == "master" || levelData.id.ToLower() == "home")
            {
                EventManager.onCreatureSpawn -= EventManager_onCreatureSpawn;
            }
        }
    }
}

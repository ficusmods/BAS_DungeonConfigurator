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
    public class LevelSelector
    {

        protected GameObject levelSelector;

        GameObject selectorAreaContent;
        GameObject toggleSelectorTemplate;
        GameObject tgroupSelector;

        public LevelData selected;

        public LevelSelector(GameObject obj)
        {
            levelSelector = obj;

            selectorAreaContent = Utils.get_child(levelSelector, "SelectorArea/ScrollView/SelectorContent");
            toggleSelectorTemplate = Utils.get_child(levelSelector, "SelectorArea/ScrollView/ToggleTemplate");
            tgroupSelector = Utils.get_child(levelSelector, "SelectorArea/ScrollView/ToggleGroupSelector");

            toggleSelectorTemplate.SetActive(false);

            selected = Catalog.GetData<LevelData>("Dungeon");
        }

        public virtual void setHidden(bool hidden)
        {
            if(hidden == false)
            {
                levelSelector.SetActive(true);
                fillSelectorWithLevel();
            }
            else
            {
                levelSelector.SetActive(false);
            }
        }

        public virtual void fillSelectorWithLevel()
        {
            clearSelector();
            var levelsData = Catalog.GetDataList(Catalog.Category.Level);
            foreach (CatalogData data in levelsData)
            {
                LevelData ldata = data as LevelData;
                GameObject entry = GameObject.Instantiate(toggleSelectorTemplate, selectorAreaContent.transform);
                Text label = entry.GetComponentInChildren<Text>(true);
                Toggle toggle = entry.GetComponentInChildren<Toggle>(true);
                toggle.onValueChanged.AddListener((bool active) => {
                    if (active)
                    {
                        Logger.Detailed("Selected Level {0}", ldata.id);
                        selected = ldata;
                    }
                });
                label.text = ldata.id;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using ThunderRoad;

namespace DungeonConfigurator
{
    class RoomEditor
    {
        GameObject roomEditor;

        Button buttonNpcCountDecrease;
        Button buttonNpcCountIncrease;
        Text textNpcCount;
        int additional_npc_count = 0;

        public RoomEditor(GameObject obj)
        {
            roomEditor = obj;

            buttonNpcCountDecrease = Utils.get_child(roomEditor, "NpcCountEditor/DecreaseButton").GetComponent<Button>();
            buttonNpcCountIncrease = Utils.get_child(roomEditor, "NpcCountEditor/IncreaseButton").GetComponent<Button>();
            textNpcCount = Utils.get_child(roomEditor, "NpcCountEditor/ValueArea/ValueText").GetComponent<Text>();

            buttonNpcCountDecrease.onClick.AddListener(decrease_npc_count);
            buttonNpcCountIncrease.onClick.AddListener(increase_npc_count);
        }
        public virtual void setHidden(bool hidden)
        {
            if (hidden)
            {
                roomEditor.SetActive(false);
            }
            else
            {
                roomEditor.SetActive(true);
            }
        }
        public virtual void increase_npc_count()
        {
            if(additional_npc_count <= int.MaxValue-1)
            {
                additional_npc_count++;
                textNpcCount.text = additional_npc_count.ToString();
            }
        }

        public virtual void decrease_npc_count()
        {
            if (additional_npc_count > 0)
            {
                additional_npc_count--;
                textNpcCount.text = additional_npc_count.ToString();
            }
        }

        public virtual void apply_changes()
        {
            EventManager.onLevelLoad += (LevelData levelData, EventTime eventTime) =>
            {
                if(eventTime == EventTime.OnEnd)
                {
                    var module = Level.current.dungeon.gameObject.AddComponent<RoomEditorModule>();
                    module.additional_npc_count = additional_npc_count;
                    module.apply_changes();
                }
            };
        }
    }
}

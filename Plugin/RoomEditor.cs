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

        Button buttonRoomNpcCountDecrease;
        Button buttonRoomNpcCountIncrease;
        Text textRoomNpcCount;
        Button buttonWaveNpcCountDecrease;
        Button buttonWaveNpcCountIncrease;
        Text textWaveNpcCount;
        Button buttonWaveAliveNpcCountDecrease;
        Button buttonWaveAliveNpcCountIncrease;
        Text textWaveAliveNpcCount;
        int room_plus_npc_count = 0;
        int wave_plus_npc_count = 0;
        int wave_alive_plus_npc_count = 0;

        public RoomEditor(GameObject obj)
        {
            roomEditor = obj;

            buttonRoomNpcCountDecrease = Utils.get_child(roomEditor, "NpcCountEditor/DecreaseButton").GetComponent<Button>();
            buttonRoomNpcCountIncrease = Utils.get_child(roomEditor, "NpcCountEditor/IncreaseButton").GetComponent<Button>();
            textRoomNpcCount = Utils.get_child(roomEditor, "NpcCountEditor/ValueArea/ValueText").GetComponent<Text>();
            buttonWaveNpcCountDecrease = Utils.get_child(roomEditor, "NpcWaveCountEditor/DecreaseButton").GetComponent<Button>();
            buttonWaveNpcCountIncrease = Utils.get_child(roomEditor, "NpcWaveCountEditor/IncreaseButton").GetComponent<Button>();
            textWaveNpcCount = Utils.get_child(roomEditor, "NpcWaveCountEditor/ValueArea/ValueText").GetComponent<Text>();
            buttonWaveAliveNpcCountDecrease = Utils.get_child(roomEditor, "NpcWaveAliveCountEditor/DecreaseButton").GetComponent<Button>();
            buttonWaveAliveNpcCountIncrease = Utils.get_child(roomEditor, "NpcWaveAliveCountEditor/IncreaseButton").GetComponent<Button>();
            textWaveAliveNpcCount = Utils.get_child(roomEditor, "NpcWaveAliveCountEditor/ValueArea/ValueText").GetComponent<Text>();

            buttonRoomNpcCountDecrease.onClick.AddListener(delegate { decrease_npc_count(ref room_plus_npc_count, textRoomNpcCount); });
            buttonRoomNpcCountIncrease.onClick.AddListener(delegate { increase_npc_count(ref room_plus_npc_count, textRoomNpcCount); });
            buttonWaveNpcCountDecrease.onClick.AddListener(delegate { decrease_npc_count(ref wave_plus_npc_count, textWaveNpcCount); });
            buttonWaveNpcCountIncrease.onClick.AddListener(delegate { increase_npc_count(ref wave_plus_npc_count, textWaveNpcCount); });
            buttonWaveAliveNpcCountDecrease.onClick.AddListener(delegate { decrease_npc_count(ref wave_alive_plus_npc_count, textWaveAliveNpcCount); });
            buttonWaveAliveNpcCountIncrease.onClick.AddListener(delegate { increase_npc_count(ref wave_alive_plus_npc_count, textWaveAliveNpcCount); });
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
        public virtual void increase_npc_count(ref int counter, Text text)
        {
            if(counter <= int.MaxValue-1)
            {
                counter++;
                text.text = counter.ToString();
            }
        }

        public virtual void decrease_npc_count(ref int counter, Text text)
        {
            if (counter > 0)
            {
                counter--;
                text.text = counter.ToString();
            }
        }

        public virtual void apply_changes()
        {
            EventManager.onLevelLoad -= apply_changes_impl;
            EventManager.onLevelLoad += apply_changes_impl;
        }

        private void apply_changes_impl(LevelData levelData, EventTime eventTime)
        {
            if (levelData.id.ToLower() != "master" && levelData.id.ToLower() != "home")
            {
                if (eventTime == EventTime.OnEnd)
                {
                    if (!Level.current.gameObject.TryGetComponent<RoomEditorModule>(out _))
                    {
                        var module = Level.current.gameObject.AddComponent<RoomEditorModule>();
                        module.additional_room_npc_count = room_plus_npc_count;
                        module.additional_wave_npc_count = wave_plus_npc_count;
                        module.additional_wave_alive_npc_count = wave_alive_plus_npc_count;
                        module.apply_changes();
                    }
                }
            }
            else
            {
                EventManager.onLevelLoad -= apply_changes_impl;
            }
        }
    }
}

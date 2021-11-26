using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ThunderRoad;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DungeonConfigurator
{
    public class DungeonConfiguratorMenuModule : MenuModule
    {
        public enum RightPageView
        {
            None,
            CreatureEditor,
            RoomEditor,
            InventoryEditor
        }

        protected GameObject pageLeft;
        protected GameObject pageRight;
        protected GameObject pageInput;

        CreatureEditor creatureEditor;
        RoomEditor roomEditor;
        InventoryEditor inventoryEditor;

        protected UnityEngine.UI.InputField fieldSeed;
        protected UnityEngine.UI.Slider sliderDifficulty;
        protected UnityEngine.UI.Slider sliderLength;

        protected UnityEngine.UI.Button buttonEditCreatures;
        protected UnityEngine.UI.Button buttonEditRooms;
        protected UnityEngine.UI.Button buttonEditInventory;

        protected UnityEngine.UI.Button buttonStart;

        protected UnityEngine.UI.Button buttonCopy;
        protected UnityEngine.UI.Button buttonPaste;
        protected UnityEngine.UI.Button buttonDelete;
        protected UnityEngine.UI.Button buttonClear;
        protected UnityEngine.UI.Button Keypad_0;
        protected UnityEngine.UI.Button Keypad_1;
        protected UnityEngine.UI.Button Keypad_2;
        protected UnityEngine.UI.Button Keypad_3;
        protected UnityEngine.UI.Button Keypad_4;
        protected UnityEngine.UI.Button Keypad_5;
        protected UnityEngine.UI.Button Keypad_6;
        protected UnityEngine.UI.Button Keypad_7;
        protected UnityEngine.UI.Button Keypad_8;
        protected UnityEngine.UI.Button Keypad_9;

        protected UnityEngine.UI.InputField lastEditedField = null;

        double timeLevelLoadStart;
        

        public override void Init(MenuData menuData, ThunderRoad.Menu menu)
        {
            Logger.Detailed("DungeonConfiguratorMenuModule Init called");
            base.Init(menuData, menu);

            pageLeft =  menu.GetCustomReference("PageLeft").gameObject;
            pageRight = menu.GetCustomReference("PageRight").gameObject;
            pageInput = menu.GetCustomReference("PageInput").gameObject;
            creatureEditor = new CreatureEditor(menu.GetCustomReference("CreatureEditor").gameObject);
            roomEditor = new RoomEditor(menu.GetCustomReference("RoomEditor").gameObject);
            inventoryEditor = new InventoryEditor(menu.GetCustomReference("InventoryEditor").gameObject);

            fieldSeed = Utils.get_child(pageLeft, "SeedField").GetComponent<UnityEngine.UI.InputField>();
            sliderDifficulty = Utils.get_child(pageLeft, "DifficultySlider").GetComponent<UnityEngine.UI.Slider>();
            sliderLength = Utils.get_child(pageLeft, "LengthSlider").GetComponent<UnityEngine.UI.Slider>();

            buttonEditCreatures = Utils.get_child(pageLeft, "CreatureEditButton").GetComponent<UnityEngine.UI.Button>();
            buttonEditRooms = Utils.get_child(pageLeft, "RoomEditButton").GetComponent<UnityEngine.UI.Button>();
            buttonEditInventory = Utils.get_child(pageLeft, "InventoryEditButton").GetComponent<UnityEngine.UI.Button>();

            buttonStart = Utils.get_child(pageLeft, "StartButton").GetComponent<UnityEngine.UI.Button>();

            buttonCopy = Utils.get_child(pageInput, "CopyButton").GetComponent<UnityEngine.UI.Button>();
            buttonPaste = Utils.get_child(pageInput, "PasteButton").GetComponent<UnityEngine.UI.Button>();
            buttonDelete = Utils.get_child(pageInput, "DeleteButton").GetComponent<UnityEngine.UI.Button>();
            buttonClear = Utils.get_child(pageInput, "ClearButton").GetComponent<UnityEngine.UI.Button>();
            Keypad_0 = Utils.get_child(pageInput, "Keypad_0").GetComponent<UnityEngine.UI.Button>();
            Keypad_1 = Utils.get_child(pageInput, "Keypad_1").GetComponent<UnityEngine.UI.Button>();
            Keypad_2 = Utils.get_child(pageInput, "Keypad_2").GetComponent<UnityEngine.UI.Button>();
            Keypad_3 = Utils.get_child(pageInput, "Keypad_3").GetComponent<UnityEngine.UI.Button>();
            Keypad_4 = Utils.get_child(pageInput, "Keypad_4").GetComponent<UnityEngine.UI.Button>();
            Keypad_5 = Utils.get_child(pageInput, "Keypad_5").GetComponent<UnityEngine.UI.Button>();
            Keypad_6 = Utils.get_child(pageInput, "Keypad_6").GetComponent<UnityEngine.UI.Button>();
            Keypad_7 = Utils.get_child(pageInput, "Keypad_7").GetComponent<UnityEngine.UI.Button>();
            Keypad_8 = Utils.get_child(pageInput, "Keypad_8").GetComponent<UnityEngine.UI.Button>();
            Keypad_9 = Utils.get_child(pageInput, "Keypad_9").GetComponent<UnityEngine.UI.Button>();

            add_field_events();
            add_button_events();

            pageInput.SetActive(false);
            SwitchToRightPageView(RightPageView.None);
        }
        public virtual void CopyFromCurrentField()
        {
            if (lastEditedField != null)
            {
                Logger.Detailed("Copied seed {0}", lastEditedField.text);
                Clipboard.SetData(DataFormats.Text, lastEditedField.text);
            }
        }
        public virtual void PasteToCurrentField()
        {
            if (lastEditedField != null)
            {
                if (Clipboard.ContainsData(DataFormats.Text))
                {
                    Logger.Detailed("Pasting seed {0}", lastEditedField.text);
                    string cpdata = (string)Clipboard.GetData(DataFormats.Text);
                    lastEditedField.text = cpdata;
                }
            }
        }

        public virtual void AddTextToCurrentField(string text)
        {
            if (lastEditedField != null)
            {
                lastEditedField.text += text;
            }
        }

        public virtual void ClearCurrentField()
        {
            if (lastEditedField != null)
            {
                lastEditedField.text = "";
            }
        }

        public virtual void RemoveLastCharFromCurrentField()
        {
            if (lastEditedField != null)
            {
                string newtext = lastEditedField.text.Substring(0, lastEditedField.text.Length - 1);
                lastEditedField.text = newtext;
            }
        }

        private void add_field_events()
        {
            EventTrigger fieldSeedEvent = fieldSeed.gameObject.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => {
                showInputPage(fieldSeed);
            });
            fieldSeedEvent.triggers.Add(entry);
            fieldSeed.onEndEdit.AddListener(delegate {
                double seed;
                double.TryParse(fieldSeed.text, out seed);
                if (seed == 0.0)
                {
                    fieldSeed.text = "";
                }
                //hideInputPage();
            });
        }


        private void add_button_events()
        {
            buttonEditCreatures.onClick.AddListener(() => { SwitchToRightPageView(RightPageView.CreatureEditor); });
            buttonEditRooms.onClick.AddListener(() => { SwitchToRightPageView(RightPageView.RoomEditor); });
            buttonEditInventory.onClick.AddListener(() => { SwitchToRightPageView(RightPageView.InventoryEditor); });
            buttonStart.onClick.AddListener(Start);
            buttonCopy.onClick.AddListener(CopyFromCurrentField);
            buttonPaste.onClick.AddListener(PasteToCurrentField);
            buttonDelete.onClick.AddListener(RemoveLastCharFromCurrentField);
            buttonClear.onClick.AddListener(ClearCurrentField);
            Keypad_0.onClick.AddListener(() => { AddTextToCurrentField("0"); });
            Keypad_1.onClick.AddListener(() => { AddTextToCurrentField("1"); });
            Keypad_2.onClick.AddListener(() => { AddTextToCurrentField("2"); });
            Keypad_3.onClick.AddListener(() => { AddTextToCurrentField("3"); });
            Keypad_4.onClick.AddListener(() => { AddTextToCurrentField("4"); });
            Keypad_5.onClick.AddListener(() => { AddTextToCurrentField("5"); });
            Keypad_6.onClick.AddListener(() => { AddTextToCurrentField("6"); });
            Keypad_7.onClick.AddListener(() => { AddTextToCurrentField("7"); });
            Keypad_8.onClick.AddListener(() => { AddTextToCurrentField("8"); });
            Keypad_9.onClick.AddListener(() => { AddTextToCurrentField("9"); });
        }

        public virtual void SwitchToRightPageView(RightPageView view)
        {
            HideAllRightPageViews();
            switch (view)
            {
                case RightPageView.CreatureEditor:
                    creatureEditor.setHidden(false);
                    break;
                case RightPageView.RoomEditor:
                    roomEditor.setHidden(false);
                    break;
                case RightPageView.InventoryEditor:
                    inventoryEditor.setHidden(false);
                    break;
                default:
                    break;
            }
        }

        public virtual void HideAllRightPageViews()
        {
            creatureEditor.setHidden(true);
            roomEditor.setHidden(true);
            inventoryEditor.setHidden(true);
        }

        public virtual void Start()
        {
            Dictionary<string, double> options = new Dictionary<string, double>();

            options["difficulty"] = sliderDifficulty.value;
            options["length"] = sliderLength.value;
            double seed = evaluate_seed();
            if(seed != 0.0)
            {
                options["seed"] = seed;
            }

            LevelData ld = Catalog.GetData<LevelData>("Dungeon");
            LevelData.Mode lm = ld.GetMode();

            creatureEditor.apply_changes();
            inventoryEditor.apply_changes();

            Logger.Basic("Starting dungeon");
            EventManager.onLevelLoad += EventManager_onLevelLoad;
            timeLevelLoadStart = Time.timeAsDouble;
            GameManager.LoadLevel(ld, lm, options);
        }

        private void EventManager_onLevelLoad(LevelData levelData, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                roomEditor.apply_changes();
            }
            if(eventTime == EventTime.OnEnd)
            {
                double timeLevelLoadEnd = Time.timeAsDouble;
                Logger.Basic("Loading the level took {0} seconds", timeLevelLoadEnd - timeLevelLoadStart);
            }
        }

        public override void OnShow(bool show)
        {
            base.OnShow(show);
            if (show)
            {
                refresh_page();
            }
        }
        public virtual void refresh_page()
        {
            hideInputPage();
            if (Level.current.options != null)
            {
                refresh_seed();
                refresh_difficulty();
                refresh_length();
            }
            else
            {
                revert_to_default();
            }
        }
        public virtual void refresh_seed()
        {
            int seed = 0;
            if(Level.current.dungeon)
            {
                seed = Level.current.dungeon.seed;
            }
            if(seed == 0)
            {
                fieldSeed.text = "";
            }
            else
            {
                fieldSeed.text = seed.ToString();
            }
        }
        public virtual void refresh_difficulty()
        {
            double difficulty;
            if (Level.current.options.TryGetValue("difficulty", out difficulty))
            {
                sliderDifficulty.value = (int)difficulty;
            }
        }
        public virtual void refresh_length()
        {
            double length;
            if (Level.current.options.TryGetValue("length", out length))
            {
                sliderLength.value = (int)length;
            }
        }

        public virtual void revert_to_default()
        {
            
        }
        public virtual double evaluate_seed()
        {
            string seedText = fieldSeed.text;
            double seed = 0.0;
            double.TryParse(seedText, out seed);
            return seed;
        }

        public virtual void showInputPage(InputField currField)
        {
            lastEditedField = currField;
            pageInput.gameObject.SetActive(true);
        }
        public virtual void hideInputPage()
        {
            pageInput.gameObject.SetActive(false);
        }

    }
}

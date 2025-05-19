

using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

#pragma warning disable UDR0005

namespace SaveSystem
{
    /// <summary>
    /// SaveManager is responsible for saving and loading data in the game.
    /// </summary>

    public static partial class SaveManager
    {

        public static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/slot{0}/"; // {0} => slot

        private static Dictionary<string, IGetSaveDataProtocol> _dataPools = null;

        public static byte currentSlotIndex => _currentSlotIndex;
        private static byte _currentSlotIndex = 0;

        public static bool isBusy => _isBusy;
        private static bool _isBusy = false;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            _dataPools = new Dictionary<string, IGetSaveDataProtocol>();
            _currentSlotIndex = 0;
            _isBusy = false;

            LoadAllSavePools();
        }

        /// <summary>
        /// Load all save pools. This method is called when the game starts.
        /// Please add all save pools here.
        /// </summary> 
        private static void LoadAllSavePools()
        {
            //SavePool<Foo> fooPool = new();
            //_dataPools.Add("FooSaves", fooPool); //File name is "FooSaves.dat/json" 
        }


        public static void RegisterListener<T>(string filename, SaveListener<T> listener)
        {

            var pool = _dataPools[filename];
            if (pool == null)
            {
                Debug.LogError($"No save pool found for {filename}. Please register the save pool first.");
                return;
            }

            if (pool is SavePool<T> savePool)
            {
                savePool.AddListener(listener);
            }
        }

        public static void UnregisterListener<T>(SaveListener<T> listener)
        {

            foreach (var kvp in _dataPools)
            {
                if (kvp.Value is SavePool<T> savePool)
                {
                    savePool.RemoveListener(listener);
                    return;
                }
            }

            Debug.LogError($"No save pool found for {listener.attachedObject}. Please register the save pool first.");
        }


        public static void ChangeSaveSlot(byte slot) => _currentSlotIndex = slot;

        public static void SaveGame()
        {

#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                Debug.LogWarning("SaveManager.SaveAll() called in editor mode. This is not allowed.");
                return;
            }
#endif

            if (_dataPools.Count == 0)
            {
                Debug.LogWarning("There are no listeners registered to save data.");
                return;
            }

            foreach (var pool in _dataPools)
            {
                string fileName = pool.Key;
                IGetSaveDataProtocol protocol = pool.Value;
                SaveDataAsync(fileName, protocol);
            }
        }


        public static void LoadGame(byte slot)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                Debug.LogWarning("SaveManager.LoadAll() called in editor mode. This is not allowed.");
                return;
            }
#endif

            _currentSlotIndex = slot;

            foreach (var pool in _dataPools)
            {
                string fileName = pool.Key;
                IGetSaveDataProtocol protocol = pool.Value;
                LoadDataAsync(fileName, protocol);
            }
        }

    }


}




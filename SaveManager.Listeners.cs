
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Game.LevelOperations;
using UnityEngine;

#pragma warning disable UDR0005

namespace SaveSystem
{
    /// <summary>
    /// SaveManager is responsible for saving and loading data in the game.
    /// </summary>

    public static partial class SaveManager
    {
        private static Dictionary<string, IGetSaveDataProtocol> _dataPools = null;

        public static void RegisterListener<T>(string filename, SaveListener<T> listener)
        {
            if (_dataPools.TryGetValue(filename, out var pool))
            {
                if (pool is SavePool<T> savePool)
                {
                    savePool.AddListener(listener);
                }
            }
            else Debug.LogError($"No save pool found for {filename}. Please register the save pool first.");
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

        public static SaveListener<T> CreateListener<T>(ISave<T> attachedObject)
        {
            var listener = new SaveListener<T>(typeof(T).Name, attachedObject);
            return listener;
        }

        public static void ReleaseAllListeners()
        {
            foreach (var kvp in _dataPools)
            {
                if (kvp.Value is ASavePool savePool) savePool.ClearListeners();
            }
        }

    }
}
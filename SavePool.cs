using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace SaveSystem
{

    /// <summary>
    /// Save Pool is a generic class that collects all save data from listeners of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SavePool<T> : IGetSaveDataProtocol
    {

        /// <summary>
        /// Event triggered when data is loaded.
        /// </summary>
        public static event Action<T> onLoaded = null;

        public SaveCollectionBehavior saveCollectionBehaviour { get; private set; } = SaveCollectionBehavior.Auto;
        private Func<List<T>> getManualData = null;

        private HashSet<SaveListener<T>> _saveListeners = new HashSet<SaveListener<T>>();

        public SavePool(Func<List<T>> manualSave, SaveCollectionBehavior behavior)
        {
            this.saveCollectionBehaviour = behavior;
            this.getManualData = manualSave;
        }

        public SavePool() { }


        /// <summary>
        /// Creates a list of all data from the save listeners and manual data. 
        /// </summary>
        private List<T> currentSaveData
        {
            get
            {

                if (saveCollectionBehaviour != SaveCollectionBehavior.Auto)
                {
                    var manualdata = getManualData?.Invoke();

                    if (manualdata == null)
                    {
                        UnityEngine.Debug.LogError($"Manual data is null for {typeof(T).Name}. Falling back to listener data.");
                        return GetCurrentDataFromListeners();
                    }

                    else if (saveCollectionBehaviour == SaveCollectionBehavior.AppendListeners)
                    {
                        var listenerdata = GetCurrentDataFromListeners();
                        manualdata.AddRange(listenerdata);
                    }

                    return manualdata;
                }

                return GetCurrentDataFromListeners();
            }
        }


        /// <summary>
        /// Gets the current data from all save listeners. 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<T> GetCurrentDataFromListeners()
        {
            List<T> currentData = new(_saveListeners.Count);
            foreach (var listener in _saveListeners) currentData.Add(listener.saveInfo);
            return currentData;
        }


        public void AddListener(SaveListener<T> listener) => _saveListeners.Add(listener);
        public void RemoveListener(SaveListener<T> listener) => _saveListeners.Remove(listener);
        

#if !SAVE_SYSTEM_USE_JSON
        public byte[] GetAllSaveData()
        {
            var formatter = SaveManager.binaryFormatter;
            byte[] bytes;

            using (var stream = new System.IO.MemoryStream())
            {
                formatter.Serialize(stream, currentSaveData);
                bytes = stream.ToArray();
            }


            return bytes;
        }

        public void OnLoadData(byte[] data)
        {
            var formatter = SaveManager.binaryFormatter;
            List<T> loadedData;

            using (var stream = new System.IO.MemoryStream(data))
            {
                loadedData = (List<T>)formatter.Deserialize(stream);
            }

            foreach (var dataItem in loadedData)
            {
                onLoaded?.Invoke(dataItem);
            }
        }


#else

        public string GetAllSaveData() => JsonConvert.SerializeObject(allData, SaveManager.jsonSettings);

        public void LoadData(string json)
        {

            var loadedData = JsonConvert.DeserializeObject<List<T>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });

            foreach (var data in loadedData)
            {
                onLoaded?.Invoke(data);
            }

        }

#endif

    }

}



using System;
using System.Collections.Generic;

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

        private HashSet<SaveListener<T>> _saveListeners = new HashSet<SaveListener<T>>();


        /// <summary>
        /// Creates a list of all data from the save listeners. (Don't use this frequently)
        /// </summary>
        private List<T> allData
        {
            get
            {
                List<T> allData = new List<T>();
                foreach (var saveListener in _saveListeners)
                {
                    allData.Add(saveListener.saveInfo);
                }

                return allData;
            }
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
                formatter.Serialize(stream, allData);
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
                UnityEngine.Debug.Log($"Loaded data: {dataItem}");
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



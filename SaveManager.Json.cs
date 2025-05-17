
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SaveSystem
{
#if SAVE_SYSTEM_USE_JSON
    public static partial class SaveManager
    {

        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static JsonSerializerSettings jsonSettings => _settings;
        public static string GetSaveFile(string fileName)
        {
            return string.Format(SAVE_FOLDER + fileName + ".json", currentSlotIndex);
        }


        private static async Task SaveDataAsync(string fileName, IGetSaveDataProtocol protocol)
        {

            string path = GetSaveFile(fileName);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            using (StreamWriter writer = new StreamWriter(path))
            {
                await writer.WriteAsync(protocol.GetAllSaveData());
            }

        }


        private static async Task LoadDataAsync(string filename, IGetSaveDataProtocol protocol) 
        {
            _isBusy = true;

            string path = GetSaveFile(filename);

            string json = await File.ReadAllTextAsync(path);
    
            protocol.LoadData(json);
        }



    }
#endif
}
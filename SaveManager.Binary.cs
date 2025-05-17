

using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;



#pragma warning disable UDR0005

namespace SaveSystem
{

#if !SAVE_SYSTEM_USE_JSON

    /// <summary>
    /// SaveManager is responsible for saving and loading data in the game.
    /// </summary>
    public static partial class SaveManager
    {

        private static readonly BinaryFormatter _formatter = new BinaryFormatter();
        public static BinaryFormatter binaryFormatter => _formatter;


        public static string GetSaveFile(string fileName)
        {
            return string.Format(SAVE_FOLDER + fileName + ".dat", currentSlotIndex);
        }

        private static async Task SaveDataAsync(string fileName, IGetSaveDataProtocol protocol) //binary + gzip
        {
            byte[] bytes = protocol.GetAllSaveData();
            string path = GetSaveFile(fileName);
            if (bytes.Length == 0)
            {
                if (File.Exists(path)) File.Delete(path); // remove old file
                return;
            }

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            CompressionLevel compressionLevel = bytes.Length > 1024 * 1024 ?
                       CompressionLevel.Optimal : CompressionLevel.Fastest;

            using FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            using GZipStream gzipStream = new GZipStream(fileStream, compressionLevel);
            await gzipStream.WriteAsync(bytes, 0, bytes.Length);
        }




        private static async Task LoadDataAsync(string filename, IGetSaveDataProtocol protocol) //binary
        {

            string path = GetSaveFile(filename);

            if (!File.Exists(path)) return;

            byte[] bytes = await File.ReadAllBytesAsync(path);

            // Decompress the data
            using (MemoryStream compressedStream = new(bytes))
            using (GZipStream gzipStream = new(compressedStream, CompressionMode.Decompress))
            using (MemoryStream decompressedStream = new())
            {
                await gzipStream.CopyToAsync(decompressedStream);
                bytes = decompressedStream.ToArray();
            }

            protocol.OnLoadData(bytes);
        }

    }
#endif

}




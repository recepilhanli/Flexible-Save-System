

namespace SaveSystem
{

    public interface ISave<T> //for objects to save
    {
        public SaveListener<T> saveListener { get; }
        public T GetSaveData();
    }



    public interface IGetSaveDataProtocol
    {

        public SaveCollectionBehavior saveCollectionBehaviour { get; }

#if !SAVE_SYSTEM_USE_JSON
        public byte[] GetAllSaveData();
        public void OnLoadData(byte[] data);
#else
        public string GetAllSaveData();
        public void LoadData(string data);
#endif

    }

}


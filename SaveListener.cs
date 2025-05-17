using System;


namespace SaveSystem
{
    [Serializable]
    public class SaveListener<T>
    {
        public ISave<T> attachedObject => _attachedObject;
        private ISave<T> _attachedObject;

        private SaveListener() { }

        public SaveListener(string fileName, ISave<T> attachedObject)
        {
            this._attachedObject = attachedObject;
            SaveManager.RegisterListener(fileName, this);
        }

        public T saveInfo => _attachedObject.GetSaveData();



        ~SaveListener()
        {
            SaveManager.UnregisterListener(this);
        }
    }




}

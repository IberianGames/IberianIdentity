namespace IberianSystem
{
    using System.IO;
    using UnityEngine;

    public class StandaloneEncryptedStorage : PlatformStorage
    {
        protected string Path
        {
            get { return Application.persistentDataPath + "/" + fileName; }
        }

        public override void ShowSavedGames()
        {
            // do nothing
        }

        override protected void LoadData()
        {
            byte[] data = null;

            if (!File.Exists(Path))
            {
                NotifyDataLoadEmpty();
                return;
            }

            bool successfulLoad = true;

            try { data = File.ReadAllBytes(Path); }
            catch { successfulLoad = false; }

            for (int i = 0; i < data.Length; i++)
            {
                data[i]--; // TODO be careful about 255 value
            }

            if (successfulLoad)
            {
                NotifyDataLoaded(data);
            }
            else
            {
                NotifyDataLoadFail("File load error");
            }
        }

        override protected void SaveData(byte[] data)
        {
            RequestResult result = RequestResult.Success;

            for (int i = 0; i < data.Length; i++)
            {
                data[i]++; // TODO be careful about 255 value
            }

            try { File.WriteAllBytes(Path, data); }
            catch { result = RequestResult.Fail; }

            if (result == RequestResult.Success)
            {
                NotifyDataSaveSuccess();
            }
            else if (result == RequestResult.Fail)
            {
                NotifyDataSaveFail("File save error");
            }
        }

    }
}

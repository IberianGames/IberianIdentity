namespace IberianSystem
{
    using System.Collections;
    using System.IO;
    using UnityEngine;

    public class StandaloneStorage : PlatformStorage
    {
        public float artificialDelay;

        protected string Path
        {
            get { return Application.persistentDataPath + "/" + fileName; }
        }

        public override void ShowSavedGames()
        {
            Debug.LogWarning("ShowSavedGames not implemented");
        }

        override protected void LoadData()
        {
            byte[] data = null;

            if (!File.Exists(Path))
            {
                StartCoroutine(
                    OnDataLoadDelayed(null, RequestResult.Empty));
                return;
            }

            RequestResult result = RequestResult.Success;

            try { data = File.ReadAllBytes(Path); }
            catch { result = RequestResult.Fail; }

            StartCoroutine(
                OnDataLoadDelayed(data, result));
        }

        override protected void SaveData(byte[] data)
        {
            RequestResult result = RequestResult.Success;

            try { File.WriteAllBytes(Path, data); }
            catch { result = RequestResult.Fail; }

            StartCoroutine(
                OnDataSaveDelayed(result));
        }

        IEnumerator OnDataLoadDelayed(byte[] data, RequestResult result)
        {
            yield return new WaitForSeconds(artificialDelay);
            if (result == RequestResult.Success)
            {
                NotifyDataLoaded(data);
            }
            else if (result == RequestResult.Fail)
            {
                NotifyDataLoadFail("File load error");
            }
            else if (result == RequestResult.Empty)
            {
                NotifyDataLoadEmpty();
            }
        }

        IEnumerator OnDataSaveDelayed(RequestResult result)
        {
            yield return new WaitForSeconds(artificialDelay);

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

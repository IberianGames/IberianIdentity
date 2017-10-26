using UnityEngine;

public class ManagerInstantiator : MonoBehaviour
{
    public GameObject[] prefabsToInstantiate;

    void Awake()
    {
        for (var i = 0; i < prefabsToInstantiate.Length; i++)
        {
            InstantiateIfNotPresent(prefabsToInstantiate[i]);
        }
    }

    void InstantiateIfNotPresent(GameObject prefab)
    {
        string prefabTag = prefab.tag;
        GameObject[] foundManagers = GameObject.FindGameObjectsWithTag(prefabTag);

        if (foundManagers.Length == 0)
        {
            Instantiate(prefab);
        }
        else if (foundManagers.Length > 1)
        {
            Debug.LogWarning("More than one manager found with tag " + prefabTag);
        }
    }
}

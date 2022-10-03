using System.Collections.Generic;
using UnityEngine;

public class PrefabsManager : MonoBehaviour
{
    public enum PrefabType
    {
        Projectile,
        Enemy,
        PlayerClone,
        CheckpointActivateEffect
    }

    private static PrefabsManager Instance;

    public List<PrefabTuple> prefabs;
    private Dictionary<PrefabType, GameObject> prefabsDict;

    void Start()
    {
        if (Instance != null)
        {
            throw new System.Exception("Impossible to have more than one PrefabsManager");
        }
        Instance = this;

        buildPrefabsDict();
    }

    public static GameObject GetPrefab(PrefabType type)
    {
        return Instance.prefabsDict[type];
    }

    private void OnValidate()
    {
        buildPrefabsDict();
    }

    private void buildPrefabsDict()
    {
        prefabsDict = new Dictionary<PrefabType, GameObject>();

        foreach (var item in prefabs)
        {
            if (prefabsDict.ContainsKey(item.type))
            {
                throw new System.Exception("Cannot contain more than one prefab per type");
            }

            prefabsDict[item.type] = item.prefab;
        }
    }

    [System.Serializable]
    public struct PrefabTuple
    {
        public PrefabType type;
        public GameObject prefab;
    }
}

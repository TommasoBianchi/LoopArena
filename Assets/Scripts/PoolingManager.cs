using System.Collections.Generic;
using UnityEngine;

public static class PoolingManager
{
    public enum Type
    {
        Projectile,
        PlayerClone,
        Enemy
    }

    private static Dictionary<Type, Stack<GameObject>> inactiveObjects;
    private static Dictionary<Type, Transform> parents;

    static PoolingManager()
    {
        inactiveObjects = new Dictionary<Type, Stack<GameObject>>();
        parents = new Dictionary<Type, Transform>();
    }

    public static void Clear()
    {
        inactiveObjects.Clear();
        parents.Clear();
    }

    public static GameObject Instantiate(Type type, GameObject prefab, Vector3 position, Quaternion rotation) { 

        if (!inactiveObjects.ContainsKey(type))
        {
            inactiveObjects[type] = new Stack<GameObject>();
        }

        if (!parents.ContainsKey(type))
        {
            parents[type] = new GameObject().transform;
            parents[type].name = type.ToString();
        }


        if (inactiveObjects[type].Count > 0)
        {
            GameObject reusedGameObject = inactiveObjects[type].Pop();
            reusedGameObject.transform.position = position;
            reusedGameObject.transform.rotation = rotation;
            reusedGameObject.SetActive(true);
            return reusedGameObject;
        }

        GameObject gameObject = Object.Instantiate(prefab, position, rotation, parents[type]);
        return gameObject;
    }

    public static void Destroy(Type type, GameObject gameObject)
    {
        gameObject.SetActive(false);

        if (!inactiveObjects.ContainsKey(type))
        {
            inactiveObjects[type] = new Stack<GameObject>();
        }

        inactiveObjects[type].Push(gameObject);
    }
}

using UnityEngine;

public static class DialogueInit
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializePrefab()
    {
        // Load the prefab from the Resources folder
        GameObject prefab = Resources.Load<GameObject>("DialogueBoxCanvas");

        if (prefab != null)
        {
            // Instantiate the prefab at the origin with no rotation
            GameObject instance = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(instance);
            Debug.Log("Prefab instantiated successfully.");
        }
        else
        {
            Debug.LogError("Prefab not found in Resources folder.");
        }
    }
}

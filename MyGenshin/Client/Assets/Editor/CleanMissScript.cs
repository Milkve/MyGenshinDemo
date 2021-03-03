using UnityEngine;
using UnityEditor;

public class CleanMissingScripts
{
    [MenuItem("Tools/Cleanup Missing Scripts")]
    static void CleanupMissingScripts()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            var gameObject = Selection.gameObjects[i];
            Remove(gameObject);
            
        }
    }

    static void Remove(GameObject go)
    {
        if (go.transform.childCount > 0) { 
            
            for(int i = 0; i < go.transform.childCount; i++)
            {
                Remove(go.transform.GetChild(i).gameObject);
            }
        }
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go); 
    }
}

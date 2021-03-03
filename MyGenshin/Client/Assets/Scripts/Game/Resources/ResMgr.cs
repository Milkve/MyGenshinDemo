
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

static class ResMgr
{
    //TODO :异步加载
    static Dictionary<string, GameObject> allGameObject = new Dictionary<string, GameObject>();
    static Dictionary<string, AssetBundle> allAssetBundle = new Dictionary<string, AssetBundle>();
    static Dictionary<string, Sprite> allSprite = new Dictionary<string, Sprite>();
    static Dictionary<string, GameObject> allPrefab = new Dictionary<string, GameObject>();

#if UNITY_EDITOR
    static string AssetBundlePath = "Assets/AssetBundlesLocal/";
#else
    static string AssetBundlePath = "AssetBundles/";
#endif



    public static Sprite GetSprite(string name, string path)
    {

        //Debug.Log(string.Format("{0}:name={1} path={2}", System.Reflection.MethodBase.GetCurrentMethod().Name, name, path));
        if (!allSprite.ContainsKey(path))
        {
            LoadAsset<Sprite>(allSprite, name, path);
        }
        return allSprite[path];
    }
    public static GameObject GetGameObject(string name, string path)
    {

        //Debug.Log(string.Format("{0}:name={1} path={2}", System.Reflection.MethodBase.GetCurrentMethod().Name, name, path));
        if (!allGameObject.ContainsKey(path))
        {
            LoadAsset<GameObject>(allGameObject, name, path);
        }
        return allGameObject[path];
    }


    public static GameObject GetPrefab(string name, string path)
    {
        //Debug.Log(string.Format("{0}:name={1} path={2}", System.Reflection.MethodBase.GetCurrentMethod().Name, name, path));
        if (!allPrefab.ContainsKey(path))
        {
            LoadAsset<GameObject>(allPrefab, name, path);
        }
        return allPrefab[path];
    }


    static void LoadAsset<T>(Dictionary<string, T> collections, string name, string path) where T : UnityEngine.Object
    {
        //Debug.Log(string.Format("{0}:name={1} path={2}", System.Reflection.MethodBase.GetCurrentMethod().Name, name, AssetBundlePath + path));
        T obj = null;
#if UNITY_EDITOR
        obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(AssetBundlePath+path);
#else
        if (!allAssetBundle.ContainsKey(path))
        {
            allAssetBundle[path] = LoadAB(AssetBundlePath+path);
        }
        obj =allAssetBundle[path].LoadAsset<T>(name);
#endif
        if (obj != null)
        {
            collections.Add(path, obj);
        }
        else
        {
            throw new Exception($"Load {path} Faild");
        }

    }
    static AssetBundle LoadAB(string path)
    {
        AssetBundle assetBundle = AssetBundle.LoadFromFile(SysDefine.PATH_ASSETBUNDLE + "/" + path);
        if (assetBundle != null)
        {
            return assetBundle;
        }
        else
        {
            throw new Exception($"Load AssetBundle {path} faild");
        }
    }

}
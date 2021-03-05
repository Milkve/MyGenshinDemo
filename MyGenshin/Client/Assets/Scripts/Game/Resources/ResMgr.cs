//#undef UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ResMgr
{
    //TODO :异步加载
    static Dictionary<string, GameObject> allGameObject = new Dictionary<string, GameObject>();
    static Dictionary<string, AssetBundle> allAssetBundle = new Dictionary<string, AssetBundle>();
    static Dictionary<string, Sprite> allSprite = new Dictionary<string, Sprite>();
    static Dictionary<string, GameObject> allPrefab = new Dictionary<string, GameObject>();

#if UNITY_EDITOR
    static string AssetBundlePath = SysDefine.PATH_ASSETBUNDLE_LOCAL;
#else
    static string AssetBundlePath = SysDefine.PATH_ASSETBUNDLE;
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


    public static void LoadAsset<T>(Dictionary<string, T> collections, string name, string path) where T : UnityEngine.Object
    {
        //Debug.Log(string.Format("{0}:name={1} path={2}", System.Reflection.MethodBase.GetCurrentMethod().Name, name, AssetBundlePath + path));
        T obj = null;
#if UNITY_EDITOR
        obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(AssetBundlePath+"/"+path);
#else

        string asset_name = PackRule.PathToAssetBundleName(path);
        Debug.Log($"asset_name:{asset_name}");
        if (!allAssetBundle.ContainsKey(asset_name))
        {
            allAssetBundle[asset_name] = LoadAB(asset_name);
        }
        Debug.Log($"path:{path}");
        Debug.Log(allAssetBundle[asset_name]);
        obj =allAssetBundle[asset_name].LoadAsset<T>(Path.GetFileName(path));
        Debug.Log(obj);
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
    public static AssetBundle LoadAB(string asset_name)
    {
        Debug.Log(SysDefine.PATH_ASSETBUNDLE + "/" + asset_name);
        AssetBundle assetBundle = AssetBundle.LoadFromFile(SysDefine.PATH_ASSETBUNDLE + "/" + asset_name);
        if (assetBundle != null)
        {
            return assetBundle;
        }
        else
        {
            throw new Exception($"Load AssetBundle {asset_name} faild");
        }
    }

}
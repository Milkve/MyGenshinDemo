using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Bindings;



    public static class PackageManager
    {

        static List<AssetBundleBuild> Maps = new List<AssetBundleBuild>();
        static List<string> Files = new List<string>();
        public static string GetStreamingAssetsTargetPathByPlatform(RuntimePlatform platform)
        {
            string dataPath = Application.dataPath.Replace("/Assets", "");
            if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WebGLPlayer)
                return dataPath + "/" + SysDefine.PATH_ASSETBUNDLE;
            else if (platform == RuntimePlatform.Android)
                return dataPath + "/StreamingAssetsAndroid/" + SysDefine.PATH_ASSETBUNDLE;
            else if (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer)
                return dataPath + "/StreamingAssetsIOS/" + SysDefine.PATH_ASSETBUNDLE;
            else
                Debug.Log("Unspport System!");

            return string.Empty;

        }
        public static RuntimePlatform BuildTargetToPlatform(BuildTarget target)
        {
            if (target == BuildTarget.StandaloneWindows64 || target == BuildTarget.StandaloneWindows)
                return RuntimePlatform.WindowsEditor;
            else if (target == BuildTarget.Android)
                return RuntimePlatform.Android;
            else if (target == BuildTarget.iOS)
                return RuntimePlatform.IPhonePlayer;
            else
                return RuntimePlatform.WindowsEditor;
        }

        [MenuItem("AssetBundleBuild/Build Windows Resource")]
        public static void BuildAssetResourceWindows()
        {

            BuildAssetResource(BuildTarget.StandaloneWindows64);
        }

        public static void BuildAssetResource(BuildTarget target)
        {

            string streamPath = GetStreamingAssetsTargetPathByPlatform(BuildTargetToPlatform(target));
            Debug.Log(streamPath);
            if (Directory.Exists(streamPath))
            {
                Directory.Delete(streamPath, true);
            }
            Directory.CreateDirectory(streamPath);
            AssetDatabase.Refresh();

            Maps.Clear();

        foreach ( var dir in Directory.GetDirectories(SysDefine.PATH_ASSETBUNDLE_LOCAL))
        {
            string dirName = Path.GetFileName(dir);
            Debug.Log($"Handle {dirName}");
            HandleBundles(dirName);
        }  
             
            BuildPipeline.BuildAssetBundles(streamPath, Maps.ToArray(), BuildAssetBundleOptions.None, target);
            AssetDatabase.Refresh();
        }
        private static void HandleBundles(string  prefix)
        {
            string path = $"{SysDefine.PATH_ASSETBUNDLE_LOCAL}/{prefix}/";
            string[] dirs = Directory.GetDirectories(path);
            Debug.Log("dirs.Length : " + dirs.Length.ToString());
            if (dirs.Length == 0)
                return;
            for (int i = 0; i < dirs.Length; i++)
            {
                string asset_name = $"{prefix}_" + Path.GetFileName(dirs[i]);
                Debug.Log("dir:" + asset_name);
                Files.Clear();
                Recursive(dirs[i], false);
                if (Files.Count > 0)
                {
                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = asset_name;
                    build.assetNames = Files.ToArray();
                    Maps.Add(build);
                }
            }
        }

        private static void Recursive(string path, bool ignore_meta=true)
        {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (var filename in names)
            {
                string ext = Path.GetExtension(filename);
                if (ignore_meta && ext.Equals(".meta")) continue;
                Files.Add(filename.Replace('\\', '/'));
            }
            foreach (var dir in dirs)
            {
                Recursive(dir, ignore_meta);
            }
        }
    }


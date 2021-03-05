using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PackRule
{
    static List<string> Prefixs = new List<string>() { "ui","sprite","module","model"};
    public static string PathToAssetBundleName(string path)
    {
        path = path.Replace('\\', '/');

        foreach (var prefix in Prefixs)
        {
            if (path.StartsWith(prefix + "/"))
            {
                string sub_path = path.Substring(prefix.Length + 1);
                string[] path_parts = sub_path.Split('/');
                if (path_parts.Length > 0)
                    return $"{prefix}_{path_parts[0]}";
            }
        }

        Debug.LogError("PackRule:PathToAssetBundleName : cannot find ab name : " + path);
        return "";
    }
}
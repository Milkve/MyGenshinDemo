using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    class UseTest
    {
        [MenuItem("Tool/PackNametest")]
        public static void PackName()

        {
            string path = SysDefine.PATH_ASSETBUNDLE_LOCAL + "/" + "ui/itemGain/UIGainItem.prefab";
            string Name= PackRule.PathToAssetBundleName(path);
            var ab= ResMgr.LoadAB(Name);
            foreach (var name in ab.GetAllAssetNames())
            {
                Debug.Log(name);
                var obj = ab.LoadAsset(name);
            }
            //ab.GetAllAssetNames
            //Debug.Log(Name);


        }
    }
}

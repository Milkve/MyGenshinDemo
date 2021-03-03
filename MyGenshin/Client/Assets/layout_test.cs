using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using SkillBridge.Message;

public class layout_test : MonoBehaviour
{
    // Start is called before the first frame update

    Toggle toggle;
    public Transform parent;
    void Start()
    {
        GameObject pwarp = ResMgr.GetPrefab("123", "ui/common/UIWarp.prefab");
        GameObject warp = GameObject.Instantiate(pwarp);
        warp.transform.SetParent(parent, false);
        GameObject pgrid = ResMgr.GetPrefab("123", "ui/bag/UIBagGrid.prefab");

        GameObject grid = GameObject.Instantiate(pgrid);
        grid.transform.SetParent(warp.transform, false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

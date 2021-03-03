using Common.Data;
using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Image Map;
    public Image Sight;
    public Image Arrow;
    MapDefine mapDefine;
    private void Start()
    {
        MapService.Instance.OnEnterMap += OnEnterMap;
        OnEnterMap(User.Instance.CurrentMap);
    }

    private void OnEnterMap(MapDefine define)
    {
        mapDefine = define;
        Sprite sp = ResMgr.GetSprite(System.IO.Path.GetFileNameWithoutExtension(define.MiniMap), define.MiniMap);
        Map.overrideSprite = sp;
        Map.SetNativeSize();

    }

    void FixedUpdate()
    {

        if (Camera.main == null || User.Instance.CurrentCharacterObject == null) return;
        GameObject player = User.Instance.CurrentCharacterObject;
        Sight.transform.rotation = Quaternion.Euler(0, 0, -Camera.main.transform.rotation.eulerAngles.y);
        Arrow.transform.rotation = Quaternion.Euler(0, 0, -player.transform.rotation.eulerAngles.y);
        Map.rectTransform.pivot = new Vector2(player.transform.position.x / mapDefine.Width, player.transform.position.z / mapDefine.Length);
        Map.transform.localPosition = Vector3.zero;
    }
}

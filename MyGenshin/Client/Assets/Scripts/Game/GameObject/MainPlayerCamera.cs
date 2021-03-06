using Common.Data;
using DG.Tweening;
using Managers;
using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public int rotateSpeed;
    public int zoomSpeed;
    public int min;
    public int max;
    float x;
    float y;
    float z;
    float d;
    Vector3 center;
    CharacterController cc;
    public Transform player;
    private bool innerEnable = false;
    bool isLookAt = false;
    Vector3 LastPositon;
    Vector3 LastDirection;

    public bool InnerEnable
    {
        get => innerEnable;
        set
        {
            if (value == true)
            {
                this.Relax(() => { innerEnable = value; });
            }
            else
            {
                innerEnable = value;
            }

        }
    }

    protected override void OnStart()
    {
        
        
        GlobalManager.Instance.OnGamePlayStateChanged += OnGamePlayStateChange;
    }
    private void OnDestroy()
    {
       
        GlobalManager.Instance.OnGamePlayStateChanged -= OnGamePlayStateChange;
    }
    public void OnGamePlayStateChange(GamePlayState gamePlayState)
    {
        switch (gamePlayState)
        {
            case GamePlayState.Playing:
                {
                    GetComponent<GaussianBlur>().enabled = false;
                    InnerEnable = true;
                    break;
                }

            case GamePlayState.Talking:
                {
                    GetComponent<GaussianBlur>().enabled = false;
                    LookAt(NPCManager.Instance.CurrentTalkToNpc.LookAt);
                    InnerEnable = false;
                    break;

                }
            case GamePlayState.UI:
                {
                    GetComponent<GaussianBlur>().enabled = true;
                    InnerEnable = false;
                    break;
                }
            default: InnerEnable = false; GetComponent<GaussianBlur>().enabled = false; break;
        }
    }


    public void LookAt(Transform target)
    {
        if (isLookAt) return;
        LastDirection = transform.rotation.eulerAngles;
        LastPositon = transform.position;
        isLookAt = true;
        MoveTo(center, 0.5f);
        RotateTo(Quaternion.LookRotation(target.position - center).eulerAngles, 0.5f);
    }

    public void Relax(Action callback)
    {
        if (!isLookAt) {
            callback?.Invoke();
            return;
        }
        isLookAt = false;
        MoveTo(LastPositon, 0.5f);
        RotateTo(LastDirection, 0.5f, null, callback);
    }

    public void SetCurrentPlayer(Transform player,GameObject playModule, CharacterController cc)
    {
        //TODO 以后整理
        this.player = player;
        transform.SetParent(playModule.transform);
        this.cc = cc;
        transform.position = cc.center * 2 + player.position;
        //transform.RotateAround(cc.center * 2 + player.position, transform.TransformVector(Vector3.left), 30);
        transform.rotation =  player.rotation;
        transform.rotation *= Quaternion.Euler(30, 0, 0);
        player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<PlayerController>().cc = cc;
        MoveTo(transform.position + transform.TransformVector(Vector3.back * 2), 1f, null, () => { GlobalManager.Instance.EnterGamePlayState( GamePlayState.Playing);
            //LuaBehaviour.Instance.SafeDoString("GlobleEventManager:Call(GlobleEvent.EnterGamePlaying)")
            LuaBehaviour.Instance.CallLuaEvent("EnterGamePlaying");
        });

    }
    public void MoveTo(Vector3 positon, float durtion, XLua.LuaFunction callback = null, Action action = null)
    {
        transform.DOMove(positon, durtion).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            callback?.Call();
            action?.Invoke();
        });
    }

    public void RotateTo(Vector3 direction, float durtion, XLua.LuaFunction callback = null, Action action = null)
    {
        transform.DORotate(direction, durtion).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            callback?.Call();
            action?.Invoke();
        });
    }
    private void Update()
    {
        if (!InnerEnable) return;
        center = cc.center * 2 + player.position;
        d = (transform.position - center).magnitude;
        x = Input.GetAxis("Mouse X");
        y = Input.GetAxis("Mouse Y");
        z = Input.GetAxis("Mouse ScrollWheel");

    }

    private void LateUpdate()
    {
        if (!InnerEnable) return;
        transform.RotateAround(cc.center * 2 + player.position, Vector3.up, x * Time.deltaTime * rotateSpeed);
        if ((z < 0 & d < max) || (z > 0 & d > min)) transform.position += transform.TransformVector(Vector3.forward * z);
        if ((y < 0 & transform.eulerAngles.x < 90 & transform.eulerAngles.z > 90) || (y > 0 & 360 - transform.eulerAngles.x < 90 & transform.eulerAngles.z > 90)) return;
        transform.RotateAround(cc.center * 2 + player.position, transform.TransformVector(Vector3.left), y * Time.deltaTime * rotateSpeed);
    }


}
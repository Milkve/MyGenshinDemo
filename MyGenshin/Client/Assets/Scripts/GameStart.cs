using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;
using Services;
using Managers;
using DG.Tweening;
using System.IO;
using System.Configuration;
using UnityEngine.EventSystems;

public class GameStart : MonoSingleton<GameStart>
{

    public enum State
    {
        GameTips,             //开场
        CheckExtractResource, //初次运行游戏时需要解压资源文件
        UpdateResourceFromNet,//热更阶段：从服务器上拿到最新的资源
        InitAssetBundle,      //初始化AssetBundle
        StartLogin,           //登录流程
        StartGame,            //正式进入场景游戏
        Playing,              //完成启动流程了，接下来把控制权交给玩法逻辑
        None,                 //无
    }

    public enum SubState
    {
        Enter,
        Update
    }

    State currentState = State.None;
    SubState currentSubState = SubState.Enter;

    Coroutine coroutine;
    public UILoadingSliderView sliderView;
    public UITip uiTip;
    protected override void OnStart()
    {

        var a = log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.xml"));
        Common.Log.Init("Unity");
        Common.Log.Info("Log4Net");
        UnityLogger.Init();
        Debug.Log("GameStart");
        this.gameObject.AddComponent<LuaBehaviour>();
        this.gameObject.AddComponent<NetClient>();
        UserService.Instance.Init();
        ItemService.Instance.Init();
        MapService.Instance.Init();
        StatusService.Instance.Init();
        QuestService.Instance.Init();
        JumpToState(State.GameTips);
        //JumpToState(State.Playing);
    }

    //private void LateUpdate()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {

    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;
    //        if (Physics.Raycast(ray, out hit,100))
    //        {
    //            print(hit.transform.name);
    //        }
    //    }

    //}
    void Update()
    {


        if (currentState == State.Playing) return;
        switch (currentState)
        {
            case State.GameTips:
                {
                    if (currentSubState == SubState.Enter)
                    {
                        currentSubState = SubState.Update;
#if UNITY_EDITOR
                        //如果EDITOR模式就跳过之前的
                        sliderView.gameObject.SetActive(true);
                        sliderView.ShowSlider();
#else
                        uiTip.gameObject.SetActive(true);
                        uiTip.Show(() =>
                        {
                            uiTip.Hide(() =>
                            {
                                uiTip.gameObject.SetActive(false);
                                sliderView.gameObject.SetActive(true);
                                sliderView.ShowSlider();
                            });
                            sliderView.gameObject.SetActive(true);
                        });
#endif

                        JumpToState(State.CheckExtractResource);
                    }
                    break;
                }

            case State.CheckExtractResource:
                {
                    if (currentSubState == SubState.Enter)
                    {
                        currentSubState = SubState.Update;

                        sliderView.Reset("解压文件中", () =>
                        {
                            StopCoroutine(coroutine);
                            JumpToState(State.UpdateResourceFromNet);
                        });
                        coroutine = StartCoroutine(Extract(sliderView.SetData));
                    }
                    break;
                }
            case State.UpdateResourceFromNet:
                {
                    if (currentSubState == SubState.Enter)
                    {
                        currentSubState = SubState.Update;
                        sliderView.Reset("下载文件中", () =>
                        {
                            StopCoroutine(coroutine);
                            JumpToState(State.InitAssetBundle);
                        });
                        coroutine = StartCoroutine(Download(sliderView.SetData));
                    }
                    break;
                }
            case State.InitAssetBundle:
                {
                    if (currentSubState == SubState.Enter)
                    {
                        currentSubState = SubState.Update;
                        sliderView.Reset("初始化中", () =>
                        {
                            StopCoroutine(coroutine);
                            JumpToState(State.StartLogin);
                        });
                        coroutine = StartCoroutine(GameInit(sliderView.SetData));
                    }
                    break;
                }
            case State.StartLogin:
                {
                    if (currentSubState == SubState.Enter)
                    {
                        currentSubState = SubState.Update;
                        LuaBehaviour.Instance.LuaEnvInit();
                        NPCManager.Instance.NpcInit();
                        sliderView.OnSliderHided += () =>
                        {
                            LuaBehaviour.Instance.CallLuaEvent("EnterLogin");

                            //sliderView.Hide();
                        };
                        sliderView.HideSlider();

                    }

                    break;
                }

        }
    }


    void JumpToState(State state)
    {

        currentState = state;
        currentSubState = SubState.Enter;
        Debug.Log($"GameState jump to {currentState}");
    }

    public static void QuitGame()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("编辑状态游戏退出");
#else
        Application.Quit();
        Debug.Log ("游戏退出");
#endif
    }

    //TODO:替换为真正的操作   
    IEnumerator Extract(Action<float, string> setData)
    {
        yield return new WaitForSeconds(0.1f);
        setData(0.25f, null);
        yield return new WaitForSeconds(0.1f);
        setData(0.5f, null);
        yield return new WaitForSeconds(0.1f);
        setData(0.75f, null);
        yield return new WaitForSeconds(0.1f);
        setData(1, "解压完成");
    }

    IEnumerator Download(Action<float, string> setData)
    {
        yield return new WaitForSeconds(0.1f);
        setData(0.25f, null);
        yield return new WaitForSeconds(0.1f);
        setData(0.5f, null);
        yield return new WaitForSeconds(0.1f);
        setData(0.75f, null);
        yield return new WaitForSeconds(0.1f);
        setData(1, "下载完成");
    }
    IEnumerator GameInit(Action<float, string> setData)
    {
        this.gameObject.AddComponent<SceneManager>();
        this.gameObject.AddComponent<GameObjectManager>();
        this.gameObject.AddComponent<EntityManager>();
        DontDestroyOnLoad(GameObject.Find("EventSystem"));
        setData(0.1f, "");
        yield return DataManager.Instance.LoadData(setData);
    }



}

using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using static Managers.QuestManager;

public class NPCController : MonoBehaviour
{

    // Use this for initialization
    public int npcID;
    public Animator ani;
    public NPCDefine Define;
    bool isActive = false;
    Vector3 last;
    public Transform LookAt;

    public BindableProperty<NpcQuestStatus> Status = new BindableProperty<NpcQuestStatus>(NpcQuestStatus.None);
    void Start()
    {
        ani = GetComponent<Animator>();
        Define = NPCManager.Instance.AddNpc(npcID, this);
        StartCoroutine(Action());
    }

    public void OnQuestStatusChange()
    {

    }



    // Update is called once per frame
    IEnumerator Action()
    {
        while (true)
        {
            if (isActive)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
            }
            else
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 3f));
            }
            SetRelax();
        }

    }

    public void DoActive()
    {
        StartCoroutine(Active());
    }
    public void DoInActive()
    {

        StartCoroutine(InActive());
    }


    IEnumerator Active()
    {
        if (!isActive)
        {
            isActive = true;
            last = this.transform.forward;
            yield return Face2Player();
            this.ani.SetBool("Talking", true);
        }
    }

    IEnumerator InActive()
    {
        if (isActive)
        {
            yield return Reflex();
            this.ani.SetBool("Talking", false);
            isActive = false;
        }
    }

    IEnumerator Face2Player()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Math.Abs(Vector3.Angle(this.transform.forward, faceTo)) > 1)
        {
            this.transform.forward = Vector3.Lerp(this.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }

    }

    IEnumerator Reflex()
    {
        while (Math.Abs(Vector3.Angle(this.transform.forward, last)) > 1)
        {
            this.transform.forward = Vector3.Lerp(this.transform.forward, last, Time.deltaTime * 5f);
            yield return null;
        }

    }



    void SetRelax()
    {
        this.ani.SetTrigger("Relax");
    }


    void SetIdle()
    {
        this.ani.SetTrigger("Idle");
    }

}

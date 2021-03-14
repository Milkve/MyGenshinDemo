using Entities;
using Managers;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Services;

public class PlayerController : MonoBehaviour
{
    //public EntityController ec;
    public Character character;
    [HideInInspector]
    public CharacterController cc;
    public EntityController ec;
    public float moveSpeed;
    public float jumpSpeed;
    public float G = 10f;
    bool enable = false;
    float v;
    float h;
    bool jumpPressed = false;
    bool battle = false;
    bool sprint = false;
    bool run = false;
   float lasttime;
    Vector3 lastV;
    Vector3 vector;
    Vector3 Velocity = Vector3.zero;
    MainPlayerCamera mainPlayerCamera;
    Vector3 lastPosition;
    Animator animator;

    public bool Battle
    {
        get => battle; set
        {

            if (battle != value)
            {
                Debug.Log("battle");
                SendSync(EntityEvent.Battle, value ? 1 : -1);
            }
            battle = value;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        mainPlayerCamera = Camera.main.GetComponent<MainPlayerCamera>();
        GlobalManager.Instance.OnGamePlayStateChanged += OnGamePlayStateChanged;
    }

    private void OnGamePlayStateChanged(GamePlayState gamePlayState)
    {
        switch (gamePlayState)
        {
            case GamePlayState.Playing: enable = true; break;
            default: enable = false; break;
        }
    }
    private void Update()
    {
        if (!enable) return;
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump")) jumpPressed = true;

        if (Input.GetButtonDown("Tab"))
        {
            Battle = !Battle;

        }
        if (Input.GetButtonDown("L"))
        {
            run = !run;
        }
        sprint = Input.GetButton("Shift");

    }
    void LateUpdate()
    {
        if (!enable) return;
        float speed = 1;
        if (!run) speed = 0.5f;
        if (sprint) speed *= 2f;
        vector = new Vector3(h, 0, v) * moveSpeed * speed;
        vector = mainPlayerCamera.transform.TransformVector(vector);
        vector.y = 0;
        Velocity.x = vector.x;
        Velocity.z = vector.z;
        if (Velocity.y > -G) Velocity.y -= G * Time.deltaTime;


    }

    private void FixedUpdate()
    {
        if (!enable) return;
        if (vector.magnitude > 0) transform.forward = vector.normalized;
        if (jumpPressed && cc.isGrounded)
        {
            Velocity.y = jumpSpeed;
            Debug.Log("Jump");
            SendSync(EntityEvent.Jump);
        }
        jumpPressed = false;
        cc.Move(Velocity * Time.deltaTime);
        character.SetEntityData(this.transform.position, this.transform.forward, cc.velocity.magnitude/moveSpeed);
        if (lastV == null) lastV = Velocity;
        if ((Velocity - lastV).magnitude>0.1f)
        {
            SendSync(EntityEvent.None);
            lastV = Velocity;
        }
        if (lastPosition == null) lastPosition = this.transform.position;
        else
        {
            //float offset = Vector3.Distance(lastPosition, this.transform.position);
            //if (offset > 0.2f)
            //{
            //    SendSync(EntityEvent.None);
            //    lastPosition = this.transform.position;
            //}
        }
        //if (Time.time - lasttime > 0.5f)
        //{
        //    SendSync(EntityEvent.None);
        //}
    }

    public void SendSync(EntityEvent type, int value = 0)
    {
        SendSync(new NEntityEvent() { Type = type, Value = value });
    }

    public void SendSync(NEntityEvent nEntityEvent)
    {      
        if (ec != null)
        {
            ec.OnEntityEvent(nEntityEvent);
        }
        lasttime = Time.time;
        MapService.Instance.SendMapEntitySync(nEntityEvent, character);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NPC")
        {
            NPCManager.Instance.NpcEnter(other.GetComponent<NPCController>().npcID);
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "NPC")
        {
            NPCManager.Instance.NpcLeave(other.GetComponent<NPCController>().npcID);
        }
    }
}

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
    Vector3 vector;
    Vector3 Velocity = Vector3.zero;
    MainPlayerCamera mainPlayerCamera;

    Animator animator;

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
            battle = !battle;

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
            animator.SetTrigger("Jump");
        }
        jumpPressed = false;
        animator.SetBool("Battle", battle);
        animator.SetFloat("Speed", vector.magnitude / moveSpeed);
        cc.Move(Velocity * Time.deltaTime);
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

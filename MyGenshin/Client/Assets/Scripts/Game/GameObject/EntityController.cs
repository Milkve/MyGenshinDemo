using Entities;
using Managers;
using SkillBridge.Message;
using System;
using UnityEngine;

public class EntityController : MonoBehaviour, EntityManager.IEntityNotify
{

    // Use this for initialization

    public Animator anim;
    private AnimatorStateInfo currentBaseState;

    public Entity entity;

    Vector3 position;
    Vector3 direction;
    Quaternion rotation;

    public Vector3 lastPosition;
    Quaternion lastRotation;
    public bool isPlayer = false;
    void Start()
    {
        if (entity != null)
        {
            UpdateTransform();
            EntityManager.Instance.RegisterEntityNotify(entity, this);
            EntityManager.Instance.AddEnity(entity);
        }
        if (isPlayer)
        {
           
            MainPlayerCamera.Instance.InitGamePlaying();
            
        }

    }

    void FixedUpdate()
    {
        return;
        if (this.entity == null)
            return;

        this.entity.OnUpdate(Time.fixedDeltaTime);

        if (!this.isPlayer)
        {
            this.UpdateTransform();
        }
    }

    void OnDestroy()
    {
        if (entity == null) return;
        //if (UIWorldElementsManager.Instance != null)
        {
            //UIWorldElementsManager.Instance.elements.Remove(this.transform);
        }
        if (EntityManager.Instance != null)
        {

            EntityManager.Instance.RemoveEntity(entity);
        }
    }
    void UpdateTransform()
    {
        position = GameObjectTool.LogicToWorld(entity.position);
        direction = GameObjectTool.LogicToWorld(entity.direction);

        transform.position = position;
        transform.forward = direction;
        lastPosition = position;
        lastRotation = rotation;
    }



    public void OnEntityEvent(EntityEvent entityEvent)
    {
        //Debug.Log("OnEVENT");
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
        }
    }

    public void OnEntityRemoved()
    {
        //throw new NotImplementedException();
    }

    public void OnEntityChanged(NEntity nEntity)
    {
        //Debug.LogFormat("Postion{0}", GameObjectTool.LogicToWorld(nEntity.Position));


        //this.transform.position =GameObjectTool.LogicToWorld(nEntity.Position) ;

        //this.transform.rotation= Quaternion.Euler((GameObjectTool.LogicToWorld(nEntity.Direction))) ; 
    }
}

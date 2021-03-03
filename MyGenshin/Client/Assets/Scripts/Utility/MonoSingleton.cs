using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{

    public bool global = true;
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType<T>();
            }
            return instance;
        }

    }
    public void Awake()
    {
        //Debug.LogWarningFormat("{0}[{1}] Awake", typeof(T), this.GetInstanceID());
        if (global)
        {
            if (instance != null && instance != this.gameObject.GetComponent<T>())
            {
                Destroy(this.gameObject);
                return;
            }
            //Debug.LogFormat("DontDestoryOnLoad:{0}", this.gameObject.name);
            DontDestroyOnLoad(this.gameObject);
            instance = this.gameObject.GetComponent<T>();
        }
        this.OnAwake();
    }
    protected virtual void OnAwake()
    {

    }

    public void Start()
    {
        OnStart();
    }
    protected virtual void OnStart()
    {

    }


}


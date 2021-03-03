using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDir : MonoBehaviour


{
    public float Speed=1;
    void Update()
    {
        this.transform.Rotate(Vector3.left * Time.deltaTime*Speed);

        //Shader.SetGlobalVector("_LightDir", -transform.forward);
    }
}

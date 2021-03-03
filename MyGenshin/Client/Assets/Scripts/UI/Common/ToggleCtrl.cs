using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleCtrl : MonoBehaviour
{
    public GameObject effect;
    public Image icon;
    void Awake()
    {
        GetComponent<Toggle>().onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(bool value)
    {
        
        effect?.SetActive(value);
        if (icon != null)
        {
            icon.color = value ? new Color(60f / 255f, 65f / 255f, 83f / 255f, 233f / 255f) : Color.white;
        }
        
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBgResize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Rect parentRect = transform.parent.GetComponent<RectTransform>().rect;
        float parentScale = parentRect.height / parentRect.width;
        Rect rect = GetComponent<RectTransform>().rect;
        float scale = rect.height / rect.width;
        if (parentScale < scale)
        {
            GetComponent<RectTransform>().sizeDelta= new Vector2( parentRect.width, scale * parentRect.width);
        }
        else
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(parentRect.height/scale,parentRect.height);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

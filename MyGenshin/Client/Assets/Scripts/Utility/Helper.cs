using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public static class Helper
{

    public static CanvasGroup AddCanvasGroup(GameObject gameObject)
    {
        if (gameObject.GetComponent<CanvasGroup>() != null)
        {
            return gameObject.GetComponent<CanvasGroup>();
        }
        else
        {
            return gameObject.AddComponent<CanvasGroup>();
        }
    }
    public static AspectRatioFitter AddAspectRatioFitter(GameObject gameObject)
    {
        if (gameObject.GetComponent<AspectRatioFitter>() != null)
        {
            return gameObject.GetComponent<AspectRatioFitter>();
        }
        else
        {
            return gameObject.AddComponent<AspectRatioFitter>();
        }

    }

    public static Type GetType(string className)
    {
        return Type.GetType(className);
    }


    public static int Enum2Int(object t)
    {
        return (int)t;

    }
}


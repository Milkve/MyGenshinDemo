using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UITip : MonoBehaviour
{

    CanvasGroup canvasGroup;
    void Awake()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }
    public void Show(Action onShowed)
    {
        canvasGroup.DOFade(1f, 0.5f).SetDelay(0.5f).OnComplete(() => onShowed());
    }


    public void Hide(Action onHided)
    {
        canvasGroup.DOFade(0f, 0.5f).SetDelay(0.5f).OnComplete(() => onHided());
    }
}

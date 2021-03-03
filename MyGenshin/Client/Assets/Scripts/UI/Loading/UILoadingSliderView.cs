using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UILoadingSliderView : MonoBehaviour
{

    // Use this for initialization

    public Text Title;
    public Text Text;
    public Slider Slider;
    public Action OnSliderRevealed;
    public Action OnSliderHided;
    public Action OnComplete;
    Vector2 ShowSize=new Vector2(600f,30f);
    Vector2 HideSize = new Vector2(-20f, 30f);

    float curProgressValue;
    float target;
    string targetTitle;
    bool isCompleted = false;
    public float CurProgressValue
    {
        get
        {
            return curProgressValue;
        }

        set
        {
            Slider.value = value<1?value:1;
            Text.text = string.Format("{0:F2}%", Slider.value * 100);
            curProgressValue = value;
        }
    }

    void Start()
    {  
        OnSliderRevealed += () => StartCoroutine(Progress());
      
    }


    IEnumerator Progress()
    {
        while (true)
        {
            while (CurProgressValue < target)
            {
                CurProgressValue += 0.02f;
                yield return new WaitForEndOfFrame();
            }
            Title.text = targetTitle;
            if (CurProgressValue >= 1 && !isCompleted )
            {
                isCompleted = true;
                yield return new WaitForSeconds(0.5f);
                OnComplete();
            }
            yield return new WaitForEndOfFrame();
        }
    }
    public void ShowSlider()
    {
        Slider.GetComponent<RectTransform>().sizeDelta = HideSize;
        Slider.GetComponent<RectTransform>().DOSizeDelta(ShowSize, 0.5f).SetDelay(0.3f).OnComplete(() => { OnSliderRevealed?.Invoke(); });
    }


    public void HideSlider()
        
    {
        Slider.GetComponent<RectTransform>().DOSizeDelta(HideSize, 0.5f).SetDelay(0.3f).OnComplete(() => { OnSliderHided?.Invoke(); });
        
    }
    // Update is called once per frame

   
    public void Hide()
    {

        var canvasGroup= this.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.DOFade(0, 1f).SetDelay(2f).SetEase(Ease.OutCubic).OnComplete(() => { this.gameObject.SetActive(false); });

    }

    public void Reset(string tips,Action onCompelete)
    {
        Title.text = tips;
        targetTitle = tips;
        Slider.value = 0;
        CurProgressValue = 0;
        target = 0;
        isCompleted = false;
        OnComplete = onCompelete;
    }
    public void SetData(float progress,string tips=null)
    {
        if (tips != null)
        {
            targetTitle= tips;
        }
        target = progress;
    }

}

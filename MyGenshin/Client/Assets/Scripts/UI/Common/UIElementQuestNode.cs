using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIElementQuestNode : MonoBehaviour, IEventSystemHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, IPointerDownHandler
{
    // Start is called before the first frame update


    public EventWraperV1<PointerEventData> ActionOnClick = new EventWraperV1<PointerEventData>();
    public EventWraperV1<BaseEventData> ActionOnSelect = new EventWraperV1<BaseEventData>();
    public EventWraperV1<BaseEventData> ActionOnDeselect = new EventWraperV1<BaseEventData>();

    public void OnPointerClick(PointerEventData eventData)
    {
        
        ActionOnClick?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject, eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        ActionOnSelect?.Invoke(eventData);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        ActionOnDeselect?.Invoke(eventData);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementQuestChapterNode : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    public EventWraperV1<PointerEventData> ActionOnClick = new EventWraperV1<PointerEventData>();
    public EventWraperV1<BaseEventData> ActionOnSelect = new EventWraperV1<BaseEventData>();
    public EventWraperV1<BaseEventData> ActionOnDeselect = new EventWraperV1<BaseEventData>();

    public void OnPointerClick(PointerEventData eventData)
    {

        ActionOnClick?.Invoke(eventData);
    }

}

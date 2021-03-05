//#define SHOWLOG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;
using DG.Tweening;
using static UI.Common.UIGroup;
using Interface;

namespace UI.Common
{
    public class UIElement :MonoBehaviour, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler
    {

        UIGroup Group;
        [HideInInspector]
        public bool MultipleSelect { get => Selectable.MultipleSelect; set => Selectable.MultipleSelect = value; }
        [HideInInspector]
        public bool SingleSelect { get => Selectable.SingeSelect; set => Selectable.SingeSelect = value; }
        public class EventWraperUI<Tkey, Tvalue> : EventWraperK1V1<Tkey, Tvalue> where Tkey : Enum
        {
            public void AddListener(int code, Action<Tvalue> callback)
            {
                foreach (var item in Enum.GetValues(typeof(Tkey)))
                {
                    if (((int)item & code) != 0)
                    {
                        AddListener((Tkey)item, callback);
                    }
                }

            }
            public override void AddListener(Tkey state, Action<Tvalue> callback)
            {
                //Debug.Log($"Add {state} {callback.GetHashCode()}");
                base.AddListener(state, callback);
            }
            public void RemoveListener(int code, Action<Tvalue> callback)
            {
                foreach (var item in Enum.GetValues(typeof(Tkey)))
                {
                    if (((int)item & code) != 0)
                    {
                        RemoveListener((Tkey)item, callback);
                    }
                }
            }
            public override void RemoveListener(Tkey state, Action<Tvalue> callback)
            {
                
                //Debug.Log($"Remove {state} {callback.GetHashCode()}");
                base.RemoveListener(state, callback);
            }
        }
        public ISelectable @Selectable;
        public UIState currentState => Group.State.Value;
        public EventWraperUI<UIState, AxisEventData> ActionOnMove = new EventWraperUI<UIState, AxisEventData>();
        public EventWraperUI<UIState, PointerEventData> ActionOnPointerClick = new EventWraperUI<UIState, PointerEventData>();
        public EventWraperUI<UIState, PointerEventData> ActionOnPointerEnter = new EventWraperUI<UIState, PointerEventData>();
        public EventWraperUI<UIState, PointerEventData> ActionOnPointerExit = new EventWraperUI<UIState, PointerEventData>();
        public EventWraperUI<UIState, PointerEventData> ActionOnPointerUp = new EventWraperUI<UIState, PointerEventData>();
        public EventWraperUI<UIState, PointerEventData> ActionOnPointerDown = new EventWraperUI<UIState, PointerEventData>();
        //public EventWraperUI<UIState, PointerEventData> ActionOnDrag = new EventWraperUI<UIState, PointerEventData>();
        //public EventWraperUI<UIState, PointerEventData> ActionOnEndDrag = new EventWraperUI<UIState, PointerEventData>();
        //public EventWraperUI<UIState, PointerEventData> ActionOnDrop = new EventWraperUI<UIState, PointerEventData>();
        public EventWraperUI<UIState, BaseEventData> ActionOnSelect = new EventWraperUI<UIState, BaseEventData>();
        public EventWraperUI<UIState, BaseEventData> ActionOnDeselect = new EventWraperUI<UIState, BaseEventData>();


        public void Init(UIGroup group,ISelectable selectable)
        {
            
            if (Group != null)
            {
                Group?.UnregisterElement(this);
            }
            @Selectable = selectable;
            Group = group;
            Group?.RegisterElement(this);

        }

        ~UIElement()
        {
            Group?.UnregisterElement(this);
        }


        #region 响应方法
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            ActionOnPointerClick?.Invoke(currentState, eventData);

#if SHOWLOG
            Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
        }
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            ActionOnPointerEnter?.Invoke(currentState, eventData);
#if SHOWLOG
            Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
        }
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            ActionOnPointerExit?.Invoke(currentState, eventData);
#if SHOWLOG
            Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            ActionOnPointerUp?.Invoke(currentState, eventData);
#if SHOWLOG
            Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
        }
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject, eventData);
            ActionOnPointerDown?.Invoke(currentState, eventData);
#if SHOWLOG
            Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
        }
        public virtual void OnSelect(BaseEventData eventData)
        {
            Group.SelectSingle(this);
            ActionOnSelect?.Invoke(currentState, eventData);
#if SHOWLOG
             Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
        }
        public virtual void OnDeselect(BaseEventData eventData)
        {
            ActionOnDeselect?.Invoke(currentState, eventData);
#if SHOWLOG
            Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
        }

        #endregion


    }
}

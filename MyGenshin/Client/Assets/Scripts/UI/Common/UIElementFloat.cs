using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Common
{
    public class UIElementFloat: MonoBehaviour, IEventSystemHandler ,IPointerEnterHandler, IPointerExitHandler

    {

        public EventWraperV1< PointerEventData> ActionOnPointerEnter = new EventWraperV1< PointerEventData>();
        public EventWraperV1< PointerEventData> ActionOnPointerExit = new EventWraperV1< PointerEventData>();
        #region 响应方法

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            ActionOnPointerEnter?.Invoke( eventData);
#if SHOWLOG
            Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
        }
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            ActionOnPointerExit?.Invoke( eventData);
#if SHOWLOG
            Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
        }
        #endregion
    }
}

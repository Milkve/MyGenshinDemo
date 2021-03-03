using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace UI.Common
{
    public class UIGroup : MonoBehaviour
    {

        public enum UIState
        {
            Single = 1,
            Multiple = 2,
        }

        public BindableProperty<UIState> State            = new BindableProperty<UIState>(UIState.Single);
        public BindableProperty<UIElement> SingleSelected = new BindableProperty<UIElement>();
        public BindableProperty<int> SelectCount          = new BindableProperty<int>(1);
        public int MaxCount => SingleSelected.Value.Selectable.Count;
        public bool AllowSwitchOff=false;

        Dictionary<int, ISelectable> MultipleSelecteds = new Dictionary<int, ISelectable>();
        public void RegisterElement(UIElement uiElement)
        {
            if (!AllowSwitchOff && SingleSelected.Value == null)
            {
                SingleSelected.Value = uiElement;
                EventSystem.current.SetSelectedGameObject(uiElement.gameObject);
            }
        }

        public void UnregisterElement(UIElement uiElement)
        {
           
        }

        public void SelectSingle(UIElement uIElement)
        {
            SingleSelected.Value = uIElement;
        }

        public void SelectMultiple(UIElement uIElement)
        {
            int uid = uIElement.Selectable.GetHashCode();
            MultipleSelecteds[uid] = uIElement.Selectable;
            uIElement.MultipleSelect = true;

        }
        public void DeselectMultiple(UIElement uIElement)
        {
            int uid = uIElement.Selectable.GetHashCode();
            if (MultipleSelecteds.ContainsKey(uid))
            {
                MultipleSelecteds.Remove(uid);
            }
            uIElement.MultipleSelect = false;
            uIElement.Selectable.SelectCount = 1;
        }


        public void SetCount(int count)
        {
            if (count>=1 && count <= MaxCount)
            {
                SingleSelected.Value.Selectable.SelectCount = count;
                SelectCount.Value = count;
            }
        }

        public void ClearSingle()
        {
            if (SingleSelected.Value != null)
            {
                SingleSelected.Value.SingleSelect = false;
            }
            SingleSelected.Value = null;
        }
        public void ClearMultiple()
        {
            foreach (var item in MultipleSelecteds)
            {
                item.Value.MultipleSelect = false;
            }
            MultipleSelecteds.Clear();
        }

        public List<ISelectable> GetSelectables()
        {
            return MultipleSelecteds.ToList().Select(x => x.Value).ToList();
        }
    }
}

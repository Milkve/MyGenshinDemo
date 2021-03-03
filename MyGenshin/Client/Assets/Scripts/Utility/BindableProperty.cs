using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// 可绑定属性,当该属性值发生改变时,触发监听事件
    /// </summary>
    /// <typeparam name="T">该可绑定属性的类型</typeparam>
    public class BindableProperty<T>
    {
        T _value ;
        public EventWraperV2<T, T> OnValueChange = new EventWraperV2<T, T>();

        public BindableProperty(T value=default){
            _value = value;
        }

        public T Value
        {
            get=> _value;        
            set
            {
                if (!Equals(_value, value))
                {
                    T oldValue = _value;
                    _value = value;
                    OnValueChange.Invoke(oldValue, _value);
                }
            }
        }

        public override string ToString()
        {

            return typeof(T).Name +_value.ToString();

        }

    }

    
}

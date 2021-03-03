using System;
using System.Collections.Generic;
using System.Linq;


namespace Common.Utils
{

    public static class SciValueUtil
    {
        public struct SciValue
        {
            int value;
            int index;


            public double GetRealValue()
            {
                return Math.Pow(10, index) * value;
            }
            public int GetValue()
            {
                return this.value;
            }
            public static SciValue operator +(SciValue value, SciValue other)
            {
                double newValue= value.GetRealValue() + other.GetRealValue();
                int newIndex = Math.Min(value.index, other.index);
                return new SciValue(newValue, newIndex);


            }
            public static SciValue operator -(SciValue value, SciValue other)
            {
                double newValue = value.GetRealValue() - other.GetRealValue();
                int newIndex = Math.Min(value.index, other.index);
                return new SciValue(newValue, newIndex);
            }

            public static SciValue operator *(SciValue value,float other)
            {
                value.value = (int)(value.value * other);
                return value;
            }


            public SciValue(double newValue,int index)
            {
                this.value = (int)(newValue * Math.Pow(10, -index));
                this.index = index;
            }
            

        }




    }




}

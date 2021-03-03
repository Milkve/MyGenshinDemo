using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface ISelectable
    {

        int ID { get;}
        bool SingeSelect { get; set; }
        bool MultipleSelect { get; set; }
        int Rare { get; }
        int Count { get;}
        int SelectCount { get; set; }
        bool isItem { get; }

    }
}

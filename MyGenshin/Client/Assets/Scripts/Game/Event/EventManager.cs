using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLua;

[LuaCallCSharp]
public static class EventManager


{

    public static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();


    public static void AddListener(string eventType, Delegate func){
        
        
        
        
        }


}


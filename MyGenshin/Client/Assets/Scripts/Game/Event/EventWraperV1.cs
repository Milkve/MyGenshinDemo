using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
// 方便Lua绑定事件


public class EventWraper
{
    Action @event;
    public virtual void AddListener(Action callback) => @event += callback;
    public virtual void RemoveListener(Action callback) => @event -= callback;
    public virtual void Invoke() => @event?.Invoke();

}
public class EventWraperV1<Tvalue>
{
    Action<Tvalue> @event;
    public virtual void AddListener(Action<Tvalue> callback) => @event += callback;
    public virtual void RemoveListener(Action<Tvalue> callback) => @event -= callback;
    public virtual void Invoke(Tvalue data) => @event?.Invoke(data);

}
public class EventWraperV2<Tvalue1, Tvalue2>
{
    Action<Tvalue1, Tvalue2> @event;
    public virtual void AddListener(Action<Tvalue1, Tvalue2> callback) => @event += callback;
    public virtual void RemoveListener(Action<Tvalue1, Tvalue2> callback) => @event -= callback;
    public virtual void Invoke(Tvalue1 data1, Tvalue2 data2) => @event?.Invoke(data1, data2);

}
public class EventWraperK1V1<Tkey, Tvalue>
{
    Dictionary<Tkey, Action<Tvalue>> Table = new Dictionary<Tkey, Action<Tvalue>>();

    public virtual void AddListener(Tkey state, Action<Tvalue> callback)
    {
        if (Table.ContainsKey(state))
        {
            Table[state] += callback;
        }
        else
        {
            Table[state] = callback;
        }


    }
    public virtual void RemoveListener(Tkey state, Action<Tvalue> callback)
    {
        if (Table.ContainsKey(state))
        {
            Table[state]-= callback;
        }
        if (Table[state] == null)
        {
            Table.Remove(state);
        }
    }
    public virtual void Invoke(Tkey state, Tvalue data)
    {
        if (Table.ContainsKey(state))
        {
            Table[state]?.Invoke(data);
        }              
    }

}

public class EventWraperK1V2<Tkey, Tvalue1, Tvalue2>
{
    Dictionary<Tkey, Action<Tvalue1, Tvalue2>> Table = new Dictionary<Tkey, Action<Tvalue1, Tvalue2>>();

    public virtual void AddListener(Tkey state, Action<Tvalue1, Tvalue2> callback)
    {
        if (Table.ContainsKey(state))
        {
            Table[state] += callback;
        }
        else
        {
            Table[state] = callback;
        }

    }
    public virtual void RemoveListener(Tkey state, Action<Tvalue1, Tvalue2> callback)
    {
        if (Table.ContainsKey(state))
        {
            Table[state] -= callback;
        }
        if (Table[state] == null)
        {
            Table.Remove(state);
        }
    }
    public virtual void Invoke(Tkey state, Tvalue1 data1, Tvalue2 data2)
    {
        if (Table.ContainsKey(state))
        {
            Table[state]?.Invoke(data1,data2);
        }
    }

}



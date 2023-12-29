using System;using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Fusion;
using UnityEngine;

public class GlobalEvents : MonoBehaviour
{
    public event EventHandler<PlayerRef> PlayerJoined;
    public event EventHandler<PlayerRef> LocalPlayerJoined;

    private static GlobalEvents _instance;

    private Dictionary<string, EventInfo> eventMap = new Dictionary<string, EventInfo>();

    private void Awake()
    {
        EventInfo[] events = typeof(GlobalEvents).GetEvents(BindingFlags.Public);

        foreach (var e in events)
        {
            eventMap[e.Name] = e;
        }
    }

    public void TriggerEventByName(string eventName, params object[] eventParams)
    {
        if (eventMap.TryGetValue(eventName, out EventInfo eventInfo))
        {
            // 获取事件委托的类型信息
            Type delegateType = eventInfo.EventHandlerType;

            // 验证参数是否与事件委托的参数匹配
            MethodInfo raiseMethod = eventInfo.GetRaiseMethod();
            ParameterInfo[] eventParameters = raiseMethod.GetParameters();

            if (eventParams.Length != eventParameters.Length)
            {
                Debug.LogError($"Parameter count mismatch for event '{eventName}'");
                return;
            }

            for (int i = 0; i < eventParameters.Length; i++)
            {
                Type expectedType = eventParameters[i].ParameterType;
                if (!expectedType.IsAssignableFrom(eventParams[i].GetType()))
                {
                    Debug.LogError($"Parameter type mismatch at index {i} for event '{eventName}'");
                    return;
                }
            }
            
            raiseMethod?.Invoke(this, eventParams);
        }
        else
        {
            Debug.LogError("Event not found:" + eventName);
        }
    }

    public static GlobalEvents Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GlobalEvents>();

                if (_instance == null)
                {
                    GameObject globalEvents = new GameObject("GlobalEvents");
                    _instance = globalEvents.AddComponent<GlobalEvents>();
                }
            }

            return _instance;
        }
    }
}
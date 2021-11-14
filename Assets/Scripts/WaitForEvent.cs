using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class WaitForEvent
{
    public static async Task getTask(UnityEvent unityEvent)
    {
        var trigger = true;
        Action action = () => trigger = false;
        unityEvent.AddListener(action.Invoke);
        while (trigger)
        {
            await Task.Yield();
        }
        unityEvent.RemoveListener(action.Invoke);
    }
}
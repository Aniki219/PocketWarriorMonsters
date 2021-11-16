using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class WaitFor
{
    public static async Task ButtonPress(string button)
    {
        while(!InputManager.getKeyPressed(button))
        {
            await Task.Yield();
        }
    }

    public static async Task<bool> ConfirmOrDeny()
    {
        bool chosen = false;
        bool choice = false;
        while (!chosen)
        {
            if (InputManager.getKeyPressed("Confirm")) {
                chosen = true;
                choice = true;
            }
            if (InputManager.getKeyPressed("Back"))
            {
                chosen = true;
                choice = false;
            }
            await Task.Yield();
        }
        return choice;
    }

    /* Here we wait until a UnityEvent is called before returning*/
    public static async Task Event(UnityEvent unityEvent)
    {
        bool trigger = true;
        Action action = () => trigger = false;
        unityEvent.AddListener(action.Invoke);
        while (trigger)
        {
            await Task.Yield();
        }
        unityEvent.RemoveListener(action.Invoke);
    }

    /* This is the same as above but allowing for a single generic type parameter */
    public static async Task<T> Event<T>(UnityEvent<T> unityEvent)
    {
        bool trigger = true;

        //default is how you initialize a general variable
        T value = default;
        Action<T> action = (e) =>
        {
            trigger = false;
            value = e;
        };
        unityEvent.AddListener(action.Invoke);
        while (trigger)
        {
            await Task.Yield();
        }
        unityEvent.RemoveListener(action.Invoke);

        /* With Tasks we can just return the value they hold, we don't return
         * a Task object
         * */
        return value;
    }
}

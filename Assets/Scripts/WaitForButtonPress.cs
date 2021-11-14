using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WaitForButtonPress
{
    public static async Task getTask(string button)
    {
        while(!InputManager.getKeyPressed(button))
        {
            await Task.Yield();
        }
    }
}

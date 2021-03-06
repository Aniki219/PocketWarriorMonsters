using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[CreateAssetMenu(menuName = "Managers/InputManager")]
public class InputManager : ScriptableObject
{
    private static InputManager instance;
    public static InputManager Instance { get { return instance; } }

    public static List<KeyState> keyStates;
    public static List<AxisState> axisStates;

    public InputMaster controls;

    public enum actionMapNames
    {
        Keyboard,
        WASDMouse,
        PS4,
        Custom
    }
    public actionMapNames actionMap = actionMapNames.Keyboard;
    List<actionMapNames> aimAtMouseMaps = new List<actionMapNames>()
    {
        actionMapNames.WASDMouse
    };

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<InputManager>("Managers")[0];
        instance.controls = new InputMaster();

        setControlScheme(instance.actionMap.ToString());
    }

    public static void setControlScheme(string name)
    {
        instance.actionMap = (actionMapNames)Enum.Parse(typeof(actionMapNames), name);
        if (instance.controls.asset != null)
        {
            foreach (InputActionMap actionMap in instance.controls.asset.actionMaps)
            {
                actionMap.Disable();
            }
        }

        InputActionMap input = instance.controls.asset.FindActionMap(name);
        input.Enable();

        axisStates = new List<AxisState>();
        axisStates.Add(new AxisState("DPad"));

        keyStates = new List<KeyState>();
        foreach (InputAction action in input.actions)
        {
            string key = action.name;
            keyStates.Add(new KeyState(key));
            action.performed += ctx => { setKeyState(key, true); };
            action.canceled += ctx => { setKeyState(key, false); };
        }
        input.FindAction("Movement").started += ctx => { setAxisState("DPad", ctx.ReadValue<Vector2>()); };
        input.FindAction("Movement").performed += ctx => { setAxisState("DPad", ctx.ReadValue<Vector2>()); };
        input.FindAction("Movement").canceled += ctx => { setAxisState("DPad", ctx.ReadValue<Vector2>()); };
    }

    static void setKeyState(string keyName, bool state)
    {
        KeyState keyState = keyStates.Find(ks => ks.keyName == keyName);
        if (keyState == null) throw new Exception("No keyState " + keyName + " Found!");
        keyState.state = state;
        keyState.startTime = Time.time;
    }

    static void setAxisState(string axisName, Vector2 state)
    {
        AxisState axisState = axisStates.Find(axis => axis.axisName == axisName);
        if (axisState == null) throw new Exception("No axisState " + axisName + " Found!");
        axisState.state = state;
        axisState.startTime = Time.time;
    }

    public static KeyState getKeyState(string keyName)
    {
        KeyState keyState = keyStates.Find(ks => ks.keyName == keyName);
        if (keyState == null) throw new Exception("No keyState " + keyName + " Found!");
        return keyState;
    }

    public static Vector2 getAxisState(string axisName)
    {
        AxisState axisState = axisStates.Find(axis => axis.axisName == axisName);
        if (axisState == null) throw new Exception("No axisState " + axisName + " Found!");
        return axisState.state;
    }

    public static bool getKeyPressed(string keyName, bool useDeltaTime = false)
    {
        KeyState keyState = getKeyState(keyName);
        return keyState.state && keyState.justPressed(useDeltaTime);
    }

    public static bool getKey(string keyName)
    {
        return getKeyState(keyName).state;
    }

    public bool aimAtMouse()
    {
        return aimAtMouseMaps.Contains(instance.actionMap);
    }

//    public Vector2 getPlayerAim(bool fourDirectional = false)
//    {
//        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameManager.Instance.player.transform.position;

//        if (fourDirectional)
//        {
//            int angle = (int)Vector2.SignedAngle(Vector2.right, direction.normalized);
//            if (angle < 0) angle += 360;
//            int vthreshold = 50;
//            if (angle > vthreshold && angle < 180 - vthreshold) {
//                return Vector2.up;
//            } else if (angle >= 180 - vthreshold && angle <= 180 + vthreshold)
//            {
//                return Vector2.left;
//            } else if (angle > 180 + vthreshold && angle < 360 - vthreshold)
//            {
//                return Vector2.down;
//            } else
//            {
//                return Vector2.right;
//            }
//        } else
//        {
//            return direction.normalized;
//        }
//    }
}

public class KeyState
{
    public string keyName;
    public bool state;
    public double startTime;

    public KeyState(string keyName)
    {
        this.keyName = keyName;
        state = false;
    }

    //Using deltaTime allows this function to work even in async methods such as "onTrigger" 
    public bool justPressed(bool useDeltaTime = false)
    {
        return (Time.time - startTime <= (useDeltaTime?Time.deltaTime:0));
    }
}

public class AxisState
{
    public string axisName;
    public Vector2 state;
    public double startTime;

    public AxisState(string axisName)
    {
        this.axisName = axisName;
        state = Vector2.zero;
    }

    public bool justPressed()
    {
        return (Time.time - startTime <= 0);
    }
}

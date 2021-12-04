using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleController;

public abstract class BattleAction : IComparable<BattleAction>
{
    public abstract string script();
    public Pokemon source;
    public List<FieldSlotController> targets;

    //All actions marked to be removed will not fire
    public bool toBeRemoved = false;

    /*Every battle action has a battle buffer in the battle controller that it should be
    * added to when used.
    */
    protected BattleBuffer battleBuffer;
    public BattleBuffer getBattleBuffer() { return battleBuffer; }

    public override string ToString()
    {
        return "Generic BattleAction: " + battleBuffer.ToString();
    }

    public virtual int CompareTo(BattleAction action)
    {
        return 0;
    }
}

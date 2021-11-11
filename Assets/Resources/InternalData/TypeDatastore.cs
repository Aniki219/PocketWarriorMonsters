using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class TypeDatastore : ScriptableObject
{
    Dictionary<Typing, PokemonType> test;
}

public enum Typing
{
    NORMAL,
    BUG,
}
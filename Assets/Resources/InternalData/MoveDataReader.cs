using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/Move Data Reader")]
public class MoveDataReader : ScriptableObject
{
    private static MoveDataReader instance;
    public static MoveDataReader Instance { get { return instance; } }

    private static string path = "Data/PokemonMoves.json";
    public static PokemonMoveData data;

    [RuntimeInitializeOnLoadMethod]
    public static void readMoveData()
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        data = JsonUtility.FromJson<PokemonMoveData>(json);
    }

    public static List<MoveData> getMovesList()
    {
        return data.moves;
    }

    public static MoveData getMoveData(Moves moveEnum)
    {
        return data.moves[(int)moveEnum];
    }
}

[Serializable]
public class PokemonMoveData
{
    public List<MoveData> moves;

    public void printMoveList()
    {
        foreach (MoveData move in moves)
        {
            Debug.Log(move.name);
        }
    }
}

[Serializable]
public struct MoveData
{
    public string name;
    public string type;
    public string category;
    public int pp;
    public int power;
    public int accuracy;
}
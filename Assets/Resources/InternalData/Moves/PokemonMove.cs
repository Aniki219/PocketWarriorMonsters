using HelperFunctions;
using StatusEffects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* This class holds the JSON data directly.
 * Pokemon will have a list of PokemonMoves, however this is an abstract class.
 * The StandardMove class is the generic implementation of this class for damaging
 * moves, simple status effects, and stat boost moves
 * */
public abstract class PokemonMove
{
    protected Moves moveEnum;
    protected Pokemon pokemon;
    protected string name;
    protected PokemonType type;
    protected MoveCategory category;
    protected int pp;
    protected int currentPp;
    protected int power;
    protected int accuracy;
    public Targets targets;
    public int priority;
    public int multihit;
    public float crit_ratio;
    public int heal;
    public MoveStatus moveStatus;

    public PokemonMove(Pokemon pokemon, Moves moveEnum)
    {
        this.pokemon = pokemon;
        this.moveEnum = moveEnum;
        getStats(moveEnum);
    }

    //Gets the JSON Data for the Move
    protected void getStats(Moves moveEnum)
    {
        this.moveEnum = moveEnum;

        MoveData data = MoveDataReader.getMoveData(moveEnum);

        name = StringHelper.ToTitleCase(data.name, true);
        type = PokemonType.get(EnumHelper.GetEnum<TypeEnum>(data.type));
        category = EnumHelper.GetEnum<MoveCategory>(data.category);
        pp = data.pp;
        currentPp = data.pp;
        power = data.power;
        accuracy = data.accuracy;
        targets = EnumHelper.GetEnum<Targets>(data.targets);
        priority = data.priority;
        multihit = data.multihit;
        crit_ratio = data.crit_ratio;
        heal = data.heal;
        moveStatus = new MoveStatus(data.status, data.statuschance, data.statustargets);
}

    public void setPokemon(Pokemon pokemon)
    {
        this.pokemon = pokemon;
    }

    public string getName()
    {
        return name;
    }

    public PokemonType getType()
    {
        return type;
    }

    public MoveCategory getCategory()
    {
        return category;
    }

    public int getPp()
    {
        return pp;
    }

    public int getCurrentPp()
    {
        return currentPp;
    }

    public void decPp()
    {
        if (currentPp > 0)
        {
            currentPp--;
        } else
        {
            throw new Exception("Move " + name + " with 0 PP decremented");
        }
    }

    public int getPower()
    {
        return power;
    }

    public int getAccuracy()
    {
        return accuracy;
    }

    public Pokemon getPokemon()
    {
        return pokemon;
    }

    public virtual List<string> getScript()
    {
        return null;
    }
}

public class MoveStatus
{
    public StatusType status;
    public int status_chance;
    public Targets status_targets;

    public MoveStatus(string statusString, int status_chance, string status_targetsString)
    {
        status = EnumHelper.GetEnum<StatusType>(statusString);
        this.status_chance = status_chance;
        status_targets = EnumHelper.GetEnum<Targets>(status_targetsString);
    }
}


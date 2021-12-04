using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* This class holds the JSON data directly.
 * Pokemon will have a list of PokemonMoves, however this is an abstract class.
 * The StandardMove class is currently the only implementation of this class and may
 * become obsolete.
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

        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        name = data.name.ToLower();
        name = name.Replace('_', ' ');
        name = textInfo.ToTitleCase(name);
        type = PokemonType.get((TypeEnum)Enum.Parse(typeof(TypeEnum), data.type.ToUpper()));
        category = (MoveCategory)Enum.Parse(typeof(MoveCategory), data.category.ToUpper());
        pp = data.pp;
        currentPp = data.pp;
        power = data.power;
        accuracy = data.accuracy;
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

    public virtual string getScript()
    {
        return "none";
    }
}


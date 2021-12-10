
using System.Collections.Generic;

class Hitself : PokemonMove
{
    public Hitself(Pokemon sourcePokemon) : base(sourcePokemon, Moves.HITSELF)
    {

    }

    public override List<string> getScript()
    {
        return new List<string>() {
            string.Format("<?zoom|{0}>It hurt itself in confusion!<br><br>", pokemon.fieldSlot.slotNumber)
        };
    }
}


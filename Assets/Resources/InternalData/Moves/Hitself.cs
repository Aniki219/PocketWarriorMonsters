
class Hitself : PokemonMove
{
    public Hitself(Pokemon sourcePokemon) : base(sourcePokemon, Moves.HITSELF)
    {

    }

    public override string getScript()
    {
        return string.Format("<?zoom|{0}>It hurt itself in confusion!<br><br>", pokemon.fieldSlot.slotNumber);
    }
}


using System;

/* This Pokemon Move belongs to a pokemon
 * When the move is used in Battle it's data will be sent to a new BattleMove object.
 * This may be unnecessary but the idea is that this is immutable and BattleMove's can be
 * destroyed after use.
 * 
 * This is a standard move, it deals its damage and may apply a status condition or a buff.
 * Right not the script is stored in the BattleMove, we'll probably move it here.
 * All other moves are custom classes which extend from PokemonMove.
 * It is seeming more and more possible that all moves will be a StandardMove. All weird effects
 * can just be treated as buffs
 * Buffs we can come up with a better name for.. effects maybe
 * TrickRoom or Stealthrock just applies this effect as a BattleAction to the correct buffer
 * - We will however have to make custom classes for each effect though, so it's the same idea mostly
 * only our structure is no longer necessary..
 */
public class StandardMove : PokemonMove
{
    public StandardMove(Moves moveEnum, Pokemon sourcePokemon) : base(sourcePokemon, moveEnum) { }
}



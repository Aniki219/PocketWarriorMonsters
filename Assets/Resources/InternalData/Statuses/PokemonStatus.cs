using HelperFunctions;
using System;
using System.Threading.Tasks;
using UnityEngine;

/* A duration of -1 makes this a non-durational status like paralysis.
 * Secondary statuses can be applied ontop of other statuses. You can be
 * confused and paralysed, but you can't be asleep and poisoned.
 * */
namespace StatusEffects
{
    public abstract class PokemonStatus : IComparable<PokemonStatus>
    {
        protected BattleController.BattleBuffer buffer;
        public int duration;
        public Pokemon pokemon;

        /* Volatile statuses go away on their own or at the end of battle
         * additionally there is no limit to the number of unique volatile
         * statuses a pokemon can have.
         * */
        public bool isVolatile;

        protected StatusType priority = StatusType.NONE;

        public PokemonStatus()
        {

        }

        public void setPokemon(Pokemon pokemon)
        {
            this.pokemon = pokemon;
        }

        /* I think the best way to do this is to have the BattleController pass in
         * a reference to the BattleMessageController. I don't like it, but this is
         * only going to ever work one way, so it's fine to have this dependency
         * */
        public virtual async Task DoStatus(BattleMessageController messageController)
        {
            if (duration == 0) return;
            duration--;
            await Task.Yield();
        }

        public virtual async Task<BattleMove> DoInterruptStatus(BattleMessageController messageController)
        {
            if (duration == 0) return null;
            duration--;
            await Task.Yield();
            return null;
        }

        /* This is for identifying which statuses should activate when
         * */
        public BattleController.BattleBuffer getBuffer()
        {
            return buffer;
        }

        public int CompareTo(PokemonStatus other)
        {
            return priority.CompareTo(other.priority);
        }

        /* This method allows us to instantiate a pokemon status from an enum and a
         * pokemon. We'll store statusEffect data as an enum and instantiate statuses
         * via this method. I added the Namespace check just so there are definitely no
         * collisions with other classes, like Sleep or None or something could be a
         * built in name.
         * */
        public static PokemonStatus create(StatusType statusType)
        {
            string status = StringHelper.ToTitleCase(statusType.ToString().ToLower());
            Type pokemonStatusType = Type.GetType(typeof(PokemonStatus).Namespace + "." + status);
            if (pokemonStatusType == null)
            {
                throw new Exception("No PokemonStatus type " + status + " found!");
            }
            else
            {
                return (PokemonStatus)Activator.CreateInstance(pokemonStatusType);
            }
        }

        /* This just tries to convert the string to a StatusType enum and then passes
         * along to the above method
         * */
        public static PokemonStatus create(string status)
        {
            StatusType statusType = EnumHelper.GetEnum<StatusType>(status);
            return create(statusType);
        }
    }

    public enum StatusType
    {
        BURN,
        FREEZE,
        PARALYSIS,
        POISONED,
        BADLY_POISONED,
        SLEEP,
        CONFUSION,
        INFATUATION,
        FLINCH,
        LEECHSEED,
        CURSE,
        CANT_ESCAPE,
        EMBARGO,
        ENCORE,
        PERISH_SONG,
        TAUNT,
        TORMENT,
        CHARGING,
        CENTER_OF_ATTENTION,
        PROTECTED,
        THRASHING,
        PETAL_DANCE,
        RAGE,
        NONE,
        BOOST,
        OHKO,
        MAX_HP_HEAL,
        REMOVE_NEGATIVE_BOOSTS,
        REMOVE_BOOSTS,
        SUBSTITUTE
    }
}

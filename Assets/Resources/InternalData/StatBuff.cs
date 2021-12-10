public class StatBuff
{
    Stats stat;
    int amount;
    int duration;

    public StatBuff(Stats stat, int amount, int duration = -1)
    {
        this.stat = stat;
        this.amount = amount;
        this.duration = duration;
    }
}

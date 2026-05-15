using Godot;

public static class StrikeAction
{
    public static StrikeResult Execute(Combatant attacker, Combatant target, int situationalBonus = 0)
    {
        if (!attacker.SpendAction())
            return new StrikeResult { Outcome = StrikeOutcome.Miss };

        int roll = Dice.Roll(20);
        int total = roll + attacker.Data.AttackBonus + attacker.MultipleAttackPenalty + situationalBonus;
        int diff = total - target.Data.ArmorClass;

        StrikeOutcome outcome;
        if (roll == 20 || diff >= 10)
            outcome = StrikeOutcome.CriticalHit;
        else if (diff >= 0)
            outcome = StrikeOutcome.Hit;
        else if (roll == 1 || diff <= -10)
            outcome = StrikeOutcome.CriticalMiss;
        else
            outcome = StrikeOutcome.Miss;

        int damage = 0;
        if (outcome is StrikeOutcome.Hit or StrikeOutcome.CriticalHit)
        {
            // PF2e crits double the number of dice, not the total
            int diceCount = outcome == StrikeOutcome.CriticalHit
                ? attacker.Data.DamageDiceCount * 2
                : attacker.Data.DamageDiceCount;

            damage = Mathf.Max(1, Dice.Roll(diceCount, attacker.Data.DamageDie) + attacker.Data.DamageBonus);
            target.TakeDamage(damage, source: attacker);
        }

        return new StrikeResult
        {
            Outcome = outcome,
            Roll = roll,
            TotalToHit = total,
            DamageDealt = damage
        };
    }
}

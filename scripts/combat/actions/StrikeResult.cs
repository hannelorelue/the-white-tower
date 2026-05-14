public enum StrikeOutcome { CriticalMiss, Miss, Hit, CriticalHit }

public struct StrikeResult
{
    public StrikeOutcome Outcome;
    public int Roll;
    public int TotalToHit;
    public int DamageDealt;
}

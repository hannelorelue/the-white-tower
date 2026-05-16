using Godot;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    public ChampionData PlayerData { get; private set; }
    public Backpack PlayerBackpack { get; private set; }
    public CombatantData PendingEnemyData { get; private set; }

    private string _returnScenePath;

    public override void _Ready()
    {
        Instance = this;
        PlayerData = GD.Load<ChampionData>("res://resources/champion.tres");
    }

    public void RegisterBackpack(Backpack backpack)
    {
        PlayerBackpack = backpack;
    }

    public void StartCombat(CombatantData enemyData)
    {
        PendingEnemyData = enemyData;
        _returnScenePath = GetTree().CurrentScene.SceneFilePath;
        GetTree().ChangeSceneToFile("res://scenes/combat/battle_arena.tscn");
    }

    public void EndCombat()
    {
        PendingEnemyData = null;
        if (_returnScenePath != null)
            GetTree().ChangeSceneToFile(_returnScenePath);
    }
}

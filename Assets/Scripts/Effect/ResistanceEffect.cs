
public class ResistanceEffect : Effect
{
    static Player player => Player.instance;
    
    public override void OnStart()
    {
        player.resistance = 0;
    }
    public override void OnEnd()
    {
        player.resistance = 1;
    }
}

using UnityEngine;

public class ResistanceEffect : Effect
{
    static Player player => Player.instance;
    [SerializeField] SpriteAlphaBlink alphaBlink;

    public override void OnStart()
    {
        player.resistance = 0;
        alphaBlink.gameObject.SetActive(true);
        alphaBlink.time = 0;
    }
    public override void OnUpdate()
    {
        if (maxTime != -1 && time < 3)
        {
            alphaBlink.enabled = true;
        }
    }
    public override void OnEnd()
    {
        player.resistance = 1;
        if (maxTime != -1)
        {
            alphaBlink.enabled = false;
            alphaBlink.gameObject.SetActive(false);
        }
    }
}

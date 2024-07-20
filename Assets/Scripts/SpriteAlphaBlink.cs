using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAlphaBlink : MonoBehaviour
{
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [SerializeField] float length = 0;
    [SerializeField] float max = 1;
    [SerializeField] float min = 0;

    public float time = 0;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        time += Time.deltaTime * 2 * Mathf.PI / length;
        if (time > 2 * Mathf.PI) time -= 2 * Mathf.PI;

        spriteRenderer.color = new Color(
            spriteRenderer.color.r, 
            spriteRenderer.color.g, 
            spriteRenderer.color.b,
            min + (max - min) * (0.5f * (Mathf.Sin(time) + 1))
        );
    }
}

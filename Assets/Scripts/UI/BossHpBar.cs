using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class BossHpBar : MonoBehaviour
{
    public static BossHpBar instance;

    public Entity target;
    [SerializeField] Transform fillSprite;
    [SerializeField] Transform backGroundSprite;

    void Awake()
    {
        instance = this;
    }
    void Update()
    {

#if UNITY_EDITOR
        if (EditorApplication.isPlaying == false)
        {
            if (instance == null) instance = this;

            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
#endif
        if (target == null)
        {
            fillSprite.gameObject.SetActive(false);
            backGroundSprite.gameObject.SetActive(false);
        }
        else
        {
            fillSprite.gameObject.SetActive(true);
            backGroundSprite.gameObject.SetActive(true);

            float fill = target.hp / target.EntityData.maxHp;

            fillSprite.transform.localPosition = new Vector3(-0.5f * (1 - fill), 0, 0);
            fillSprite.transform.localScale = new Vector3(fill, 1, 1);
        }
    }
}

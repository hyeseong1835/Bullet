using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI surviveTimeText;
    [SerializeField] TextMeshProUGUI killEnemyText;

    public void Show(string title)
    {
        titleText.text = title;
        scoreText.text = $"Score: {Player.instance.exp}";
        surviveTimeText.text = $"Survive Time: {GameManager.instance.time.ToString("F1")}";
        killEnemyText.text = $"Kill Enemy: {GameManager.instance.killEnemy}";

        gameObject.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

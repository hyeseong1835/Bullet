#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
#endif
using UnityEngine;

public enum GameState
{
    Editor, Ready, Play, Pause
}
[ExecuteAlways]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    static Player player => Player.instance;

    public KeySetting key;

    public GameState state;
    public static bool IsEditor => instance.state == GameState.Editor;
    public static float deltaTime => instance.gameSpeed * Time.deltaTime;

    [SerializeField] GameObject mainPanel;
    [SerializeField] GameOverWindow gameOverWindow;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] HpBar bossBar;
    public Stage stage;

    [SerializeField] GameObject bossPrefab;

    public float gameSpeed = 1;
    [NonSerialized] public float time = -1;

    int eventIndex = 0;
    int enemyIndex = 0;

    [Header("Debug")]
    //F1
    [SerializeField] ResistanceEffect ResistanceEffect;
    //F5
    [SerializeField] WeaponItemData[] weaponItemData;
    //F6
    [SerializeField] ItemData[] activeItem;

    [NonSerialized] public int killEnemy = 0;

    void Awake()
    {
        instance = this;

#if UNITY_EDITOR
        if (EditorApplication.isPlaying) state = GameState.Ready;
        else state = GameState.Editor;
#else
        state = GameState.Ready;
#endif
    }
    void Start()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying) stage.Init();
#else
        stage.Init();
#endif
    }

    void Update()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying == false) state = GameState.Editor;
#endif
        switch (state)
        {
            case GameState.Ready:
                Ready();
                break;

            case GameState.Play:
                Play();
                break;

            case GameState.Pause:
                Pause();
                break;
        }
    }
    public void StartButtonDown()
    {
        state = GameState.Play;
        mainPanel.SetActive(false);
        time = 0;
#if UNITY_EDITOR
        StageEditor.instance?.Repaint();
#endif
    }
    void Ready()
    {

    }
    void Play()
    {
        if (Input.GetKeyDown(KeyCode.F1)) Debug_Resistance();
        if (Input.GetKeyDown(KeyCode.F2)) Debug_LevelUp();
        if (Input.GetKeyDown(KeyCode.F3)) Debug_LevelDown();
        if (Input.GetKeyDown(KeyCode.F4)) Debug_Heal();
        if (Input.GetKeyDown(KeyCode.F5)) Debug_SpawnWeapon();
        if (Input.GetKeyDown(KeyCode.F6)) Debug_SpawnActiveItem();
        if (Input.GetKeyDown(KeyCode.F7)) Debug_TimeMove();

        if (Input.GetKeyDown(KeyCode.P)) Debug_StopToggle();

        stage.Read(
            ref enemyIndex,
            ref eventIndex,
            time
        );
        time += deltaTime;
        timeText.text = time.ToString("F1");

#if UNITY_EDITOR
        StageEditor.instance?.Repaint();
#endif
    }

    #region Debug

    void Debug_Resistance()
    {
        if (ResistanceEffect.gameObject.activeInHierarchy)
        {
            ResistanceEffect.Stop();
        }
        else
        {
            ResistanceEffect.Execute(-1);
        }
    }
    void Debug_LevelUp()
    {
        if (player.level + 1 < player.levelUpExp.Length)
        {
            player.LevelUp();
        }
    }
    void Debug_LevelDown()
    {
        if (player.level - 1 >= 0)
        {
            player.LevelDown();
        }
    }
    void Debug_Heal()
    {
        player.hp = player.maxHp;
    }
    void Debug_SpawnWeapon()
    {
        if (weaponItemData.Length == 0) return;

        int random = UnityEngine.Random.Range(0, weaponItemData.Length);
        weaponItemData[random].Drop(
            new Vector2(0, 2.5f),
            UnityEngine.Random.insideUnitCircle.normalized
        );
    }
    void Debug_SpawnActiveItem()
    {
        if (activeItem.Length == 0) return;

        int random = UnityEngine.Random.Range(0, activeItem.Length);
        activeItem[random].Drop(
            new Vector2(0, 2.5f),
            UnityEngine.Random.insideUnitCircle.normalized
        );
    }
    void Debug_TimeMove()
    {
        time += 1;
#if UNITY_EDITOR
        StageEditor.instance?.Repaint();
#endif
    }
    void Debug_StopToggle()
    {
        if (gameSpeed == 0)
        {
            gameSpeed = 1;
        }
        else
        {
            gameSpeed = 0;
        }
    }

    #endregion

    void Pause()
    {

    }

    public static void StartBoss()
    {
        Stage1Boss boss = Instantiate(instance.bossPrefab).GetComponent<Stage1Boss>();
        boss.transform.position = new Vector2(0, 3);
        instance.bossBar.target = boss;
        ScrollManager.instance.speed = 0.1f;
    }

    public void GameFail()
    {
        state = GameState.Pause;
        gameOverWindow.Show("FAIL");
    }
    public void GameWin() 
    { 
        state = GameState.Pause;
        gameOverWindow.Show("CLEAR");
    }
    void OnValidate()
    {
        instance = this;
    }
}

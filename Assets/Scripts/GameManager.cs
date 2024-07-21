#if UNITY_EDITOR
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
    [SerializeField] TextMeshProUGUI timeText;
    public Stage stage;

    [SerializeField] GameObject bossPrefab;
    
    public float gameSpeed = 1;
    public float time = -1;

    int eventIndex = 0;

    [Header("Debug")]
    //F1
    [SerializeField] ResistanceEffect ResistanceEffect;
    //F5
    [SerializeField] WeaponItemData[] weaponItemData;
    //F6
    [SerializeField] ItemData[] activeItem;

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
        
        int random = Random.Range(0, weaponItemData.Length);
        weaponItemData[random].Drop(
            new Vector2(0, 2.5f),
            new Vector2(0, 0)
        );
    }
    void Debug_SpawnActiveItem()
    {
        if (activeItem.Length == 0) return;

        int random = Random.Range(0, activeItem.Length);
        activeItem[random].Drop(
            new Vector2(0, 2.5f),
            new Vector2(0, 0)
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

    public void StartButtonDown()
    {
        state = GameState.Play;
        mainPanel.SetActive(false);
        time = 0;
        stage.lastIndex = -1;
#if UNITY_EDITOR
        StageEditor.instance?.Repaint();
#endif
    }
    void Ready()
    {

    }
    void Play()
    {
        stage.Read(
            stage.lastIndex + 1,
            ref eventIndex,
            time
        );

        if (time >= stage.timeLength)
        {
            stage.lastIndex = -1;
            time = 0;
#if UNITY_EDITOR
            StageEditor.instance?.Repaint();
#endif
            return;
        }
        
        time += deltaTime;
        timeText.text = time.ToString("F1");
#if UNITY_EDITOR
        StageEditor.instance?.Repaint();
#endif

        if (Input.GetKeyDown(KeyCode.F1)) Debug_Resistance();
        if (Input.GetKeyDown(KeyCode.F2)) Debug_LevelUp();
        if (Input.GetKeyDown(KeyCode.F3)) Debug_LevelDown();
        if (Input.GetKeyDown(KeyCode.F4)) Debug_Heal();
        if (Input.GetKeyDown(KeyCode.F5)) Debug_SpawnWeapon();
        if (Input.GetKeyDown(KeyCode.F6)) Debug_SpawnActiveItem();
        if (Input.GetKeyDown(KeyCode.F7)) Debug_TimeMove();

        if (Input.GetKeyDown(KeyCode.P)) Debug_StopToggle();
    }
    void Pause()
    {

    }

    public static void StartBoss()
    {
        Instantiate(instance.bossPrefab);
    }
    void OnValidate()
    {
        instance = this;
    }
}

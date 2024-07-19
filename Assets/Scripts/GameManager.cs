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

    public GameState state;
    public static bool IsEditor => instance.state == GameState.Editor;
    public static float deltaTime => instance.gameSpeed * Time.deltaTime;

    [SerializeField] GameObject mainPanel;
    [SerializeField] TextMeshProUGUI timeText;
    public Stage stage;

    [SerializeField] ResistanceEffect Resistance2Effect;
    public float gameSpeed = 1;
    public float time = -1;

    int eventIndex = 0;

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
                if (Input.GetKeyDown(KeyCode.F1)) Debug_Resist2();
                if (Input.GetKeyDown(KeyCode.F2)) Debug_LevelUp();
                if (Input.GetKeyDown(KeyCode.F3)) Debug_LevelDown();
                if (Input.GetKeyDown(KeyCode.F4)) Debug_Heal();
                if (Input.GetKeyDown(KeyCode.F7)) Debug_TimeMove();

                if (Input.GetKeyDown(KeyCode.P)) Debug_StopToggle();

                Play();
                break;

            case GameState.Pause:
                Pause();
                break;
        }
    }
    void Debug_Resist2()
    {
        if (Resistance2Effect.gameObject.activeInHierarchy)
        {
            Resistance2Effect.Stop();
        }
        else
        {
            Resistance2Effect.Execute(99999);
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
    }
    void Pause()
    {

    }

    public static void StartBoss()
    {
        Debug.Log("Start Boss");
    }
    void OnValidate()
    {
        instance = this;
    }
}

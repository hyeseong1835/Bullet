#if UNITY_EDITOR
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

    [SerializeField] GameObject mainPanel;
    public Stage stage;

    [SerializeField] ResistanceEffect Resistance2Effect;
    public float gameSpeed = 1;
    public float time = -1;

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
                if (gameSpeed == 0)
                {
                    state = GameState.Pause;
                    break;
                }
                Play();
                DebugInput();
                break;

            case GameState.Pause:
                if (gameSpeed != 0)
                {
                    state = GameState.Play;
                    break;
                }
                Pause();
                break;
        }
    }
    void DebugInput()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if(Resistance2Effect.gameObject.activeInHierarchy)
            {
                Resistance2Effect.Stop();
            }
            else
            {
                Resistance2Effect.Execute(99999);
            }
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if(player.level + 1 < player.levelUpExp.Length)
            {
                player.LevelUp();
            }
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (player.level - 1 >= 0)
            {
                player.LevelDown();
            }
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            player.hp = player.maxHp;
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            if (state == GameState.Play || state == GameState.Pause)
            {
                time += 1;
#if UNITY_EDITOR
                StageEditor.instance?.Repaint();
#endif
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(gameSpeed == 0)
            {
                gameSpeed = 1;
            }
            else
            {
                gameSpeed = 0;
            }
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
        stage.lastIndex = -1;
    }
    void Ready()
    {

    }
    void Play()
    {
        stage.Read(
            stage.lastIndex + 1,
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
        
        time += gameSpeed * Time.deltaTime;
#if UNITY_EDITOR
        StageEditor.instance?.Repaint();
#endif
    }
    void Pause()
    {

    }
    void OnValidate()
    {
        instance = this;
    }
}

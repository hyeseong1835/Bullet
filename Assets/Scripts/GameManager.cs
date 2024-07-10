using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum GameState
{
    Editor, Ready, Play
}
[ExecuteAlways]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState state;
    public static bool IsEditor => instance.state == GameState.Editor;

    [SerializeField] GameObject mainPanel;
    [SerializeField] Stage stage1;

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
        if (EditorApplication.isPlaying) stage1.Init();
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
        }
    }
    void Ready()
    {
        if (Input.anyKeyDown)
        {
            state = GameState.Play;

            mainPanel.SetActive(false);
            StartCoroutine(stage1.Start());
        }
    }
    void Play()
    {

    }
    void OnValidate()
    {
        instance = this;
    }
}

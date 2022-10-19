using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;
    [Header("State")]
    public GameState state;
    public enum GameState
    {
        start,
        game,
        end
    }


    void Awake()
    {
        Instance = this;
        state = GameState.start;
    }


    private void FixedUpdate()
    {
        switch (state)
        {
            case GameState.start:
                break;
            case GameState.game:
                break;
            case GameState.end:
                break;
        }
    }

    public void PlayButtonClick()
    {
        ChangeScene("Game", GameState.game);
    }

    public void ChangeScene(string name, GameState state)
    {
        StartCoroutine(TransitionToGame(name, state));
    }

    IEnumerator TransitionToGame(string name, GameState state)
    {
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(name);
        this.state = state;
    }
}

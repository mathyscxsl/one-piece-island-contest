using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Starting,
        Playing,
        Paused,
        Victory,
        GameOver
    }

    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Starting;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
    }
}
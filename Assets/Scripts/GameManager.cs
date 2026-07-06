using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public enum GameState { Starting, Playing, Paused, Victory, GameOver }

    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Starting;
    public event Action<GameState> OnStateChanged;

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

    private void Start()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
            player.OnDeath += () => SetState(GameState.GameOver);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}
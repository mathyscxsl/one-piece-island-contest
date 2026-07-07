using UnityEngine;

public class IslandManager : MonoBehaviour
{
    public static IslandManager Instance { get; private set; }

    [Header("Island")]
    [SerializeField] private IslandData currentIsland;

    public IslandData CurrentIsland => currentIsland;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

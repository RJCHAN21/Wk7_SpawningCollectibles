using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int score = 0;
    public HighScore highScoreData;

    public static bool isGameOver { get; private set; }
    public event Action OnScoreChanged;
    public event Action OnIsGameOver;

    private bool _raisedGameOver;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (highScoreData == null)
        {
            Debug.LogError("[GameManager] HighScore ScriptableObject not assigned.");
        }
        else
        {
            highScoreData.LoadHighScore();
        }
    }

    public void AddScore(int points)
    {
        score += points;
        OnScoreChanged?.Invoke();
    }

    public void TriggerGameOver()
    {
        if (_raisedGameOver) return;
        _raisedGameOver = true;

        isGameOver = true;

        highScoreData.TrySaveNewScore(score);

        Time.timeScale = 0f;

        OnScoreChanged?.Invoke();
        OnIsGameOver?.Invoke();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        score = 0;
        isGameOver = false;
        _raisedGameOver = false;

        OnScoreChanged?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("User quit the game.");
    }
}

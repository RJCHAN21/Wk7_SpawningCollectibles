using UnityEngine;

[CreateAssetMenu(fileName = "HighScore", menuName = "Scriptable Objects/HighScore")]
public class HighScore : ScriptableObject
{
    [SerializeField] private string prefsKey = "HighScore";
    public int highScore { get; private set; }

    void OnEnable() => Load();

    public void Load() => highScore = PlayerPrefs.GetInt(prefsKey, 0);

    public bool TrySaveNewScore(int newScore)
    {
        if (newScore > highScore)
        {
            highScore = newScore;
            PlayerPrefs.SetInt(prefsKey, highScore);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    public void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(prefsKey, 0);
    }

    public void ResetHighScore()
    {
        highScore = 0;
        PlayerPrefs.SetInt(prefsKey, 0);
        PlayerPrefs.Save();
    }
}

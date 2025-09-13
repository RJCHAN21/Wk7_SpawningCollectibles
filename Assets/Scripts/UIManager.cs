using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI HighScoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject resetHighScoreButton;

    private CanvasGroup _cg;
    private Coroutine _fadeRoutine;
    private Coroutine _revertTextRoutine;
    private bool _gameOverShown;

    void Awake()
    {
        if (restartButton)
        {
            var btn = restartButton.GetComponent<Button>();
            if (btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => GameManager.Instance?.RestartGame());
            }
        }
        if (quitButton)
        {
            var btn = quitButton.GetComponent<Button>();
            if (btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => GameManager.Instance?.QuitGame());
            }
        }
        if (resetHighScoreButton)
        {
            var btn = resetHighScoreButton.GetComponent<Button>();
            if (btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    var gm = GameManager.Instance;
                    if (gm != null && gm.highScoreData != null)
                    {
                        gm.highScoreData.ResetHighScore();
                        UpdateScore();
                        if (resetHighScoreButton != null)
                        {
                            var tmp = resetHighScoreButton.GetComponentInChildren<TextMeshProUGUI>();
                            if (tmp != null)
                            {
                                string originalText = tmp.text;
                                tmp.text = "High Score Reset!";

                                if (_revertTextRoutine != null) StopCoroutine(_revertTextRoutine);
                                _revertTextRoutine = StartCoroutine(RevertTextAfterDelay(tmp, originalText, 2f));
                            }
                        }
                    }
                });
            }
        }
        if (gameOverPanel)
        {
            _cg = gameOverPanel.GetComponent<CanvasGroup>();
            if (_cg == null) _cg = gameOverPanel.AddComponent<CanvasGroup>();
            _cg.alpha = 0f;
            _cg.interactable = false;
            _cg.blocksRaycasts = false;
        }
    }

    void OnEnable()
    {
        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.OnScoreChanged += UpdateScore;
            gm.OnIsGameOver += ShowGameOverScreen;
        }

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (gameOverScreen) gameOverScreen.SetActive(false);

        _gameOverShown = false;
    }

    void OnDisable()
    {
        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.OnScoreChanged -= UpdateScore;
            gm.OnIsGameOver  -= ShowGameOverScreen;
        }

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        if (_revertTextRoutine != null) StopCoroutine(_revertTextRoutine);
        _fadeRoutine = null;
        _revertTextRoutine = null;

        if (restartButton)
        {
            var btn = restartButton.GetComponent<Button>();
            if (btn) btn.onClick.RemoveAllListeners();
        }
        if (quitButton)
        {
            var btn = quitButton.GetComponent<Button>();
            if (btn) btn.onClick.RemoveAllListeners();
        }
        if (resetHighScoreButton)
        {
            var btn = resetHighScoreButton.GetComponent<Button>();
            if (btn) btn.onClick.RemoveAllListeners();
        }
    }

    void Start()
    {
        var gm = GameManager.Instance;
        if (gm != null && ScoreText != null)
            ScoreText.text = gm.score.ToString();
    }

    void OnDestroy()
    {
        OnDisable();
    }

    void UpdateScore()
    {
        var gm = GameManager.Instance;
        if (gm != null && ScoreText != null)
            ScoreText.text = gm.score.ToString();
    }

    void ShowGameOverScreen()
    {
        if (_gameOverShown) return;
        _gameOverShown = true;

        if (gameOverPanel == null)
        {
            Debug.LogError("[UIManager] gameOverPanel is not assigned.");
            return;
        }
        if (_cg == null)
        {
            _cg = gameOverPanel.GetComponent<CanvasGroup>() ?? gameOverPanel.AddComponent<CanvasGroup>();
        }

        gameOverPanel.SetActive(true);

        _cg.interactable = false;
        _cg.blocksRaycasts = true;

        int hs = GameManager.Instance?.highScoreData?.highScore ?? 0;
        if (HighScoreText != null) HighScoreText.text = $"Highest Score: {hs}";
        else Debug.LogWarning("[UIManager] HighScoreText is not assigned.");

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeCanvasGroup(_cg, 0f, 0.92f, 1f));
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        cg.alpha = from;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        cg.alpha = to;

        if (gameOverScreen) gameOverScreen.SetActive(true);
        _fadeRoutine = null;
    }

    private IEnumerator RevertTextAfterDelay(TextMeshProUGUI textMesh, string originalText, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (textMesh != null) textMesh.text = originalText;
        _revertTextRoutine = null;
    }

}

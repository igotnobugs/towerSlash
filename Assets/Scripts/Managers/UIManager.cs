using System;
using UnityEngine;

public class UIManager : Singleton<UIManager> 
{
    public TMPro.TextMeshProUGUI lowerDisplayText;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI coinText;

    [SerializeField] private PopUpScript GameOverPanel;
    [SerializeField] private PopUpScript YouWinPanel;

    private void Awake() {
        GameManager.Instance.OnAddCoin += UpdateCoinText;
        GameManager.Instance.OnAddScore += UpdateScoreText;
        GameManager.Instance.OnGameOver += ShowGameOver;
        GameManager.Instance.OnGameWin += ShowGameWin;
    }

    private void UpdateScoreText() {
        scoreText.text = "Score: " + GameManager.Instance.score.ToString();
    }

    private void UpdateCoinText() {
        coinText.text = "Coins: " + GameManager.Instance.coins.ToString();
    }

    private void ShowGameOver() {
        GameOverPanel.Show();
    }

    private void ShowGameWin() {
        YouWinPanel.Show();
    }


    public void SetLowerDisplayText(string text) {
        lowerDisplayText.text = text;
    }
}

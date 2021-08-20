using UnityEngine;

public class UIManager : MonoBehaviour 
{
    public TMPro.TextMeshProUGUI lowerDisplayText;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI coinText;

    public void SetLowerDisplayText(string text) {
        lowerDisplayText.text = text;
    }

    public void SetScoreText(int score) {
        scoreText.text = "Score: " + score.ToString();
    }

    public void SetScoreText(string score) {
        scoreText.text = "Score: " + score;
    }

    public void SetCoinText(int coin) {
        coinText.text = "Coins: " + coin.ToString();
    }

    public void SetCoinText(string coin) {
        coinText.text = "Coins: " + coin;
    }
}

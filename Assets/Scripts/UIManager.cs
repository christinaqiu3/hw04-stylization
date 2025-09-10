using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public GameObject winScreen;
    public GameObject loseScreen;

    private int score = 0;
    private float timer = 30f;
    private bool gameOver = false;
    private int winScore = 3;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreText();
        UpdateTimerText();

        if (winScreen) winScreen.SetActive(false);
        if (loseScreen) loseScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver) return;

        // Countdown timer
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            LoseGame();
        }
        UpdateTimerText();
    }
    public void AddScore(int amount)
    {
        if (gameOver) return;

        score += amount;
        UpdateScoreText();

        if (score >= winScore)
        {
            WinGame();
        }
    }

    void UpdateScoreText()
    {
        if (scoreText)
            scoreText.text = "Score: " + score + "/" + winScore;
    }

    void UpdateTimerText()
    {
        if (timerText)
            timerText.text = "Time: " + Mathf.CeilToInt(timer);
    }
    void WinGame()
    {
        gameOver = true;
        Debug.Log("You Win!");
        if (winScreen) winScreen.SetActive(true);
    }
    void LoseGame()
    {
        gameOver = true;
        Debug.Log("You Lose!");
        if (loseScreen) loseScreen.SetActive(true);
    }
}

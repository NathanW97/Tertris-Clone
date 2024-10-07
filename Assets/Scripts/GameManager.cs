using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int Difficulty { get; private set; }
    private int hiScore = 0;
    private int currentScore = 0;

    //scoring text fields
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hiScoreText;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        // Clear and set score on screen
        // score = 0;
        // scoreText.text = score.ToString();
        // hiScoreText.text = "Hi Score: " + hiScore.ToString();  
    }

    public void CalclineScore(int score)
    {
        Difficulty = 1;

        switch (score)
        {
            case 1:
                score = 40 * (Difficulty + 1);
                break;
            case 2:
                score = 100 * (Difficulty + 1);
                break;
            case 3:
                score = 300 * (Difficulty + 1);
                break;
            case 4:
                score = 1200 * (Difficulty + 1);
                break;
            default:
                score = 0;
                break;
        }
        currentScore += score;
        scoreText.text = currentScore.ToString();
        if (currentScore > hiScore)
        {
            hiScore = currentScore;
            hiScoreText.text = hiScore.ToString();
        }
    }
}

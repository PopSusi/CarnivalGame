using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTextOutput;

    [SerializeField] private GameObject endMenu;
    [SerializeField] private TextMeshProUGUI endScoreTextOutput;

    private int score = 0;

    // Start is called before the first frame update
    void Awake()
    {
        Enemy.EnemyDeath += ScoreUp;
        Player.PlayerDie += StopGame;
    }

    private void ScoreUp(){
        score++;
        scoreTextOutput.text = "Score: " + score;
    }

    private void StopGame(){
        Time.timeScale = 0f;

        endMenu.SetActive(true);
        endScoreTextOutput.text = "Score: " + score;
    }

    ~UI(){
        Enemy.EnemyDeath -= ScoreUp;
        Player.PlayerDie -= StopGame;
    }
}

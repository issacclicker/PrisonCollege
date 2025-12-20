using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSystem : MonoBehaviour
{
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject gameWinText;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI escapedGraduateStudentCounterText;

    public float startTime;
    private float currentTime;
    private bool isRunning = true;

    private int escapedGraduateStudentCounter;

    void Start()
    {
        gameOverText.SetActive(false);
        gameWinText.SetActive(false);
        currentTime = startTime;
        escapedGraduateStudentCounter = 0;
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                isRunning = false;
                GameWin();
            }

            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = "남은 시간 - " + string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void UpdateEscapeCounter()
    {
        escapedGraduateStudentCounter++;
        escapedGraduateStudentCounterText.text = "탈출한 대학원생 수 : " + escapedGraduateStudentCounter;
        if(escapedGraduateStudentCounter>=3) GameOver(); 
    }


    public void GameOver()
    {
        gameOverText.SetActive(true);
    }

    public void GameWin()
    {
        gameWinText.SetActive(true);
    }
}

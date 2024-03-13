using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager: MonoBehaviour
{
    [SerializeField] private int countdownTime = 3;
    [SerializeField] private TextMeshProUGUI countdown;
    [SerializeField] private int timeLimit = 60;
    [SerializeField] private Image timeBar;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private GameObject popupWin;
    [SerializeField] private GameObject popupFail;

    private PlayerController _player;
    private Animator _canvasAnimator;
    private static readonly int DisplayPause = Animator.StringToHash("display");

    private bool _stopTimer = true;
    private float _timeRemains;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _canvasAnimator = FindObjectOfType<Canvas>().GetComponent<Animator>();
        timer.text = FormatTime(timeLimit);
        StartCoroutine(Countdown());
    }

    private void Update()
    {
        if (_stopTimer) return;
        _timeRemains -= Time.deltaTime;
        timeBar.fillAmount = _timeRemains / timeLimit;
    }

    private IEnumerator LaunchTimer()
    {
        _timeRemains = timeLimit;
        _stopTimer = false;
        while((int) _timeRemains >= 0)
        {
            if (_stopTimer) yield break;
            timer.text = FormatTime(_timeRemains);
            yield return new WaitForSeconds(1);
        }
        StopGame(false);
    }

    private static string FormatTime(float time) => $"{(int) time / 60}:{time % 60:00}";

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator Countdown()
    {
        for (var t = countdownTime; t > 0; t--)
        {
            countdown.text = t.ToString();
            yield return new WaitForSeconds(1);
        }

        countdown.enabled = false;
        StartGame();
    }

    private void StartGame()
    {
        StartCoroutine(LaunchTimer());
        _player.canMove = true;
    }

    public void StopGame(bool win)
    {
        _stopTimer = true;
        popupWin.SetActive(win);
        popupFail.SetActive(!win);
        _canvasAnimator.SetBool(DisplayPause, true);
    }
}
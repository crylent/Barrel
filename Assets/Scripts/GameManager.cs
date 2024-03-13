using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour
{
    [SerializeField] private int countdownTime = 3;
    [SerializeField] private TextMeshProUGUI countdown;
    [SerializeField] private GameObject popupWin;
    [SerializeField] private GameObject popupFail;

    private PlayerController _player;
    private Animator _canvasAnimator;
    private static readonly int DisplayPause = Animator.StringToHash("display");

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _canvasAnimator = FindObjectOfType<Canvas>().GetComponent<Animator>();
        StartCoroutine(Countdown());
    }

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
        _player.canMove = true;
    }

    public void StopGame(bool win)
    {
        popupWin.SetActive(win);
        popupFail.SetActive(!win);
        _canvasAnimator.SetBool(DisplayPause, true);
    }
}
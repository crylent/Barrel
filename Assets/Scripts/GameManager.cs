using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    [SerializeField] private int countdownTime = 3;
    [SerializeField] private TextMeshProUGUI countdown;

    private PlayerController _player;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        StartCoroutine(Countdown());
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
}
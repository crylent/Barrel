using UnityEngine;

public class FinishTrigger: MonoBehaviour
{
    private GameManager _gameManager;
    private PlayerController _player;
    
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _player = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _player.Win();
        _gameManager.StopGame(true);
    }
}
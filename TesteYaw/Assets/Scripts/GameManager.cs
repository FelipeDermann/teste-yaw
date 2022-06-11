using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    GameStart,
    GameOver,
    GameVictory,
    GameDraw
}

public enum CurrentTurn
{
    None,
    PlayerTurn,
    EnemyTurn
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("States")]
    [SerializeField] private GameState _gameState;
    [SerializeField] private CurrentTurn _currentTurn = CurrentTurn.None;
    public CurrentTurn CurrentTurn => _currentTurn;

    private int _numberOfRounds; 
    
    [Header("Points")] 
    [SerializeField] private int _playerPoints;
    [SerializeField] private int _enemyPoints;

    public int PlayerPoints => _playerPoints;
    public int EnemyPoints => _enemyPoints;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    private void Start()
    {
        GameStart();
    }

    void GameStart()
    {
        InterfaceManager.Instance.UpdateScoreText(PlayerPrefs.GetInt("Score"));
        
        _gameState = GameState.GameStart;
        CardManager.Instance.DealCards();
    }

    public CurrentTurn GetState()
    {
        return _currentTurn;
    }

    public void EnterTurn(CurrentTurn turnToEnter)
    {
        _numberOfRounds += 1;

        if (_currentTurn == CurrentTurn.PlayerTurn && turnToEnter == CurrentTurn.EnemyTurn)
        {
            if (_playerPoints < _enemyPoints)
            {
                Defeat();
                return;
            }
        }
        if (_currentTurn == CurrentTurn.EnemyTurn && turnToEnter == CurrentTurn.PlayerTurn)
        {
            if (_enemyPoints < _playerPoints)
            {
                Victory();
                return;
            }
        }

        if (_playerPoints == _enemyPoints)
        {
            if ((CardManager.Instance.PlayerCards.Count == 0 && CardManager.Instance.EnemyCards.Count == 0))
            {
                Draw();
                return;
            }
            else
            {
                CardManager.Instance.SpawnInitialCards();
                return;
            }
        }
        
        if (turnToEnter == CurrentTurn.PlayerTurn) PlayerTurn();
        else EnemyTurn();
    }

    public void ChangePlayerPoints(int points)
    {
        _playerPoints += points;
        InterfaceManager.Instance.UpdatePlayerPoints(_playerPoints);
    }

    public void ChangeEnemyPoints(int points)
    {
        _enemyPoints += points;
        InterfaceManager.Instance.UpdateEnemyPoints(_enemyPoints);
    }

    public void SwapPoints()
    {
        int playerPoints = _playerPoints;
        int enemyPoints = _enemyPoints;

        _playerPoints = enemyPoints;
        _enemyPoints = playerPoints;
        
        InterfaceManager.Instance.UpdatePlayerPoints(_playerPoints);
        InterfaceManager.Instance.UpdateEnemyPoints(_enemyPoints);
    }

    public int GetRoundNumber()
    {
        return _numberOfRounds;
    }
    
    void PlayerTurn()
    {
        ControlsManager.Instance.ToggleInputAvailability(true);
        InterfaceManager.Instance.ShowTurnText(CurrentTurn.PlayerTurn);
        
        if (CardManager.Instance.PlayerCards.Count == 0)
        {
            EnterTurn(CurrentTurn.EnemyTurn);
            return;
        }
        
        _currentTurn = CurrentTurn.PlayerTurn;
    }

    void EnemyTurn()
    {
        ControlsManager.Instance.ToggleInputAvailability(false);
        InterfaceManager.Instance.ShowTurnText(CurrentTurn.EnemyTurn);
        
        if (CardManager.Instance.EnemyCards.Count == 0)
        {
            EnterTurn(CurrentTurn.PlayerTurn);
            return;
        }

        _currentTurn = CurrentTurn.EnemyTurn;
        AIManager.Instance.SelectCardToPlay();
    }

    void Defeat()
    {
        _gameState = GameState.GameOver;
        Debug.Log("You lost!");
        StartCoroutine(ShowEndScreen());
    }
    
    void Victory()
    {
        _gameState = GameState.GameVictory;
        Debug.Log("You win!");
        PlayerPrefs.SetInt("Score", _playerPoints);
        StartCoroutine(ShowEndScreen());
    }

    void Draw()
    {
        _gameState = GameState.GameDraw;
        Debug.Log("Draw!");
        StartCoroutine(ShowEndScreen());
    }

    IEnumerator ShowEndScreen()
    {
        ControlsManager.Instance.ToggleInputAvailability(false);
        
        yield return new WaitForSeconds(1);
        if (_gameState == GameState.GameVictory) InterfaceManager.Instance.WinScreen(); 
        if (_gameState == GameState.GameOver) InterfaceManager.Instance.LoseScreen();
        if (_gameState == GameState.GameDraw) InterfaceManager.Instance.DrawScreen();
    }
}

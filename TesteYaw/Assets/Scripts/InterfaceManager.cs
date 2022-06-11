using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance;

    private Animator anim;
    
    [SerializeField] private TextMeshProUGUI _enemyPointsText;
    [SerializeField] private TextMeshProUGUI _playerPointsText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject _rulesScreen;
    [SerializeField] private GameObject _playerPoints;
    [SerializeField] private GameObject _playerTurnTextGameObject;
    [SerializeField] private GameObject _enemyTurnTextGameObject;
    [SerializeField] private GameObject _drawingTurnTextGameObject;
    
    private readonly int Win = Animator.StringToHash("Win");
    private readonly int Lose = Animator.StringToHash("Lose");
    private readonly int Draw = Animator.StringToHash("Draw");

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        anim = GetComponent<Animator>();
    }

    public void UpdatePlayerPoints(int playerPoints)
    {
        _playerPointsText.text = playerPoints.ToString();
    }

    public void UpdateEnemyPoints(int enemyPoints)
    {
        _enemyPointsText.text = enemyPoints.ToString();
    }

    public void WinScreen()
    {
        anim.SetTrigger(Win);
    }

    public void LoseScreen()
    {
        anim.SetTrigger(Lose);
    }
    
    public void DrawScreen()
    {
        anim.SetTrigger(Draw);
    }
    
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToTitleScreen()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowPoints()
    {
        _playerPoints.SetActive(true);
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.text = score.ToString();
    }

    public void ShowRulesScreen()
    {
        _rulesScreen.SetActive(true);
        ControlsManager.Instance.ToggleInputAvailability(false);
    }
    
    public void HideRulesScreen()
    {
        _rulesScreen.SetActive(false);
        ControlsManager.Instance.ToggleInputAvailability(true);
    }

    public void ShowTurnText(CurrentTurn currentTurn)
    {
        _drawingTurnTextGameObject.SetActive(false);

        if (currentTurn == CurrentTurn.PlayerTurn)
        {
            _playerTurnTextGameObject.SetActive(true);
            _enemyTurnTextGameObject.SetActive(false);
        }
        else
        {
            _playerTurnTextGameObject.SetActive(false);
            _enemyTurnTextGameObject.SetActive(true);
        }
    }

    public void ShowDrawingText()
    {
        _playerTurnTextGameObject.SetActive(false);
        _enemyTurnTextGameObject.SetActive(false);
        _drawingTurnTextGameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public delegate void GameDelegate(); //Allows creation of certain events
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance; //Static accessibility reference - Lets others access public members within this class

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;
   // Rigidbody2D rigidBody = new Rigidbody2D();
    enum PageState
    {
        None,
        Start,
        GameOver,
        Countdown
    }
    int score = 0;
    bool gameOver = false;
    public bool GameOver {  get { return gameOver; } } //other classes can get gameOver but cannot modify
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        Instance = this;
        startPage.SetActive(true);
        
        //rigidBody = GetComponent<Rigidbody2D>(); //gets component of object
       // rigidBody.simulated = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetPageState(PageState state)
    {
        switch (state) //Different things happen according to page state
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;

        }
    }
     void OnEnable()
    {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerScored += OnPlayerScored;
        TapController.OnPlayerDied += OnPlayerDied;
    }
     void OnDisable()
    {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerScored -= OnPlayerScored;
        TapController.OnPlayerDied -= OnPlayerDied;
    }
    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted(); //event sent to TapController
        score = 0;
        gameOver = false;

    }
     void OnPlayerDied()
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore"); //Get high score
        if (score > savedScore) //check if current score is higher
        {
            PlayerPrefs.SetInt("HighScore", score);//Set new high score if true
        }
        SetPageState(PageState.GameOver);
    }
    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }
    public void ConfirmGameOver() ///When replay button is tapped
    {
        OnGameOverConfirmed(); //event sent to tapController
        scoreText.text = "0";//reset score
        SetPageState(PageState.Start); //Back to start menu
    }
    public void StartGame() //When play button is tapped
    {
        SetPageState(PageState.Countdown); //Start countdown
    }
}

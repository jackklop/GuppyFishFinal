using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{

    GameManager game; //reference to game manager
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos = new Vector3(-3, 1,0);

    Rigidbody2D rigidBody = new Rigidbody2D();
    Quaternion upRotation;
    Quaternion forwardRotation;

    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource dieAudio;
    // public Text scoreText;
    // public int score;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>(); //gets component of object
        rigidBody.simulated = false;
        upRotation = Quaternion.Euler(0, 0, -270); //up rotation for floating up
        forwardRotation = Quaternion.Euler(0, 0, -35);//down rotation for taps
        game = GameManager.Instance;

    }
    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }
    void onDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }
    void OnGameStarted()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.simulated = true;
     }
    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos; //Use local position if inside parent
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        if (game.GameOver){
            return; //No updates if gameOver
        }
        if (Input.GetMouseButtonDown(0)) //left click on mouse OR tap on phone
        {
            tapAudio.Play();
            transform.rotation = forwardRotation; //Rotates up every time it is tapped
            rigidBody.velocity = Vector3.zero; //sets gravity to 0 first
            rigidBody.AddForce(Vector2.up * tapForce, ForceMode2D.Force); //adds force up direction
        }
        //Lerp is going from source value to target value over certain amount of time
        transform.rotation = Quaternion.Lerp(transform.rotation, upRotation, tiltSmooth * Time.deltaTime); //First value is current, second is target, third is how fast
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "ScoreZone")
        {

            scoreAudio.Play();
           // scoreText.text = score.ToString();
            
            OnPlayerScored();
            //scoreSound.Play();
        }
        if (col.gameObject.tag == "DeadZone")
        {
            dieAudio.Play();
            rigidBody.simulated = false;
            OnPlayerDied();
          //  dieSound.Play();
        }
    }


}

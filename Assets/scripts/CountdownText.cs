using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour
{

    public delegate void CountdownFinished();
    public static event CountdownFinished OnCountdownFinished;

    Text countdown;

    private void OnEnable() //any time gamemanager sets countdown page to active, this will run
    {
        countdown = GetComponent<Text>();
        countdown.text = "3";
        StartCoroutine("Countdown");
    }
   
    IEnumerator Countdown() //Ability to wait
    {
        int count = 3;
        for (int i = 0; i < count; i++)
        {
            countdown.text = (count - i).ToString();
            yield return new WaitForSeconds(1);
        }
        OnCountdownFinished();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

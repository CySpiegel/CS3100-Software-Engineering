using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public Text timerText;
    public GameObject objectB;
    public GameObject objectC;
    private float startTime;
    public bool flip = true;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        timerText.text = "Hi";
    }

    // Update is called once per frame
    void Update()
    {
        
        float t = Time.time - startTime;
        t = 10 - t;
        string minutes = ((int)t / 60).ToString();
        string seconds = (t % 60).ToString("f2");
        if((t % 60) > 0)
        {
            timerText.text = seconds;
        }
        else
        {
            timerText.text = "0";
            if(flip)
            {
                StartCoroutine(objectB.GetComponent<FlipCard>().flip());
                StartCoroutine(objectC.GetComponent<FlipCard>().flip());
                flip = false;
            }

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCard : MonoBehaviour
{
    public int fps = 60;
    public float rotateDegreePerSecond = 180f;
    public bool isFaceUp = false;

    const float FLIP_LIMIT_DEGREE = 180f;
    //float LIMIT = FLIP_LIMIT_DEGREE;
    float currentRotation = 0f;

    float waitTime;
    bool isAnimationProcessing = false;
    bool COMPARE;

    void Start()
    {
        waitTime = 1.0f / fps;
        //StartCoroutine(flip());
    }

    void OnMouseDown()
    {
        if(isAnimationProcessing)
        {
            return;
        }
        StartCoroutine(flip());
    }

    public IEnumerator flip()
    {
        //isAnimationProcessing = true;

        bool done = false;
        while(!done)
        {
            float degree = rotateDegreePerSecond = Time.deltaTime * 200;
            if(isFaceUp)
            {
                degree = -degree;
            }
            transform.Rotate(new Vector3(0, degree, 0));
            if(isFaceUp)
            {
                COMPARE = (0 > (transform.eulerAngles.y + degree - 1));
            }
            else
            {
                COMPARE = (FLIP_LIMIT_DEGREE < (transform.eulerAngles.y + 1));
            }
            Debug.Log("LIMIT AND ROTATION");
            Debug.Log(transform.eulerAngles.y);
            Debug.Log(currentRotation);
            if (COMPARE)
            {
                //transform.Rotate(new Vector3(0, degree/100, 0));
                if (isFaceUp)
                    isFaceUp = false;
                else
                    isFaceUp = true;
                done = true;
            }
            //isAnimationProcessing = false;
            yield return new WaitForSeconds(waitTime);
        }
    }
}

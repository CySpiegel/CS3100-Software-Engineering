using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardText : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GetComponent<TextMesh>().text = "I just changed the text inside a C# script, and all of these newlines are automatic!";
        FormatString(GetComponent<TextMesh>().text);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FormatString(string text)
    {
        //int currentLine = 1;
        int maxLineChars = 18;
        int charCount = 0;
        string[] words = text.Split(" "[0]);
        string result = "";

        for (int index = 0; index < words.Length; index++)
        {
            string word = words[index].Trim();

            if (0 == index)
            {
                charCount += word.Length + 1;
                result = words[0];
                GetComponent<TextMesh>().text = result;
            }

            if (index > 0)
            {
                charCount += word.Length + 1;
                if (charCount <= maxLineChars)
                {
                    result += " " + word;
                }
                else
                {
                    charCount = word.Length + 1;
                    result += "\n" + word;
                }

                GetComponent<TextMesh>().text = result;
            }
        }
    }
}

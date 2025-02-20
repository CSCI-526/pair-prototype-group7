using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChipTracker : MonoBehaviour
{
    public Text t;
    public bool isPot;

    public void updateValue(int chips)
    {
        string finalText = "";
        if (isPot)
        {
            finalText = "Pot: " + chips;
        }
        else
        {
            finalText = "Money: " + chips;
        }
        t.text = finalText;
    }
}

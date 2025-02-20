using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<CardData> handData;
    public Deck deck;

    public void getStartingHand()
    {
        handData.Add(deck.DealCard());
        handData.Add(deck.DealCard());
        handData.Add(deck.DealCard());
    }

    public void showHand()
    {
        for(int i = 0; i < handData.Count; i++)
        {
            
        }
    }

    public void hideHand()
    {

    }
}

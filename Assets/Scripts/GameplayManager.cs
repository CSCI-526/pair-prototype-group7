using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    //objects that carry over from turn to turn
    public Deck deck;
    public ChipTracker pot;
    public ChipTracker money;
    public int potTotal;
    public River river;

    //p1 objects
    public Hand p1Hand;
    public int p1Chips;
    public Bank p1Bank;

    //p2 objects
    public Hand p2Hand;
    public int p2Chips;
    public Bank p2Bank;

    //p3 objects
    public Hand p3Hand;
    public int p3Chips;
    public Bank p3Bank;

    //initialize everything needed at the top of the game
    void Start()
    {
        deck.InitializeDeck();
        deck.ShuffleDeck();
        p1Hand.GetStartingHand(deck);
        p1Hand.ShowHand();
        pot.updateValue(potTotal);
        money.updateValue(p1Chips);
        river.Flop(deck);
    }

    //control handler
    void Update()
    {
        //dev debug keys - h to hide hand, s to show hand
        if (Input.GetKeyDown(KeyCode.H))
        {
            p1Hand.HideHand();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            p1Hand.ShowHand();
        }

        //add to river - r
        if (Input.GetKeyDown(KeyCode.R))
        {
            river.addToRiver(deck);
        }
    }

    //search the river and the active player's hand for a card and bank it
    public void LocateAndBank(CardData cd)
    {
        if (!river.LocateAndBank(cd))
        {
            p1Hand.LocateAndBank(cd);
        }
    }
}

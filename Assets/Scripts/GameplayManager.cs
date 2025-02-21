using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayManager : MonoBehaviour
{

    //Made GamePlayer to encapsulate the player objects in the gameplay manager
    //There is a Player class but I didn't want to break gameplaying
    public class GamePlayer
    {
        public int playerNum;
        public Hand hand;
        public int chipTotal;
        public Bank bank;

        public GamePlayer(int playerNum, Hand hand, int chipTotal, Bank bank)
        {
            this.playerNum = playerNum;
            this.hand = hand;
            this.chipTotal = chipTotal ;
            this.bank = bank;
        }
    }
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

    //Player list
    GamePlayer[] playerList = new GamePlayer[3];
    GamePlayer activePlayer;

    //UI elements
    public TMP_Text currEventText;

    //initialize everything needed at the top of the game
    void Start()
    {
        /*
        deck.InitializeDeck();
        deck.ShuffleDeck();
        p1Hand.GetStartingHand(deck);
        p1Hand.ShowHand();
        p2Hand.GetStartingHand(deck);
        pot.updateValue(potTotal);
        money.updateValue(p1Chips);
        river.Flop(deck);

        HandleBettingRound();*/
        InitializePlayers();
        InitializeGame();
        HandleBettingRound();
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

        //Debug testing setting active player
        if (Input.GetKeyDown(KeyCode.D))
        {
            int currPlayerNum = activePlayer.playerNum - 1;
            Debug.Log(currPlayerNum);
            if (currPlayerNum >= 0 && currPlayerNum < 2)
            {
                SetActivePlayer(currPlayerNum + 1);
            } else if (currPlayerNum == 2)
            {
                SetActivePlayer(0);
            }
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

    //Initialize players from the player objects given
    void InitializePlayers()
    {
        //Using GamePlayer and not Player. Can be changed but doing this to not break things that rely on player
        GamePlayer p1 = new GamePlayer(1, p1Hand, p1Chips, p1Bank);
        GamePlayer p2 = new GamePlayer(2, p2Hand, p2Chips, p2Bank);
        GamePlayer p3 = new GamePlayer(3, p3Hand, p3Chips, p3Bank);

        playerList[0] = p1;
        playerList[1] = p2;
        playerList[2] = p3;
    }

    //Init game
    void InitializeGame()
    {
        deck.InitializeDeck();
        deck.ShuffleDeck();
        //deal to the players
        for(int i = 0; i < playerList.Length; i++)
        {
            playerList[i].hand.GetStartingHand(deck);
        }

        //for now let's just start with player one as the dealer (conveniently they'll bet first since blinds have put in already)
        SetActivePlayer(0);
    }

    
    //Handle the betting round
    //Take bets from each player, add it to the pot
    //This is where the pot gets updated
    public void HandleBettingRound()
    {
        currEventText.text = "Betting Round!";
    }

    //Only happens once at the start of the game
    //Each player takes one of three cards and banks it
    public void BankingRound()
    {
        currEventText.text = "Bank a card!";
    }

    //Method to call the river's flop method
    public void DealFlop()
    {
        river.Flop(deck);
    }

    //Method to call to handle players banking from the river
    public void RiverBankingRound()
    {
        currEventText.text = "Bank a river card!";
    }

    //Each player picks cards from their bank
    //and makes a hand
    public void Withdrawal()
    {
        currEventText.text = "Take cards from your bank!";
    }

    //Helper to switch to a different player
    //Show that player's hand, bank, and current chips
    void SetActivePlayer(int playerNum)
    {
        if (activePlayer != null)
        {
            activePlayer.hand.HideHand();
        }
        activePlayer = playerList[playerNum];
        activePlayer.hand.ShowHand();
        money.updateValue(activePlayer.chipTotal);
    }
}

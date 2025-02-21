using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

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

    public enum GamePhase{
        BeforeStart,//0//everyone can see the screen //show rules
        BlindBetting,//1//if player i is dealer, player i+1 and i+2 have to bet 10
        Deal3Hands_PreFlopBetting,//2//player i+3 start betting until everyone is call or fold
        Flop3Cards_HandBanking,//3//Select banking card from player i+1 to dealer
        FlopBetting,//4//from player i+1 until everyone is call or fold
        FlopBanking,//5//from player i+1 to dealer, can use 15 money to bank a card from community
                    //if a card is bought, deal another card from the deck
        TurnBetting,//6//from player i+1 until everyone is call or fold
        TurnBanking,//7//from player i+1 to dealer, can use 20 money to bank a card from community
                    //if a card is bought, deal another card from the decFlopBetting,
        RiverBetting,//8//from player i+1 until everyone is call or fold
        RiverBanking,//9//from player i+1 to dealer, can use 25 money to bank a card from community
                    //if a card is bought, deal another card from the deck
        Withdrawal,//10//choose cards to use in this round from bank
        Showdown,//11//show the winner, all players' hands and what combination each player has
        BeforeNextround //12//everyone can see the screen
                        //put all showdown cards to deck and reshuffle
                        //give pot money to the winner
                        //show all player's remaining money on the scene
                        //check all player's money, if money == 0, game over
    }

    //objects that carry over from turn to turn
    public Deck deck;
    public ChipTracker pot;
    public ChipTracker money;
    public int potTotal;
    public River river;
    public int dealer;
    public int bet_amount;
    public int curr_phase;
    public int min_bet_amount;

    //int count;

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

    //Betting
    private bool betPlaced = false;
    private int lastBet;


    //UI elements
    public TMP_Text currEventText;
    public TMP_Text currPlayerText;
    public TMP_Text potText;
    public TMP_InputField bet_input;
    public Button next_button;
    public Button next_player_button;

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
        StartBettingRound();
        //InitializeGame();
        //HandleBettingRound();
    }

    //control handler
    void Update()
    {
        //dev debug keys - h to hide hand, s to show hand
        if (Input.GetKeyDown(KeyCode.H))
        {
            activePlayer.hand.HideHand();
            activePlayer.bank.bankText.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            activePlayer.hand.ShowHand();
            activePlayer.bank.bankText.gameObject.SetActive(true);
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
        dealer = 1;
        potTotal = 0;
        

        //buttons
        next_button.gameObject.SetActive(true);
        next_player_button.gameObject.SetActive(false);
        next_button.onClick.AddListener(() => OnButtonClick(1));
        next_player_button.onClick.AddListener(() => OnButtonClick(2));

        //pot
        potText.text = "Pot: " + potTotal;


        curr_phase = 0;
        ChangePhase(curr_phase);
    }


    void OnSubmitBet(string betAmount)
    {
        if (!string.IsNullOrEmpty(betAmount))
        {
            Debug.Log("User submitted: " + betAmount);
            lastBet = int.Parse(betAmount);
            betPlaced = true;
            bet_input.text = "";
        }
    }
    
    //Calls the coroutine
    public void StartBettingRound()
    {
        currEventText.text = "Betting Round! Input bet and press enter!";
        bet_input.onEndEdit.AddListener(OnSubmitBet);
        StartCoroutine(HandleBettingRound());  
    }
    //Handle the betting round
    //Take bets from each player, add it to the pot
    //This is where the pot gets updated
    public IEnumerator HandleBettingRound()
    {
       
        for (int i = 0; i < playerList.Length; i++)
        {
            SetActivePlayer(i);
            bet_input.text = "";
            bet_input.ActivateInputField();
            betPlaced = false;

            bool validBet = false;

            while (!validBet)
            {
                yield return new WaitUntil(() => betPlaced);

                if(lastBet <= activePlayer.chipTotal)
                {
                    validBet = true;
                } else
                {
                    Debug.Log("Invalid bet given! Bet is more than what player currently has.");
                    bet_input.text = "";
                    bet_input.ActivateInputField();
                    betPlaced = false;
                }
            }

            activePlayer.chipTotal -= lastBet;
            money.updateValue(activePlayer.chipTotal);
            potTotal += lastBet;
            pot.updateValue(potTotal);
        }

        //At this point all bets have been submitted move on to the next phase
    }

    /*
    void Betting(GamePlayer player){
        currEventText.text = "Please Bet at least " + min_bet_amount + "or fold";

    }
    */

    //Only happens once at the start of the game
    //Each player takes one of three cards and banks it
    public void BankingRound()
    {
        currEventText.text = "Bank a card!";

        if(activePlayer.playerNum == dealer){
            next_button.gameObject.SetActive(true);
            next_player_button.gameObject.SetActive(false);
        }
        else{
            next_button.gameObject.SetActive(false);
            next_player_button.gameObject.SetActive(true);
        }
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
        //loop this for all players
        int card_no;
        int.TryParse(bet_input.text, out card_no);
        WithdrawFromBankToHand(activePlayer, card_no);

        if(activePlayer.playerNum == dealer){
            next_button.gameObject.SetActive(true);
            next_player_button.gameObject.SetActive(false);
        }
        else{
            next_button.gameObject.SetActive(false);
            next_player_button.gameObject.SetActive(true);
        }

    }

    public void WithdrawFromBankToHand(GamePlayer player,int num)
    {
        CardData temp = player.bank.bankData[num];
        player.hand.handData.Add(temp);
        player.bank.bankData.RemoveAt(num);
        player.bank.UpdateBankText();
        player.hand.HideHand();
        player.hand.ShowHand();
    }

    //Helper to switch to a different player
    //Show that player's hand, bank, and current chips
    void SetActivePlayer(int playerNum)
    {
        if (activePlayer != null)
        {
            activePlayer.hand.HideHand();
            activePlayer.bank.bankText.gameObject.SetActive(false);
        }
        activePlayer = playerList[playerNum];
        activePlayer.hand.ShowHand();
        activePlayer.bank.bankText.gameObject.SetActive(true);
        money.updateValue(activePlayer.chipTotal);
        currPlayerText.text = "Player" + activePlayer.playerNum + "'s turn";
    }

    void DoBeforeStart(){
        //show information
        currEventText.text ="Player" + dealer +  " is dealer.";
    }

    void DoBlindBetting(){
        Debug.Log("DoBlindBetting");
        currEventText.text ="Player" + activePlayer.playerNum +  " blind bet 10";
        //Debug.Log("curr_event");
        GamePlayer blind1 = playerList[activePlayer.playerNum-1];
        blind1.chipTotal = blind1.chipTotal - 10;

        //put into pot
        potTotal += 10;
        potText.text = "Pot: " + potTotal;
        money.updateValue(activePlayer.chipTotal);

        if(activePlayer.playerNum == dealer+2){
            next_button.gameObject.SetActive(true);
            next_player_button.gameObject.SetActive(false);
        }
        else{
            next_button.gameObject.SetActive(false);
            next_player_button.gameObject.SetActive(true);
        }
        //next_button.gameObject.SetActive(true);
        //Debug.Log("Deck found with " + deck.deck.Count + " cards.");
    }

    public void DoDeal3Hands_PreFlopBetting(){
        currEventText.text = "Press S to see your hands. Press H to hide hands.";

        //PreFlopBetting
        //betting = true;
        //Betting();
        
        
        next_button.gameObject.SetActive(true);
    }

    void DoFlop3Cards_HandBanking(){
        next_button.gameObject.SetActive(true);
    }

    void DoFlopBetting(){
        //curr_player = curr_dealer+1;
        //Betting();
        next_button.gameObject.SetActive(true);
    }

    void DoFlopBanking(){
        next_button.gameObject.SetActive(true);
    }

    void DoTurnBetting(){
        //curr_player = curr_dealer+1;
        //Betting();
        next_button.gameObject.SetActive(true);
    }

    void DoTurnBanking(){
        next_button.gameObject.SetActive(true);
    }

    void DoRiverBetting(){
        //curr_player = curr_dealer+1;
        //Betting();
        next_button.gameObject.SetActive(true);
    }

    void DoRiverBanking(){
        next_button.gameObject.SetActive(true);
    }

    void DoWithdrawal(){
        next_button.gameObject.SetActive(true);
    }

    void DoShowdown(){
        next_button.gameObject.SetActive(true);
    }

    void DoBeforeNextround(){
        next_button.gameObject.SetActive(true);
        
        curr_phase = 0;
        ChangePhase(curr_phase);
    }

    void OnButtonClick(int buttonID)
    {   
        switch (buttonID)
        {
            case 1:
                Debug.Log("next_button clicked!");
                if (activePlayer.playerNum == 3){
                    SetActivePlayer(0);
                }
                else{
                    SetActivePlayer(activePlayer.playerNum);
                }
                curr_phase++;
                Debug.Log("curr_phase: " + curr_phase);
                Debug.Log("player "+ activePlayer.playerNum);
                //next_button.gameObject.SetActive(false);
                ChangePhase(curr_phase);
                break;
            case 2:
                Debug.Log("next_player_button clicked!");
                Debug.Log("curr_phase: " + curr_phase);
                Debug.Log("player "+ activePlayer.playerNum);
                if (activePlayer.playerNum == 3){
                    SetActivePlayer(0);
                }
                else{
                    SetActivePlayer(activePlayer.playerNum);
                }
                //next_button.gameObject.SetActive(false);
                ChangePhase(curr_phase);
                break;
            default:
                Debug.Log("Unknown button clicked!");
                break;
        }
    }

    void ChangePhase(int curr_event){
        if(curr_event == (int)GamePhase.BeforeStart){
            DoBeforeStart();
        }
        else if(curr_event == (int)GamePhase.BlindBetting){
            DoBlindBetting();
        }
        else if(curr_event == (int)GamePhase.Deal3Hands_PreFlopBetting){
            //DoDeal3Hands_PreFlopBetting();
            BankingRound();
        }
        else if(curr_event == (int)GamePhase.Flop3Cards_HandBanking){
            //DoFlop3Cards_HandBanking();
            Withdrawal();
        }
        else if(curr_event == (int)GamePhase.FlopBetting){
            DoFlopBetting();
        }
        else if(curr_event == (int)GamePhase.FlopBanking){
            DoFlopBanking();
        }
        else if(curr_event == (int)GamePhase.TurnBetting){
            DoTurnBetting();
        }
        else if(curr_event == (int)GamePhase.TurnBanking){
            DoTurnBanking();
        }
        else if(curr_event == (int)GamePhase.RiverBetting){
            DoRiverBetting();
        }
        else if(curr_event == (int)GamePhase.RiverBanking){
            DoRiverBanking();
        }
        else if(curr_event == (int)GamePhase.Withdrawal){
            DoWithdrawal();
        }
        else if(curr_event == (int)GamePhase.Showdown){
            DoShowdown();
        }
        else if(curr_event == (int)GamePhase.BeforeNextround){
            DoBeforeNextround();
        }
    }
}

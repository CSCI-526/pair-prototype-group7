using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine.XR;

public class GamePlaying : MonoBehaviour
{
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

    
    public Button next_button;//1
    public Button skip_button;//2
    public int player_num;
    public int curr_player;
    public int curr_event;
    public int curr_dealer;
    public List<Player> players = new List<Player>();
    public TMP_InputField bet_input;
    public bool betting;

    //Displays
    public TMP_Text text_money;
    public TMP_Text text_name;
    public TMP_Text text_curent_event;

    //objects that carry over from turn to turn
    public Deck deck;

    // Start is called before the first frame update
    void Start()
    {
        //players initialization
        player_num = 3;
        for(int i = 0; i < player_num; i++){
            Player p = new Player(player_num+1);
            if(i == 0){
                p.dealer = true;
            }
            players.Add(p);
        }
        curr_dealer = 0;
        curr_player = (curr_dealer + 3) % player_num;
 
        //buttons
        next_button.gameObject.SetActive(false);
        skip_button.gameObject.SetActive(false);
        next_button.onClick.AddListener(() => OnButtonClick(1));
        skip_button.onClick.AddListener(() => OnButtonClick(2));

        //deck
        deck = FindObjectOfType<Deck>();
        if (deck != null){
            Debug.Log("Deck found with " + deck.deck.Count + " cards.");
        }
        else{
            Debug.LogError("Deck not found in the scene!");
        }

        //event
        curr_event = 0;
        ChangePhase(curr_event);

        //initialize the game components
    }

    void DoBeforeStart(){
        //show information
        
        //show next button
        next_button.gameObject.SetActive(true);
    }

    void DoBlindBetting(){
        //Debug.Log("DoBlindBetting");
        //Debug.Log("curr_event");
        Player blind1 = players[curr_dealer+1];
        Player blind2 = players[curr_dealer+2];
        blind1.player_money = blind1.player_money - 10;
        blind2.player_money = blind2.player_money - 10;
        //put into pot
        //????????
        //show blind betting

        next_button.gameObject.SetActive(true);
        Debug.Log("Deck found with " + deck.deck.Count + " cards.");
    }

    public void DoDeal3Hands_PreFlopBetting(){
        //Deal 3 hands to each player
        for(int i = 0; i < player_num; i++){
            for(int j = 0; j < 3; j++){
                CardData tempCardData = deck.DealCard();
                //add to hands
                players[i].hands.Add(tempCardData);
            }
            //Debug.Log(players[i].hands.Count);
        }
        
        //PreFlopBetting
        betting = true;
        Betting();
        
        
        next_button.gameObject.SetActive(true);
    }

    void DoFlop3Cards_HandBanking(){
        next_button.gameObject.SetActive(true);
    }

    void DoFlopBetting(){
        curr_player = curr_dealer+1;
        Betting();
        next_button.gameObject.SetActive(true);
    }

    void DoFlopBanking(){
        next_button.gameObject.SetActive(true);
    }

    void DoTurnBetting(){
        curr_player = curr_dealer+1;
        Betting();
        next_button.gameObject.SetActive(true);
    }

    void DoTurnBanking(){
        next_button.gameObject.SetActive(true);
    }

    void DoRiverBetting(){
        curr_player = curr_dealer+1;
        Betting();
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
        curr_event = 0;
    }

    void OnButtonClick(int buttonID)
    {
        switch (buttonID)
        {
            case 1:
                Debug.Log("next_button clicked!");
                curr_event++;
                Debug.Log(curr_event);
                next_button.gameObject.SetActive(false);
                ChangePhase(curr_event);
                break;
            case 2:
                Debug.Log("skip_button clicked!");
                curr_event++;
                next_button.gameObject.SetActive(false);
                ChangePhase(curr_event);
                break;
            default:
                Debug.Log("Unknown button clicked!");
                break;
        }
    }

    void Betting(){
        /*
        while(betting){
            Player p = players[curr_player];
            //show handcards
            //?????
            int bet_amount;
            if (int.TryParse(bet_input.text, out bet_amount)){
                Debug.Log("Converted Number: " + bet_amount);
            }
            else{   
                Debug.LogError("Invalid input! Please enter a number.");
            }
            p.Bet(bet_amount);
            
        }
        */
    }

    void Changeplayer(){
        curr_player = (curr_player + 1) % player_num;

        Player p = players[curr_player];
        text_money.text = "Money: " + p.player_money;
        text_name.text = "Player" + p.player_no;
        //text_curent_event.text = curr_event;
    }

    void ChangePhase(int curr_event){
        if(curr_event == (int)GamePhase.BeforeStart){
            DoBeforeStart();
        }
        else if(curr_event == (int)GamePhase.BlindBetting){
            DoBlindBetting();
        }
        else if(curr_event == (int)GamePhase.Deal3Hands_PreFlopBetting){
            DoDeal3Hands_PreFlopBetting();

        }
        else if(curr_event == (int)GamePhase.Flop3Cards_HandBanking){
            DoFlop3Cards_HandBanking();
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


    // Update is called once per frame
    void Update()
    {
        
    }
}

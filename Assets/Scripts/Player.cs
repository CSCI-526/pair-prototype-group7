using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player //: MonoBehaviour
{
    public int player_no;
    public int player_money = 100;
    public string curent_event = "Game Start!";
    public bool dealer = false;
    public List<CardData> hands = new List<CardData>();
    public List<CardData> banks = new List<CardData>();

    //Displays
    public TMP_Text text_money;
    public TMP_Text text_title;
    public TMP_Text text_curent_event;

    public Player(int player_no){
        this.player_no = player_no;
    }
    
 
    // Start is called before the first frame update
    void Start()
    {
        Initialize(player_no);
    }

    public void Initialize(int player_no){
        text_money.text = "Money: " + player_money;
        text_title.text = "Player" + player_no;
        text_curent_event.text = curent_event;
    }

    public void Bet(int money_reduce){

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

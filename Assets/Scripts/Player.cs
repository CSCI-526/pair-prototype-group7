using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player //: MonoBehaviour
{
    public int player_no;
    public int player_money = 100;
    //public string curent_event = "Game Start!";
    public bool dealer = false;
    public List<CardData> hands = new List<CardData>();
    public List<CardData> banks = new List<CardData>();


    public Player(int player_no){
        this.player_no = player_no;
    }
    
 
    // Start is called before the first frame update
    void Start()
    {
        //Initialize(player_no);
    }

    public void Initialize(int player_no){
        
    }

    public void Bet(int money_reduce){

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

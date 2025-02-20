using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public class CardData
    {
        //this class is just for storing values which we'll use to 
        //instantiate multiple cards from the card prefab
        public Card.CardValue cardValue;
        public Card.CardSuit cardSuit;
        public Texture2D texture;

        public CardData(Card.CardValue value, Card.CardSuit suit, Texture2D texture)
        {
            this.cardValue = value;
            this.cardSuit = suit;
            this.texture = texture;
        }

    }

    public GameObject cardPrefab;

    //Our deck is just a list of card data, when we want a card, we pop and instantiate a card
    public List<CardData> deck = new List<CardData>();
    public Texture2D[] cardTextures;

    // Start is called before the first frame update
    void Start()
    {
        InitializeDeck();
        //PrintDeck();
        ShuffleDeck();
        //PrintDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Populate the deck of cards represented as a stack, needs shuffling
    void InitializeDeck()
    {
        //For right now deck consists of 5 suits, cards numbered 1-5
        for(int i = 0; i < cardTextures.Length; i++)
        {
            //right now deck creation depends on the filename
            CardData cardData = ParseTextureFileName(cardTextures[i]);
            deck.Add(cardData);  
        }
        //Debug.Log(deck.Count);
    }

    //Shuffle the deck using Fischer-Yates
    void ShuffleDeck()
    {
        System.Random rand = new System.Random();
        int i = deck.Count - 1;
        while (i > 0)
        {
            int randomNumber = rand.Next(0, i + 1);
            CardData temp = deck[randomNumber];
            deck[randomNumber] = deck[i];
            deck[i] = temp;
            i--;
        }
    }

    //Just a bunch of debug.logs to see the cards
    void PrintDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Debug.Log("Card value: " + deck[i].cardValue +
                " Card suit: " + deck[i].cardSuit);

        }
    }


    //Using the name of the texture, generate a CardData object
    //Filename has to be in the format "1_spades.png" otherwise this will break
    //Suit is case insensitive
    CardData ParseTextureFileName(Texture2D texture)
    {
        //Using the name of the texture, create card data
        string fileName = texture.name;
        string[] parts = fileName.Split('_');

        //Should be only 2 parts to a filename separated by "_"
        if(parts.Length != 2 )
        {
            Debug.Log("Invalid texture filename, cannot create card");
            return null;
        }

        //Check parts[0] which is the number of the card
        int value;
        int.TryParse(parts[0], out value);
        if(value < 1 || value > 5)
        {
            Debug.Log("Invalid card value in texture filename, cannot create card");
            return null;
        }

        Card.CardSuit suit;
        if(!System.Enum.TryParse(parts[1], true, out suit))
        {
            Debug.Log(parts[1]);
            Debug.Log("Invalid suit in texture filename, cannot create card");
            return null;
        }

        return new CardData((Card.CardValue)value, suit, texture);
    }
}

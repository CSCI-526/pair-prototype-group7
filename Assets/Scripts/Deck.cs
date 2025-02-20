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
    public Card[] deck;
    public Texture2D[] cardTextures;

    // Start is called before the first frame update
    void Start()
    {
        InitializeDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitializeDeck()
    {
        //For right now deck consists of 5 suits, cards numbered 1-5
        for(int i = 0; i < cardTextures.Length; i++)
        {
            //right now deck creation depends on the filename
            ParseTextureFileName(cardTextures[i]);
        }
    }
    CardData ParseTextureFileName(Texture2D texture)
    {
        //Using the name of the texture, create card data
        string fileName = texture.name;
        Debug.Log(fileName);
        return null;
    }
}

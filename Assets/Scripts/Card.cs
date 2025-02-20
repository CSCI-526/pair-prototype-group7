using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardValue
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5
    }

    public enum CardSuit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades,
        Stars
    }

    public CardValue cardValue;
    public CardSuit cardSuit;
    public SpriteRenderer spriteRenderer;
    public Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {
        Initialize(CardValue.One, CardSuit.Hearts);
    }

    public void Initialize(CardValue cV, CardSuit cS)
    {
        this.cardValue = cV;
        this.cardSuit = cS;

        if (spriteRenderer != null && texture != null)
        {
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height)
                , new Vector2(0.5f, 0.5f));
        }

        spriteRenderer.sortingOrder = 10; 

        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        float cardWidth = transform.localScale.x;
        float cardHeight = transform.localScale.y;

        float scaleX = cardWidth / spriteWidth;
        float scaleY = cardHeight / spriteHeight;
        float finalScale = Mathf.Min(scaleX, scaleY);
 
        spriteRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

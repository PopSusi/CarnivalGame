using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float speed;
    private SpriteRenderer sprite;
    private int health;

    // Start is called before the first frame update
    void Awake()
    { 
      sprite = GetComponent<SpriteRenderer>();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetSettings(SO_EnemyTypes type){
        speed = type.speed;
        sprite.sprite = type.sprite;
        health = type.health;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public delegate void EnemyAnnouncements();
    public static event EnemyAnnouncements EnemyDeath;

    private float speed;
    private SpriteRenderer sprite;
    private int health;

    [SerializeField] private float leftSide;
    [SerializeField] private float rightSide;
    [SerializeField] private int downstepInterval;
    private bool rightMovement;
    // Start is called before the first frame update
    void Awake()
    { 
      sprite = GetComponent<SpriteRenderer>();  
    }

    // Update is called once per frame
    void Update()
    {
      int movementDirectionModifier;
        if(rightMovement){
          movementDirectionModifier = 1;
        } else {
          movementDirectionModifier = -1;
        }
        transform.position += Time.deltaTime * speed * new Vector3(movementDirectionModifier, 0, 0);

        if(transform.position.x >= rightSide || transform.position.x <= leftSide){
          rightMovement = !rightMovement;
          transform.position += new Vector3(0, downstepInterval, 0);
        }
    }
    
    public void SetSettings(SO_EnemyTypes type){
        speed = type.speed;
        sprite.sprite = type.sprite;
        health = type.health;
    }

    void OnCollisionEnter2D(Collision2D collision){
      if(collision.gameObject.CompareTag("Bullet")) TakeDamage();
    }

    void TakeDamage(){
      health -= 1;
      if(health <= 0){
        EnemyDeath();
        Destroy(gameObject);
      }
    }
}

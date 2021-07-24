using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlimeEnemy : MonoBehaviour
{

    [SerializeField]
    Rigidbody2D rb2dPlayer;

    [SerializeField]
    Transform player;

    [SerializeField]
    Transform leftWalkBound;
    float leftBound;

    [SerializeField]
    Transform rightWalkBound;
    float rightBound;

    [SerializeField]
    float agroRange = 4;

    [SerializeField]
    float moveSpeed = 1;

    [SerializeField]
    int health = 4;
    int direction = 1;
    float changeDirTime = 3;

    int agroLevel = 1;//same as sakeLevel

    Rigidbody2D rb2d;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        leftBound = leftWalkBound.position.x;
        rightBound = rightWalkBound.position.x;
        //Debug.Log(rightBound);
    }

    // Update is called once per frame
    void Update()
    {
        if(agroLevel != GlobalVars.SakeLevel)
        {
            updateSlime();//change speed according to level
        }
        //distance to player
        if (health == 4)//only move if full health
        {
            float playerDist = Vector2.Distance(transform.position, player.position);

            if (playerDist <= agroRange)
            {
                //code to chase player
                ChasePlayer();
            }
            else
            {
                //stop chasing
                StopChase();
                Patrol();
            }

        }
        else
        {
            rb2d.velocity = new Vector2(0, 0);
        }

        UpdateAnimations();

        UpdateHealth();
        
    }

    private void ChasePlayer()
    {
        if((transform.position.x < player.position.x) && (rb2d.position.x < rightBound) && (Math.Abs(player.position.y - transform.position.y) <= 5))
        {
            //move right
            rb2d.velocity = new Vector2(moveSpeed*1.5f, 0);
            direction = 1;
        }
        else if(transform.position.x > player.position.x && (rb2d.position.x > leftBound))
        {
            //move left
            rb2d.velocity = new Vector2(-moveSpeed*1.5f, 0);
            direction = -1;
        }
    }

    private void StopChase()
    {
        rb2d.velocity = new Vector2(0, 0);
    }

    private void UpdateAnimations()
    {
        if (health == 4)
        {
            //transform.localScale = new Vector(-1, 1);reflect enemy
            if (rb2d.velocity.x > 0)
            {
                animator.Play("iceSlime_move_right");
            }
            else if (rb2d.velocity.x < 0)
            {
                animator.Play("iceSlime_move_left");
            }
            else if(rb2d.velocity.x == 0)
            {
                animator.Play("iceSlime_idle");
            }
        }
        
        if (health == 3)
        {
            animator.Play("iceSlime_hit");
        }
        else if (health == 2)
        {
            animator.Play("iceSlime_hit2");
        }

        if (health == 1)
        {
            //destroy enemy
            Destroy(gameObject);
        }

    }

    private void UpdateHealth()
    {
        //player collision from top = subtract 1 hp + bounce player away
    }

    private void Patrol()
    {
        if(direction == 1)//move towards right bound
        {
            if(rb2d.position.x < rightBound)
            {
                rb2d.velocity = new Vector2(moveSpeed, 0);
            }
            else//reached end 
            {
                Invoke("ChangeDirection", changeDirTime);
            }
        }
        else if(direction == -1) { //move towards left bound
            if (rb2d.position.x > leftBound)
            {
                rb2d.velocity = new Vector2(-moveSpeed, 0);
            }
            else//reached end 
            {
                Invoke("ChangeDirection", changeDirTime);
            }
        }
    }

    //adjust stats based on current sakeLevel
    private void updateSlime()
    {
       if(agroLevel < GlobalVars.SakeLevel)//sakeLevel incremented
        {
            moveSpeed += 3;
            agroLevel++;
        }
        else if(agroLevel > GlobalVars.SakeLevel)//sakeLevel decremented
        {
            moveSpeed -= 3;
            agroLevel--;
        }
    }

    //change direction during patrol (invoked)
    private void ChangeDirection()
    {
        if (rb2d.velocity.x == 0)
        {
            direction *= -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("crystalProjectile"))
        {
            health--;
        }
        else if (collision.CompareTag("Player"))
        {
            health--;
            //rb2dPlayer.velocity = new Vector2(rb2d.velocity.x, -10);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy_Slime")//change direction if hitting another slime
        {
            direction *= -1;
            
            rb2d.velocity = new Vector2(moveSpeed * direction, 0);
        }
    }
}

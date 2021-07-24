using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Enemy script template for enemies
 * Enemy specific code can be found in their respective files
 * Includes code for moving between two points that are decided based on starting location
 * Enemy will follow player until reaching one of these endpoints
 */
public class BasicEnemyScript : MonoBehaviour
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
    public float moveSpeed = 1;
    [SerializeField]
    public float moveSpeedY = 0;

    [SerializeField]
    public int health = 2;
    public int direction = 1;
    [SerializeField]
    float changeDirTime = 3;
    bool isGrounded = true;
    int agroLevel = 1;//same as sakeLevel
    [SerializeField]
    public bool player_can_hit = true;//whether player can cause the enemy to take damage by a collision
    public bool reap = true;//whether the object should be destroyed when 0 health

    Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {

        rb2d = GetComponent<Rigidbody2D>();
        leftBound = leftWalkBound.position.x;
        rightBound = rightWalkBound.position.x;
        //Debug.Log(rightBound);
    }

    // Update is called once per frame
    void Update()
    {
       
        if (agroLevel != GlobalVars.SakeLevel)
        {
            updateEnemy();//change speed according to level
        }
        //distance to player
        if (health != 0)
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

        if (health <= 0 && reap)//less than to avoid bugs?
        {
            //destroy enemy
            Debug.Log("reap");
            Destroy(gameObject);
        }
    }

    //follow player while the player is within agro range
    private void ChasePlayer()
    {
        if ((transform.position.x < player.position.x) && (rb2d.position.x < rightBound))
        {
            //move right
            if (isGrounded)
            {
                //Debug.Log("jump");
                rb2d.velocity = new Vector2(moveSpeed * 1.5f, moveSpeedY);
                isGrounded = false;
            }
            else
            {
                rb2d.velocity = new Vector2(moveSpeed * 1.5f, rb2d.velocity.y);
            }
            direction = 1;
        }
        else if (transform.position.x > player.position.x && (rb2d.position.x > leftBound))
        {
            //move left
            if (isGrounded)
            {
                //Debug.Log("jump");
                rb2d.velocity = new Vector2(-moveSpeed * 1.5f, moveSpeedY);
                isGrounded = false;
            }
            else
            {
                rb2d.velocity = new Vector2(-moveSpeed * 1.5f, rb2d.velocity.y);
            }
            direction = -1;
        }
        
    }

    //return to original movement
    private void StopChase()
    {
        if (isGrounded)
        {
            rb2d.velocity = new Vector2(0, 0);
        }
    }

    //move back and forth when the player is not nearby
    private void Patrol()
    {
        if (direction == 1)//move towards right bound
        {
            if (rb2d.position.x < rightBound)
            {
                if (isGrounded)
                {
                    //Debug.Log("jump");
                    rb2d.velocity = new Vector2(moveSpeed, moveSpeedY);
                    isGrounded = false;
                }
                else
                {
                    rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
                }
            }
            else//reached end 
            {
                Invoke("ChangeDirection", changeDirTime);
            }
        }
        else if (direction == -1)
        { //move towards left bound
            if (rb2d.position.x > leftBound)
            {
                if (isGrounded)
                {
                    //Debug.Log("jump");
                    rb2d.velocity = new Vector2(-moveSpeed, moveSpeedY);
                    isGrounded = false;
                }
                else
                {
                    rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
                }
            }
            else//reached end 
            {
                Invoke("ChangeDirection", changeDirTime);
            }
        }
    }

    //adjust stats based on current sakeLevel
    private void updateEnemy()
    {
        if (agroLevel < GlobalVars.SakeLevel)//sakeLevel incremented
        {
            moveSpeed += 3;
            agroLevel++;
        }
        else if (agroLevel > GlobalVars.SakeLevel)//sakeLevel decremented
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
        if (collision.gameObject.tag == "ground"){
            isGrounded = true;
            //Debug.Log("grounded");
        }
        //Debug.Log("ground collision");
        if (collision.gameObject.CompareTag("crystalProjectile"))
        {
            health--;
            //Debug.Log("HIT");
        }
        else if (collision.gameObject.CompareTag("Player") && player_can_hit)
        {
            health--;
            //Debug.Log("HIT");
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController2D : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb2d;
    SpriteRenderer spriteRenderer;
    bool firing = false;
    bool isGrounded;
    bool canJump = false;//make sure player is on ground(avoid climbing walls)
    bool inputEnabled = true;

    private float runSpeed = 5;
    private float jumpSpeed = 15;
    private int direction = 1;//start out right
    private int lastDirection = 1;//for firing
    private float lastPos;
    private int jumps = 2;//allow double jump
    private int knockBackSpeed = 10;

    private float fireDelay = 1;
    private float chargeTime = 0;
    [SerializeField]
    GameObject crystalProjectileObject;
    
    [SerializeField]
    GameObject sleepImage;//play when player is idle for more than 3s
    private bool isSleeping;
    private int sleepTime = 3;//sleep after 3 seconds
    private float idleTime = 0;//how long player has been idle
    private bool isIdle = false;//whether or not the player is already idle
    SpriteRenderer sleepSprite;
    Animator sleepAnim;
    private int blockedX = 0;//whether the player can move (wall collision) (-1 left, 1 right, 0 not blocked)

    private int sakeCount = 0;
    private int sakeBoost = 100;//threshold for next boost
    //int sakeLevel = 1;//current movement multiplier
    public int playerHealth = 5;//

    [SerializeField]//make availible in unity
    Transform groundCheck;//used to detect ground
    [SerializeField]
    Transform groundCheckR;
    [SerializeField]
    Transform groundCheckL;
    [SerializeField]
    Transform BulletSpawnPos;
    [SerializeField]
    Transform BulletSpawnPosL;

    //variable for dealing with wall collisions (avoid sticking to wall)
    [SerializeField]
    Tilemap wallTiles;
    public ContactPoint2D[] contacts = new ContactPoint2D[10];//list of tiles the player has collided with
    [SerializeField]
    Transform Wall_pos;
    [SerializeField]
    Transform head;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPos = rb2d.position.x;
        sleepSprite = sleepImage.GetComponent<SpriteRenderer>();
        sleepSprite.enabled = false;
        isSleeping = false;
        sleepAnim = sleepImage.GetComponent<Animator>();
    }

    void Update()
    {
        checkSake();
        checkGround();
        //Debug.Log(lastDirection);
        //Add separate script for firing projectiles
        if (Input.GetKey("f") && isGrounded && inputEnabled)
        {
            //reset sleep parameters
            isIdle = false;
            isSleeping = false;
            idleTime = 0;
            StopSleep();
            

            if (!firing)
            {
                chargeTime = Time.time;//start time check
            }
            //Debug.Log(chargeTime);
          
            firing = true;

            if (direction == 1 || (direction == 0 && lastDirection == 1))
            {
                animator.Play("Lamy_skill_left");

            }
            else
            {
                animator.Play("Lamy_skill_right");
            }

            //shoot projectile
            //Invoke("Fire", fireDelay);call function after fireDelay time (does NOT work with firing (will fire 2 projectiles sometimes))
            if ((Time.time - chargeTime) >= fireDelay)//fire after fireDelay time has passed
            {
                Fire();
            }
            
        }
        else
        {
            firing = false;
        }
        //Debug.Log(sakeCount);
    }

    
    void Fire()
    {
        if (firing)
        {
            //Debug.Log(lastDirection);
            GameObject newBullet = Instantiate(crystalProjectileObject);
            newBullet.GetComponent<crystalProjectile>().StartShoot(lastDirection);
            Rigidbody2D rb2dBullet = newBullet.GetComponent<Rigidbody2D>();
            if (lastDirection == 1)
            {
                newBullet.transform.position = BulletSpawnPos.transform.position;
            }
            else
            {
                newBullet.transform.position = BulletSpawnPosL.transform.position;
            }
            
            firing = false;
            chargeTime = Time.time;
        }
    }
    void ResetShoot()//reset after firing
    {
        firing = false;
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        //Debug.Log(blockedX);
        //Debug.Log(Time.time - idleTime);
        if (!firing && inputEnabled)//player movement
        {
            CheckSleeping();
            if((rb2d.velocity.x == 0 && rb2d.velocity.y == 0) && !isIdle)//player is not moving or firing a projectile
            {
                idleTime = Time.time;
                isIdle = true;
            }
            else if((rb2d.velocity.x != 0 || rb2d.velocity.y != 0))
            {
                isIdle = false;
                isSleeping = false;
                idleTime = 0;
            }

            //Debug.Log(isSleeping);
            if ((Input.GetKey("d") || Input.GetKey("right")))
            {
                if (blockedX != 1)//cannot move right due to a wall
                {
                    //Debug.Log("Moving right");
                    rb2d.velocity = new Vector2(runSpeed, rb2d.velocity.y);
                    if (isGrounded)
                    {
                        animator.Play("Lamy_left_move");
                    }
                    setDirection(1);
                    blockedX = 0;
                }
                else
                {
                    animator.Play("Lamy_Idle");
                }
            }
            else if (Input.GetKey("a") || Input.GetKey("left"))
            {
                if (blockedX != -1)//cannot move left due to a wall
                {
                    rb2d.velocity = new Vector2(-runSpeed, rb2d.velocity.y);
                    if (isGrounded)
                    {
                        animator.Play("Lamy_right_move");
                    }
                    setDirection(-1);
                    blockedX = 0;
                }
                else
                {
                    animator.Play("Lamy_Idle");
                }
            }
            else
            {
                rb2d.velocity = new Vector2(0, rb2d.velocity.y);
                setDirection(0);
                if (!isSleeping)
                {
                    animator.Play("Lamy_Idle");
                }
                else
                {
                    animator.Play("Lamy_idle_sleep");
                }
                
            }
        }

        //jump
        if (Input.GetKey("space") && ((isGrounded && !firing && canJump) || (jumps != 0 && rb2d.velocity.y < 0)))
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
            //Debug.Log(rb2d.velocity.x);
            jumps--;
            
            playJump();
        }
        //Debug.Log(jumps);
        
        
        lastPos = rb2d.position.x;

    }

    private void CheckGrounded()
    {
        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("ground")))//use raycasting to check if player is on ground
        {
            isGrounded = true;
            canJump = true;
        }
        else if (Physics2D.Linecast(transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("ground")) ||
                Physics2D.Linecast(transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("ground")))
        {
            isGrounded = true;
            canJump = false;//not on flat ground
            
        }
        else
        {
            playJump();
            isGrounded = false;
        }
        
    }

    //enable sleep animation after 3 seconds of being idle
    private void CheckSleeping()
    {
        if (isIdle)//only deal with if idle
        {
            if ((Time.time - idleTime) >= sleepTime)
            {
                isSleeping = true;
            }
            else
            {
                isSleeping = false;
            }
        }
        else
        {
            isSleeping = false;
            sleepSprite.enabled = false;
        }

        if (!isSleeping)
        {
            StopSleep();
        }
        else
        {
            sleepSprite.enabled = true;
            sleepAnim.enabled = true;
            sleepAnim.Play("asleep_sleeping");
        }

    }

    //reset animation and stop it from playing
    private void StopSleep()
    {
        sleepSprite.enabled = false;
        if (sleepAnim.enabled == true)
        {
            sleepAnim.Play("asleep_sleeping", 0, 0);//reset frame before disabling animation
        }
        sleepAnim.enabled = false;
    }

    //set direction for firing
    private void setDirection(int newDir)
    {
        if (direction != 0)
        {
            lastDirection = direction;
        }
        direction = newDir;
    }

    //check whether player can jump and play animation if able to
    private void checkGround()
    {
        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("ground")))
        {
            isGrounded = true;
            canJump = true;
            jumps = 2;//reset double jump
        }
        else if (Physics2D.Linecast(transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("ground")) ||
                Physics2D.Linecast(transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("ground")))
        {
            isGrounded = true;
            canJump = false;//not on flat ground
        }
        else
        {
            playJump();
            isGrounded = false;
        }
    }

    //adjust parameters based on amount of sake collected
    private void checkSake()
    {
        if(sakeCount >= sakeBoost)
        {
            UpgradeStats();
            
            sakeBoost += 100;//set new threshold
            GlobalVars.SakeLevel++;
            Debug.Log("Sake threshold reached");
        }
    }

    //improve player movement and skill casting after obtaining enough sake
    private void UpgradeStats()
    {
        //move faster
        runSpeed+= 2;
        jumpSpeed+= 2;
        //fire faster
        if (fireDelay != 0)
        {
            fireDelay *= .5f;
        }
        animator.speed *= 1.5f;
    }

    private void playJump()
    {
        //choose which animation to play based on direction
        if (rb2d.velocity.x != 0)
        {
            if (direction == 1)
            {
                animator.Play("Lamy_jump_right");

            }
            else
            {
                animator.Play("lamy_jump_left");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //bounce off of slime
        if (collision.CompareTag("Enemy_Slime") || collision.CompareTag("Enemy_Snowman"))
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
        }
        else if (collision.CompareTag("Sake"))
        {
            sakeCount++;
        }
    }

    /// <summary>
    /// FIX PLAYER IS BEING KNOCKED BACK ON FLAT GROUND
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy_Slime" || collision.gameObject.tag == "Enemy_Snowman") {
            if (groundCheck.position.y <= collision.transform.position.y)//knock back the player if the collision is not a jump attack
            {
                playerHealth--;
                Debug.Log(playerHealth);
                rb2d.velocity = new Vector2(-1 * direction * knockBackSpeed, rb2d.velocity.y + 5);
                DisableInput(1);//disable input for 1 second
            }
        }
        else if (collision.gameObject.tag == "ground")
        {
            //only do if player is not on top of wall
            if (CheckWallCondtion(collision))
            {

                Debug.Log("wall collision");

                DisableInput(0.25f);
                rb2d.velocity = new Vector2(-3 * direction, rb2d.velocity.y);
                animator.Play("Lamy_Idle");
                if (collision.transform.position.x < groundCheck.position.x)//left side collision
                {
                    blockedX = -1;
                }
                else
                {
                    blockedX = 1;
                }
            }
        }
    }

    //allow player to move normally
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            blockedX = 0;
        }
    }

    //check tilemaps to see if player is above all current collisions
    bool CheckWallCondtion(Collision2D collision)
    {
        int contactNum = collision.contactCount;//number of collisions
        if (contactNum > contacts.Length)//increase array size if necessary
        {
            contacts = new ContactPoint2D[contactNum];
        }
        collision.GetContacts(contacts);//get list of collisions

        UnityEngine.Vector3 hitPosition = UnityEngine.Vector3.zero;

        //iterate through each colliding tile and destroy it
        for (int i = 0; i < contactNum; i++)
        {
            hitPosition.y = contacts[i].point.y;//get collision y
            if (Wall_pos.position.y < hitPosition.y && head.position.y > hitPosition.y)//avoid getting stuck after hitting ceiling
            {
                //Debug.Log("Wall Collision");
                return true;//player is next to tile
            }
             
        }
        return false;
    }


    //prevent user from moving for a fixed amount of time
    private void DisableInput(float time)
    {
        inputEnabled = false;
        Invoke("EnableInput", time);
    }

    private void EnableInput()
    {
        inputEnabled = true;
    }

    public int getSakeCount()
    {
        return sakeCount;
    }
}

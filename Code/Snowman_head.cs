using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//will be created when a snowman is hit the first time with velocity away from the player
public class Snowman_head : MonoBehaviour
{
    [SerializeField]
    float speed = 1;

    
    public void StartFlying(int direction)
    {
      
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed * direction, 1);
       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy_Snowman"))//dont destroy if colliding with self
        {
            
            if (gameObject != null)
            {
                Destroy(gameObject, 3);
            }
        }
    }
}

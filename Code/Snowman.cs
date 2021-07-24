using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Snowman enemy 
 * Upon being hit, send head flying and increase movement speed (create two separate objects as a result and destroy this object)
 */
public class Snowman : MonoBehaviour
{
    Animator animator;
    BasicEnemyScript SnowmanScript;//movement and health information
    bool hit = false;

    //snowman head
    [SerializeField]
    GameObject head;

    [SerializeField]
    GameObject body;

    [SerializeField]
    Transform head_spawn;

    GameObject snow_head;
    GameObject snow_body;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        SnowmanScript = GetComponent<BasicEnemyScript>();
        SnowmanScript.player_can_hit = false;//player can't damage snowman directly
       
    }

    // Update is called once per frame
    void Update()
    {
        //change to no head and send head flying
        //Debug.Log(SnowmanScript.health);
        if (SnowmanScript.health == 1 && !hit)
        {
            SnowmanScript.reap = false;
            //Debug.Log("Spawn snowman");
            animator.Play("Snowman_broken");
            //create snowman head
            snow_head = Instantiate(head);
            if (snow_head != null)
            {
                snow_head.GetComponent<Snowman_head>().StartFlying(SnowmanScript.direction * -1);
                snow_head.transform.position = head_spawn.transform.position;

            }
           
            hit = true;
            float delay = .5F;
            Destroy(gameObject, delay);
        }
    }
}

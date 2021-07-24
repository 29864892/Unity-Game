using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crystalProjectile : MonoBehaviour
{
    [SerializeField]
    float speed = 10;

    [SerializeField]
    int damage;

    [SerializeField]
    float destroyTime = 3;//3 seconds
    public void StartShoot(int direction)
    {
        float speedIncrease = 0;
        if (GlobalVars.SakeLevel > 1) {//increase speed based on level
            speedIncrease = GlobalVars.SakeLevel;
        }
        GetComponent<Rigidbody2D>().velocity = new Vector2((speed + speedIncrease) * direction, 0);

        Destroy(gameObject, destroyTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*Code to destroy a tile that the player steps on*/
public class destroyable_tile : MonoBehaviour
{
    //[SerializeField]
    //int destroy_delay = 1;

    public Tilemap tilemap;//tilemap containing destructible tiles

    public ContactPoint2D[] contacts = new ContactPoint2D[10];//list of tiles the player has collided with

    // Start is called before the first frame update
    void Start()
    {
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();//use current gameObject if not assigned
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            
            //Debug.Log("Player collision");

            int contactNum = collision.contactCount;//number of collisions
            if(contactNum > contacts.Length)//increase array size if necessary
            {
                contacts = new ContactPoint2D[contactNum];
            }
            collision.GetContacts(contacts);//get list of collisions

            UnityEngine.Vector3 hitPosition = UnityEngine.Vector3.zero;
            
            //iterate through each colliding tile and destroy it
            for (int i = 0; i < contactNum; i++)
            {
                
                hitPosition.x = contacts[i].point.x;
                hitPosition.y = contacts[i].point.y-1;//-1 to destroy tile under player

                tilemap.SetTile(tilemap.WorldToCell(hitPosition), null);

                UnityEngine.Vector3 temp = hitPosition;
                temp.x = hitPosition.x - 1;
                tilemap.SetTile(tilemap.WorldToCell(hitPosition), null);

                hitPosition.x = contacts[i].point.x + 1;
                tilemap.SetTile(tilemap.WorldToCell(hitPosition), null);
            }
        }
    }
}

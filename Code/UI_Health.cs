using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//always add for UI things

public class UI_Health : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    public Text healthText;
    PlayerController2D playerScript;
    string currHealth = "";
    // Start is called before the first frame update
    void Start()
    {
        //get player script info
        playerScript = player.GetComponent<PlayerController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        currHealth = playerScript.playerHealth.ToString();
        Debug.Log(currHealth);
        healthText.text = "Health : " + currHealth + "";

    }
}

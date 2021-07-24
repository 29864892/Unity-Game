using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//always add for UI things


public class UI_Sake : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    public Text SakeText;
    PlayerController2D playerScript;
    string currSake = "";

    // Start is called before the first frame update
    void Start()
    {
        //get player script info
        playerScript = player.GetComponent<PlayerController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        currSake = playerScript.getSakeCount().ToString();
        Debug.Log(currSake);
        SakeText.text = "Score: " + currSake;
    }
}

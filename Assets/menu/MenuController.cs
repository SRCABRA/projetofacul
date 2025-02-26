using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour

{
    public GameObject MenuEscolhas;
    public GameObject MenuTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MenuEscolhas.SetActive(false); // Hide the menu at the start
        MenuTrigger.SetActive(true); // Show the trigger at the start
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            MenuEscolhas.SetActive(true); // Show the menu when any key is pressed
            MenuTrigger.SetActive(false); // Hide the trigger when any key is pressed

        }
    }
}

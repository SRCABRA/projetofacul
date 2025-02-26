using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropedPizzaController : MonoBehaviour
{
    public float timer = 1f;
    public GameObject pizzacollectible;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timer <= 0)
        {
            Instantiate(pizzacollectible, transform.position, transform.rotation); // instancia a pizza coletável
            Destroy(gameObject); // destroi a pizza dropada
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}

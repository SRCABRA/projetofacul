using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropedPizzaController : MonoBehaviour
{
    public GameObject pizzacollectible;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision) // Changed method name to OnCollisionEnter
    {
        if(collision.gameObject.CompareTag("chao")){ // se colidir com algo que não seja o player ou inimigo
            Instantiate(pizzacollectible, transform.position, transform.rotation); // instancia a pizza coletável
            Destroy(gameObject); // destroi a pizza dropada
        }
    }
}

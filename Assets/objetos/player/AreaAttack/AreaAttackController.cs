using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttackController : MonoBehaviour
{
    public float timerDuration = 0.5f;
    private float timer = 0.0f;

    void Start()
    {
        timer = timerDuration;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime; // decrementa o tempo restante
        
        if (timer <= 0.0f) // verifica se o tempo acabou
        {
            Destroy(gameObject); // destroi o objeto
        }
    }
}
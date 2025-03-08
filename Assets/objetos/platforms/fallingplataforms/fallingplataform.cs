using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingplataform : MonoBehaviour
{
    public float delayTime = 2f; // Tempo de delay antes de desativar o isKinematic

    private Rigidbody rb;
    private BoxCollider boxCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DisableKinematicAndActivateTriggerAfterDelay());
        }
    }

    IEnumerator DisableKinematicAndActivateTriggerAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }
    }

    void Update()
    {
        // ...existing code...
    }
}
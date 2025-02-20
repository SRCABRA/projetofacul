using UnityEngine;

public class Destruction : MonoBehaviour {

    public GameObject fractured;
    public float delay = 3.0f;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou na trigger possui o componente PlayerController
        if (other.gameObject.name == "Player")
        {
            triggered = true;
            Debug.Log("Player detectado via OnTriggerEnter. Timer iniciado.");
            Invoke(nameof(BreakTheThing), delay);
        }
    }

    private void BreakTheThing()
    {
        Debug.Log("Delay concluído. Quebrando a plataforma.");
        Instantiate(fractured, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}

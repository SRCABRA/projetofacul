using UnityEngine;

public class WaterController : MonoBehaviour
{
    public float speed = 0.01f;

    void Start()
    {
        // Certifica-se de que o objeto tem um Rigidbody configurado corretamente
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false; // Para evitar que a água caia
            rb.isKinematic = true; // Para não ser afetado por física
        }
    }

    void Update()
    {
        transform.position += new Vector3(0f, speed, 0f);
        speed += 0.000001f;
    }

    void OnTriggerEnter(Collider other)
    {

        // se tocar no player ou no inimigo, destrói o inimigo ou o player
        if (other.CompareTag("Player") || other.CompareTag("Enemy")){
            Destroy(other.gameObject);
        }
    }
}

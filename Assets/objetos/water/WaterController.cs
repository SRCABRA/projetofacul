using UnityEngine;

public class WaterController : MonoBehaviour
{
    public float speed = 0.04f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0f, speed, 0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<PlayerController1>().gravity = 0f;
            Destroy(collision.gameObject);
            Debug.Log("Player morreu");
        }
    }
}

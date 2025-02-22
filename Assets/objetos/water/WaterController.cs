using UnityEngine;

public class WaterController : MonoBehaviour
{
    public float speed = 0.01f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0f, speed, 0f);
        speed = speed += 0.000001f;
    }

    void OnCollisionEnter(Collision collision)
    {
        
            Destroy(collision.gameObject);
            Debug.Log(collision.gameObject.name + "foi destruido");
        
    }
}

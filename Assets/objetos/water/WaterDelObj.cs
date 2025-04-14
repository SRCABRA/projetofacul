using UnityEngine;

public class WaterDelObj : MonoBehaviour
{
    public GameObject predio;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}

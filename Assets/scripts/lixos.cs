using UnityEditor.Callbacks;
using UnityEngine;

public class lixos : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if(collision.gameObject.CompareTag("water")){
            Debug.Log("toquei na água");
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}

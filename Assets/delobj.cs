using UnityEngine;
using UnityEngine.SceneManagement;

public class delObj : MonoBehaviour
{
    public GameObject player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class delObj : MonoBehaviour
{
    public GameObject player;
    public float speedX = 100, speedY = 23, speedZ= 3;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         transform.Rotate(new Vector3(1f * speedX, 10 * speedY, 2f * speedZ), Space.Self);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

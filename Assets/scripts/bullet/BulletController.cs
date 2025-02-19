using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject enemy;
    public float speed = 10.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enemy != null){
            Vector3 direction = (enemy.transform.position - transform.position).normalized;

            transform.position += direction * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
        }

    }
}
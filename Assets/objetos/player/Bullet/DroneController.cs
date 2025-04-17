using UnityEngine;

public class DroneController : MonoBehaviour
{
    
    public GameObject StayHereDrone;
    public float speed;
    public float maxSpeed;
    public float acceleration = 0.5f;
    void Start()
    {
        
    }


    void Update()
    {
        DroneMovement();

        if(speed < maxSpeed){
            speed += acceleration * Time.deltaTime;
            speed = Mathf.Min(speed, maxSpeed);
        }
    }

    void DroneMovement(){
        transform.position = Vector3.Lerp(transform.position, StayHereDrone.transform.position, speed * Time.deltaTime);
    }


}

using UnityEngine;

public class droneactivatorcontroller : MonoBehaviour
{

    public GameObject drone;
    public GameObject player;
    private bool isactive;
    void Start()
    {
        isactive = false;
        drone.transform.SetParent(player.transform);
    }


    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")){
            isactive = true;
            drone.transform.parent = null;
            drone.SetActive(isactive);


        }
    }
}

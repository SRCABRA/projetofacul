using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour{

public Transform target; // The target object the camera will orbit around

  public float distance = 10.0f; // Distance from the target object

  public float rotationSpeed = 20.0f; // Speed of rotation around the target

  public float yMinLimit = -20f; // Minimum vertical angle

  public float yMaxLimit = 80f; // Maximum vertical angle



  private float x = 0.0f; // Horizontal rotation angle

  private float y = 20.0f; // Vertical rotation angle (initial value)



  void Start()

  {

    // Make sure the target is set

    if (!target)

    {

      Debug.LogError("Target not assigned!");

      return;

    }



    // Initialize angles

    Vector3 angles = transform.eulerAngles;

    x = angles.y;

    y = angles.x;



    // Set the initial position of the camera

    UpdateCameraPosition();

  }



  void Update()

  {

    // Automatically rotate the camera around the target

    x += rotationSpeed * Time.deltaTime;



    // Optionally, add vertical oscillation if desired

    // y = Mathf.PingPong(Time.time * rotationSpeed, yMaxLimit - yMinLimit) + yMinLimit;



    // Clamp vertical rotation

    y = Mathf.Clamp(y, yMinLimit, yMaxLimit);



    // Rotate the camera around the target

    Quaternion rotation = Quaternion.Euler(y, x, 0);

    transform.rotation = rotation;



    // Update the camera position

    UpdateCameraPosition();

  }



  void UpdateCameraPosition()

  {

    // Set the camera position behind the target

    Vector3 position = target.position - transform.forward * distance;

    transform.position = position;
    }
}

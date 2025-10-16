using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public float speed = 12f;
    public float turnSpeed = 180f;
    private Rigidbody rb; //Reference to rigid body component
    private float movementInputValue; //Value for moving up and down
    private float turnInputValue; //Value for turning left and right

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; //Make sure rigid body is not stationary
        rb.angularDamping = 0f; //No angular drag
    }
    void FixedUpdate()
    {
        movementInputValue = Input.GetAxis("Vertical");
        turnInputValue = Input.GetAxis("Horizontal");
        Move(); //handle forward movement
        Turn(); //handle turning
    }
    private void Move()
    {
        Vector3 movement = transform.forward * movementInputValue * speed * Time.deltaTime; //Calculate movement
        rb.MovePosition(rb.position + movement); //Apply movement to rigid body
    }

    private void Turn()
    {
        float turn = turnInputValue * turnSpeed * Time.deltaTime; //Calculate turning
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f); //Create a rotation based on turning
        rb.MoveRotation(rb.rotation * turnRotation); //Apply rotation to rigid body
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Joystick joystick;
    public float speed = 2f;
    public float maxVelocityChange = 4f;
    private Vector3 velocityVector = Vector3.zero; //initial velocity
    private Rigidbody rb;
    public float tiltAmount = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float xInput = joystick.Horizontal;
        float zInput = joystick.Vertical;

        Vector3 horizontal = transform.right * xInput;
        Vector3 vertical = transform.forward * zInput;

        Vector3 movementVelocityVector = (horizontal + vertical).normalized * speed;

        velocityVector = movementVelocityVector;

        transform.rotation = Quaternion.Euler(joystick.Vertical * speed * tiltAmount, 0, -1 * joystick.Horizontal * speed * tiltAmount);
    }


    private void FixedUpdate()
    {
        if (velocityVector != Vector3.zero)
        {
            print("dd");
            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = velocityVector - velocity;

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0f;
            rb.AddForce(velocityChange, ForceMode.Acceleration);
        }

;
    }
}

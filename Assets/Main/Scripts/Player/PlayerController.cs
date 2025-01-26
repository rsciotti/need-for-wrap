using UnityEngine;
using UnityEngine.Events;

public class PlayerController : BaseVehicle
{
    float x;
    float y = 1;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
    }

    protected override void FixedUpdate()
    {
        if (Input.GetJoystickNames().Length > 0 && x != 0 || y != 0)
        {
            // Handle your controller-specific logic here
            // Move the object based on controller input
            Move(x, y);
        //Debug.DrawLine(rb.position, rb.GetRelativePoint(relativeForce), Color.green, 2, false);
        }
    }
}

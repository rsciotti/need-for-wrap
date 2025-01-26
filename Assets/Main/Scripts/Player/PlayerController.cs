using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseVehicle
{
    float x;
    float y = 1;

    // Update is called once per frame

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        x = input.x;
        y = input.y;
    }

    protected override void FixedUpdate()
    {
        if (Input.GetJoystickNames().Length > 0 && x != 0 || y != 0)
        {
            Move(x, y);
        }
    }
}

using UnityEngine;

public class AICarController : BaseVehicle
{
    // Update is called once per frame
    protected override void FixedUpdate()
    {
        Move(1, 1);
    }
}

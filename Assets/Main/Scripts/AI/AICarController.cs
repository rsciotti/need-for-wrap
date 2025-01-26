using UnityEngine;

public class AICarController : PlayerController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move(1, 1);
    }
}

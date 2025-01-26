using UnityEngine;

public class TrailDisabler : MonoBehaviour
{
    private TrailRenderer[] _trailRenderers;
    private Rigidbody2D _rb;
    
    void Start()
    {
        _trailRenderers = gameObject.GetComponentsInChildren<TrailRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        if (Vector2.Dot(_rb.linearVelocity, transform.up) > -.5)
        {
            foreach (TrailRenderer tr in _trailRenderers)
            {
                tr.enabled = true;
            }
        }
        else
        {
            foreach (TrailRenderer tr in _trailRenderers)
            {
                tr.enabled = false;
            }
        }
    }
}

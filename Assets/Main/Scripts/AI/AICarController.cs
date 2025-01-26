using System;
using UnityEngine;

public class AICarController : BaseVehicle
{
    public enum State
    {
        Idle,
        Attacking,
        GettingPickup
    }

    private State _currentState;
    private GameObject _targetPlayer;
    private GameObject _pickup;

    protected override void Start()
    {
        base.Start();
        _currentState = State.Idle;
    }

    protected override void FixedUpdate()
    {
        UpdateNearbyObjects();

        switch (_currentState)
        {
            case State.Attacking:
                MoveTowards(_targetPlayer.transform.position);
                break;
            case State.GettingPickup:
                MoveTowards(_pickup.transform.position);
                break;
            case State.Idle:
            default:
                Move(.7f, 1);
                break;
        }
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        float angleRad = Mathf.Clamp(Mathf.Atan2(direction.y, direction.x), -1, 1);
        Move(angleRad, 1);
    }


    private GameObject[] GetNearbyObjects()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10f);
        GameObject[] nearbyObjects = new GameObject[colliders.Length];

        int index = 0;
        foreach (var collider in colliders)
        {
            nearbyObjects[index] = collider.gameObject;
            index++;
        }

        return nearbyObjects;
    }

    private void UpdateNearbyObjects()
    {
        GameObject[] nearbyObjects = GetNearbyObjects();

        _targetPlayer = null;
        _pickup = null;

        foreach (var obj in nearbyObjects)
        {
            if (obj.CompareTag("Player"))
            {
                _targetPlayer = obj;
            }
            
            if (obj.CompareTag("Pickup"))
            {
                _pickup = obj;
            }
        }

        if (_targetPlayer)
        {
            _currentState = State.Attacking;
        } else if (_pickup)
        {
            _currentState = State.GettingPickup;
        }
        else
        {
            _currentState = State.Idle;
        }
    }
}

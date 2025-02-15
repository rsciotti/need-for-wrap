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

    private bool selected = false;
    private GameObject targetObj;

    public Sprite outlineSprite;
    public Sprite targetSprite;
    private GameObject outlineObj;
    private SpriteRenderer rend1;
    private SpriteRenderer rend2;
    private SpriteRenderer rend3;

    protected override void Start()
    {
        base.Start();
        _currentState = State.Idle;

        targetObj = new GameObject("targetObj");
        targetObj.transform.localScale = Vector3.one * 5f;
        targetObj.transform.SetParent(transform);
        targetObj.transform.localPosition = Vector3.zero;

        outlineObj = new GameObject("outlineObj");
        outlineObj.transform.localScale = transform.localScale;
        rend1 = GetComponent<SpriteRenderer>();
        rend2 = outlineObj.AddComponent<SpriteRenderer>();
        rend2.transform.SetParent(transform);
        rend2.transform.localPosition = Vector3.zero;
        rend2.sprite = outlineSprite;
        rend2.enabled = false;
        rend2.sortingLayerName = "Cars";
        rend2.sortingOrder = rend1.sortingOrder + 1;
        rend3 = targetObj.AddComponent<SpriteRenderer>();
        rend3.transform.position = transform.position;
        rend3.transform.rotation = Quaternion.identity;
        rend3.sprite = targetSprite;
        rend3.enabled = false;
        rend3.sortingLayerName = "Cars";
        rend3.sortingOrder = rend1.sortingOrder + 2;
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

        rend3.transform.position = targetObj.transform.position;
        rend3.transform.rotation = Quaternion.identity;
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
            targetObj.transform.SetParent(_targetPlayer.transform);
            targetObj.transform.localPosition = Vector3.zero;
            rend3.enabled = selected;
        } else if (_pickup)
        {
            _currentState = State.GettingPickup;
            targetObj.transform.SetParent(null);
            targetObj.transform.localPosition = Vector3.zero;
            targetObj.transform.position = _pickup.transform.position;
            rend3.enabled = selected;
        }
        else
        {
            _currentState = State.Idle;
            targetObj.transform.SetParent(transform);
            targetObj.transform.localPosition = Vector3.zero;
            rend3.enabled = false;
        }
    }

    public void selectOn()
    {
        selected = true;
        rend2.enabled = true;
        rend3.enabled = _currentState != State.Idle;
    }

    public void selectOff()
    {
        selected = false;
        rend2.enabled = false;
        rend3.enabled = false;
    }

    protected void OnDisable()
    {
        selectOff();
    }
}

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

    private GameObject targetObj;
    private GameObject futureObj;
    
    private bool selected = false;
    public Sprite outlineSprite;
    public Sprite crosshairSprite;
    public Sprite crosshairSpriteRed;
    public Sprite crosshairSpriteGreen;
    public Sprite crosshairSpriteBlue;
    private GameObject outlineObj;
    private SpriteRenderer rend1;
    private SpriteRenderer rend2;
    private SpriteRenderer rend3;
    private SpriteRenderer rend4;

    protected override void Start()
    {
        base.Start();
        _currentState = State.Idle;

        targetObj = new GameObject("targetObj");
        targetObj.transform.localScale = Vector3.one * 5f;
        targetObj.transform.SetParent(transform);
        targetObj.transform.localPosition = Vector3.zero;
        futureObj = new GameObject("futureObj");
        futureObj.transform.localScale = Vector3.one * 5f;
        futureObj.transform.localPosition = Vector3.zero;
        futureObj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);

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
        rend3.sprite = crosshairSprite;
        rend3.enabled = false;
        rend3.sortingLayerName = "Cars";
        rend3.sortingOrder = rend1.sortingOrder + 2;
        rend4 = futureObj.AddComponent<SpriteRenderer>();
        rend4.transform.position = transform.position;
        rend4.transform.rotation = Quaternion.identity;
        rend4.sprite = crosshairSpriteBlue;
        rend4.enabled = false;
        rend4.sortingLayerName = "Cars";
        rend4.sortingOrder = rend1.sortingOrder + 3;
    }

    protected override void FixedUpdate()
    {
        UpdateNearbyObjects();

        futureObj.transform.SetPositionAndRotation(transform.position + (Vector3)rb.linearVelocity, Quaternion.identity);

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
                Move(0.7f, 1f);
                break;
        }

        rend3.transform.position = targetObj.transform.position;
        rend3.transform.rotation = Quaternion.identity;
        rend4.transform.position = futureObj.transform.position;
        rend4.transform.rotation = Quaternion.identity;
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 targetDir = (target - (Vector2)transform.position);
        float targetDistance = targetDir.magnitude;
        float targetLocalAng = Mathf.Atan2(targetDir.y, targetDir.x) - ((transform.rotation.z + 90f) % 180f) * Mathf.Deg2Rad;
        Vector2 predictDir = (Vector2)(futureObj.transform.position - transform.position);
        float predictDistance = predictDir.magnitude;
        float predictLocalAng = Mathf.Atan2(predictDir.y, predictDir.x) - ((transform.rotation.z + 90f) % 180f) * Mathf.Deg2Rad;
        float moveX = 0.7f;
        float moveY = 1f;
        if (Mathf.Abs(targetLocalAng - predictLocalAng) > Mathf.PI / 36f)
        {
            if (targetLocalAng >= 0f && targetLocalAng < Mathf.PI / 2f)
            {
                moveX = Mathf.Clamp(Mathf.Sin(targetLocalAng) * -2f, -0.75f, -0.5f);
                moveY = Mathf.Clamp(Mathf.Sin(Mathf.Abs(predictLocalAng - targetLocalAng) / 2f) * 2f, 0.5f, 1f);
            }
            else if (targetLocalAng > Mathf.PI / -2f)
            {
                moveX = Mathf.Clamp(Mathf.Sin(targetLocalAng) * -2f, 0.5f, 0.75f);
                moveY = Mathf.Clamp(Mathf.Sin(Mathf.Abs(predictLocalAng - targetLocalAng) / 2f) * 2f, 0.5f, 1f);
            }
            else if (targetLocalAng > 0f)
            {
                if (_currentState == State.Attacking)
                {
                    moveX = Mathf.Clamp(Mathf.Sin(Mathf.PI - targetLocalAng) * -2f, -0.75f, -0.5f);
                    moveY = 0.5f;
                }
                else
                {
                    moveX = Mathf.Clamp(Mathf.Sin(Mathf.PI - targetLocalAng) * -2f, -0.75f, -0.5f);
                    moveY = Mathf.Clamp(Mathf.Sin(Mathf.Abs(predictLocalAng - targetLocalAng) / 2f) * -2f, -1f, -0.5f);
                }
            }
            else
            {
                if (_currentState == State.Attacking)
                {
                    moveX = Mathf.Clamp(Mathf.Sin(Mathf.PI + targetLocalAng) * 2f, 0.5f, 0.75f);
                    moveY = 0.5f;
                }
                else
                {
                    moveX = Mathf.Clamp(Mathf.Sin(Mathf.PI + targetLocalAng) * 2f, 0.5f, 0.75f);
                    moveY = Mathf.Clamp(Mathf.Sin(Mathf.Abs(predictLocalAng - targetLocalAng) / 2f) * -2f, -1f, -0.5f);
                }
            }
        }
        else if (targetDistance <= predictDistance)
        {
            moveX = 0f;
            if (_currentState == State.Attacking) moveY = 1f;
            else moveY = 0f;
        }
        else if (Mathf.Abs(targetLocalAng) <= Mathf.PI / 180f)
        {
            moveX = 0f;
            moveY = 1f;
        }
        else if (Mathf.PI - Mathf.Abs(targetLocalAng) <= Mathf.PI / 180f)
        {
            if (_currentState == State.Attacking)
            {
                moveX = 1f;
                moveY = 1f;
            }
            else
            {
                moveX = 0f;
                moveY = -1f;
            }
        }
        else
        {
            if (_currentState == State.Attacking)
            {
                moveX = Mathf.Sign(Mathf.Sign(targetLocalAng) + 0.5f) * -0.1f;
                if (targetDistance >= 20f) moveY = -1f;
                else moveY = 1f;
            }
            else
            {
                moveX = Mathf.Sign(Mathf.Sign(targetLocalAng) + 0.5f) * -0.5f;
                moveY = Mathf.Sign(Mathf.Sign(targetLocalAng) + 0.5f) * 0.5f;
            }
        }
        Move(moveX, moveY);
    }


    private GameObject[] GetNearbyObjects()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25f);
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
        rend4.enabled = true;
    }

    public void selectOff()
    {
        selected = false;
        rend2.enabled = false;
        rend3.enabled = false;
        rend4.enabled = false;
    }

    protected void OnDisable()
    {
        selectOff();
    }
}

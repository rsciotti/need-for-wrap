using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AICarController : BaseVehicle
{
    public enum State
    {
        Idle,
        Attacking,
        GettingPickup,
        AvoidTennis
    }

    private State _currentState;
    private GameObject _targetPlayer;
    private GameObject _pickup;
    private GameObject _tennisBall;

    private GameObject targetObj;
    private GameObject futureObj;
    private int updateCounter;
    private int wiggleFlipper;
    private float idleMoveX;

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
        updateCounter = 0;
        wiggleFlipper = 10;
        idleMoveX = UnityEngine.Random.Range(0.6f, 0.8f) * Mathf.Sign(Mathf.Sign(UnityEngine.Random.Range(-1f, 1f)) + 0.5f);

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
        futureObj.transform.SetPositionAndRotation(transform.position + (Vector3)rb.linearVelocity, Quaternion.identity);

        UpdateNearbyObjects();

        switch (_currentState)
        {
            case State.Attacking:
                MoveTowards(_targetPlayer.transform.position);
                break;
            case State.GettingPickup:
                MoveTowards(_pickup.transform.position);
                break;
            case State.AvoidTennis:
                MoveTowards(targetObj.transform.position);
                break;
            case State.Idle:
            default:
                Move(idleMoveX, 1f);
                break;
        }

        updateCounter++;
        if (updateCounter == Math.Abs(wiggleFlipper))
        {
            wiggleFlipper = -wiggleFlipper;
            updateCounter = 0;
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
        float targetLocalAng = Mathf.Atan2(targetDir.y, targetDir.x) -
                               Mathf.Atan2(Mathf.Sin((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad),
                                           Mathf.Cos((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad));
        Vector2 predictDir = (Vector2)(futureObj.transform.position - transform.position);
        float predictDistance = predictDir.magnitude;
        float predictLocalAng = Mathf.Atan2(predictDir.y, predictDir.x) -
                                Mathf.Atan2(Mathf.Sin((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad),
                                            Mathf.Cos((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad));
        float moveX = idleMoveX;
        float moveY = 1f;
        if (Mathf.Abs(targetLocalAng) <= Mathf.PI / 36f)
        {
            moveX = Mathf.Sin(targetLocalAng) * -3f;
            moveY = 1f;
        }
        else if (Mathf.PI - Mathf.Abs(targetLocalAng) <= Mathf.PI / 36f)
        {
            if (_currentState == State.Attacking)
            {
                moveX = (float)Math.Sign(wiggleFlipper);
                moveY = 1f;
            }
            else
            {
                moveX = Mathf.Sin(targetLocalAng) * 3f;
                moveY = -1f;
            }
        }
        else if (Mathf.Abs(targetLocalAng - predictLocalAng) <= Mathf.PI / 36f && targetDistance <= predictDistance)
        {
            if (_currentState == State.Attacking)
            {
                if (Mathf.PI - Mathf.Abs(targetLocalAng) <= Mathf.PI / 180f)
                {
                    moveX = (float)Math.Sign(wiggleFlipper);
                    moveY = 1f;
                }
                else
                {
                    moveX = 0f;
                    moveY = 1f;
                }
            }
            else
            {
                moveX = 0f;
                moveY = 0f;
            }
        }
        else
        {
            if (predictDistance >= 2.5f)
            {
                if (Mathf.Abs(predictLocalAng) <= Mathf.PI / 18f)
                {
                    moveX = 0f;
                    moveY = -1f;
                }
                else if (Mathf.PI - Mathf.Abs(predictLocalAng) <= Mathf.PI / 18f)
                {
                    moveX = 0f;
                    moveY = 1f;
                }
                else
                {
                    moveX = -Mathf.Sign(Mathf.Sign(targetLocalAng) + 0.5f);
                    moveY = (float)Math.Sign(wiggleFlipper);
                }
            }
            else
            {
                moveX = -Mathf.Sign(Mathf.Sign(targetLocalAng) + 0.5f);
                moveY = (float)Math.Sign(wiggleFlipper);
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
        Vector2 tennisDir = Vector2.zero;
        float tennisDistance = 0f;
        float tennisLocalAng = 0f;
        Vector2 predictDir = Vector2.zero;
        float predictDistance = 0f;
        float predictLocalAng = 0f;
        Vector2 targetDir = Vector2.zero;
        float targetDistance = 0f;

        if (_currentState == State.AvoidTennis)
        {
            if (_tennisBall) //Unity treats it as null if object is destroyed
            {
                tennisDir = ((Vector2)_tennisBall.transform.position - (Vector2)transform.position);
                tennisDistance = tennisDir.magnitude;
                targetDir = ((Vector2)targetObj.transform.position - (Vector2)transform.position);
                targetDistance = targetDir.magnitude;
                if (targetDistance > 2.5f && tennisDistance < 10f) return;
            }
        }

        GameObject[] nearbyObjects = GetNearbyObjects();

        _targetPlayer = null;
        _pickup = null;
        _tennisBall = null;

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

            if (obj.CompareTag("TennisBall"))
            {
                _tennisBall = obj;
            }
        }

        if (_tennisBall)
        {
            tennisDir = ((Vector2)_tennisBall.transform.position - (Vector2)transform.position);
            tennisDistance = tennisDir.magnitude;
            tennisLocalAng = Mathf.Atan2(tennisDir.y, tennisDir.x) -
                             Mathf.Atan2(Mathf.Sin((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad),
                                         Mathf.Cos((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad));
            predictDir = (Vector2)(futureObj.transform.position - transform.position);
            predictDistance = predictDir.magnitude;
            predictLocalAng = Mathf.Atan2(predictDir.y, predictDir.x) -
                              Mathf.Atan2(Mathf.Sin((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad),
                                          Mathf.Cos((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad));
            if ((tennisDir - predictDir).magnitude <= 10f)
            {
                targetObj.transform.SetParent(null);
                targetObj.transform.localPosition = Vector3.zero;
                if (predictDistance < 2.5f || Mathf.Abs(predictLocalAng) > Mathf.PI / 2)
                    targetObj.transform.position = transform.position - (Vector3)tennisDir;
                else if (_currentState == State.Attacking)
                    targetObj.transform.position = transform.position +
                                                   (Vector3)(Vector2.Perpendicular(predictDir) *
                                                             (float)Math.Sign(wiggleFlipper));
                else switch (UnityEngine.Random.Range(0f, 1f))
                    {
                        case float f when (f > 0.1f && f <= 1f):
                            targetObj.transform.position = transform.position +
                                                           (Vector3)(Vector2.Perpendicular(predictDir) *
                                                                     (float)Math.Sign(wiggleFlipper));
                            break;
                        default:
                            targetObj.transform.position = transform.position - (Vector3)predictDir;
                            break;
                    }
                rend3.enabled = selected;
                _currentState = State.AvoidTennis;
                return;
            }
        }

        if (_targetPlayer)
        {
            _currentState = State.Attacking;
            targetObj.transform.SetParent(_targetPlayer.transform);
            targetObj.transform.localPosition = Vector3.zero;
            rend3.enabled = selected;
        }
        else if (_pickup)
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

using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AICarController : BaseVehicle
{
    public struct State
    {
        public static readonly State Idle = new(0, 0, "", 0f, 0f, -1f);
        public static readonly State Attacking = new(1, 1, "Player", 20f, 25f, 0.1f);
        public static readonly State GettingPickup = new(2, 1, "Pickup", 25f, 30f, 0.2f);
        public static readonly State AvoidTennis = new(3, 2, "TennisBall", 25f, 10f, 1f);

        public static readonly State[] stateArray =
        {
            Idle,
            Attacking,
            GettingPickup,
            AvoidTennis
        };
        public static readonly int stateCount = stateArray.Length;

        public int ID { get; }
        public int Priority { get; } //higher overrides lower, equal does not override
        public string ParentTag { get; }
        public float startDistance { get; }
        public float stopDistance { get; }
        public float startChance { get; }

        private State(int id, int prio, string tag, float startDist, float stopDist, float chance)
        {
            ID = id;
            Priority = prio;
            ParentTag = tag;
            startDistance = startDist;
            stopDistance = stopDist;
            startChance = chance;
        }
    }

    private State _currentState;
    private GameObject _targetParent;

    private GameObject targetObj;
    private GameObject futureObj;
    private int updateCounter;
    private int wiggleFlipper;
    private float idleMoveX;

    private Vector2 parentDir;
    private float parentDistance;
    private float parentLocalAng;
    private Vector2 predictDir;
    private float predictDistance;
    private float predictLocalAng;
    private Vector2 targetDir;
    private float targetDistance;
    private float targetLocalAng;

    //sprites assigned to Assets/Main/Prefabs/AI/MotorcycleAI.prefab
    /*DEBUGSTART
    private bool selected = false;
    public Sprite outlineSprite; //Assets/Main/Assets/PlayerMotorCycleOutline.png
    public Sprite crosshairSprite; //Assets/Main/Assets/Crosshair.png
    public Sprite crosshairSpriteRed; //Assets/Main/Assets/CrosshairRed.png
    public Sprite crosshairSpriteGreen; //Assets/Main/Assets/CrosshairGreen.png
    public Sprite crosshairSpriteBlue; //Assets/Main/Assets/CrosshairBlue.png
    private GameObject outlineObj;
    private SpriteRenderer rend1;
    private SpriteRenderer rend2;
    private SpriteRenderer rend3;
    private SpriteRenderer rend4;
    DEBUGEND*/

    protected override void Start()
    {
        base.Start();
        _currentState = State.Idle;
        _targetParent = null;

        parentDir = Vector2.zero;
        parentDistance = 0f;
        parentLocalAng = 0f;
        predictDir = Vector2.zero;
        predictDistance = 0f;
        predictLocalAng = 0f;
        targetDir = Vector2.zero;
        targetDistance = 0f;

        targetObj = new GameObject("targetObj");
        targetObj.transform.localScale = Vector3.one * 5f;
        targetObj.transform.localPosition = Vector3.zero;
        targetObj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        futureObj = new GameObject("futureObj");
        futureObj.transform.localScale = Vector3.one * 5f;
        futureObj.transform.localPosition = Vector3.zero;
        futureObj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        updateCounter = 0;
        wiggleFlipper = 10;
        idleMoveX = UnityEngine.Random.Range(0.6f, 0.8f) * Mathf.Sign(Mathf.Sign(UnityEngine.Random.Range(-1f, 1f)) + 0.5f);

        /*DEBUGSTART
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
        DEBUGEND*/
    }

    protected override void FixedUpdate()
    {
        futureObj.transform.SetPositionAndRotation(transform.position + (Vector3)rb.linearVelocity, Quaternion.identity);
        predictDir = (Vector2)(futureObj.transform.position - transform.position);
        predictDistance = predictDir.magnitude;
        predictLocalAng = Mathf.Atan2(predictDir.y, predictDir.x) -
                          Mathf.Atan2(Mathf.Sin((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad),
                                      Mathf.Cos((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad));

        DecideState();

        if (_currentState.ID == State.Idle.ID) Move(idleMoveX, 1f);
        else MoveTowards(targetObj.transform.position);

        //holding left or right while rapidly toggling between forward and backward
        //allows rotation without having to build up speed
        //wiggleFlipper's value controls how often the AI toggles when rotating
        updateCounter++;
        if (updateCounter == Math.Abs(wiggleFlipper))
        {
            wiggleFlipper = -wiggleFlipper;
            updateCounter = 0;
        }

        /*DEBUGSTART
        rend3.transform.position = targetObj.transform.position;
        rend3.transform.rotation = Quaternion.identity;
        rend4.transform.position = futureObj.transform.position;
        rend4.transform.rotation = Quaternion.identity;
        DEBUGEND*/
    }

    private void MoveTowards(Vector2 target)
    {
        targetDir = target - (Vector2)transform.position;
        targetDistance = targetDir.magnitude;
        targetLocalAng = Mathf.Atan2(targetDir.y, targetDir.x) -
                         Mathf.Atan2(Mathf.Sin((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad),
                                     Mathf.Cos((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad));
        float moveX = idleMoveX;
        float moveY = 1f;
        //target ahead, +/- 5 degrees
        if (Mathf.Abs(targetLocalAng) <= Mathf.PI / 36f)
        {
            //go forward, slight turn to target
            moveX = Mathf.Sin(targetLocalAng) * -3f;
            moveY = 1f;
        }
        //target behind, +/- 5 degrees
        else if (Mathf.PI - Mathf.Abs(targetLocalAng) <= Mathf.PI / 36f)
        {
            if (_currentState.ID == State.Attacking.ID)
            {
                //if attacking, go forward, turn left or right; to avoid damage from rear-end collision
                moveX = (float)Math.Sign(wiggleFlipper);
                moveY = 1f;
            }
            else
            {
                //if not attacking, go backward, slight turn to target
                moveX = Mathf.Sin(targetLocalAng) * 3f;
                moveY = -1f;
            }
        }
        //predicted motion to target, +/- 2.5 degrees, within 1 second
        else if (Mathf.Abs(targetLocalAng - predictLocalAng) <= Mathf.PI / 36f && targetDistance <= predictDistance)
        {
            if (_currentState.ID== State.Attacking.ID)
            {
                if (Mathf.PI - Mathf.Abs(targetLocalAng) <= Mathf.PI / 180f)
                {
                    //if attacking, target behind AI, go forward, turn left or right; to avoid damage from rear-end collision
                    moveX = (float)Math.Sign(wiggleFlipper);
                    moveY = 1f;
                }
                else
                {
                    //if attacking, player not behind AI, go forward
                    moveX = 0f;
                    moveY = 1f;
                }
            }
            else
            {
                //if not attacking, do not accelerate or decelerate; to maintain direction of motion toward target
                moveX = 0f;
                moveY = 0f;
            }
        }
        //not facing toward target, not facing away from target, not moving toward target
        else
        {
            //not moving slowly or standing still
            if (predictDistance >= 2.5f)
            {
                //facing same direction as movement, +/- 10 degrees
                if (Mathf.Abs(predictLocalAng) <= Mathf.PI / 18f)
                {
                    //go backward; to slow down
                    moveX = 0f;
                    moveY = -1f;
                }
                //facing opposite direction as movement, +/- 10 degrees
                else if (Mathf.PI - Mathf.Abs(predictLocalAng) <= Mathf.PI / 18f)
                {
                    //go forward; to slow down
                    moveX = 0f;
                    moveY = 1f;
                }
                else
                {
                    //rotate toward target; since going forward/backward doesn't slow sideways movement
                    moveX = -Mathf.Sign(Mathf.Sign(targetLocalAng) + 0.5f);
                    moveY = (float)Math.Sign(wiggleFlipper);
                }
            }
            else
            {
                //if moving slowly or standing still, rotate toward target
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

    private void DecideState()
    {
        State nextState = _currentState;

        if (_currentState.ID == State.AvoidTennis.ID)
        {
            if (_targetParent) //Unity treats it as null if object is destroyed
            {
                parentDir = (Vector2)(_targetParent.transform.position - transform.position);
                parentDistance = parentDir.magnitude;
                targetDir = (Vector2)(targetObj.transform.position - transform.position);
                targetDistance = targetDir.magnitude;
                if (targetDistance > 2.5f && parentDistance < 10f) return;
            }
            else _currentState = State.Idle;
        }
        else if (_targetParent && _targetParent.activeSelf)
        {
            targetDir = (Vector2)(targetObj.transform.position - transform.position);
            targetDistance = targetDir.magnitude;
            if (targetDistance >= _currentState.stopDistance)
            {
                _targetParent = null;
                _currentState = State.Idle;
            }
        }
        else
        {
            _targetParent = null;
            _currentState = State.Idle;
        }

        GameObject[] nearbyObjects = GetNearbyObjects();

        for (int i = 1; i < State.stateCount; i++)
        {
            if (State.stateArray[i].Priority > _currentState.Priority)
            {
                foreach (var obj in nearbyObjects)
                {
                    if (obj.CompareTag(State.stateArray[i].ParentTag) &&
                        UnityEngine.Random.Range(0f, 1f) <= State.stateArray[i].startChance)
                    {
                        targetDir = (Vector2)(obj.transform.position - transform.position);
                        targetDistance = targetDir.magnitude;

                        if (State.stateArray[i].ID == State.AvoidTennis.ID)
                        {
                            if ((targetDir - predictDir).magnitude <= 10f) //Tennis ball diameter is around 11 units
                            {
                                targetObj.transform.SetParent(null);
                                targetObj.transform.localPosition = Vector3.zero;
                                if (predictDistance < 2.5f || Mathf.Abs(predictLocalAng) > Mathf.PI / 2)
                                    targetObj.transform.position = transform.position - (Vector3)targetDir;
                                else if (_currentState.ID == State.Attacking.ID ||
                                         UnityEngine.Random.Range(0f, 1f) > 0.1f)
                                    targetObj.transform.position = transform.position +
                                                                   (Vector3)(Vector2.Perpendicular(predictDir) *
                                                                             (float)Math.Sign(wiggleFlipper));
                                else
                                    targetObj.transform.position = transform.position - (Vector3)predictDir;
                                /*DEBUGSTART
                                rend3.enabled = selected;
                                DEBUGEND*/
                                _currentState = State.AvoidTennis;
                                return;
                            }
                        }
                        else if (targetDistance <= State.stateArray[i].startDistance)
                        {
                            _targetParent = obj;
                            nextState = State.stateArray[i];
                        }
                    }
                }
            }
        }

        if (_targetParent)
        {
            _currentState = nextState;
            targetObj.transform.SetPositionAndRotation(_targetParent.transform.position, Quaternion.identity);
            /*DEBUGSTART
            rend3.enabled = selected;
            DEBUGEND*/
        }
        else
        {
            _currentState = State.Idle;
            targetObj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            /*DEBUGSTART
            rend3.enabled = false;
            DEBUGEND*/
        }
    }

    /*DEBUGSTART
    public void selectOn()
    {
        selected = true;
        rend2.enabled = true;
        rend3.enabled = _currentState.ID != State.Idle.ID;
        rend4.enabled = true;
    }

    public void selectOff()
    {
        selected = false;
        rend2.enabled = false;
        rend3.enabled = false;
        rend4.enabled = false;
    }
    DEBUGEND*/

    protected void OnDisable()
    {
        targetObj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        targetObj.transform.localPosition = Vector3.zero;
        /*DEBUGSTART
        selectOff();
        DEBUGEND*/
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanderer : MonoBehaviour
{
    // Start is called before the first frame update

    private enum State
    {
        Idle,
        Rotating, 
        Moving
    }

    private State state = State.Idle;

    [HideInInspector] public WanderRegion region;

    [Header("References")]
    public Transform trans;
    public Transform modelTrans;

    [Header("Stats")]
    public float movespeed = 18;

    [Tooltip("Minimum wait time before retargeting again.")]
    public float minRetargetInterval = 4.4f;

    [Tooltip("Maximum wait time before retargeting again.")]
    public float maxRetargetInterval = 6.2f;

    [Tooltip("Time in seconds taken to rotate after targeting, before movement begins.")]
    public float rotationTime = .6f;

    [Tooltip("Time in seconds after rotation finishes before movement starts.")]
    public float postRotationWaitTime = .3f;

    private Vector3 currentTarget; // Position we're currently targeting
    private Quaternion initialRotation; // Our rotation when we first retarget
    private Quaternion targetRotation; // The rotation we're aiming to reach at target
    private float rotationStartTime; // Time.time at which we started rotating

    void Retarget()
    {
        //Set our current target to a new random point in the region:
        currentTarget = region.GetRandomPointWithin();

        //Mark our initial rotation:
        initialRotation = modelTrans.rotation;

        //Mark the rotation required to look at the target:
        targetRotation = Quaternion.LookRotation((currentTarget - trans.position).normalized);

        //Start rotating:
        state = State.Rotating;
        rotationStartTime = Time.time;

        //Begin moving again, 'postRotationWaitTime' seconds after rotation ends:
        Invoke("BeginMoving", rotationTime + postRotationWaitTime);
    }

    void BeginMoving()
    {
        //Make double sure that we're facing the targetRotation:
        modelTrans.rotation = targetRotation;

        //Set state to Moving:
        state = State.Moving;
    }

    void Start()
    {
        Retarget();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Moving){
            //Measure the distance we're moving this frame
            float delta = movespeed * Time.deltaTime;

            //Move towards the target by the delta:
            trans.position = Vector3.MoveTowards(trans.position, currentTarget, delta);

            //Become Idle and invoke the Retarget once we hit the point:
            if(trans.position == currentTarget){
                state = State.Idle;
                Invoke("Retarget", Random.Range(minRetargetInterval, maxRetargetInterval));
            }
        } else if(state == State.Rotating)
        {
            //Measure the time we've spent rotating so far, in seconds:
            float timeSpentRotating = Time.time - rotationStartTime;

            //Rotate from initialRotation towards targetRotation:
            modelTrans.rotation = Quaternion.Slerp(initialRotation, targetRotation, timeSpentRotating/rotationTime);
        }
    }
}

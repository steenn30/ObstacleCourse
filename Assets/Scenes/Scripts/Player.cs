using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //References
    [Header("References")]
    public Transform trans;
    public Transform modelTrans;
    public CharacterController characterController;

    //Movement
    [Header("Movement")]
    [Tooltip("Units moved per second at maximum speed.")]
    public float movespeed = 24;

    [Tooltip("Time, in seconds, to reach maximum speed.")]
    public float timeToMaxSpeed = .26f;

    private float VelocityGainPerSecond => movespeed / timeToMaxSpeed;

    [Tooltip("Time, in seconds, to go from maximum speed to Stationary.")]
    public float timeToLoseMaxSpeed = .2f;

    private float VelocityLossPerSecond => movespeed / timeToLoseMaxSpeed;

    [Tooltip("Multiplier for momentum when attempting to move in a direction opposite the current traveling direction (e.g. trying to move right when already moving left).")]
    public float reverseMomentumMultiplier = 2.2f;

    private Vector3 movementVelocity = Vector3.zero;
    
    [Header("Death and Respawning")]
    [Tooltip("How long after the player's death, in seconds, before they are respawned")]
    public float respawnWaitTime = 2f;

    private bool dead = false;

    private Vector3 spawnPoint;

    private Quaternion spawnRotation;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = trans.position;
        spawnRotation = modelTrans.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)){
            Die();
        }
         Movement();
    }

    public void Die(){
        if(!dead){
            dead = true;
            Invoke("Respawn", respawnWaitTime);
            movementVelocity = Vector3.zero;
            enabled = false;
            characterController.enabled = false;
            modelTrans.gameObject.SetActive(false);
        }
    }

    public void Respawn(){
        dead = false;
        trans.position = spawnPoint;
        enabled = true;
        characterController.enabled = true;
        modelTrans.gameObject.SetActive(true);
        modelTrans.rotation = spawnRotation;
    }

    private void Movement(){
        // Up/Down movement
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
            if(movementVelocity.z >=0) {            //If we're already moving forward
            
                //Increase Z Velocity by VelocityGainPerSecond, but don't go higher than 'movespeed':
                movementVelocity.z = Mathf.Min(movespeed, movementVelocity.z + VelocityGainPerSecond * Time.deltaTime);
            } else {                                //Else if we're moving back

                //Increase Z velocity by VelocityGainPerSecond, using thge reverseMomentumMultiplier, but don't raise higher than 0:
                movementVelocity.z = Mathf.Min(0, movementVelocity.z + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
            }
            //Increase
        } else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
            if(movementVelocity.z > 0){             //If we're already moaving forward
                movementVelocity.z = Mathf.Max(0, movementVelocity.z - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
            }
            else{                                   //If we're moving back or not moving at all
                movementVelocity.z = Mathf.Max(-movespeed, movementVelocity.z - VelocityGainPerSecond * Time.deltaTime);
            }
        } else {                                    //If neither forward nor back are being held

            //We must bring the Z velocity back to 0 over time.
            if(movementVelocity.z > 0){             // If we're moving up

                //Decrease Z velocity by VelocityLossPerSecond, but don't go any lower than 0:
                movementVelocity.z = Mathf.Max(0, movementVelocity.z - VelocityLossPerSecond * Time.deltaTime);
            } else {                                //If we're moving down

                //Increase Z velocity (back towards 0) by VelocityLossPerSecond, but don't go any higher than 0:
                movementVelocity.z = Mathf.Min(0, movementVelocity.z + VelocityLossPerSecond * Time.deltaTime);
            }
      }

      // Left/Right movement
      if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
        if(movementVelocity.x >= 0){                //If we're already moving right

            //Increase X velocity by VelocityGainPerSecond, but don't go higher than 'movespeed':
            movementVelocity.x = Mathf.Min(movespeed, movementVelocity.x + VelocityGainPerSecond * Time.deltaTime);

        } else{                                     //If we're moving left

            //Increase x velocity by VelocityGainPerSecond, using the reverseMomentumMultiplier, but don't raise higher than 0:
            movementVelocity.x = Mathf.Min(0, movementVelocity.x + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
        }
      } else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
        if(movementVelocity.x > 0){                 //If we're already moving right
            movementVelocity.x  = Mathf.Max(0, movementVelocity.x - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
        } else {                                    //If we're moving left or not moving at all 
            movementVelocity.x = Mathf.Max(-movespeed, movementVelocity.x - VelocityGainPerSecond * Time.deltaTime);
        }
      } else{                                       //If neither right nor left are being held 
            //We must bring the X velocity back to 0 over time.
            if(movementVelocity.x > 0){             //If we're moving right,

                //Decrease X velocity by VelocityLossPerSecond, but don't go any lower than 0:
                movementVelocity.x = Mathf.Max(0, movementVelocity.x - VelocityLossPerSecond * Time.deltaTime);
            } else {                                //If we're moving left,

                //Increase X velocity (back towards 0) by VelocityLossPerSecond, but don't go any higher than 0:
                movementVelocity.x = Mathf.Min(0, movementVelocity.x + VelocityLossPerSecond * Time.deltaTime);
            }
      }

      // Applying the movement
      if(movementVelocity.x != 0 || movementVelocity.z != 0){
        //Applying the movement velocity:
        characterController.Move(movementVelocity * Time.deltaTime);

        //Keeping the model holder roated towards the last movement direction:
        // A Quarternion is a data type that resembles a rotation. 
            //  The "Transform.rotation" member is a Quarternion that resmbles a Transform's current facing direction
        modelTrans.rotation = Quaternion.Slerp(modelTrans.rotation, Quaternion.LookRotation(movementVelocity), .18f);  //Slerp method can be used to move one value toward another by a fraction of the difference between those two values
      }

    }
}

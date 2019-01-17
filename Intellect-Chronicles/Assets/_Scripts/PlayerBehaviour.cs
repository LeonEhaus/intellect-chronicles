using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveSettings
{
    public float runVelocity = 12;
    public float jumpVelocity = 8;
    public float jumpForce = 50 ;
    public float distanceToGround = 1.01f;
    public float gravity = 40.0F;
    public float jumpTime = 0.2f;
    public AnimationCurve forceTime;
    public LayerMask ground;
}

[System.Serializable]
public class InputSettings
{
    public string FORWARD_AXIS = "Vertical";
    public string SIDEWAY_AXIS = "Horizontal";
    public string JUMP_AXIS = "Jump";
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    public MoveSettings moveSettings;
    public InputSettings inputSettings;

    private Rigidbody playerRigidbody;
    private Vector3 velocity;
    private float forwardInput, sidewaysInput, jumpInput;
    private bool isJumping = false;

    public bool Grounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, moveSettings.distanceToGround, moveSettings.ground);
    }

    private void Awake()
    {
        velocity = Vector3.zero;
        forwardInput = sidewaysInput = jumpInput = 0.0f;
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Physics.gravity = new Vector3(0.0F, -moveSettings.gravity, 0.0F);
        GetInput();    
    }

    private void FixedUpdate()
    {
        Run();
        Jump();
    }

    private void GetInput()
    {
        if(inputSettings.FORWARD_AXIS.Length != 0) forwardInput = Input.GetAxis(inputSettings.FORWARD_AXIS);
        if (inputSettings.SIDEWAY_AXIS.Length != 0) sidewaysInput = Input.GetAxis(inputSettings.SIDEWAY_AXIS);
        if (inputSettings.JUMP_AXIS.Length != 0) jumpInput = Input.GetAxisRaw(inputSettings.JUMP_AXIS);
    }

    private void Run()
    {
        velocity.x = sidewaysInput * moveSettings.runVelocity;
        velocity.y = playerRigidbody.velocity.y;
        playerRigidbody.velocity = transform.TransformDirection(velocity);
    }

    private void Jump()
    {
        if (jumpInput != 0 && Grounded() && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpRoutine());
            //playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, moveSettings.jumpVelocity, playerRigidbody.velocity.z);
        }
    }

    IEnumerator JumpRoutine()
    {
        playerRigidbody.velocity = new Vector3 (playerRigidbody.velocity.x, moveSettings.jumpVelocity, playerRigidbody.velocity.z);
        float timer = 0;

        while (jumpInput != 0 && timer < moveSettings.jumpTime)
        {
            float proportionCompleted = timer / moveSettings.jumpTime;
            Vector3 thisFrameJumpVector = Vector3.Lerp(new Vector3(0, moveSettings.jumpForce, 0), Vector3.zero, moveSettings.forceTime.Evaluate(proportionCompleted));
            playerRigidbody.AddForce(thisFrameJumpVector);
            timer += Time.deltaTime;
            yield return null;
        }

        isJumping = false;
    }
}

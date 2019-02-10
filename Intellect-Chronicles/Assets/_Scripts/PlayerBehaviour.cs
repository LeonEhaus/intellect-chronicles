using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveSettings
{
    public float runVelocity = 12;
    public float crouchVelocity = 8;
    public float jumpVelocity = 8;
    public float cJumpVelocity = 7;
    public float jumpForce = 50 ;
    public float cJumpForce = 48;
    public float distanceToGround = 1.01f;
    public float gravity = 40.0F;
    public float jumpTime = 0.2f;
    public AnimationCurve forceTime;
    public LayerMask ground;
}

[System.Serializable]
public class InputSettings
{
    public string VERTICAL_AXIS = "Vertical";
    public string SIDEWAY_AXIS = "Horizontal";
    public string JUMP_AXIS = "Jump";
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    public Animator animator;

    public GameManager gamemanager;
    protected bool paused;
    public MoveSettings moveSettings;
    public InputSettings inputSettings;
    public BoxCollider boxCollider;
    public GameObject mesh;

    private Rigidbody playerRigidbody;
    private Vector3 velocity;
    private float verticalInput, sidewaysInput, jumpInput;
    private bool isJumping = false;
    public bool crouching = false;  
    private bool lastC= false;

    public bool Grounded()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.down, moveSettings.distanceToGround, moveSettings.ground);
    }

    private void Awake()
    {
        velocity = Vector3.zero;
        verticalInput = sidewaysInput = jumpInput = 0.0f;
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!paused)
        {
            Physics.gravity = new Vector3(0.0F, -moveSettings.gravity, 0.0F);
            GetInput();

            animator.SetBool("Grounded", Grounded());
            animator.SetBool("crouching", crouching);

            if(crouching)
            {
                boxCollider.size = new Vector3(0.6f, 1.1f, 0.9f);
                boxCollider.center = new Vector3(0, 0.55f, 0.15f);
            }
            else
            {
                boxCollider.center = new Vector3(0, 0.96f, 0);
                boxCollider.size = new Vector3(0.6f, 1.89f, 0.5f);
            }

            animator.SetBool("walking", Input.GetAxis(inputSettings.SIDEWAY_AXIS) != 0);
            mesh.transform.eulerAngles = new Vector3(0, Input.GetAxis(inputSettings.SIDEWAY_AXIS) < 0 ? 270 : 90, 0);
        }


    }

    private void FixedUpdate()
    {
        if(!paused)
        {
            Run();
            Jump();
        }
        
    }

    private void GetInput()
    {
        if(inputSettings.VERTICAL_AXIS.Length != 0) verticalInput = Input.GetAxis(inputSettings.VERTICAL_AXIS);
        if (inputSettings.SIDEWAY_AXIS.Length != 0) sidewaysInput = Input.GetAxis(inputSettings.SIDEWAY_AXIS);
        if (inputSettings.JUMP_AXIS.Length != 0) jumpInput = Input.GetAxisRaw(inputSettings.JUMP_AXIS);
        crouching = (verticalInput<0);
    }

    private void Run()
    {
        velocity.z = sidewaysInput * (crouching ? moveSettings.crouchVelocity : moveSettings.runVelocity);
        velocity.y = playerRigidbody.velocity.y;
        playerRigidbody.velocity = transform.TransformDirection(velocity);
    }

    private void Jump()
    {
        if (jumpInput != 0 && Grounded() && !isJumping)
        {
            animator.SetTrigger("jump");
            isJumping = true;
            StartCoroutine(JumpRoutine());
        }
    }

    IEnumerator JumpRoutine()
    {
        playerRigidbody.velocity = new Vector3 (playerRigidbody.velocity.x, (crouching ? moveSettings.cJumpVelocity : moveSettings.jumpVelocity), playerRigidbody.velocity.z);
        float timer = 0;
        if (crouching)
        {
            while (jumpInput != 0 && timer < moveSettings.jumpTime)
            {
                float proportionCompleted = timer / moveSettings.jumpTime;
                Vector3 thisFrameJumpVector = Vector3.Lerp(new Vector3(0, moveSettings.cJumpForce, 0), Vector3.zero, moveSettings.forceTime.Evaluate(proportionCompleted));
                playerRigidbody.AddForce(thisFrameJumpVector);
                timer += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (jumpInput != 0 && timer < moveSettings.jumpTime)
            {
                float proportionCompleted = timer / moveSettings.jumpTime;
                Vector3 thisFrameJumpVector = Vector3.Lerp(new Vector3(0, moveSettings.jumpForce, 0), Vector3.zero, moveSettings.forceTime.Evaluate(proportionCompleted));
                playerRigidbody.AddForce(thisFrameJumpVector);
                timer += Time.deltaTime;
                yield return null;
            }
        }

        isJumping = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Objective"))
        {
            other.gameObject.SetActive(false);
            gamemanager.NextObjective();
        }
        else if (other.gameObject.CompareTag("Finish"))
        {

            gamemanager.endLevel();
        }
        else if (other.gameObject.CompareTag("Interactable"))
        {
            other.gameObject.GetComponent<ActionText>().enableHalo();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            other.gameObject.GetComponent<ActionText>().deactivateHalo();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyHearing") && !crouching)
        {
            gamemanager.DetectedByEnemy(gameObject);
        }
        if(other.gameObject.CompareTag("Interactable"))
        {
            if(Input.GetButtonDown("Interact"))
            {
                gamemanager.interactableText(other.gameObject.GetComponent<ActionText>().getText());
            }
        }
    }

    void OnPauseGame()
    {
        paused = true;
        playerRigidbody.isKinematic = true;
        animator.enabled = false;
    }


    void OnResumeGame()
    {
        paused = false;
        playerRigidbody.isKinematic = false;
        animator.enabled = true;
    }

}

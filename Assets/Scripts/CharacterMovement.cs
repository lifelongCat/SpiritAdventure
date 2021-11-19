using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    [Header("Set in Inspector")]
    public float speed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 500f;
    
    
    bool facingRight = true;
    bool grounded = false;
    bool isHolding = false;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    public Animator animator;
    public GameObject idleSprite;

    private List<IInteractableObject> interactableObjects = new List<IInteractableObject>();
    
    private static readonly int isGrounded = Animator.StringToHash("isGrounded");
    private static readonly int isWalking = Animator.StringToHash("isWalking");
    



    private void Start()
    {
        idleSprite.SetActive(true);
    }
    private static readonly int isJumping = Animator.StringToHash("isJumping");
    private static readonly int isRunning = Animator.StringToHash("isRunning");

    

 
    void FixedUpdate()
    {
        
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

    }

    private void Update()
    {
        Movement();

        Animate();

        CheckInteraction();
    }


    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void pickUp()
    {
        isHolding = true;
    }

    public void AddInteractableObject(IInteractableObject interactable)
    {
        interactableObjects.Add(interactable);
    }

    public void TryToRemoveInteractableObject(IInteractableObject interactable) // sponsored by good method naming
    {
        if (interactableObjects.Contains(interactable))
        {
            interactableObjects.Remove(interactable);
        }
    }

    void Movement()
    {
        float xAxis = Input.GetAxis("Horizontal");

        if (!PlantClimb.isClimbing)
        {
            if (!isHolding)
            {
                if (grounded && (Input.GetKeyDown(KeyCode.Space)))
                {

                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
                }
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(xAxis * runSpeed, GetComponent<Rigidbody2D>().velocity.y);
                }
                else
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(xAxis * speed, GetComponent<Rigidbody2D>().velocity.y);
                }
            }
            else
            {
                if (grounded && (Input.GetKeyDown(KeyCode.Space)))
                {

                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce - 100));
                }
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(xAxis * runSpeed - 4, GetComponent<Rigidbody2D>().velocity.y);
                }
                else
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(xAxis * speed - 2, GetComponent<Rigidbody2D>().velocity.y);
                }
            }
        }

        if (xAxis > 0 && !facingRight)
            Flip();
        else if (xAxis < 0 && facingRight)
            Flip();
    }

    void Animate()
    {
        float xAxis = Input.GetAxis("Horizontal");

        if (grounded || PlantClimb.isClimbing)
        {
            animator.SetBool(isGrounded, true);
        }

        else
        {
            animator.SetBool(isGrounded, false);
        }

        if (Mathf.Abs(xAxis) > 0)
        {
            animator.SetBool(isWalking, true);
        }
        else
        {
            animator.SetBool(isWalking, false);
        }
    }


    void CheckInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            print("Input.GetKeyDown(KeyCode.E");
            IInteractableObject selectedObject = null;
            foreach(IInteractableObject obj in interactableObjects)
            {
                if(selectedObject == null)
                {
                    selectedObject = obj;
                    continue;
                }
                if (Vector3.Distance(this.transform.position, obj.position) < Vector3.Distance(this.transform.position, selectedObject.position))
                    selectedObject = obj;
            }
            if (selectedObject != null)
            {
                selectedObject.Interaction();
            }
        }
    }
}

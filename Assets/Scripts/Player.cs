using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private float moveSpeed;
    [SerializeField] private float distanceToGround = 0.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private ParticleSystem confetti;

    private Ray ray;
    private float groundCheckRadius = 0.3f;
    private Vector2 moveInput;
    private Rigidbody rb;
    
    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying())
        {
            rb.useGravity = true;
            Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            confetti.transform.position = transform.position + Vector3.up * 1.7f;
            // faceDirection
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * 10f);
        }
        else
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;

        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().normalized;
        // isMoving = moveInput == Vector2.zero;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            rb.velocity = Vector3.up * 3f;
            // rb.AddForce(Vector3.up,Fo);
        }
    }
    

    // public bool IsGrounded()
    // {
    //     float extraHeightText = 0.1f;
    //     bool grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
    //     return grounded;
    // }
    
    public bool IsGrounded()
    {
        bool grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
        return grounded;
    }



    public GameObject GetFloorName()
    {
        ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, distanceToGround))
        {
            return hit.collider.gameObject;
        }

        return null;
    }
}
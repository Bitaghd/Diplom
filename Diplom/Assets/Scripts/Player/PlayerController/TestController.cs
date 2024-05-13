using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestController : MonoBehaviour
{
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;

    public float walkSpeed = 15.0f;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;

    Transform cameraT;
    float verticalLookRotation;

    Rigidbody rigidbodyR;
    public bool freeze = false;

    float jumpForce = 250.0f;
    [SerializeField] bool grounded;
    public LayerMask groundedMask;

    // Use this for initialization
    void Start()
    {
        cameraT = Camera.main.transform;
        rigidbodyR = GetComponent<Rigidbody>();
        LockMouse();
    }

    // Update is called once per frame
    void Update()
    {
        if (freeze)
            return;

        // rotation
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

        // movement
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 targetMoveAmount = moveDir * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .1f);

        // jump
        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
            {
                rigidbodyR.AddForce(transform.up * jumpForce);
            }
        }

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        /* Lock/unlock mouse on click */
        //if (Input.GetMouseButtonUp(0))
        //{
        //    if (!cursorVisible)
        //    {
        //        UnlockMouse();
        //    }
        //    else
        //    {
        //        LockMouse();
        //    }
        //}
    }

    void FixedUpdate()
    {
        rigidbodyR.MovePosition(rigidbodyR.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    //void UnlockMouse()
    //{
    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = true;
    //    cursorVisible = true;
    //}

    void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //cursorVisible = false;
    }


    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        freeze = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        //enableMovementOnNextTouch = true;
        rigidbodyR.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        freeze = false;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

}

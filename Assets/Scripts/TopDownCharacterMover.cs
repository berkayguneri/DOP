using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputHandler))]
public class TopDownCharacterMover : MonoBehaviour
{
    private InputHandler _input;
    private Animator anim;

    [SerializeField]
    private bool RotateTowardMouse;

   

    [SerializeField]
    private float MovementSpeed;
    [SerializeField]
    private float RotationSpeed;

    [SerializeField]
    private Camera Camera;

   

    [Header("Dash")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    private float dashTimer = 0f;
    private bool isDashing = false;
    public KeyCode dashKey = KeyCode.H;
    private Vector3 dashDirection;
    private TrailRenderer tr;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        _input = GetComponent<InputHandler>();
    }

    private void Start()
    {
        tr = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        
        var targetVector = new Vector3(_input.InputVector.x, 0, _input.InputVector.y);
        var movementVector = MoveTowardTarget(targetVector);

        

        if (!RotateTowardMouse)
        {
            RotateTowardMovementVector(movementVector);
        }
        if (RotateTowardMouse)
        {
            RotateFromMouseVector();
        }
        InputManager();

        bool isMoving = movementVector.magnitude > 0;
        anim.SetBool("run", isMoving);
    }

    private void RotateFromMouseVector()
    {
        Ray ray = Camera.ScreenPointToRay(_input.MousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            transform.LookAt(target);

        }
    }

    private Vector3 MoveTowardTarget(Vector3 targetVector)
    {
        var speed = MovementSpeed * Time.deltaTime;
        

        targetVector = Quaternion.Euler(0, Camera.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
        var targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
        return targetVector;
    }

    private void RotateTowardMovementVector(Vector3 movementDirection)
    {
        if(movementDirection.magnitude == 0) { return; }
        var rotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, RotationSpeed);
 
    }
    #region Input
    void InputManager()
    {
        if (Input.GetKeyDown(dashKey) && !isDashing)
        {
            isDashing = true;
            dashTimer = 0.0f;

            // Karakterin bakış yönünü al
            Vector3 dashDirection = GetDashDirection();

            // Dash yönünü normalize et ve hızı uygula
            dashDirection.Normalize();
            Vector3 dashVelocity = dashDirection * dashSpeed;
            tr.emitting = true;

            // Karakteri hızlandır
            GetComponent<Rigidbody>().velocity = dashVelocity;
            tr.emitting = true;

        }
        if (isDashing)
        {
            dashTimer += Time.deltaTime;

            if (dashTimer >= dashDuration)
            {
                isDashing = false;

                // Dash sona erdiğinde karakterin hızını sıfırla
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                tr.emitting = false;
            }
        }

       
    }

   

    #endregion
    Vector3 GetDashDirection()
    {
        // Karakterin mevcut bakış yönünü al
        Vector3 characterForward = transform.forward;

        // Yatay düzlemde bakış yönünü kullanmak istiyorsanız, yani karakter zıplama yaparken yukarı ve aşağıya gitmemesi gerekiyorsa
        characterForward.y = 0;

        return characterForward;
    }
}

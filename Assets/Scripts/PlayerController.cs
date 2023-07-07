using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Vector2 _input;
    private CharacterController _characterController;
    private Vector3 _direction;

    [SerializeField] private float smoothTime = .05f;
    private float _currentVelocity;
    [SerializeField] private float movementSpeed;

    private float _gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float _velocity;

    [SerializeField] private float jumpPower;
    private int _numberOfJumps;
    [SerializeField] private int maxNumberOfJumps = 2;

    [SerializeField] private float sprintPower;
    private bool isSprinting = false;

    public Transform cameraTransform;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyRotation();
        ApplyMovement();
        ApplyGravity();
    }

    private void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0)
            return;

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTime);
    }

    private void ApplyMovement()
    {
        Vector3 moveDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * _direction;
        float originalY = moveDirection.y;
        Vector3 speedDirection = moveDirection * GetMovementSpeed();
        speedDirection.y = originalY;

        _characterController.Move(speedDirection * Time.deltaTime);
    }

    private float GetMovementSpeed()
    {
        return isSprinting ? movementSpeed + sprintPower : movementSpeed;
    }

    private void ApplyGravity()
    {
        if (IsGrounded() && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += _gravity * gravityMultiplier * Time.deltaTime;
        }

        _direction.y = _velocity;
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _direction = new Vector3(_input.x, 0.0f, _input.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!IsGrounded() && _numberOfJumps >= maxNumberOfJumps) return;
        if (_numberOfJumps == 0)
            StartCoroutine(WaitForLanding());

        _numberOfJumps++;
        _velocity = jumpPower;
    }

    private IEnumerator WaitForLanding()
    {
        yield return new WaitUntil(() => !IsGrounded());
        yield return new WaitUntil(IsGrounded);

        _numberOfJumps = 0;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if(IsGrounded())
            if (context.started)
            {
                isSprinting = true;
            }
        if (context.canceled)
        {
            isSprinting = false;
        }
    }

    private bool IsGrounded() => _characterController.isGrounded;

}

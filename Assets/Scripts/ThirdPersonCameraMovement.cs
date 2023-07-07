using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ThirdPersonCameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 _position;
    public float rotationSpeed = 80f;
    public float height = 3f;
    public float distance = 5f;
    public LayerMask obstructionMask;

    private bool isRotating = false;

    void Start()
    {
        Vector3 offset = new Vector3(0f, height, -distance);
        transform.position = target.position + offset;
        transform.LookAt(target);
    }

    private void LateUpdate()
    {
        if (isRotating)
        {
            RotateCamera();
        }
        else
        {
            Vector3 offset = transform.rotation * new Vector3(0f, 0f, -distance - 0.3f);
            transform.position = target.position + offset;
            transform.LookAt(target);

            CheckForObstructions();
        }
    }

    public void ApplyCameraMovement(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            isRotating = true;
        }
        else if(context.canceled)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            isRotating = false;
        }
    }

    private void RotateCamera()
    {
        _position.x += Mouse.current.delta.x.ReadValue() * (rotationSpeed) * Time.deltaTime;
        _position.y -= Mouse.current.delta.y.ReadValue() * (rotationSpeed) * Time.deltaTime;
        _position.y = Mathf.Clamp(_position.y, -60f, 59f);

        Quaternion rotation = Quaternion.Euler(_position.y, _position.x, 0f);
        Vector3 offset = rotation * new Vector3(0f, height, -distance);
        Vector3 desiredPosition = target.position + offset;

        transform.rotation = rotation;
        transform.position = desiredPosition;
        transform.LookAt(target);

        CheckForObstructions();
    }

    private void CheckForObstructions()
    {
        Vector3 direction = transform.position - target.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = new RaycastHit[10];
        int hitCount = Physics.RaycastNonAlloc(target.position, direction, hits, distance, obstructionMask);

        for (int i = 0; i < hitCount; i++)
        {
            if (!hits[i].collider.isTrigger)
            {
                transform.position = hits[i].point;
                break;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform camPosition;
    public float mouseSensitivity = 1f;
    private Vector2 mouseInput;
    public float verticalAngleLimit = 60f;
    private float verticalRotationVariable;

    public float movementSpeed = 5f;
    private Vector3 moveDirection, movement;

    public CharacterController charController;

    public float jumpHeight = 5f;
    public float gravityMultiplier = 1.5f;

    public Transform groundPoint;
    public LayerMask groundLayer;
    private bool isGrounded;

    private void Start()
    {
        Camera.main.transform.position = camPosition.position;
        Camera.main.transform.rotation = camPosition.rotation;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        MouseInputHandler();

        MovementHandler();
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = camPosition.position;
        Camera.main.transform.rotation = camPosition.rotation;
    }

    private void MouseInputHandler()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + mouseInput.x,
            transform.rotation.eulerAngles.z);

        verticalRotationVariable += mouseInput.y;
        verticalRotationVariable = Mathf.Clamp(verticalRotationVariable, -verticalAngleLimit, verticalAngleLimit);

        camPosition.rotation = Quaternion.Euler(
            -verticalRotationVariable,
            camPosition.rotation.eulerAngles.y,
            camPosition.rotation.eulerAngles.z);
    }

    private void MovementHandler()
    {
        float yVelocity = movement.y;

        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        yVelocity += Physics.gravity.y * Time.deltaTime * gravityMultiplier;

        isGrounded = Physics.Raycast(groundPoint.position, Vector3.down, 0.3f, groundLayer);

        if (isGrounded)
        {
            yVelocity = 0f;
        }

        movement = (transform.forward * moveDirection.z) + (transform.right * moveDirection.x);

        movement.y += yVelocity;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            movement.y = jumpHeight;
        }

        charController.Move(movement * movementSpeed * Time.deltaTime);
    }


}

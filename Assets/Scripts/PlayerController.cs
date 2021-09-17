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
    private Camera mainCam;

    public float movementSpeed = 5f;
    private Vector3 moveDirection, movement;

    public CharacterController charController;

    public float jumpHeight = 5f;
    public float gravityMultiplier = 1.5f;

    public Transform groundPoint;
    public LayerMask groundLayer;
    private bool isGrounded;

    public GameObject bulletImpactVFX;
    public GameObject muzzleFlashVFX;
    public Transform localMuzzlePoint;
    public Transform networkMuzzlePoint;

    public Animator playerAnim;

    public Gun[] guns;
    private int selectedGunIndex;
    private float weaponCooldownTime = Mathf.Infinity;

    private void Start()
    {
        Camera.main.transform.position = camPosition.position;
        Camera.main.transform.rotation = camPosition.rotation;

        mainCam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;

        EquipWeapon(0);
    }

    private void Update()
    {
        MouseInputHandler();

        MovementHandler();

        if (Input.GetMouseButton(0))
        {
            if (guns[selectedGunIndex].timeBetweenShots < weaponCooldownTime)
            {
                weaponCooldownTime = 0;
                Shoot();
            }
        }

        ChangeWeaponHandler();

        weaponCooldownTime += Time.deltaTime;
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void MovementHandler()
    {
        float yVelocity = movement.y;

        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (playerAnim.isActiveAndEnabled)
        {
            playerAnim.SetFloat("Position X", moveDirection.x);
            playerAnim.SetFloat("Position Y", moveDirection.z);
        }

        yVelocity += Physics.gravity.y * Time.deltaTime * gravityMultiplier;

        isGrounded = Physics.Raycast(groundPoint.position, Vector3.down, 0.3f, groundLayer);

        if (isGrounded)
        {
            yVelocity = 0f;

            if (playerAnim.isActiveAndEnabled)
            {
                playerAnim.SetBool("isGrounded", isGrounded);
            }
        }

        movement = (transform.forward * moveDirection.z) + (transform.right * moveDirection.x);

        movement.y += yVelocity;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            movement.y = jumpHeight;

            if (playerAnim.isActiveAndEnabled)
            {
                playerAnim.SetBool("isGrounded", false);
            }
        }

        charController.Move(movement * movementSpeed * Time.deltaTime);
    }
    private void Shoot()
    {
        Instantiate(muzzleFlashVFX, playerAnim.isActiveAndEnabled ? networkMuzzlePoint : localMuzzlePoint);

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        ray.origin = mainCam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit, guns[selectedGunIndex].weaponRange))
        {
            Instantiate(bulletImpactVFX, hit.point + (hit.normal * 0.003f), Quaternion.LookRotation(hit.normal, Vector3.up));
        }
    }

    private void EquipWeapon(int gunIndex)
    {
        selectedGunIndex = gunIndex;

        foreach(Gun gun in guns)
        {
            gun.gameObject.SetActive(false);
        }

        guns[selectedGunIndex].gameObject.SetActive(true);
    }

    private void ChangeWeaponHandler()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedGunIndex += 1;

            if (guns.Length > selectedGunIndex)
            {
                EquipWeapon(selectedGunIndex);
            }
            else
            {
                selectedGunIndex = 0;
                EquipWeapon(selectedGunIndex);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedGunIndex -= 1;

            if (selectedGunIndex >= 0)
            {
                EquipWeapon(selectedGunIndex);
            }
            else
            {
                selectedGunIndex = guns.Length - 1;
                EquipWeapon(selectedGunIndex);
            }
        }
    }

}

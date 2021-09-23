using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public bool activateUnityEditorControls = false;

    public int maxHealthPoints;
    private int currentHealthPoints;

    public Transform camPosition;
    public float mouseSensitivity = 50f;
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
    public bool isGrounded;

    public GameObject bulletImpactVFX;
    public GameObject muzzleFlashVFX;
    public Transform localMuzzlePoint;
    public Transform networkMuzzlePoint;

    public Animator playerAnim;

    public Gun[] guns;
    private int selectedGunIndex;
    private float weaponCooldownTime = Mathf.Infinity;
    Coroutine firingCo;

    public GameObject playerBodyOverNetwork;
    public GameObject playerBodyLocal;

    public static PlayerController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentHealthPoints = maxHealthPoints;

        Camera.main.transform.position = camPosition.position;
        Camera.main.transform.rotation = camPosition.rotation;

        mainCam = Camera.main;

        EquipWeapon(0);

        WeaponSwitcherUI.instance.UpdateSlotSwitcherInfo(0);

        if (photonView.IsMine)
        {
            playerBodyLocal.SetActive(true);
            playerBodyOverNetwork.SetActive(false);
        }
        else
        {
            playerBodyLocal.SetActive(false);
            playerBodyOverNetwork.SetActive(true);
        }

#if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
#endif
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        MouseInputHandler();

        MovementHandler();

        weaponCooldownTime += Time.deltaTime;

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerSpawner.instance.PlayerDie();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchWeaponOnLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchWeaponOnRight();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            activateUnityEditorControls = !activateUnityEditorControls;
        }

        if (activateUnityEditorControls == false) return;

        if (Input.GetMouseButton(0))
        {
            if (guns[selectedGunIndex].timeBetweenShots < weaponCooldownTime)
            {
                weaponCooldownTime = 0;
                Shoot();
            }
        }
#endif

    }

    private void LateUpdate()
    {
        if (!photonView.IsMine) return;

        mainCam.transform.position = camPosition.position;
        mainCam.transform.rotation = camPosition.rotation;
    }

    private void MouseInputHandler()
    {
        mouseInput = bl_TouchPad.GetInputSmooth();

#if UNITY_EDITOR
        if (activateUnityEditorControls == false) return;

        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

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
#endif

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

        //moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        moveDirection = new Vector3(bl_MovementJoystick.Instance.Horizontal, 0, bl_MovementJoystick.Instance.Vertical);

        if (true)
        {
            playerAnim.SetFloat("Position X", moveDirection.x);
            playerAnim.SetFloat("Position Y", moveDirection.z);
        }

        yVelocity += Physics.gravity.y * Time.deltaTime * gravityMultiplier;

        isGrounded = Physics.Raycast(groundPoint.position, Vector3.down, 0.3f, groundLayer);

        if (isGrounded)
        {
            yVelocity = 0f;

            if (true)
            {
                playerAnim.SetBool("isGrounded", isGrounded);
            }
        }

        movement = (transform.forward * moveDirection.z) + (transform.right * moveDirection.x);

        movement.y += yVelocity;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        charController.Move(movement * movementSpeed * Time.deltaTime);
    }

    public void Jump()
    {
        if (!isGrounded) return;


        movement.y = jumpHeight;

        if (true)
        {
            playerAnim.SetBool("isGrounded", false);
        }

        charController.Move(movement * movementSpeed * Time.deltaTime);


    }

    private void Shoot() // using mouse
    {    
        GameObject muzzleInstance = PhotonNetwork.Instantiate(muzzleFlashVFX.name,
            networkMuzzlePoint.position,
            networkMuzzlePoint.rotation);

        if (photonView.IsMine)
        {
            Destroy(muzzleInstance);
            Instantiate(muzzleFlashVFX, localMuzzlePoint);
        }

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        ray.origin = mainCam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit, guns[selectedGunIndex].weaponRange))
        {
            PhotonNetwork.Instantiate(bulletImpactVFX.name,
                hit.point + (hit.normal * 0.003f),
                Quaternion.LookRotation(hit.normal,
                Vector3.up));

            if (hit.collider.gameObject.tag == "Player")
            {
                hit.collider.GetComponent<PhotonView>().RPC("DealDamage",
                    RpcTarget.All,
                    photonView.Owner.NickName,
                    guns[selectedGunIndex].damage);
            }
        }

        weaponCooldownTime = 0;
    }

    private IEnumerator KeepShooting() // using UI buttons
    {
        while(true)
        {
            GameObject muzzleInstance = PhotonNetwork.Instantiate(muzzleFlashVFX.name,
                networkMuzzlePoint.position,
                networkMuzzlePoint.rotation);

            if (photonView.IsMine)
            {
                Destroy(muzzleInstance);
                Instantiate(muzzleFlashVFX, localMuzzlePoint);
            }

            Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            ray.origin = mainCam.transform.position;

            if (Physics.Raycast(ray, out RaycastHit hit, guns[selectedGunIndex].weaponRange))
            {
                PhotonNetwork.Instantiate(bulletImpactVFX.name,
                               hit.point + (hit.normal * 0.003f),
                               Quaternion.LookRotation(hit.normal,
                               Vector3.up));

                if (hit.collider.gameObject.tag == "Player")
                {
                    hit.collider.GetComponent<PhotonView>().RPC("DealDamage",
                        RpcTarget.All,
                        photonView.Owner.NickName,
                        guns[selectedGunIndex].damage);
                }
            }
            yield return new WaitForSecondsRealtime(guns[selectedGunIndex].timeBetweenShots);

            weaponCooldownTime = 0;
        }
    }

    public void StartShooting()
    {
        if (guns[selectedGunIndex].timeBetweenShots > weaponCooldownTime) return;

        weaponCooldownTime = 0;

        firingCo = StartCoroutine(KeepShooting());
    }

    public void StopShooting()
    {
        StopCoroutine(firingCo);
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

    public void SwitchWeaponOnRight()
    {
        selectedGunIndex += 1;

        if (selectedGunIndex > guns.Length - 1)
        {
            selectedGunIndex = 0;
        }
  
        EquipWeapon(selectedGunIndex);
        WeaponSwitcherUI.instance.UpdateSlotSwitcherInfo(selectedGunIndex);
    }

    public void SwitchWeaponOnLeft()
    {
        selectedGunIndex -= 1;

        if (selectedGunIndex < 0)
        {
            selectedGunIndex = guns.Length - 1;
        }

        EquipWeapon(selectedGunIndex);
        WeaponSwitcherUI.instance.UpdateSlotSwitcherInfo(selectedGunIndex);
    }

    [PunRPC]
    public void DealDamage(string damager, int damage)
    {
        TakeDamage(damager, damage);
    }

    private void TakeDamage(string damager, int damage)
    {
        if (!photonView.IsMine) return;

        currentHealthPoints -= damage;

        if (currentHealthPoints <= 0)
        {
            currentHealthPoints = 0;
            PlayerSpawner.instance.PlayerDie();
        }

        HUDController.instance.healthText.text = currentHealthPoints.ToString();

        Debug.Log(photonView.Owner.NickName + " health: " + currentHealthPoints);
    }

}

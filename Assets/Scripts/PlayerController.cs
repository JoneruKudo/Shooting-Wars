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
    public bool isDead = false;

    public Transform camPosition;
    public float mouseSensitivity = 50f;
    private Vector2 mouseInput;
    public float verticalAngleLimit = 60f;
    private float verticalRotationVariable;
    private Camera mainCam;
    public Transform camPointWhenDead;

    public float movementSpeed = 5f;
    private Vector3 moveDirection, movement;

    public CharacterController charController;

    public float jumpHeight = 5f;
    public float gravityMultiplier = 1.5f;

    public Transform groundPoint;
    public LayerMask groundLayer;
    public bool isGrounded;

    public GameObject bulletImpactBodyVFX;
    public GameObject bulletImpactVFX;
    public GameObject muzzleFlashVFX;
    public Transform localMuzzlePoint;
    public Transform networkMuzzlePoint;

    public Animator playerAnim;

    public Gun[] guns;
    private int selectedGunIndex;
    private float weaponCooldownTime = Mathf.Infinity;
    Coroutine firingCo;
    public bool isAiming = false;
    public float standardCameraSensitivity = 20;
    private bool isReloading = false;

    public GameObject playerBodyOverNetwork;
    public GameObject playerBodyLocal;

    public AmmoPickupSpawner[] spawners;

    public int kills;
    public int deaths;

    public AudioSource audioSource;

    public static PlayerController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainCam = Camera.main;

        spawners = FindObjectsOfType<AmmoPickupSpawner>();

        SetPlayerSpawnInfo();

#if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
#endif
    }

    private void SetPlayerSpawnInfo()
    {
        if (photonView.IsMine)
        {
            Camera.main.transform.position = camPosition.position;
            Camera.main.transform.rotation = camPosition.rotation;

            playerBodyLocal.SetActive(true);
            playerBodyOverNetwork.SetActive(false);

            currentHealthPoints = maxHealthPoints;

            HUDController.instance.healthText.text = currentHealthPoints.ToString();

            foreach (Gun gun in guns)
            {
                gun.LoadAmmo();
            }

            EquipWeapon(0);

            WeaponSwitcherUI.instance.UpdateSlotSwitcherInfo(0);
        }
        else
        {
            playerBodyLocal.SetActive(false);
            playerBodyOverNetwork.SetActive(true);
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (isDead) return;

        MouseInputHandler();

        MovementHandler();

        weaponCooldownTime += Time.deltaTime;

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (HUDController.instance.leaderBoardPanel.activeInHierarchy)
            {
                HideLeaderBoard();
            }
            else
            {
                ShowLeaderBoard();
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage("", 100);
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!isAiming)
            {
                Aim();
            }
            else
            {
                ReleaseAim();
            }
        }
#endif

    }

    private void LateUpdate()
    {
        if (!photonView.IsMine) return;

        if (isDead) return;

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

        if (MatchManager.instance.isMatchEnded) return;

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

    public void Aim()
    {
        mainCam.fieldOfView = guns[selectedGunIndex].weaponFieldOfView;

        bl_MobileInput.TouchPadSensitivity = bl_MobileInput.TouchPadSensitivity / guns[selectedGunIndex].touchpadFactorWhenAiming;

        isAiming = true;
    }

    public void ReleaseAim()
    {
        mainCam.fieldOfView = 60f;

        bl_MobileInput.TouchPadSensitivity = standardCameraSensitivity;

        isAiming = false;
    }

    private void MovementHandler()
    {
        if (MatchManager.instance.isMatchEnded) return;

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

        float baseMovementSpeed = 0;

        if (isAiming)
        {
            baseMovementSpeed = movementSpeed / guns[selectedGunIndex].movespeedFactorWhenAiming;
        }
        else
        {
            baseMovementSpeed = movementSpeed;
        }

        charController.Move(movement * baseMovementSpeed * Time.deltaTime);
    }

    public void Jump()
    {
        if (!isGrounded) return;

        movement.y = jumpHeight;

        playerAnim.SetBool("isGrounded", false);

        charController.Move(movement * movementSpeed * Time.deltaTime);
    }

    private void Shoot() // using mouse
    {
        if (isReloading) return;

        if (guns[selectedGunIndex].GetCurrentAmmo() <= 0)
        {
            if (guns[selectedGunIndex].GetCurrentAmmoReserve() <= 0)
            {
                HUDController.instance.ShowWarningText("No more ammo left!", 2f, Color.red);
            }
            else
            {
                Reload();
            }

            return;
        }

        audioSource.PlayOneShot(guns[selectedGunIndex].shootSFX);

        guns[selectedGunIndex].ReduceCurrentAmmo();

        guns[selectedGunIndex].ShowAmmoDisplay();

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
            if (hit.collider.gameObject.tag == "Player")
            {
                PhotonNetwork.Instantiate(bulletImpactBodyVFX.name,
                hit.point + (hit.normal * 0.003f),
                Quaternion.LookRotation(hit.normal,
                Vector3.up));

                hit.collider.GetComponent<PhotonView>().RPC("RpcDealDamage",
                    RpcTarget.All,
                    photonView.Owner.NickName,
                    guns[selectedGunIndex].damage);
            }
            else
            {
                PhotonNetwork.Instantiate(bulletImpactVFX.name,
                hit.point + (hit.normal * 0.003f),
                Quaternion.LookRotation(hit.normal,
                Vector3.up));
            }
        }

        weaponCooldownTime = 0;
    }

    private IEnumerator KeepShooting() // using UI buttons
    {
        while (true)
        {
            if (isReloading) break;

            if (guns[selectedGunIndex].GetCurrentAmmo() <= 0)
            {
                if (guns[selectedGunIndex].GetCurrentAmmoReserve() <= 0)
                {
                    HUDController.instance.ShowWarningText("No more ammo left!", 2f, Color.red);
                }
                else
                {
                    Reload();
                }

                break;
            }

            audioSource.PlayOneShot(guns[selectedGunIndex].shootSFX);

            guns[selectedGunIndex].ReduceCurrentAmmo();

            guns[selectedGunIndex].ShowAmmoDisplay();

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
                if (hit.collider.gameObject.tag == "Player")
                {
                    PhotonNetwork.Instantiate(bulletImpactBodyVFX.name,
                    hit.point + (hit.normal * 0.003f),
                    Quaternion.LookRotation(hit.normal,
                    Vector3.up));

                    hit.collider.GetComponent<PhotonView>().RPC("RpcDealDamage",
                        RpcTarget.All,
                        photonView.Owner.NickName,
                        guns[selectedGunIndex].damage);
                }
                else
                {
                    PhotonNetwork.Instantiate(bulletImpactVFX.name,
                    hit.point + (hit.normal * 0.003f),
                    Quaternion.LookRotation(hit.normal,
                    Vector3.up));
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
        if (isAiming)
        {
            ReleaseAim();
        }

        selectedGunIndex = gunIndex;

        foreach (Gun gun in guns)
        {
            gun.gameObject.SetActive(false);
        }

        guns[selectedGunIndex].gameObject.SetActive(true);

        guns[selectedGunIndex].ShowAmmoDisplay();
    }

    public void SwitchWeaponOnRight()
    {
        if (isReloading) return;

        selectedGunIndex += 1;

        if (selectedGunIndex > guns.Length - 1)
        {
            selectedGunIndex = 0;
        }

        EquipWeapon(selectedGunIndex);
        WeaponSwitcherUI.instance.UpdateSlotSwitcherInfo(selectedGunIndex);

        guns[selectedGunIndex].ShowAmmoDisplay();
    }

    public void SwitchWeaponOnLeft()
    {
        if (isReloading) return;

        selectedGunIndex -= 1;

        if (selectedGunIndex < 0)
        {
            selectedGunIndex = guns.Length - 1;
        }

        EquipWeapon(selectedGunIndex);
        WeaponSwitcherUI.instance.UpdateSlotSwitcherInfo(selectedGunIndex);

        guns[selectedGunIndex].ShowAmmoDisplay();
    }

    public void Reload()
    {
        if (isReloading) return;

        if (guns[selectedGunIndex].IsMagazineFull()) return;

        if (guns[selectedGunIndex].GetCurrentAmmoReserve() <= 0 && guns[selectedGunIndex].GetCurrentAmmo() <= 0)
        {
            HUDController.instance.ShowWarningText("No more ammo to Reload!", 2f, Color.red);

            return;
        }

        if (guns[selectedGunIndex].GetCurrentAmmoReserve() <= 0) return;

        audioSource.PlayOneShot(guns[selectedGunIndex].reloadSFX);

        StartCoroutine(ReloadCor());
    }

    private IEnumerator ReloadCor()
    {
        isReloading = true;

        HUDController.instance.ShowWarningText("Reloading...", guns[selectedGunIndex].reloadingTime, Color.white);

        HUDController.instance.ShowReloadingFillBar(guns[selectedGunIndex].reloadingTime);

        yield return new WaitForSecondsRealtime(guns[selectedGunIndex].reloadingTime);

        guns[selectedGunIndex].Reload();

        guns[selectedGunIndex].ShowAmmoDisplay();

        isReloading = false;
    }

    public void ShowLeaderBoard()
    {
        HUDController.instance.leaderBoardPanel.SetActive(true);
    }

    public void HideLeaderBoard()
    {
        HUDController.instance.leaderBoardPanel.SetActive(false);
    }

    [PunRPC]
    public void RPCAddAmmo(AmmoType ammoType, int amount)
    {
        if (!photonView.IsMine) return;

        foreach (Gun gun in guns)
        {
            if (gun.ammoType == ammoType)
            {
                gun.AddAmmoReserve(amount);
            }
        }

        HUDController.instance.ShowWarningText("Picked up " + ammoType + " Ammo", 3f, Color.white);

        guns[selectedGunIndex].ShowAmmoDisplay();
    }

    [PunRPC]
    public void RPCDestroyPickup(int spawnerIndex)
    {
        foreach (AmmoPickupSpawner spawner in spawners)
        {
            if (spawner.spawnerIndex == spawnerIndex)
            {
                spawner.DestroyPickup();
                break;
            }
        }
    }

    [PunRPC]
    public void RpcDealDamage(string damager, int damage)
    {
        TakeDamage(damager, damage);
    }

    private void TakeDamage(string damager, int damage)
    {
        if (!photonView.IsMine) return;

        if (isDead) return;

        currentHealthPoints -= damage;

        if (currentHealthPoints <= 0)
        {
            currentHealthPoints = 0;

            isDead = true;

            photonView.RPC("RpcDie", RpcTarget.All);

            PlayerSpawner.instance.PlayerDie();

            MatchManager.instance.UpdatePlayerInfoSend(damager, 0, 1);
        }

        HUDController.instance.healthText.text = currentHealthPoints.ToString();
    }

    [PunRPC]
    public void RpcDie()
    {
        ChangeCameraFocus();

        playerBodyLocal.SetActive(false);
        playerBodyOverNetwork.SetActive(true);

        playerAnim.SetTrigger("Dead");

        if (photonView.IsMine)
        {
            HUDController.instance.warningText.text = "";
        }
    }

    private void ChangeCameraFocus()
    {
        if (!photonView.IsMine) return;

        mainCam.transform.position = camPointWhenDead.position;
        mainCam.transform.rotation = camPointWhenDead.rotation;
    }

    [PunRPC]
    public void RpcRespawn(Vector3 position, Quaternion rotation)
    {
        isDead = false;

        charController.enabled = false;
        charController.transform.position = position;
        charController.enabled = true;

        transform.rotation = rotation;

        playerAnim.SetTrigger("Alive");
        
        if (!photonView.IsMine)
        {
            playerBodyOverNetwork.SetActive(true);
        }

        if (photonView.IsMine)
        {
            playerBodyLocal.SetActive(true);
            playerBodyOverNetwork.SetActive(false);
        }

        SetPlayerSpawnInfo();
    }

    [PunRPC]
    public void RpcDisablePlayerBodyOverNetwork()
    {
        if (photonView.IsMine) return;

        playerBodyOverNetwork.SetActive(false);
    }
}

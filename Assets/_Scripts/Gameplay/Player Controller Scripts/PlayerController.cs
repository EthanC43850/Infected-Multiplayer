using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using System.Collections;

public class PlayerController : Targetable, IDamageable
{

    #region Variables

    public static bool debugMode = false;

    [Header("Player Properties")]
    public int maxHealth = 100;
    public int currentHealth;
    public WorldSpacePlayerUI worldSpaceUI;     // AI needs this too
    public Item[] items;
    public bool isAiming;


    [Header("Physics Properties:")]
    public float speed;
    public float jumpHeight = 3;
    public float gravity = -9.81f;
    [Tooltip("How far the player is from the ground to be considered grounded")]
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;


    [Header("Connections:")]
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject playerUI;       // Eventually delete and start calling worldspaceUI.gameobject in functions
    [SerializeField] Transform cam;
    [SerializeField] CinemachineVirtualCamera birdEyeCam;
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] Collider playerHitBox;
    public Animator playerAnimator;
    [HideInInspector] public PlayerManager playerManager;


    [Header("Helper Variables")]
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity = 1f;
    private Vector3 velocity;
    private Vector3 currentMoveInput;
    private Vector3 smoothInputVelocity;
    private float smoothInputSpeed = 0.1f;
    private int previousItemIndex = -1;
    private float grenadeThrowDistance;
    private Vector3 airStrikePositionInput;
    [HideInInspector] public int currentItemIndex;
    [HideInInspector] public Vector3 moveInput;
    [HideInInspector] public AirstrikePhone airstrikePhone;


    #endregion


    #region Monobehaviours


    public override void Awake()
    {
        base.Awake();

        if (!debugMode)
        {
            playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>(); // InstantiationData [0] is simply the view ID which was passed
        }

    } // END Awake


    private void Start()
    {
        currentHealth = maxHealth;

        worldSpaceUI.SetHealthBarMax(maxHealth);

        if (PV.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<CinemachineVirtualCamera>().gameObject);     //Ensure that each player is using their correct camera

            Destroy(GetComponentInChildren<PlayerInput>());                             //Allows controllers to work when multiple people connect  
        }

    } // END Start


    private void Update()
    {

        #region Player Movement

        if (!PV.IsMine)
        {
            return;
        }


        //jump
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //only apply gravity when grounded
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;

        }


        controller.Move(velocity * Time.deltaTime);

        if (isDead) // Stop all movement when dead
        {
            return;
        }

        currentMoveInput = Vector3.SmoothDamp(currentMoveInput, moveInput, ref smoothInputVelocity, smoothInputSpeed);
        Vector3 desiredDirection = cam.forward * currentMoveInput.z + cam.right * currentMoveInput.x;
        AnimateThePlayer(desiredDirection);

        if (moveInput.magnitude >= 0.1f && !isAiming)
        {
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection * speed * Time.deltaTime);




        }
        else if (isAiming)  // Keep track of the direction of projectiles 
        {
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (moveInput.magnitude >= 0.1f)
            {
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDirection * speed * Time.deltaTime);
            }

            // if aiming with grenade (should i move this into a recursive coroutine instead to clean up update?)
            Grenade grenade = items[currentItemIndex].GetComponent<Grenade>();
            if (grenade != null)
            {

                grenade.throwDistance += grenadeThrowDistance;
                grenade.throwDistance = Mathf.Clamp(grenade.throwDistance, grenade.minThrowDistance, grenade.maxThrowDistance);

            }

            AirstrikePhone airStrike = items[currentItemIndex].GetComponent<AirstrikePhone>();
            if (airStrike != null)
            {

                //float airStrikeTargetAngle = Mathf.Atan2(airStrikePositionInput.x, airStrikePositionInput.z) * Mathf.Rad2Deg + birdEyeCam.transform.eulerAngles.y;
                //float airStrikeAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

                Vector3 airStrikeIndicatorMoveDirection = new Vector3(-airStrikePositionInput.x, 0f, -airStrikePositionInput.z);
                if (airStrikeIndicatorMoveDirection.magnitude >= 0.1)
                {
                    airStrike.airStrikeIndicator.transform.Translate(airStrikeIndicatorMoveDirection * airStrike.indicatorMoveSpeed * Time.deltaTime);
                }
                //airStrike.airStrikeIndicatorTransform.Translate



            }


        }


        #endregion

        if (transform.position.y < -5f) // Fell to death
        {
            Die();
        }

    } // END Update


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

        // Call this function across all devices and make sure that only this specific player's gun switches, not everybody else's
        if (!PV.IsMine && targetPlayer == PV.Owner && changedProps["itemIndex"] != null)
        {
            //Debug.Log("Player item changed!");
            EquipItem((int)changedProps["itemIndex"]);
        }

        if (!PV.IsMine && targetPlayer == PV.Owner && changedProps["DeathState"] != null) // Make sure this specific players UI and hitbox is turned off
        {
            playerUI.SetActive(false);
            playerHitBox.enabled = false;
        }

        if (!PV.IsMine && targetPlayer == PV.Owner && changedProps["airstrikeActive"] != null)
        {
            //Debug.Log("AIR STRIKE BROADCASTED ACROSS NETWORK");
            airstrikePhone.airStrikeIndicator.SetActive((bool)changedProps["airstrikeActive"]);
            airstrikePhone.airStrikeIndicator.transform.position = transform.position;
        }


    } // END OnPlayerPropertiesUpdate


    #endregion


    #region Methods

    /* public void OnEvent(EventData photonEvent)
     {

         byte eventCode = photonEvent.Code;
         if (eventCode == OnAirstrikeAimEventCode)
         {

             object[] data = (object[])photonEvent.CustomData;
             airstrikePhone.airStrikeIndicator.SetActive((bool)data[0]);
             airstrikePhone.airStrikeIndicator.transform.position = transform.position;
         }

     } // END OnEvent*/


    //-------------------------------------------//
    public virtual void AnimateThePlayer(Vector3 desiredDirection)
    {
        Vector3 movement = new Vector3(desiredDirection.x, 0f, desiredDirection.z);
        float forw = Vector3.Dot(movement, transform.forward);
        float stra = Vector3.Dot(movement, transform.right);

        playerAnimator.SetFloat("Forward", forw);
        playerAnimator.SetFloat("Strafe", stra);


    } // END AnimatePlayer


    //-------------------------------------------//
    private void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
        {
            return;     // Prevent deactiving the same item twice
        }

        currentItemIndex = _index;
        //Debug.Log("About to equip item " + items[currentItemIndex].itemGameObject.name);
        items[currentItemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = currentItemIndex;

        //Sync gun switch for all players to see
        if (PV.IsMine && !debugMode)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", currentItemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        }

    } // END EqupItem


    //-------------------------------------------//
    public void TakeDamage(int damage)
    {
        if (debugMode)
        {
            currentHealth -= damage;
            worldSpaceUI.UpdateHealthUI(currentHealth);
            worldSpaceUI.DisplayFloatingText(damage);
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        else
        {
            PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);   // Signals to all that this specific PV owner is taking damage, (The shooter runs this call)
        }

    } // END TakeDamage


    //-------------------------------------------//
    [PunRPC]
    public void RPC_TakeDamage(int damage, PhotonMessageInfo info)
    {

        currentHealth -= damage; 
        worldSpaceUI.UpdateHealthUI(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();  // Find the playermanager associated with the player who sent this RPC, then call getkill on this playermanager;
        }

    } // END RPC_TakeDamage


    //-------------------------------------------//
    public void Die()
    {
        if (debugMode == false && isDead == false) // Online 
        {
            isDead = true;
            playerAnimator.SetTrigger("IsDead");
            playerUI.SetActive(false);


            // Disable this target players UI over clients 
            Hashtable hash = new Hashtable();
            hash.Add("DeathState", true); // throwing random value to key
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            StartCoroutine(IWaitToRespawn());

        }
        else if (isDead == false)
        {

            isDead = true;
            playerAnimator.SetTrigger("IsDead");
            playerUI.SetActive(false);
        }


        playerHitBox.enabled = false;



    } // END Die


    


    //-------------------------------------------//
    IEnumerator IWaitToRespawn()
    {
        yield return new WaitForSeconds(3.0f);
        playerManager.Die();



    }


    #endregion


    #region Player Input Events

    //-------------------------------------------//
    public void OnMove(InputAction.CallbackContext value)
    {
        if (!PV.IsMine) { return; }

        Vector2 inputMovement = value.ReadValue<Vector2>();
        moveInput = new Vector3(inputMovement.x, 0, inputMovement.y).normalized; // Normalize is necessary to make sure object does not speed up when two keys are pressed 
                                                                                 // to go diagonally

    } // END OnMovement


    //-------------------------------------------//
    public void OnSprint(InputAction.CallbackContext value)
    {
        if (!PV.IsMine) { return; }

        if (value.started)
        {
            speed *= 2;
        }
        else if (value.canceled)
        {
            speed /= 2;
        }

    } // END OnSprint


    //-------------------------------------------//
    public void OnJump(InputAction.CallbackContext value)
    {
        if (!PV.IsMine) { return; }

        if (value.started && isGrounded)
        {
            Debug.Log("JUMP!");
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

        }

    } // END OnJump


    //-------------------------------------------//
    public void OnAim(InputAction.CallbackContext value)
    {

        Grenade grenade = items[currentItemIndex].GetComponent<Grenade>();
        AirstrikePhone _airStrikePhone = items[currentItemIndex].GetComponent<AirstrikePhone>();

        if (value.started)
        {
            isAiming = true;
            if (grenade != null)
            {
                Debug.Log("TURN ON GRENADE");
                grenade.drawProjectionScript.lineRenderer.enabled = true;
            }
            if (_airStrikePhone != null)
            {
                if (PlayerController.debugMode == false)
                {
                    /*airstrikePhone = _airStrikePhone;
                    object[] content = new object[] { true };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(OnAirstrikeAimEventCode, content, raiseEventOptions, SendOptions.SendReliable);*/

                    _airStrikePhone.airStrikeIndicator.SetActive(true);
                    _airStrikePhone.airStrikeIndicator.transform.position = transform.position;
                    // Project to all other users that this specific user is aiming an airstrike
                    if (PV.IsMine && !debugMode)
                    {
                        Hashtable hash = new Hashtable();
                        hash.Add("airstrikeActive", true);
                        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                    }
                }
                else
                {
                    _airStrikePhone.airStrikeIndicator.SetActive(true);
                    _airStrikePhone.airStrikeIndicator.transform.position = transform.position;
                }

                birdEyeCam.Priority = 11;

            }

        }

        if (value.canceled)
        {

            isAiming = false;
            if (grenade != null)
            {
                grenade.drawProjectionScript.lineRenderer.enabled = false;
            }
            if (_airStrikePhone != null)
            {

                if (PlayerController.debugMode == false)
                {
                    /*airstrikePhone = _airStrikePhone;
                    object[] content = new object[] { false };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(OnAirstrikeAimEventCode, content, raiseEventOptions, SendOptions.SendReliable);*/

                    _airStrikePhone.airStrikeIndicator.SetActive(false);
                    _airStrikePhone.airStrikeIndicator.transform.position = transform.position;

                    if (PV.IsMine && !debugMode)
                    {
                        Hashtable hash = new Hashtable();
                        hash.Add("airstrikeActive", false);
                        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                    }
                }
                else
                {
                    _airStrikePhone.airStrikeIndicator.gameObject.SetActive(false);
                }
                birdEyeCam.Priority = 9;
            }

        }


    } // END OnAim


    //-------------------------------------------//
    public void SwitchToPrimaryItem(InputAction.CallbackContext value)
    {
        if (!PV.IsMine) { return; }
        EquipItem(0);


    } // END SwitchToPrimaryItem


    //-------------------------------------------//
    public void SwitchToSecondaryItem(InputAction.CallbackContext value)
    {
        if (!PV.IsMine) { return; }
        EquipItem(1);


    } // END SwitchToSecondaryItem


    //-------------------------------------------//
    public virtual void OnAttack(InputAction.CallbackContext value)
    {
        if (!PV.IsMine) { return; }

        if (value.performed)
        {

            items[currentItemIndex].Use();
        }




    } // END OnAttack


    //-------------------------------------------//
    public void OnGrenadeAim(InputAction.CallbackContext value) // This function is controlled by the right stick / mouse
    {
        // Increase denominator to slow down the change in speed of the line renderer

        grenadeThrowDistance = value.ReadValue<Vector2>().y / 8;



    } // END OnGrenadeAim


    //-------------------------------------------//
    public void OnAirstrikeAim(InputAction.CallbackContext value)
    {
        Vector2 airStrikeInputMovement = value.ReadValue<Vector2>();
        airStrikePositionInput = new Vector3(airStrikeInputMovement.x, 0, airStrikeInputMovement.y).normalized;

    } // END OnAirstrikeAim


    //-------------------------------------------//
    public void ToggleScoreboard(InputAction.CallbackContext value)
    {
        Vector2 airStrikeInputMovement = value.ReadValue<Vector2>();
        airStrikePositionInput = new Vector3(airStrikeInputMovement.x, 0, airStrikeInputMovement.y).normalized;



    } // END OnAirstrikeAim

    #endregion


} // END Class

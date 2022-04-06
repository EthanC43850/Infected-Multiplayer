using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{

    #region Variables

    public static bool debugMode = false;

    public const byte OnAirstrikeAimEventCode = 1;

    [Header("Player Properties")]
    public int maxHealth = 100;
    public int currentHealth;
    public Item[] items;
    public bool isAiming;


    [Header("Physics Properties:")]
    public float speed;
    public float jumpHeight = 3;
    public float gravity = -9.81f;
    private bool isGrounded;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;


    [Header("Object Assignments:")]
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject playerUI;
    [SerializeField] Transform cam;
    [SerializeField] CinemachineVirtualCamera birdEyeCam;
    [SerializeField] Transform groundCheck;


    [Header("Scripts")]
    public WorldSpacePlayerUI worldSpaceUI;
    private PlayerManager playerManager;



    [HideInInspector] public PhotonView PV;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity = 1f;
    private Vector3 moveInput;
    private Vector3 velocity;
    private int currentItemIndex;
    private int previousItemIndex = -1;

    private float grenadeThrowDistance;
    private Vector3 airStrikePositionInput;

    [HideInInspector]
    public AirstrikePhone airstrikePhone;

    #endregion


    #region Monobehaviours

    /*private new void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private new void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }*/

    public void Awake()
    {
        PV = GetComponent<PhotonView>();

        if (!debugMode)
        {
            playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        }
        

    } //END Awake

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

            Destroy(playerUI);



        }

    } // END Start


    private void Update()
    {
        #region Player Movement
        //jump
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (moveInput.magnitude >= 0.1f && !isAiming)
        {
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection * speed * Time.deltaTime);


        }
        else if (isAiming)
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
            if(airStrike != null)
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

        if (transform.position.y < -70f)
        {
            Die();
        }

    } // END Update


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        
        // Call this function across all devices and make sure that only this specific player's gun switches, not everybody else's
        if (!PV.IsMine && targetPlayer == PV.Owner && changedProps["itemIndex"] != null)
        {
            Debug.Log("Player item changed!");
            EquipItem((int)changedProps["itemIndex"]);


        }



        if (!PV.IsMine && targetPlayer == PV.Owner && changedProps["airstrikeActive"] != null)
        {
            Debug.Log("AIR STRIKE BROADCASTED ACROSS NETWORK");
            airstrikePhone.airStrikeIndicator.SetActive((bool)changedProps["airstrikeActive"]);
            airstrikePhone.airStrikeIndicator.transform.position = transform.position;

        }


    } // END OnPlayerPropertiesUpdate


    private void OnTriggerEnter(Collider other)
    {
        // IMPORTANT: This function will run as many times as there are players in the game. The collider on my local computer && all other clients
        // In this example, a player will take 60 damage (20 * 3) if there are 3 clients
        /*if (other.gameObject.GetComponent<Spikes>())
        {
            Debug.Log("Stepped on spikes");
            TakeDamage(20);
            //worldSpaceUI.DisplayFloatingText(20);
        }*/
    }

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


    private void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
        {
            return;     // Prevent deactiving the same item twice
        }

        currentItemIndex = _index;
        Debug.Log("About to equip item " + items[currentItemIndex].itemGameObject.name);
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

            PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
            worldSpaceUI.networkLerpTimer = 0;
            
            
        }

        
    } // END TakeDamage


    [PunRPC]
    public void RPC_TakeDamage(int damage)
    {

        if (!PV.IsMine) { return; }         // Pun RPC runs on everyones computer, but the !pv.ismine makes sure the function only runs on the victims computer

        currentHealth -= damage;    // In the multiplayer tutorial, this function is placed after !PV.IsMine. Keep this in mind if problems arise in the future.

        worldSpaceUI.UpdateHealthUI(currentHealth);


        if (currentHealth <= 0)
        {
            Die();
        }

    } // END RPC_TakeDamage


    void Die()
    {
        playerManager.Die();

    } // END Die

    #endregion



    #region Player Actions
    public void OnMove(InputAction.CallbackContext value)
    {
        if (!PV.IsMine) { return; }
        
        Vector2 inputMovement = value.ReadValue<Vector2>();
        moveInput = new Vector3(inputMovement.x, 0, inputMovement.y).normalized; // Normalize is necessary to make sure object does not speed up when two keys are pressed 
                                                                                 // to go diagonally
    } // END OnMovement


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


    public void OnJump(InputAction.CallbackContext value)
    {
        if (!PV.IsMine) { return; }

        if (value.started && isGrounded)
        {
            Debug.Log("JUMP!");
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

        }

    } // END OnJump


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
            if(grenade != null)
            {
                grenade.drawProjectionScript.lineRenderer.enabled = false;
            }
            if(_airStrikePhone != null)
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


    public void SwitchToPrimaryItem(InputAction.CallbackContext value)
    {
        if (!PV.IsMine) { return; }
        EquipItem(0);


    } // END SwitchToPrimaryItem

    public void SwitchToSecondaryItem(InputAction.CallbackContext value)
    {
        if (!PV.IsMine) { return; }
        EquipItem(1);


    } // END SwitchToSecondaryItem


    public void OnAttack(InputAction.CallbackContext value)
    {
        if (!PV.IsMine) { return; }

        if (value.started)
        {

            items[currentItemIndex].Use();
        }

        

    } // END OnAttack


    // This function is controlled by the right stick / mouse
    public void OnGrenadeAim(InputAction.CallbackContext value)
    {
        // Increase denominator to slow down the change in speed of the line renderer
        
        //Debug.Log("grenade is aiming and value is " + value.ReadValue<Vector2>().y);
        grenadeThrowDistance = value.ReadValue<Vector2>().y / 8;



    } // END OnGrenadeAim


    public void OnAirstrikeAim(InputAction.CallbackContext value)
    {
        Vector2 airStrikeInputMovement = value.ReadValue<Vector2>();
        airStrikePositionInput = new Vector3(airStrikeInputMovement.x, 0, airStrikeInputMovement.y).normalized;



    } // END OnAirstrikeAim

    #endregion





}

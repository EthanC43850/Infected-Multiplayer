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

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable, IPunObservable
{

    #region Variables

    public bool debugMode;
    [Header("Player Properties")]
    const int maxHealth = 100;
    public int currentHealth = maxHealth;
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
    [SerializeField] Text userNameText;
    [SerializeField] Slider frontHealthBar_Slider;
    [SerializeField] Slider backHealthBar_Slider;
    [SerializeField] Image frontHealthBar_Fill;
    [SerializeField] Image backHealthBar_Fill;
    [SerializeField] GameObject floatingText;
    [SerializeField] GameObject worldCanvas;
    [SerializeField] CharacterController controller;
    [SerializeField] Transform cam;
    [SerializeField] Transform groundCheck;

    private PhotonView PV;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity = 1f;
    private Vector3 moveInput;
    private Vector3 velocity;

    private float changeInHealthBarDuration = 2.0f;
    private int correctHealthAmt = 0;
    private float networkLerpTimer;

    private int currentItemIndex;
    private int previousItemIndex = -1;

    private PlayerManager playerManager;

    private bool isDamaged = false;
    private int damageTaken;

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (debugMode) { return; }

        Debug.Log("sending sending sending");
        // Write
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
        }
        // Read
        else
        {
            correctHealthAmt = (int)stream.ReceiveNext();
        }
    }
    


    #region Monobehaviours
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
        if (!debugMode)
        {
            userNameText.text = PV.Owner.NickName;
        }

        SetHealthBarMax(maxHealth);

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

    private void UpdateNetworkHealthBars()
    {
        float fillFront = frontHealthBar_Slider.value;
        float fillBack = backHealthBar_Slider.value;

        if(currentHealth == maxHealth)
        {
            frontHealthBar_Slider.value = maxHealth;    //Without this, theres a weird bug where player spawns without front health bar
        }

        networkLerpTimer += Time.deltaTime;
        if (fillBack > currentHealth)
        {
            if (isDamaged)
            {
                GameObject _floatingText = Instantiate(floatingText, transform.position, Quaternion.identity, worldCanvas.transform);
                TextMeshProUGUI textAttributes = _floatingText.GetComponent<TextMeshProUGUI>();
                textAttributes.text = damageTaken.ToString();
                if (damageTaken >= 40)
                {
                    textAttributes.color = Color.red;
                }

                _floatingText.transform.localRotation = Quaternion.Euler(0, 0, 0);

                Destroy(_floatingText, 1.0f);
                isDamaged = false;
            }
            


            backHealthBar_Fill.color = Color.white;
            frontHealthBar_Slider.value = currentHealth;
            float percentComplete = networkLerpTimer / changeInHealthBarDuration;
            percentComplete *= percentComplete;           // The more you square this, the slower health will drop

            backHealthBar_Slider.value = Mathf.Lerp(fillBack, currentHealth, percentComplete);
        }
    }


    private void Update()
    {
        if (!PV.IsMine) {
            currentHealth = correctHealthAmt;
            UpdateNetworkHealthBars();

            return; 
        }

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
        }


        if(transform.position.y < -70f)
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

        if (!PV.IsMine && targetPlayer == PV.Owner && changedProps["currentHealth"] != null)
        {
            Debug.Log("Player health changed!");
            UpdateHealthUI((int)changedProps["currentHealth"]);


        }

        if (!PV.IsMine && targetPlayer == PV.Owner && changedProps["setHealthMax"] != null)
        {
            SetHealthBarMax((int)changedProps["setHealthMax"]);
        }

    } // END OnPlayerPropertiesUpdate


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Spikes>())
        {
            Debug.Log("Stepped on spikes");
            DisplayFloatingText(20);
            TakeDamage(20);
        }
    }



    #endregion


    #region Methods
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
            UpdateHealthUI(currentHealth);
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        else
        {
            PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
            networkLerpTimer = 0;
        }

        
    } // END TakeDamage


    [PunRPC]
    public void RPC_TakeDamage(int damage)
    {
        if (!PV.IsMine) { return; }         // Makes sure the function only runs on the victims computer

        currentHealth -= damage;

        
        UpdateHealthUI(currentHealth);

        //Sync health bar for all players to see

        Hashtable hash = new Hashtable();
        hash.Add("currentHealth", currentHealth);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        
        

        if (currentHealth <= 0)
        {
            Die();
        }

    } // END RPC_TakeDamage


    void Die()
    {
        playerManager.Die();

    } // END Die


    private void SetHealthBarMax(int _maxHealth)
    {

        frontHealthBar_Slider.maxValue = _maxHealth;
        frontHealthBar_Slider.value = _maxHealth;
        backHealthBar_Slider.maxValue = _maxHealth;
        backHealthBar_Slider.value = _maxHealth;

        if (PV.IsMine && !debugMode)
        {
            Hashtable hash = new Hashtable();
            hash.Add("setHealthMax", _maxHealth);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        

    } // END SetMaxHealth

    
    public void UpdateHealthUI(int _currentHealth)
    {
        Debug.Log("Update Health");
        /*if (!PV.IsMine)
        {
            Instantiate(floatingText, transform.position + Vector3.up, Quaternion.identity, );

        }*/
        // frontHealthBar_Slider.value = _currentHealth;            // No longer need this because I now update other players health bars through Update function
        
        // PUN REFUSES TO RUN THE COROUTINES. IS THERE A REASON WHY?
        StopAllCoroutines();        
        StartCoroutine(ChangeHealthValue());

    } // END SetHealthBar

    //This is called through the network to better sync numbers and health bars
    public void DisplayFloatingText(int damage) {
        isDamaged = true;
        damageTaken = damage;


        // For testing different positions of floating numbers
        GameObject _floatingText = Instantiate(floatingText, transform.position, Quaternion.identity, worldCanvas.transform);
        TextMeshProUGUI textAttributes = _floatingText.GetComponent<TextMeshProUGUI>();
        textAttributes.text = damageTaken.ToString();
        if (damageTaken >= 40)
        {
            textAttributes.color = Color.red;
        }

        _floatingText.transform.localRotation = Quaternion.Euler(0, 0, 0);

        //Destroy(_floatingText, 1.0f);
    }

    IEnumerator ChangeHealthValue()
    {
        Debug.Log("Entered Coroutine");
        float lerpTimer = 0;
        

        while (lerpTimer < changeInHealthBarDuration)
        {

            float fillFront = frontHealthBar_Slider.value;
            float fillBack = backHealthBar_Slider.value;

            lerpTimer += Time.deltaTime;
            // If lose health
            if (fillBack > currentHealth)
            {
                
                backHealthBar_Fill.color = Color.white;
                frontHealthBar_Slider.value = currentHealth;
                float percentComplete = lerpTimer / changeInHealthBarDuration;
                percentComplete *= percentComplete; //* percentComplete;           // The more you square this, the slower health will drop
                
                backHealthBar_Slider.value = Mathf.Lerp(fillBack, currentHealth, percentComplete);

            }

            // For Healing
            /*if (fillFront < hFraction)
            {
                backHealthBar_Fill.color = Color.green;
                backHealthBar_Slider.value = hFraction;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / changeInHealthBarDuration;
                percentComplete *= percentComplete;
                frontHealthBar_Slider.value = Mathf.Lerp(fillFront, backHealthBar_Slider.value, percentComplete);

            }*/

            //backHealthBar_Slider.value = hFraction;

            //isRunning = false;
            yield return null;
        }
        Debug.Log("While loop has broken!");




    } // END ChangeHealthValue

    



    #endregion


    #region Player Actions
    public void OnMove(InputAction.CallbackContext value)
    {
        if (!PV.IsMine) { return; }
        
        Vector2 inputMovement = value.ReadValue<Vector2>();
        moveInput = new Vector3(inputMovement.x, 0, inputMovement.y).normalized; // Is normalize necessary?

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
        if (!PV.IsMine) { return; }


        if (value.started)
        {
            isAiming = true;
        }
        else if (value.canceled)
        {
            isAiming = false;
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


    #endregion

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class WorldSpacePlayerUI : MonoBehaviourPunCallbacks
{

    #region Variables

    [Header("Player Connections")]
    //[SerializeField] PlayerController player;
    [SerializeField] Text userNameText;
    [SerializeField] GameObject floatingText;
    [SerializeField] Gradient floatingTextGradient;
    [SerializeField] GameObject worldCanvas;

    [Header("Healthbar Connections")]
    [SerializeField] Slider frontHealthBar_Slider;
    [SerializeField] Slider backHealthBar_Slider;
    [SerializeField] Image frontHealthBar_Fill;
    [SerializeField] Image backHealthBar_Fill;


    private float changeInHealthBarDuration = 2.0f;
    private int currentHealth;
    private int damageTaken;

    PhotonView pv;

    #endregion


    #region Monobehaviors

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        PlayerController player = gameObject.GetComponentInParent<PlayerController>();

        if (!PlayerController.debugMode && player != null)
        {
            userNameText.text = player.PV.Owner.NickName;
        }

    } // END Start

    #endregion

    #region Methods

    //-------------------------------------------//
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)  
    {
        // This changes the health values of the target player in the lobby
        if (!pv.IsMine && targetPlayer == pv.Owner && changedProps["currentHealth"] != null)
        {
            UpdateHealthUI((int)changedProps["currentHealth"]);

            StopAllCoroutines();

            StartCoroutine(LerpHealthValue());

        }

        if (!pv.IsMine && targetPlayer == pv.Owner && changedProps["setHealthMax"] != null)
        {
            SetHealthBarMax((int)changedProps["setHealthMax"]);
        }

    } // END OnPlayerPropertiesUpdate


    public void SetHealthBarMax(int _maxHealth)
    {
        //Debug.Log("Max health is being set to " + _maxHealth);
        frontHealthBar_Slider.maxValue = _maxHealth;
        frontHealthBar_Slider.value = _maxHealth;
        backHealthBar_Slider.maxValue = _maxHealth;
        backHealthBar_Slider.value = _maxHealth;

        if (gameObject.GetComponent<PlayerController>() == null) { return; }

        if (pv.IsMine && !PlayerController.debugMode)
        {
            Hashtable hash = new Hashtable();
            hash.Add("setHealthMax", _maxHealth);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

    } // END SetMaxHealth


    #region No Longer need to use this update loop, will leave here for future reference if I need to send any kind of packet over a network
    /*

        Must Include IPunObservable interface to send packets over a network, which means a Photonview is not required on gameobject
        // Update is called once per frame
        void Update()
        {
            if (!player.PV.IsMine)
            {
                player.currentHealth = correctHealthAmt;
                //UpdateNetworkHealthBars();

                return;
            }
        }



        private void UpdateNetworkHealthBars()
        {
            float fillFront = frontHealthBar_Slider.value;
            float fillBack = backHealthBar_Slider.value;

            if (player.currentHealth == player.maxHealth)
            {
                frontHealthBar_Slider.value = player.maxHealth;    //Without this, theres a weird bug where player spawns without front health bar
            }

            networkLerpTimer += Time.deltaTime;
            if (fillBack > player.currentHealth)
            {


                backHealthBar_Fill.color = Color.white;
                frontHealthBar_Slider.value = player.currentHealth;
                float percentComplete = networkLerpTimer / changeInHealthBarDuration;
                percentComplete *= percentComplete;           // The more you square this, the slower health will drop

                backHealthBar_Slider.value = Mathf.Lerp(fillBack, player.currentHealth, percentComplete);
            }

        } // END UpdateNetworkHealthBars
    */


    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (PlayerController.debugMode) { return; }

        Debug.Log("sending sending sending");
        // Write
        if (stream.IsWriting)
        {
            stream.SendNext(player.currentHealth);
        }
        // Read
        else
        {
            correctHealthAmt = (int)stream.ReceiveNext();
        }
    }*/

    #endregion


    //-------------------------------------------//
    public void UpdateHealthUI(int _currentHealth)
    {
        //Debug.Log("Update Health");
        currentHealth = _currentHealth;
        StopAllCoroutines();
        StartCoroutine(LerpHealthValue());

        if (!PlayerController.debugMode && pv.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("currentHealth", _currentHealth);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }


    } // END SetHealthBar


    //-------------------------------------------//
    IEnumerator LerpHealthValue()
    {
        // Lerp the movement of health bars for increasing and decreasing health and change color of health bar for healing or taking damage 
        float lerpTimer = 0;

        while (lerpTimer < changeInHealthBarDuration)
        {

            float fillFront = frontHealthBar_Slider.value;
            float fillBack = backHealthBar_Slider.value;

            lerpTimer += Time.deltaTime;

            // If took damage
            if (fillBack > currentHealth)
            {
                backHealthBar_Fill.color = Color.white;
                frontHealthBar_Slider.value = currentHealth;

                float percentComplete = lerpTimer / changeInHealthBarDuration;
                percentComplete *= percentComplete; //* percentComplete;           // The more you square this, the slower the health bar movement will be

                backHealthBar_Slider.value = Mathf.Lerp(fillBack, currentHealth, percentComplete);
            }

            // For Healing
            if (fillFront < currentHealth)
            {
                backHealthBar_Fill.color = new Color32(101, 255, 23, 255); // Light green
                backHealthBar_Slider.value = currentHealth;

                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / changeInHealthBarDuration *0.5f;  // Add me to slow down health bar regen fill speed.
                percentComplete *= percentComplete;

                frontHealthBar_Slider.value = Mathf.Lerp(fillFront, backHealthBar_Slider.value, percentComplete);
            }


            yield return null;
        }

    } // END ChangeHealthValue


    //-------------------------------------------//
    public void DisplayFloatingText(int damage)
    {
        damageTaken = damage;


        ///////////////////////////////////////// 
        //For testing different positions of floating numbers
        //if (PlayerController.debugMode == true)
        //{

        GameObject _floatingText = Instantiate(floatingText, transform.position, Quaternion.identity, worldCanvas.transform);

        TextMeshProUGUI textAttributes = _floatingText.GetComponent<TextMeshProUGUI>();
        textAttributes.text = damageTaken.ToString();

        textAttributes.color = floatingTextGradient.Evaluate(damageTaken / 100f);

        _floatingText.transform.localRotation = Quaternion.Euler(0, 0, 0);
        Destroy(_floatingText, 1.5f);

        //}
        ///////////////////////////////////////////


    } // END DisplayFloatingText

    #endregion

} // END Class

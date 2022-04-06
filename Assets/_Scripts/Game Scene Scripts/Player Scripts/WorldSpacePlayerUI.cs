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
    [SerializeField] PlayerController player;
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
    private int correctHealthAmt;
    [HideInInspector]
    public float networkLerpTimer;
    private bool isDamaged = false;
    private int damageTaken;

    PhotonView pv;

    #endregion

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        
        if (!PlayerController.debugMode)
        {
            userNameText.text = player.PV.Owner.NickName;
        }
        
    }


    public void SetHealthBarMax(int _maxHealth)
    {

        frontHealthBar_Slider.maxValue = _maxHealth;
        frontHealthBar_Slider.value = _maxHealth;
        backHealthBar_Slider.maxValue = _maxHealth;
        backHealthBar_Slider.value = _maxHealth;

        if (pv.IsMine && !PlayerController.debugMode)
        {
            Hashtable hash = new Hashtable();
            hash.Add("setHealthMax", _maxHealth);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }


    } // END SetMaxHealth


    #region No Longer need to use this update loop, will leave here for future reference if I need to send any kind of packet over a network
    /*

        Must Include IPunObservable interface to send packets over a network
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


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
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
    }

    #endregion




    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

        if (!pv.IsMine && targetPlayer == pv.Owner && changedProps["currentHealth"] != null)
        {
            UpdateHealthUI((int)changedProps["currentHealth"]);

            StopAllCoroutines();

            StartCoroutine(ChangeHealthValue());

        }

        if (!pv.IsMine && targetPlayer == pv.Owner && changedProps["setHealthMax"] != null)
        {
            SetHealthBarMax((int)changedProps["setHealthMax"]);
        }

    } // END OnPlayerPropertiesUpdate


    public void UpdateHealthUI(int _currentHealth)
    {
        Debug.Log("Update Health");
        player.currentHealth = _currentHealth;
        StopAllCoroutines();
        StartCoroutine(ChangeHealthValue());

        if (pv.IsMine && !PlayerController.debugMode)
        {
            Hashtable hash = new Hashtable();
            hash.Add("currentHealth", _currentHealth);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }


        // PUN REFUSES TO RUN THE COROUTINES. IS THERE A REASON WHY?

    } // END SetHealthBar


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
            if (fillBack > player.currentHealth)
            {

                backHealthBar_Fill.color = Color.white;
                frontHealthBar_Slider.value = player.currentHealth;
                float percentComplete = lerpTimer / changeInHealthBarDuration;
                percentComplete *= percentComplete; //* percentComplete;           // The more you square this, the slower health will drop

                backHealthBar_Slider.value = Mathf.Lerp(fillBack, player.currentHealth, percentComplete);

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

            yield return null;
        }

    }// END ChangeHealthValue


    public void DisplayFloatingText(int damage)
    {
        isDamaged = true;
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


}

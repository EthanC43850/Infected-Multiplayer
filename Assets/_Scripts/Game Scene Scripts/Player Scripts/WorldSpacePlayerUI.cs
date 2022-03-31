using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class WorldSpacePlayerUI : MonoBehaviourPunCallbacks, IPunObservable
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
    #endregion

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

        if (player.PV.IsMine && !PlayerController.debugMode)
        {
            Hashtable hash = new Hashtable();
            hash.Add("setHealthMax", _maxHealth);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }


    } // END SetMaxHealth


    // Update is called once per frame
    void Update()
    {
        if (!player.PV.IsMine)
        {
            player.currentHealth = correctHealthAmt;
            UpdateNetworkHealthBars();

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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

        if (!player.PV.IsMine && targetPlayer == player.PV.Owner && changedProps["currentHealth"] != null)
        {
            Debug.Log("Player health changed!");
            UpdateHealthUI((int)changedProps["currentHealth"]);

        }

        if (!player.PV.IsMine && targetPlayer == player.PV.Owner && changedProps["setHealthMax"] != null)
        {
            SetHealthBarMax((int)changedProps["setHealthMax"]);
        }

    } // END OnPlayerPropertiesUpdate


    //This is called through the network to better sync numbers and health bars
    public void UpdateHealthUI(int _currentHealth)
    {
        Debug.Log("Update Health");

        // PUN REFUSES TO RUN THE COROUTINES. IS THERE A REASON WHY?
        StopAllCoroutines();
        StartCoroutine(ChangeHealthValue());

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

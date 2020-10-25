using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class BattleScript : MonoBehaviour
{
    public Spinner spinnerScript;

    private float startSpinSpeed;
    private float currentSpinSpeed;
    public Image spinSpeedBar_Image;
    public TextMeshProUGUI spinSpeedRatioText;

    public float commonDamageCoefficient = 0.04f;

    public bool isAttacker;
    public bool isDefender;

    [Header("Player Type Damage Coefficients")]
    public float doDamageCoefficientAttacker = 10f;
    public float getDamagedCoefficientAttacker = 1.2f;

    public float doDamageCoefficientDefender = 0.75f;
    public float getDamagedCoefficientDefender = 0.2f;

    private void Awake()
    {

        startSpinSpeed = spinnerScript.spinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
    }

    private void CheckPlayerType()
    {
        if(gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;
        } else if (gameObject.name.Contains("Defender"))
        {
            isAttacker = false;
            isDefender = true;
            spinnerScript.spinSpeed = 4400;
            startSpinSpeed = spinnerScript.spinSpeed;
            currentSpinSpeed = spinnerScript.spinSpeed;
            spinSpeedRatioText.text = currentSpinSpeed + "/" + startSpinSpeed;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            //comparing the speeds of the spinnertops
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            if (mySpeed > otherPlayerSpeed)
            {
                float defaultDamageAmount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600 * commonDamageCoefficient;

                if (isAttacker)
                {
                    defaultDamageAmount *= doDamageCoefficientAttacker;
                }
                else if (isDefender)
                {
                    defaultDamageAmount *= doDamageCoefficientDefender;
                }

                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, defaultDamageAmount);
                }
            } 
        }
    }

    [PunRPC]
    public void DoDamage(float _damageAmount)
    {

        if (isAttacker)
        {
            _damageAmount *= getDamagedCoefficientAttacker;
        } else if (isDefender)
        {
            _damageAmount *= getDamagedCoefficientDefender;
        }

        spinnerScript.spinSpeed -= _damageAmount;
        currentSpinSpeed = spinnerScript.spinSpeed;
        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatioText.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;
    }

    private void Start()
    {
        CheckPlayerType();
    }
}

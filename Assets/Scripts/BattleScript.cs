using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class BattleScript : MonoBehaviourPun
{
    public Spinner spinnerScript;

    public GameObject UI3D;
    public GameObject deathPanelUIPrefab;
    private GameObject deathPanelUIGameobject;

    private Rigidbody rb;

    private float startSpinSpeed;
    private float currentSpinSpeed;
    public Image spinSpeedBar_Image;
    public TextMeshProUGUI spinSpeedRatioText;

    public float commonDamageCoefficient = 0.04f;

    public bool isAttacker;
    public bool isDefender;
    private bool isDead;

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
        if (!isDead)
        {
            if (isAttacker)
            {
                _damageAmount *= getDamagedCoefficientAttacker;
            }
            else if (isDefender)
            {
                _damageAmount *= getDamagedCoefficientDefender;
            }

            spinnerScript.spinSpeed -= _damageAmount;
            currentSpinSpeed = spinnerScript.spinSpeed;
            spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
            spinSpeedRatioText.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;

            if (currentSpinSpeed < 100)
            {
                //Die
                Die();
            }
        }
    }

    void Die()
    {
        isDead = true;
        GetComponent<MovementController>().enabled = false;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        spinnerScript.spinSpeed = 0f;
        UI3D.gameObject.SetActive(false);

        if (photonView.IsMine)
        {
            //countdown for respawn
            StartCoroutine(Respawn());
        }
    }


    IEnumerator Respawn()
    {
        GameObject canvasGameobject = GameObject.Find("Canvas");
        if (deathPanelUIGameobject==null)
        {
            deathPanelUIGameobject = Instantiate(deathPanelUIPrefab, canvasGameobject.transform);
        } else
        {
            deathPanelUIGameobject.SetActive(true);
        }

        Text respawnTimeText = deathPanelUIGameobject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;
        respawnTimeText.text = respawnTime.ToString("F0");

        while (respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(".00");
            GetComponent<MovementController>().enabled = false;
        }

        deathPanelUIGameobject.SetActive(false);
        GetComponent<MovementController>().enabled = true;

        photonView.RPC("Reborn", RpcTarget.AllBuffered);

    }

    [PunRPC]
    public void Reborn()
    {
        spinnerScript.spinSpeed = startSpinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;
        spinSpeedRatioText.text = currentSpinSpeed + "/" + startSpinSpeed;
        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        rb.freezeRotation = false;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        UI3D.SetActive(true);
        isDead = false;

    }

    private void Start()
    {
        CheckPlayerType();
        rb = GetComponent<Rigidbody>();
    }
}

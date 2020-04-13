using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{

    public Transform playerSwitcherTransform;
    
    public int playerSelectionNumber;

    public GameObject[] spinnerModels;

    [Header("UI")]
    public TextMeshProUGUI playerModelType_Text;
    public GameObject UI_Selection;
    public GameObject UI_AfterSelection;
    public Button nextButton;
    public Button prevButton;

    #region UNITY Methods
    // Start is called before the first frame update
    void Start()
    {
        UI_Selection.SetActive(true);
        UI_AfterSelection.SetActive(false);
        playerSelectionNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI Callback Methods

    public void NextPlayer()
    {
        nextButton.enabled = false;
        prevButton.enabled = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, 90, 1.0f));

        playerSelectionNumber += 1;

        if (playerSelectionNumber == spinnerModels.Length)
        {
            playerSelectionNumber = 0;
        }

        if (playerSelectionNumber == 0 || playerSelectionNumber ==1)
        {
            playerModelType_Text.text = "Attack";
        } else
        {
            playerModelType_Text.text = "Defend";
        }
    }

    public void PreviousPlayer()
    {
        nextButton.enabled = false;
        prevButton.enabled = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, -90, 1.0f));

        playerSelectionNumber -= 1;

        if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            playerModelType_Text.text = "Attack";
        }
        else
        {
            playerModelType_Text.text = "Defend";
        }
    }

    public void OnSelectButtonClicked()
    {
        UI_Selection.SetActive(false);
        UI_AfterSelection.SetActive(true);
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { {MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }

    public void OnReSelectButtonClicked()
    {
        UI_Selection.SetActive(true);
        UI_AfterSelection.SetActive(false);
    }

    public void OnBattleButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Gameplay");   
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }

    #endregion

    #region Private Methods
    IEnumerator Rotate(Vector3 axis, Transform transformToRotate, float angle, float duration = 1.0f)
    {
        Quaternion originalRotation = transformToRotate.rotation;
        Quaternion finalRotation = transformToRotate.rotation * Quaternion.Euler(axis * angle);
        float elapseTime = 0.0f;
        while (elapseTime < duration)
        {
            transformToRotate.rotation = Quaternion.Slerp(originalRotation, finalRotation, elapseTime / duration);
            elapseTime += Time.deltaTime;
            yield return null;
        }

        transformToRotate.rotation = finalRotation;
        nextButton.enabled = true;
        prevButton.enabled = true;

    }
    #endregion
}

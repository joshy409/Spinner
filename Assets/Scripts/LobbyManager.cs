using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField playerNameInputField;
    public GameObject UI_LoginGameObject;

    [Header("Lobby UI")]
    public GameObject UI_LobbyGameObject;
    public GameObject UI_3DGameObject;

    [Header("Connection Status UI")]
    public GameObject UI_ConnectionStatusGameOject;
    public Text connectionStatusText;
    public bool showConnectionStatus = false;

    #region UNITY Methods
    private void Start()
    {
        UI_LobbyGameObject.SetActive(false);
        UI_3DGameObject.SetActive(false);
        UI_ConnectionStatusGameOject.SetActive(false);
        UI_LoginGameObject.SetActive(true);
    }

    private void Update()
    {
        if (showConnectionStatus)
        {
            connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
        }
    }
    #endregion

    #region UI Callback Methods
    public void OnEnterGameButtonClicked()
    {
        string playerName = playerNameInputField.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            UI_LobbyGameObject.SetActive(false);
            UI_3DGameObject.SetActive(false);
            UI_LoginGameObject.SetActive(false);

            showConnectionStatus = true;
            UI_ConnectionStatusGameOject.SetActive(true);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        } else
        {
            Debug.Log("Player name is invalid or empty!");
        }
    }

    public void OnQuickMatchButtonClicked()
    {
        //SceneManager.LoadScene("Scene_Loading");
        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
    }
    #endregion

    #region PHOTON Callbakc Methods
    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log("we connected to internet");
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon");

        UI_LobbyGameObject.SetActive(true);
        UI_3DGameObject.SetActive(true);

        UI_LoginGameObject.SetActive(false);
        UI_ConnectionStatusGameOject.SetActive(false);
    }
    #endregion
}

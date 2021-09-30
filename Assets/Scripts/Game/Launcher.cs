using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Collections;

/*
>Connects you to the network
>Puts you in a room when play button is pressed
*/

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject joiningRoomText;
    [SerializeField] GameObject playButton;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        playButton.SetActive(false);
        joiningRoomText.SetActive(false);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Connect();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        playButton.SetActive(true);
    }

    public void Play()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();

            playButton.SetActive(false);
            joiningRoomText.SetActive(true);
        }
        else
        {
            Connect();
        }
    }

    public void QuitGame()
    {
        print("Quit");
        Application.Quit();   
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);

        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.LoadLevel("MP Test");
    }
}

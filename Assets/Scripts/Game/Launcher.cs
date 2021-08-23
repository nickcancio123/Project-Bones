using UnityEngine;

using Photon.Realtime;
using Photon.Pun;

/*
>Connects you to the network
>Puts you in a room when play button is pressed
*/

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject joiningRoomText;
    [SerializeField] GameObject playButton;

    void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        Connect();
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
            Play();
        }
    }


    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        joiningRoomText.SetActive(false);
        playButton.SetActive(true);
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

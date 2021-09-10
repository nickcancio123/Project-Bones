using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class PlayerUIManager : MonoBehaviourPunCallbacks
{
    GameManager gameManager;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject gameplayMenu;


    void Awake()
    {
        if (photonView.IsMine)
        {
            ParentAllMenusToCanvas();
        }
    }

    void Start()
    {
        gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();

        if (photonView.IsMine)
        {
            ClosePauseMenu();
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            ManagePauseMenu();
        }
    }


    #region General UI Methods
    void ToggleCursor(bool visible)
    {
        if (visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void ParentAllMenusToCanvas()
    {
        //Last one parented (lowest in heirarchy) will have highest priority 
        gameplayMenu.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        pauseMenu.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }
    #endregion



    #region Pause Menu
    void ManagePauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OpenPauseMenu();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            ClosePauseMenu();
        }
    }

    void OpenPauseMenu()
    {
        ToggleCursor(true);
        pauseMenu.SetActive(true);
    }

    void ClosePauseMenu()
    {
        ToggleCursor(false);
        pauseMenu.SetActive(false);
    }

    public void LeaveButton_OnClick()
    {
        if (gameManager)
        {
            gameManager.LeaveRoom();
        }
    }
    #endregion



    #region Gameplay Menu
    public void TakeDamageEffectStart()
    {
        print("effect");
        Image gameplayMenuBackground = gameplayMenu.GetComponent<Image>();
        gameplayMenuBackground.color = new Color(200, 0, 0, 0.3f);
        StartCoroutine(TakeDamageEffectEnd());
    }

    IEnumerator TakeDamageEffectEnd()
    {
        Image gameplayMenuBackground = gameplayMenu.GetComponent<Image>();
        gameplayMenuBackground.color = Color.clear;
        yield return new WaitForSeconds(0.2f);
    }
    #endregion
}

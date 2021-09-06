using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Stamina : MonoBehaviourPunCallbacks
{
    [SerializeField] Slider staminaBar;
    [SerializeField] CharacterController characterController;

    [SerializeField] float maxStamina = 100;
    [SerializeField] float refillRate = 14; //Per second
    float currentStamina = 0;

    bool drainedLastFrame = false; //Only refill if not being drained

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        if (!photonView.IsMine) { return; }

        UpdateStaminaBar();
        RefillStamina();
    }

    void UpdateStaminaBar()
    {
        if (!staminaBar) { return; }

        staminaBar.value = Mathf.Clamp(currentStamina / maxStamina, 0, 1);
    }

    void RefillStamina()
    {
        if (drainedLastFrame)
        {
            drainedLastFrame = false;
            return;
        }

        if (!characterController.isGrounded) { return; }    //Only refill stamina if grounded

        currentStamina = Mathf.Clamp(currentStamina + refillRate * Time.deltaTime, 0, maxStamina);
    }


    #region Public Interface
    public void DrainStamina(float drainAmount)
    {
        currentStamina = Mathf.Clamp(currentStamina - drainAmount, 0, maxStamina);
        drainedLastFrame = true;
    }

    public bool CanDrainStaminaBy(float drainAmount)
    {
        return (currentStamina - drainAmount >= 0);
    }

    public float GetCurrentStamina() => currentStamina;
    #endregion
}

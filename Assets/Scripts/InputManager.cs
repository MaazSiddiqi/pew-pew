using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    private PlayerLook playerLook;
    private PlayerMotor motor;
    private PlayerInteract playerInteract;
    [SerializeField] private WeaponManager weaponManager;

    void Awake(){
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        playerLook = GetComponent<PlayerLook>();
        motor = GetComponent<PlayerMotor>();
        playerInteract = GetComponent<PlayerInteract>();
        if (weaponManager == null) weaponManager = GetComponentInChildren<WeaponManager>();

        onFoot.Jump.performed += ctx => motor.Jump(); //call back context jump function state
        
        // Hook up shooting and reloading
        if (weaponManager != null)
        {
            onFoot.Shoot.performed += ctx => weaponManager.Shoot();
            onFoot.Reload.performed += ctx => weaponManager.Reload();
        }

        // Hook up interaction
        if (playerInteract != null)
        {
            onFoot.Interact.performed += ctx => playerInteract.Interact();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    void LateUpdate(){
        playerLook.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable(){
        onFoot.Enable();
    }

    private void OnDisable(){
        onFoot.Disable();
    }
}

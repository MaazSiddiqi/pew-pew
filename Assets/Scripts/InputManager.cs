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
    void Awake(){
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        playerLook = GetComponent<PlayerLook>();
        motor = GetComponent<PlayerMotor>();
        onFoot.Jump.performed += ctx => motor.Jump(); //call back context jump function state
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

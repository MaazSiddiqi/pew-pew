using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float distance = 3f;
    [SerializeField]
    private LayerMask mask;
    private PlayerUI playerUI;
    private InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<PlayerLook>().playerCamera;
        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        playerUI.UpdateText(string.Empty); // Clear text by default
        
        // Create a ray at the center of the camera, shooting outwards.
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        
        // Debug: Check if we are hitting ANYTHING at all
        RaycastHit debugHit;
        if (Physics.Raycast(ray, out debugHit, distance))
        {
             Debug.Log($"Debug Ray hit: {debugHit.collider.name} on Layer: {LayerMask.LayerToName(debugHit.collider.gameObject.layer)}");
        }

        RaycastHit hitInfo; 
        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
                Debug.Log($"Looking at Interactable: {interactable.name}");
            }
        }
    }

    public void Interact()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                interactable.BaseInteract();
            }
        }
    }
}

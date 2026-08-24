using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   
    
    private float speed = 5.0f;
    private Rigidbody2D playerRb;

    // player move
    public InputAction moveAction;
    public InputAction interactAction;

    private Vector2 moveInput;

    private HashSet<Interactable> intercactedObjects = new HashSet<Interactable>();    

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        // setting input Action
        moveAction.Enable();
        interactAction.Enable();
    }

    void Update()
    {   
        // getting input from player
        moveInput = moveAction.ReadValue<Vector2>();

        // checking for interaction input
        if (interactAction.WasPressedThisFrame())
        {
            Interactable closest = GetClosestInteractable();
            if (closest != null)
            {
                closest.Interact();
            }
        }

    }

    void FixedUpdate()
    {
        // actually moving the player
        playerRb.MovePosition(playerRb.position + moveInput * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            NPC np = collision.gameObject.GetComponent<NPC>();
            if (np != null)
            {
                intercactedObjects.Add(np);
            }
        }
    }

    private Interactable GetClosestInteractable()
    {
        Interactable closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var interactable in intercactedObjects)
        {
            float distance = Vector2.Distance(transform.position, interactable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }

        }

        return closest;
    }
}

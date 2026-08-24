using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   
    
    private float speed = 5.0f;
    private Rigidbody2D playerRb;

    // player move
    public InputAction moveAction;
    private Vector2 moveInput;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        // setting input Action
        moveAction.Enable();
    }

    void Update()
    {   
        // getting input from player
        moveInput = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // actually moving the player
        playerRb.MovePosition(playerRb.position + moveInput * speed * Time.fixedDeltaTime);
    }
}

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
        if (interactAction.triggered)
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

    // 플레이어의 근처에 있는 오브젝트를 감지. 영역에 들어서면 hashset에 추가
    void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable interactable =
            collision.GetComponentInParent<Interactable>();

        if (interactable != null)
        {
            intercactedObjects.Add(interactable);
        }
    }
    // 플레이어의 근처에 있는 오브젝트를 감지. 영역에서 나가면 hashset에서 제거
    void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable =
            collision.GetComponentInParent<Interactable>();

        if (interactable != null)
        {
            intercactedObjects.Remove(interactable);
        }
    }
    // 가장 가까이 있는 오브젝트를 찾는 함수
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

    // 플레이어 잠금 및 잠금 해제 함수 // TalkManager.cs에서 대화창이 열릴 때 플레이어를 잠금 
    public void PlayerLock()       
    {
        moveAction.Disable();
        interactAction.Disable();
    }
    public void PlayerUnlock()
    {
        moveAction.Enable();
        interactAction.Enable();
    }
}

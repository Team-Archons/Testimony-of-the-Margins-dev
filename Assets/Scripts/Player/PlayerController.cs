using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   
    public static PlayerController Instance { get; private set; }
    
    [SerializeField] private float speed = 5.0f;
    private Rigidbody2D playerRb;

    // player move
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction interactAction;

    private Vector2 moveInput;

    private readonly HashSet<Interactable> nearbyInteractables = new HashSet<Interactable>();
    private bool isLocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerRb = GetComponent<Rigidbody2D>();
        if (playerRb == null)
            Debug.LogError("PlayerController requires a Rigidbody2D.", this);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        interactAction.Enable();
    }

    private void OnDisable()
    {   
        moveAction.Disable();
        interactAction.Disable();
        moveInput = Vector2.zero;
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        if (interactAction.triggered)
        {
            Interactable closest = GetClosestInteractable();
            closest?.Interact();
        }
    }

    private void FixedUpdate()
    {
        if (playerRb != null && !isLocked)
            playerRb.MovePosition(playerRb.position + moveInput.normalized * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponentInParent<Interactable>();
        if (interactable != null)
            nearbyInteractables.Add(interactable);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponentInParent<Interactable>();
        if (interactable != null)
            nearbyInteractables.Remove(interactable);
    }

    private Interactable GetClosestInteractable()
    {
        Interactable closest = null;
        float closestDistance = Mathf.Infinity;

        nearbyInteractables.RemoveWhere(item => item == null || !item.isActiveAndEnabled);
        foreach (Interactable interactable in nearbyInteractables)
        {
            float distance = (transform.position - interactable.transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }
        return closest;
    }

    public void PlayerLock()
    {
        isLocked = true;
        moveInput = Vector2.zero;
        moveAction.Disable();
        interactAction.Disable();
    }

    public void PlayerUnlock()
    {
        isLocked = false;
        moveAction.Enable();
        interactAction.Enable();
    }
}

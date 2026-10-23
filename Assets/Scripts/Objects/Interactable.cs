using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private string interactionName;

    public string InteractionName => string.IsNullOrWhiteSpace(interactionName) ? name : interactionName;

    public abstract void Interact();
}

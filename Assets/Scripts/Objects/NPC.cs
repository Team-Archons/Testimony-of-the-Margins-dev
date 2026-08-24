using UnityEngine;

public class NPC : Interactable
{
    public override void Interact()
    {
        OpenDialogue();
    }

    public void OpenDialogue()
    {
        Debug.Log("Dialogue opened");
    }
}

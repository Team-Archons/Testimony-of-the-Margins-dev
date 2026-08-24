using UnityEngine;

public class NPC : Interactable
{
    public TalkManager talkManager;

    public string[] dialogue
        = { "안녕하세요",
            "이것은 테스트 입니다",
            "잘되고 있는건가요?"
        };

    void Start()
    {
        talkManager = GameObject.Find("TalkManager").GetComponent<TalkManager>();
    }

    public override void Interact()
    {   
        if(!talkManager.isDialogueActive)
        {
            OpenDialogue();
        }
    }

    public void OpenDialogue()
    {
        talkManager.BringDialogue(dialogue);
    }
}

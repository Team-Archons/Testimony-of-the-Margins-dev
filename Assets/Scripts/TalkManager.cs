using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
    public Image dialogueBox;

    public TMP_Text ScriptText_dialogue;
    public string[] dialogueLines;
    public int currentLineIndex = 0;

    public InputAction nextDialogueAction;

    public bool isDialogueActive = false;

    PlayerController playerController;

    void Start()
    {
        dialogueBox.gameObject.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }
    void Update()
    {
        if (!isDialogueActive)
            return;
        if (nextDialogueAction.triggered)
        {
            ShowNextDialogue();
        }
    }

    public void BringDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("Dialogue lines are empty or null.");
            return;
        }
        // 플레이어 잠금
        playerController.PlayerLock();
        // 대화 진행을 위한 입력 액션 활성화
        nextDialogueAction.Enable();

        dialogueBox.gameObject.SetActive(true);
        
        isDialogueActive = true;
        currentLineIndex = 0;
        dialogueLines = lines;
        
    }

    private void ShowNextDialogue()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            ScriptText_dialogue.text = dialogueLines[currentLineIndex];
            currentLineIndex++;
        }
        else
        {
            CloseDialogue();
        }
    }
    
    private void CloseDialogue()
    {
        dialogueBox.gameObject.SetActive(false);
        isDialogueActive = false;
        currentLineIndex = 0;
        dialogueLines = null;
        
        StartCoroutine(UnlockPlayerAfterInputReleased());
    }

    private IEnumerator UnlockPlayerAfterInputReleased()
    {
        while (nextDialogueAction.IsPressed())
        {
            yield return null;
        }

        nextDialogueAction.Disable();

        yield return null;

        playerController.PlayerUnlock();
    }
}

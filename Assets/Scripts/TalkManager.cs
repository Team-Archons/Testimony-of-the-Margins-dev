using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
    [SerializeField] private Image dialogueBox;

    [SerializeField] private TMP_Text ScriptText_dialogue;
    private string[] dialogueLines;
    private int currentLineIndex;
    private bool canAdvanceDialogue;

    [SerializeField] private InputAction nextDialogueAction;

    public bool isDialogueActive { get; private set; }

    [SerializeField] private PlayerController playerController;

    private void Awake()
    {
        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();
        if (dialogueBox == null || ScriptText_dialogue == null || playerController == null)
            Debug.LogError("TalkManager requires a dialogue box, text, and PlayerController.", this);
        if (dialogueBox != null)
            dialogueBox.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        nextDialogueAction.Disable();
    }

    private void Update()
    {
        if (!isDialogueActive || !canAdvanceDialogue)
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
        if (isDialogueActive)
            return;

        playerController.PlayerLock();
        nextDialogueAction.Enable();
        currentLineIndex = 0;
        dialogueLines = (string[])lines.Clone();
        canAdvanceDialogue = false;
        ScriptText_dialogue.text = dialogueLines[currentLineIndex++];
        dialogueBox.gameObject.SetActive(true);
        isDialogueActive = true;
        StartCoroutine(WaitForOpenInputRelease());
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
        ScriptText_dialogue.text = string.Empty;
        isDialogueActive = false;
        currentLineIndex = 0;
        dialogueLines = null;
        canAdvanceDialogue = false;
        StartCoroutine(UnlockPlayerAfterInputReleased());
    }

    private IEnumerator WaitForOpenInputRelease()
    {
        // 상호작용에 사용한 F 입력으로 첫 대사가 넘어가지 않게 한다.
        yield return null;
        while (nextDialogueAction.IsPressed())
            yield return null;

        if (isDialogueActive)
            canAdvanceDialogue = true;
    }

    private IEnumerator UnlockPlayerAfterInputReleased()
    {
        // 대화 종료에 사용한 F 키가 상호작용 입력으로 재사용되지 않도록 뗄 때까지 기다린다.
        while (nextDialogueAction.IsPressed())
            yield return null;

        nextDialogueAction.Disable();

        // 같은 입력 이벤트에서 바로 NPC 대화가 다시 시작되는 것을 막는다.
        yield return null;
        playerController.PlayerUnlock();
    }
}

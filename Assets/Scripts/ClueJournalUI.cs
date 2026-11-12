using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClueJournalUI : MonoBehaviour
{
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private TMP_Text journalText;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InputAction openJournalAction = new InputAction("Open Journal", InputActionType.Button, "<Keyboard>/tab");
    [SerializeField] private InputAction closeJournalAction = new InputAction("Close Journal", InputActionType.Button, "<Keyboard>/escape");

    private ClueJournal clueJournal;
    private bool isOpen;

    private void Awake()
    {
        if (journalPanel != null)
            journalPanel.SetActive(false);

        ResolveReferences();
    }

    private void OnEnable()
    {
        openJournalAction.Enable();
        closeJournalAction.Enable();
    }

    private void OnDisable()
    {
        openJournalAction.Disable();
        closeJournalAction.Disable();

        if (clueJournal != null)
            clueJournal.CluesChanged -= RefreshJournal;

        if (isOpen && playerController != null)
            playerController.PlayerUnlock();

        isOpen = false;
    }

    private void Update()
    {
        ResolveReferences();

        if (isOpen)
        {
            if (closeJournalAction.triggered || openJournalAction.triggered)
                CloseJournal();
        }
        else if (openJournalAction.triggered)
        {
            OpenJournal();
        }
    }

    private void ResolveReferences()
    {
        PlayerController currentPlayer = PlayerController.Instance != null
            ? PlayerController.Instance
            : playerController;
        if (currentPlayer == null)
            return;

        if (playerController != currentPlayer)
        {
            playerController = currentPlayer;
            BindJournal(playerController.GetComponent<ClueJournal>());
        }
        else if (clueJournal == null)
        {
            BindJournal(playerController.GetComponent<ClueJournal>());
        }
    }

    private void BindJournal(ClueJournal newJournal)
    {
        if (clueJournal == newJournal)
            return;

        if (clueJournal != null)
            clueJournal.CluesChanged -= RefreshJournal;

        clueJournal = newJournal;
        if (clueJournal != null)
            clueJournal.CluesChanged += RefreshJournal;
    }

    private void OpenJournal()
    {
        if (journalPanel == null || playerController == null)
            return;

        TalkManager talkManager = FindFirstObjectByType<TalkManager>();
        if (talkManager != null && talkManager.isDialogueActive)
            return;

        RefreshJournal();
        journalPanel.SetActive(true);
        playerController.PlayerLock();
        isOpen = true;
    }

    private void CloseJournal()
    {
        if (journalPanel != null)
            journalPanel.SetActive(false);

        isOpen = false;
        if (playerController != null)
            playerController.PlayerUnlock();
    }

    private void RefreshJournal()
    {
        if (journalText == null)
            return;

        journalText.text = clueJournal != null
            ? clueJournal.GetJournalText()
            : "아직 기록한 단서가 없다.";
    }
}

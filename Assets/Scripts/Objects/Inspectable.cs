using UnityEngine;

public class Inspectable : Interactable
{
    [SerializeField] private string clueId;
    [TextArea(2, 5)]
    [SerializeField] private string observation;
    [SerializeField] private ClueJournal clueJournal;
    [SerializeField] private TalkManager talkManager;

    public override void Interact()
    {
        if (clueJournal == null && PlayerController.Instance != null)
            clueJournal = PlayerController.Instance.GetComponent<ClueJournal>();
        if (talkManager == null)
            talkManager = FindFirstObjectByType<TalkManager>();

        if (clueJournal == null || talkManager == null)
        {
            Debug.LogError($"Inspectable '{name}' needs a ClueJournal and TalkManager reference.", this);
            return;
        }

        bool isNewClue = clueJournal.Record(clueId, InteractionName, observation);
        talkManager.BringDialogue(new[]
        {
            isNewClue ? observation : $"{observation}\n(이미 기록한 단서다.)"
        });
    }
}

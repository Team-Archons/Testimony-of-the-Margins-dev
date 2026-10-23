using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClueJournal : MonoBehaviour
{
    [Serializable]
    public class Clue
    {
        public string id;
        public string title;
        [TextArea(2, 5)] public string observation;
    }

    [SerializeField] private TalkManager talkManager;
    [SerializeField] private string firstClueId;
    [SerializeField] private string secondClueId;
    [TextArea(2, 4)]
    [SerializeField] private string hypothesisText = "두 흔적은 한 사람의 생활을 가리킨다.";
    [TextArea(2, 4)]
    [SerializeField] private string counterevidenceText = "두 표식이 같은 간격으로 반복된다. 누군가 조사자가 볼 것을 예상해 남겼을 수 있다.";

    private readonly Dictionary<string, Clue> clues = new Dictionary<string, Clue>();

    public event Action CluesChanged;
    public bool HasHypothesis => !string.IsNullOrEmpty(firstClueId) && clues.ContainsKey(firstClueId)
        && !string.IsNullOrEmpty(secondClueId) && clues.ContainsKey(secondClueId);

    private void Awake()
    {
        if (talkManager == null)
            talkManager = FindFirstObjectByType<TalkManager>();
    }

    public bool Record(string id, string title, string observation)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError("A clue needs a stable, non-empty ID.", this);
            return false;
        }

        if (clues.ContainsKey(id))
            return false;

        clues.Add(id, new Clue { id = id, title = title, observation = observation });
        CluesChanged?.Invoke();

        return true;
    }

    public bool HasClue(string id) => clues.ContainsKey(id);

    public string GetJournalText()
    {
        if (clues.Count == 0)
            return "아직 기록한 단서가 없다.";

        IEnumerable<string> lines = clues.Values.Select(clue => $"• {clue.title}: {clue.observation}");
        if (HasHypothesis)
            lines = lines.Append($"\n가설: {hypothesisText}");
        return string.Join("\n", lines);
    }

    public bool TryConnectCounterevidence(string firstId, string secondId)
    {
        if (!clues.ContainsKey(firstId) || !clues.ContainsKey(secondId) || firstId == secondId)
            return false;

        if (talkManager != null)
            talkManager.BringDialogue(new[] { counterevidenceText });
        return true;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class SceneTransitionPortal : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private Vector2 destinationPlayerPosition;

    private bool isTransitioning;
    private static bool hasPendingDestination;
    private static Vector2 pendingDestination;

    private void Awake()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        if (trigger != null)
            trigger.isTrigger = true;
    }

    private void Reset()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        if (trigger != null)
            trigger.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryTransition(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryTransition(other);
    }

    private void TryTransition(Collider2D other)
    {
        if (isTransitioning || other.GetComponentInParent<PlayerController>() == null)
            return;

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning($"Portal '{name}' has no target scene assigned.", this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError($"Target scene '{targetSceneName}' is not available in the active Build Profile.", this);
            return;
        }

        isTransitioning = true;
        pendingDestination = destinationPlayerPosition;
        hasPendingDestination = true;
        SceneManager.sceneLoaded += PlacePlayerAtDestination;
        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
    }

    private static void PlacePlayerAtDestination(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= PlacePlayerAtDestination;

        if (!hasPendingDestination)
            return;

        hasPendingDestination = false;
        PlayerController player = PlayerController.Instance;
        if (player == null)
            player = Object.FindFirstObjectByType<PlayerController>();

        if (player == null)
        {
            Debug.LogError("Scene transition completed, but no PlayerController was found to place at the destination.");
            return;
        }

        Vector3 position = player.transform.position;
        player.transform.position = new Vector3(pendingDestination.x, pendingDestination.y, position.z);
    }
}

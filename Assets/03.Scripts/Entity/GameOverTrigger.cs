using UnityEngine;
using UnityEngine.Events;

public class GameOverTrigger : MonoBehaviour
{
    [field: SerializeField] public UnityEvent OnGameOverEvent { get; private set; }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        OnGameOverEvent?.Invoke();
    }

    public void PlayAbsorbEnding()
    {
        Managers.Instance.GameManager.TriggerEnding(EndingType.Absorb);
    }
}

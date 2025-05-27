using UnityEngine;

public class MapBlockerNotifier : MonoBehaviour
{
    [SerializeField] private bool isBoxNotifier;
    private const string DefaultDialog = "아직 해결해야 하는 일이 있다.";
    private const string BoxDialog = "박스를 가져와야 할 것 같다.";
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isBoxNotifier && other.gameObject.CompareTag("Box"))
        {
            gameObject.SetActive(false);
            return;
        }
        
        if (other.gameObject.CompareTag("Player"))
        {
            var dialog = isBoxNotifier ? BoxDialog : DefaultDialog;
            var dialogManager = Managers.Instance.DialogueManager;
            dialogManager.SetInteractObjectDialog(dialog);
        }
    }
}

using System.Collections;
using UnityEngine;

public class ExclamationMark : MonoBehaviour
{
    [SerializeField] private Transform tr;
    [SerializeField] private float amplitude = 0.15f;
    [SerializeField] private float speed = .0f;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);

    private void OnEnable()
    {
        StartCoroutine(ExclamationMarkRoutine());
    }

    private IEnumerator ExclamationMarkRoutine()
    {
        yield return waitForSeconds;
        
        Vector3 startPos = tr.localPosition;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime * speed;
            float offsetY = Mathf.Sin(timer) * amplitude;
            tr.localPosition = startPos + new Vector3(0f, offsetY, 0f);
            yield return null;
        }
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
        tr.localPosition = Vector3.zero;
    }
}

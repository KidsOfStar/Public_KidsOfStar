using TMPro;
using UnityEngine;

public class NameTag : MonoBehaviour
{
    [Header("PC용 Material")]
    public Material pcMaterial;

    [Header("모바일용 Material")]
    public Material mobileMaterial;

    private void Awake()
    {
        var tem = GetComponent<TextMeshPro>();

#if UNITY_WEBGL || UNITY_EDITOR
    tem.fontMaterial = pcMaterial;
#elif UNITY_ANDROID || UNITY_IOS
    tem.fontMaterial = mobileMaterial;
#endif
    }
}

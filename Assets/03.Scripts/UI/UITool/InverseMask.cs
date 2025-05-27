using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Inverted Mask")]
public class InvertedMask : Mask
{
    // baseMaterial이 null일 경우 기본 UI 쉐이더로 대체
    public override Material GetModifiedMaterial(Material baseMaterial)
    {
        var mat = new Material(baseMaterial ?? new Material(Shader.Find("UI/Default")));
        // 스텐실 비교 함수를 NotEqual로 설정 → 역마스크 효과
        mat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
        return mat;
    }
}

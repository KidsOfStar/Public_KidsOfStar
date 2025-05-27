using UnityEngine;
using Cinemachine;

[ExecuteAlways]
public class ConstantWidthCinemachineExtension : CinemachineExtension
{
    [Tooltip("가로로 보이고 싶은 world 단위 길이")]
    public float targetWorldWidth = 18f;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        // Finalize 단계에서 Orthographic 카메라일 때만 처리
        if (stage == CinemachineCore.Stage.Finalize && state.Lens.Orthographic)
        {
            float aspect = (float)Screen.width / Screen.height;
            state.Lens.OrthographicSize = (targetWorldWidth / aspect) * 0.5f;
        }
    }
}
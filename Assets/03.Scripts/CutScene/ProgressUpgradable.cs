using UnityEngine;

// 컷씬용 업그레이드 프로그레스
public class ProgressUpgradable : MonoBehaviour
{
    public void UpgradeProgress()
    {
        Managers.Instance.GameManager.UpdateProgress();
    }
}
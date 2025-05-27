using UnityEngine;

/// <summary>
/// 배선 퍼즐 볼트
/// </summary>
public class BoltButtonHandler : MonoBehaviour
{
    private int x;
    public int X {  get { return x; } }
    private int y;
    public int Y { get { return y; } }

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

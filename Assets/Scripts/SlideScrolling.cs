using UnityEngine;

// Main Camera에 붙이세요. 플레이어에는 Tag "Player"가 설정되어 있어야 합니다.
public class SlideScrolling : MonoBehaviour
{
    [Tooltip("플레이어를 화면 중앙보다 왼쪽에 두기 위한 값")]
    public float offsetX = 1.5f;

    [Header("카메라가 움직일 수 있는 범위 (맵에 맞게 조정)")]
    public float minX = 0f;
    public float maxX = 200f;

    private Transform player;

    private void Awake()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 pos = transform.position;
        pos.x = Mathf.Max(pos.x, player.position.x + offsetX); // 뒤로는 안 감
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
    }
}
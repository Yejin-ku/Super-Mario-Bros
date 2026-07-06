using UnityEngine;

public enum BlockType { Brick, Question }

// 벽돌 블록과 물음표 블록을 하나의 스크립트로 처리합니다.
// Inspector에서 blockType만 바꿔서 두 종류 모두 만들 수 있습니다.
public class Block : MonoBehaviour
{
    public BlockType blockType = BlockType.Question;

    [Tooltip("Question 블록일 때 나올 아이템 프리팹 (예: 버섯)")]
    public GameObject itemPrefab;

    private bool used;
    private Collider2D myCollider;

    private void Start()
    {
        myCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null) return;

        Collider2D playerCollider = collision.collider;

        // pivot 위치와 무관하게, 콜라이더의 실제 경계(bounds)로 비교
        // 플레이어의 머리(위쪽)가 블록의 아래쪽보다 위에 있으면 "아래에서 친 것"으로 판정
        bool hitFromBelow = playerCollider.bounds.max.y <= myCollider.bounds.min.y + 0.15f;

        Debug.Log("블록 충돌 - 플레이어 top: " + playerCollider.bounds.max.y +
                  " / 블록 bottom: " + myCollider.bounds.min.y + " / hitFromBelow: " + hitFromBelow);

        if (!hitFromBelow) return;

        Hit();
    }

    private void Hit()
    {
        if (blockType == BlockType.Question)
        {
            if (used) return;
            used = true;

            if (itemPrefab != null)
            {
                // 블록 위치에서 생성 → Mushroom.cs가 알아서 위로 솟아오르는 연출을 함
                Instantiate(itemPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
using System.Collections;
using UnityEngine;

public enum BlockType { Brick, Question }

// 벽돌 블록과 물음표 블록을 하나의 스크립트로 처리합니다.
// Inspector에서 blockType만 바꿔서 두 종류 모두 만들 수 있습니다.
public class Block : MonoBehaviour
{
    public BlockType blockType = BlockType.Question;

    [Tooltip("Question 블록일 때 나올 아이템 프리팹 (예: 버섯)")]
    public GameObject itemPrefab;

    [Tooltip("Question 블록을 사용한 뒤 바뀔 스프라이트 (비워두면 그대로)")]
    public Sprite usedSprite;

    [Header("맞았을 때 튕기는 연출")]
    public float bumpHeight = 0.25f;
    public float bumpDuration = 0.16f;

    private bool used;
    private bool bumping;
    private Collider2D myCollider;
    private SpriteRenderer sr;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null) return;

        Collider2D playerCollider = collision.collider;

        // pivot 위치와 무관하게, 콜라이더의 실제 경계(bounds)로 비교
        // 플레이어의 머리가 블록의 아래쪽보다 아래에 있으면 "아래에서 친 것"으로 판정
        bool hitFromBelow = playerCollider.bounds.max.y <= myCollider.bounds.min.y + 0.2f;
        if (!hitFromBelow) return;

        Hit(player);
    }

    private void Hit(PlayerController player)
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

            if (usedSprite != null && sr != null)
                sr.sprite = usedSprite;

            StartCoroutine(BumpRoutine());
        }
        else // Brick
        {
            if (player.IsBig)
                Destroy(gameObject);            // 큰 마리오는 벽돌을 부순다
            else
                StartCoroutine(BumpRoutine());  // 작은 마리오는 튕기기만
        }
    }

    private IEnumerator BumpRoutine()
    {
        if (bumping) yield break;
        bumping = true;

        Vector3 start = transform.position;
        Vector3 top = start + Vector3.up * bumpHeight;
        float half = bumpDuration * 0.5f;
        float t = 0f;

        while (t < half)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, top, t / half);
            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(top, start, t / half);
            yield return null;
        }

        transform.position = start;
        bumping = false;
    }
}
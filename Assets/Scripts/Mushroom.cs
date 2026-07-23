using System.Collections;
using UnityEngine;

// Rigidbody2D, Collider2D가 필요합니다. (Rigidbody2D의 Freeze Rotation Z 체크)
// Block.cs의 itemPrefab에 이 오브젝트의 프리팹을 연결하세요.
public class Mushroom : MonoBehaviour
{
    [Header("블록에서 솟아오르는 연출")]
    public float emergeHeight = 1f;      // 블록 위로 올라오는 높이
    public float emergeDuration = 0.4f;  // 올라오는 데 걸리는 시간

    [Header("이동")]
    public float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Collider2D myCollider;
    private int direction = 1;   // 처음엔 오른쪽으로
    private bool emerged;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        myCollider.enabled = false;  // 솟아오르는 동안엔 블록과 부딪히지 않게
    }

    private void Start()
    {
        StartCoroutine(EmergeRoutine());
    }

    private IEnumerator EmergeRoutine()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * emergeHeight;
        float elapsed = 0f;

        while (elapsed < emergeDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / emergeDuration);
            yield return null;
        }
        transform.position = targetPos;

        // 다 올라왔으면 물리 켜고 굴러가기 시작
        myCollider.enabled = true;
        rb.gravityScale = 1f;
        emerged = true;
    }

    private void FixedUpdate()
    {
        if (!emerged) return;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.Grow();          // 플레이어가 커짐
            Destroy(gameObject);
            return;
        }

        // 벽이나 블록에 옆으로 부딪히면 방향 전환
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                direction *= -1;
                break;
            }
        }
    }
}
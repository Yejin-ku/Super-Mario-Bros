using System.Collections;
using UnityEngine;

// Rigidbody2D, Collider2D가 필요합니다.
// Block.cs의 itemPrefab에 이 오브젝트의 프리팹을 연결하세요.
public class Mushroom : MonoBehaviour
{
    [Header("블록에서 솟아오르는 연출")]
    public float emergeHeight = 1f;      // 블록 위로 올라오는 높이
    public float emergeDuration = 0.3f;  // 올라오는 데 걸리는 시간

    private Rigidbody2D rb;
    private Vector3 startPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;          // 중력 없이 제자리에 고정
        rb.linearVelocity = Vector2.zero;
    }

    private void Start()
    {
        startPos = transform.position;
        StartCoroutine(EmergeRoutine());
    }

    private IEnumerator EmergeRoutine()
    {
        Vector3 targetPos = startPos + Vector3.up * emergeHeight;
        float elapsed = 0f;

        while (elapsed < emergeDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / emergeDuration);
            yield return null;
        }

        transform.position = targetPos; // 다 올라온 뒤 그 자리에 고정
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            // 플레이어가 먹으면 사라짐
            Destroy(gameObject);
        }
    }
}
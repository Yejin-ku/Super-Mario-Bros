using UnityEngine;

// Rigidbody2D(Gravity Scale 1), Collider2D가 필요합니다.
// 프리팹으로 만들어 맵에 배치하세요.
public class Goomba : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float stompBounceForce = 6f;

    private Rigidbody2D rb;
    private Collider2D myCollider;
    private int direction = -1; // 처음엔 왼쪽으로 이동

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 벽이나 다른 굼바 등에 수평으로 부딪히면 방향 전환
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Mathf.Abs(contact.normal.x) > 0.5f)
                {
                    direction *= -1;
                    break;
                }
            }
            return;
        }

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null) return;

        Collider2D playerCollider = collision.collider;

        // pivot 위치와 무관하게, 콜라이더의 실제 경계(bounds)로 비교
        // 플레이어의 바닥이 굼바의 윗면보다 위에 있으면 밟은 것으로 판정
        bool isAbove = playerCollider.bounds.min.y >= myCollider.bounds.max.y - 0.15f;

        Debug.Log("굼바 충돌 - 플레이어 bottom: " + playerCollider.bounds.min.y +
                  " / 굼바 top: " + myCollider.bounds.max.y + " / isAbove: " + isAbove);

        if (isAbove)
        {
            player.Bounce(stompBounceForce);
            Destroy(gameObject);
        }
        else
        {
            player.Die();
        }
    }
}
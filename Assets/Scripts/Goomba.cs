using UnityEngine;

// Rigidbody2D(Gravity Scale 1, Freeze Rotation Z), Collider2D가 필요합니다.
// 프리팹으로 만들어 맵에 배치하세요.
public class Goomba : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float stompBounceForce = 6f;

    private Rigidbody2D rb;
    private Collider2D myCollider;
    private int direction = -1;  // 처음엔 왼쪽으로 이동
    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        // 플레이어가 아니면(벽, 블록, 파이프, 다른 굼바) 옆면 충돌 시 방향 전환
        if (player == null)
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

        Collider2D playerCollider = collision.collider;

        // 플레이어의 바닥이 굼바의 윗면보다 위에 있으면 밟은 것으로 판정
        bool isStomped = playerCollider.bounds.min.y >= myCollider.bounds.max.y - 0.2f;

        if (isStomped)
        {
            player.Bounce(stompBounceForce);
            Squash();
        }
        else
        {
            player.TakeDamage();  // 큰 상태면 작아지고, 작은 상태면 죽음
        }
    }

    // 납작해진 뒤 잠시 후 사라짐
    private void Squash()
    {
        isDead = true;
        myCollider.enabled = false;
        rb.simulated = false;

        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(s.x, s.y * 0.4f, s.z);

        Destroy(gameObject, 0.4f);
    }
}
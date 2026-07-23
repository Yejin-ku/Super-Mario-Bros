using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    [Header("버섯 / 무적")]
    public float bigScale = 1.5f;        // 버섯 먹었을 때 크기 배율
    public float invincibleTime = 1.5f;  // 작아진 직후 무적 시간

    // 벽돌 부수기, 피격 판정에서 사용
    public bool IsBig { get; private set; }

    private Rigidbody2D rb;
    private Collider2D myCollider;
    private SpriteRenderer sr;

    private Vector3 baseScale;
    private bool isGrounded;
    private bool isInvincible;
    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (isDead) return;

        CheckGround();
        Move();
        Jump();
    }

    // 발밑의 얇은 영역을 직접 검사한다.
    // 태그("Ground")에 의존하지 않으므로 블록 위, 굼바 위, 파이프 위에서도 점프가 된다.
    private void CheckGround()
    {
        Bounds b = myCollider.bounds;
        Collider2D[] hits = Physics2D.OverlapAreaAll(
            new Vector2(b.min.x + 0.05f, b.min.y - 0.08f),
            new Vector2(b.max.x - 0.05f, b.min.y + 0.02f));

        isGrounded = false;
        foreach (Collider2D hit in hits)
        {
            if (hit == myCollider) continue;
            if (hit.isTrigger) continue;   // KillZone, 깃발 같은 트리거는 무시
            isGrounded = true;
            break;
        }
    }

    private void Move()
    {
        float move = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (sr != null && move != 0f)
            sr.flipX = move < 0f;
    }

    private void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    // 굼바를 밟았을 때 (Goomba.cs에서 호출)
    public void Bounce(float bounceForce = 6f)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
    }

    // 버섯을 먹었을 때 (Mushroom.cs에서 호출)
    public void Grow()
    {
        if (isDead || IsBig) return;

        IsBig = true;
        transform.localScale = baseScale * bigScale;
        transform.position += Vector3.up * 0.2f; // 커지면서 바닥에 끼는 것 방지
    }

    // 굼바에게 옆에서 부딪혔을 때 (Goomba.cs에서 호출)
    public void TakeDamage()
    {
        if (isDead || isInvincible) return;

        if (IsBig)
        {
            IsBig = false;
            transform.localScale = baseScale;
            StartCoroutine(InvincibleRoutine());
        }
        else
        {
            Die();
        }
    }

    private IEnumerator InvincibleRoutine()
    {
        isInvincible = true;

        float t = 0f;
        while (t < invincibleTime)
        {
            if (sr != null) sr.enabled = !sr.enabled;  // 깜빡이는 연출
            t += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (sr != null) sr.enabled = true;
        isInvincible = false;
    }

    // 낙사(KillZone) 또는 작은 상태에서 피격
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        myCollider.enabled = false;                       // 바닥을 뚫고 떨어지도록
        rb.linearVelocity = new Vector2(0f, jumpForce * 0.8f); // 살짝 튀어오른 뒤 추락
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(1.2f);   // 죽는 연출을 보여준 뒤

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();

        gameObject.SetActive(false);
    }
}
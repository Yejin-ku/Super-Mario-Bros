using System.Collections;
using UnityEngine;

// 깃대 전체를 덮는 세로로 긴 BoxCollider2D를 만들고 Is Trigger를 체크한 뒤 이 스크립트를 붙이세요.
public class GoalFlag : MonoBehaviour
{
    [Header("깃발 내려오는 연출 (안 쓰면 비워두세요)")]
    public Transform flag;          // 깃발 스프라이트의 Transform
    public float flagBottomY = 0f;  // 깃발이 내려가서 멈출 높이
    public float flagSpeed = 4f;

    [Header("클리어 판정")]
    public float clearDelay = 1f;   // 연출 후 클리어 패널이 뜨기까지 시간

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        // 콜라이더가 자식에 있어도 찾을 수 있게 InParent 사용
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        triggered = true;
        StartCoroutine(ClearRoutine(player));
    }

    private IEnumerator ClearRoutine(PlayerController player)
    {
        // 플레이어 조작 정지 (Update가 멈추므로 입력을 안 받음)
        player.enabled = false;

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;

        // 깃발 내리기
        if (flag != null)
        {
            while (flag.position.y > flagBottomY)
            {
                flag.position += Vector3.down * flagSpeed * Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitForSeconds(clearDelay);

        if (GameManager.Instance != null)
            GameManager.Instance.LevelClear();
    }
}
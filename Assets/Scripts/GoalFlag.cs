using UnityEngine;

// Collider2D의 Is Trigger를 체크한 오브젝트에 붙이세요. (예: 맵 끝 깃발)
public class GoalFlag : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.LevelClear();
        }
    }
}

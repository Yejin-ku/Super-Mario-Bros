using UnityEngine;

// Collider2D의 Is Trigger를 체크한 오브젝트에 붙이세요. (예: 맵 맨 아래 긴 사각형)
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.Die();
        }
    }
}

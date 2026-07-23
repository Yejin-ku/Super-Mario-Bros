using UnityEngine;
using UnityEngine.SceneManagement;

// 빈 오브젝트(예: "GameManager")를 씬에 하나 만들고 이 스크립트를 붙이세요.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, GameOver, Clear }
    public GameState State { get; private set; } = GameState.Playing;

    [Header("UI 패널 (씬의 Canvas 밑에 만든 패널을 드래그)")]
    public GameObject gameOverPanel;
    public GameObject clearPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Time.timeScale = 1f;  // 재시작했을 때 멈춘 채로 시작하지 않도록

        // 시작할 땐 패널이 꺼져 있어야 함
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (clearPanel != null) clearPanel.SetActive(false);
    }

    private void Update()
    {
        // 게임이 끝난 상태에선 R 키로도 재시작 가능
        if (State != GameState.Playing && Input.GetKeyDown(KeyCode.R))
            RestartLevel();
    }

    // 낙사(KillZone) 또는 작은 상태에서 피격 시 PlayerController.Die()에서 호출
    public void GameOver()
    {
        if (State != GameState.Playing) return;
        State = GameState.GameOver;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        Debug.Log("Game Over");
    }

    // 깃발/골 지점 도착 시 GoalFlag.cs에서 호출
    public void LevelClear()
    {
        if (State != GameState.Playing) return;
        State = GameState.Clear;

        if (clearPanel != null) clearPanel.SetActive(true);
        Time.timeScale = 0f;

        Debug.Log("Level Clear!");
    }

    // 재시작 버튼(UI Button의 OnClick)에 연결해서 사용
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
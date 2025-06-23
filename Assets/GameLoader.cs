using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoader : MonoBehaviour
{   
    [Header("Config")]
    [SerializeField] private PlayerStats stats;
    [SerializeField] private string newGameSceneName = "SampleScene"; 
    [SerializeField] private Button loadGameButton;

    public PlayerStats Stats => stats;
    public PlayerMana PlayerMana { get; private set; }
    public PlayerHealth PlayerHealth { get; private set; }
    public PlayerAttack PlayerAttack { get; private set; }
    
    private void Start()
    {
        if (!PlayerPrefs.HasKey("LastScene"))
        {
            loadGameButton.interactable = false;
        }
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteKey("LastScene");
        Inventory.Instance.ClearInventory();
        stats.ResetPlayer();
        // SceneManager.LoadScene(newGameSceneName);
        SceneManager.LoadScene("PRE_TEST");
    }

    public void LoadGame()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", newGameSceneName);
        SceneManager.LoadScene(lastScene);
    }
}

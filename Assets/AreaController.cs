using UnityEngine;

public class AreaController : MonoBehaviour
{
    [Header("Konfigurasi Area")]
    [SerializeField] private int areaID; // Area ke-berapa (1, 2, dst)
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject area;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (wall != null) wall.SetActive(true);
            if (area != null) area.SetActive(false);

            // Kirim info area ke EnemyDeathManager
            EnemyDeathManager.Instance?.ActivateArea(areaID);

            // Mainkan battle BGM
            BackgroundSoundManager.Instance?.PlayBattleBGM();
        }
    }
}

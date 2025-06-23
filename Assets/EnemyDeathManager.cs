using UnityEngine;

public class EnemyDeathManager : MonoBehaviour
{
    public static EnemyDeathManager Instance { get; private set; }

    [System.Serializable]
    public class AreaConfig
    {
        public int areaID;
        public GameObject objectToSpawn;
        public int deathThreshold;
        public Transform spawnPosition;
        public bool isCompleted;
    }

    [Header("Area Settings")]
    public AreaConfig[] areaConfigs;
    public GameObject wall;

    [Header("Final Object Settings")]
    public GameObject finalObjectToActivate; // ganti nama jadi lebih jelas

    private int currentDeathCount = 0;
    private AreaConfig currentArea;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ActivateArea(int areaID)
    {
        foreach (var config in areaConfigs)
        {
            if (config.areaID == areaID)
            {
                currentArea = config;
                currentDeathCount = 0;
                Debug.Log("Area " + areaID + " aktif.");
                break;
            }
        }
    }

    public void ReportEnemyDeath(GameObject enemy)
    {
        if (currentArea == null) return;

        currentDeathCount++;
        Debug.Log($"Enemy mati di Area {currentArea.areaID}: {enemy.name} | Total mati: {currentDeathCount}");

        if (currentDeathCount >= currentArea.deathThreshold)
        {
            if (currentArea.objectToSpawn != null)
                Instantiate(currentArea.objectToSpawn, currentArea.spawnPosition.position, currentArea.spawnPosition.rotation);

            if (wall != null)
                wall.SetActive(false);

            currentArea.isCompleted = true;
            currentDeathCount = 0;

            // Mainkan Victory BGM saat area selesai
            BackgroundSoundManager.Instance.PlayVictoryBGMThenBackToNormal();

            currentArea = null;
            CheckIfAllAreasCompleted();
        }
    }


    private void CheckIfAllAreasCompleted()
    {
        foreach (var config in areaConfigs)
        {
            if (!config.isCompleted) return;
        }

        Debug.Log("Semua area selesai!");

        if (finalObjectToActivate != null)
        {
            finalObjectToActivate.SetActive(true);
            Debug.Log("Final object diaktifkan.");
        }
    }
}

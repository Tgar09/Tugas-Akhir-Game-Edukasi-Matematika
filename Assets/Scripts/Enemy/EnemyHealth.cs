using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

[Serializable]
public class QuestionDataBoss
{
    public int id;
    public string question;
    public string option_a;
    public string option_b;
    public string option_c;
    public string option_d;
    public string correct_answer;
}

[Serializable]
public class QuestionApiResponse34
{
    public string status;
    public List<QuestionDataBoss> data;
}

[Serializable]
public class AnswerData2
{
    public int user_id;
    public int final_test_id;
    public string selected_answer;
    public bool is_correct;
}

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public static event Action OnEnemyDeadEvent;

    [Header("Config")]
    [SerializeField] private float health = 10f;
    [SerializeField] private bool isBoss = false;

    [Header("Health Bar")]
    [SerializeField] private EnemyHealthBarUI healthBarUI;

    [Header("API")]
    [SerializeField] private string questionsApiUrl = "https://www.matemakitagame.my.id/api/final-tests";
    [SerializeField] private string answerApiUrl = "https://www.matemakitagame.my.id/api/final-test-results";

    [Header("UI")]
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button buttonA;
    [SerializeField] private TMP_Text buttonAText;
    [SerializeField] private Button buttonB;
    [SerializeField] private TMP_Text buttonBText;
    [SerializeField] private Button buttonC;
    [SerializeField] private TMP_Text buttonCText;
    [SerializeField] private Button buttonD;
    [SerializeField] private TMP_Text buttonDText;

    [Header("Others")]
    public GameObject projectile;
    public GameObject doors;

    private Animator animator;
    private Rigidbody2D rb2D;
    private EnemyBrain enemyBrain;
    private EnemyLoot enemyLoot;
    private EnemySelector enemySelector;
    private EnemyDeathManager deathManager;

    private float currentHealth;
    public float CurrentHealth => currentHealth;
    
    private List<QuestionDataBoss> questions = new List<QuestionDataBoss>();
    private QuestionDataBoss currentQuestion;
    private int currentQuestionIndex = 0;
    private bool hasTriggeredHalfHealth = false;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyBrain = GetComponent<EnemyBrain>();
        enemyLoot = GetComponent<EnemyLoot>();
        enemySelector = GetComponent<EnemySelector>();
        deathManager = FindObjectOfType<EnemyDeathManager>();
    }

    private void Start()
    {
        currentHealth = health;
        healthBarUI?.Initialize(transform);
        healthBarUI?.SetHealth(currentHealth, health);
        if (doors != null) doors.SetActive(false);
        if (isBoss) StartCoroutine(FetchQuestionsFromApi());
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBarUI?.SetHealth(currentHealth, health);

        if (isBoss && !hasTriggeredHalfHealth && currentHealth <= health / 2f)
        {
            hasTriggeredHalfHealth = true;
            TriggerHalfHealthWarning();
        }

        if (currentHealth <= 0f)
            Die();
        else
            DamageManager.Instance.ShowDamageText(amount, transform);
    }

    private IEnumerator FetchQuestionsFromApi()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(questionsApiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Gagal ambil soal: " + request.error);
                yield break;
            }

            try
            {
                string json = request.downloadHandler.text;
                QuestionApiResponse34 response = JsonUtility.FromJson<QuestionApiResponse34>(json);
                questions = response.data;
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing soal: " + e.Message);
            }
        }
    }

    private void TriggerHalfHealthWarning()
    {
        FreezeGame();
        currentQuestionIndex = 0;

        if (questions == null || questions.Count == 0)
        {
            Debug.LogWarning("Soal kosong.");
            ResumeGame();
            return;
        }

        ShowNextQuestion();
    }

    private void ShowNextQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            ResumeGame();
            return;
        }

        currentQuestion = questions[currentQuestionIndex];
        questionPanel.SetActive(true);

        questionText.text = currentQuestion.question;
        buttonAText.text = currentQuestion.option_a;
        buttonBText.text = currentQuestion.option_b;
        buttonCText.text = currentQuestion.option_c;
        buttonDText.text = currentQuestion.option_d;

        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();
        buttonC.onClick.RemoveAllListeners();
        buttonD.onClick.RemoveAllListeners();

        buttonA.onClick.AddListener(() => OnAnswerSelected("A"));
        buttonB.onClick.AddListener(() => OnAnswerSelected("B"));
        buttonC.onClick.AddListener(() => OnAnswerSelected("C"));
        buttonD.onClick.AddListener(() => OnAnswerSelected("D"));
    }

    private void OnAnswerSelected(string selected)
    {
        StartCoroutine(SendAnswerToApi(currentQuestion.id, selected));
    }

        private IEnumerator SendAnswerToApi(int questionId, string selectedAnswer)
        {
            int userId = PlayerPrefs.GetInt("user_id", 0);
            bool isCorrect = selectedAnswer == currentQuestion.correct_answer;

            AnswerData2 answer = new AnswerData2
            {
                user_id = userId,
                final_test_id = questionId,
                selected_answer = selectedAnswer,
                is_correct = isCorrect
            };

            string json = JsonUtility.ToJson(answer);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest request = new UnityWebRequest(answerApiUrl, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                    Debug.LogError("Gagal kirim jawaban: " + request.error);
                else
                    Debug.Log("Jawaban berhasil dikirim");
            }

            currentQuestionIndex++;
            ShowNextQuestion();
        }

    private void FreezeGame()
    {
        GameObject player = GameObject.FindWithTag("Player");
        player?.GetComponent<PlayerAttack>()?.Freeze();
        player?.GetComponent<PlayerMovement>()?.Freeze();
        enemyBrain?.Freeze();
        if (projectile != null) projectile.SetActive(false);
    }

    private void ResumeGame()
    {
        questionPanel.SetActive(false);
        GameObject player = GameObject.FindWithTag("Player");
        player?.GetComponent<PlayerAttack>()?.Unfreeze();
        player?.GetComponent<PlayerMovement>()?.Unfreeze();
        enemyBrain?.Unfreeze();
    }

    private void Die()
    {
        DisableEnemy();

        QuestManager.Instance.AddProgress("Kill2Enemy", 1);
        QuestManager.Instance.AddProgress("Kill5Enemy", 1);
        QuestManager.Instance.AddProgress("Kill10Enemy", 1);

        if (isBoss)
        {
            BackgroundSoundManager.Instance?.PlayNormalBGM();
            if (doors != null) doors.SetActive(true);
        }
    }

    private void DisableEnemy()
    {
        animator.SetTrigger("Dead");
        enemyBrain.enabled = false;
        enemySelector.NoSelectionCallback();
        rb2D.bodyType = RigidbodyType2D.Static;

        OnEnemyDeadEvent?.Invoke();
        GameManager.Instance.AddPlayerExp(enemyLoot.ExpDrop);

        if (projectile != null) projectile.SetActive(false);
        deathManager?.ReportEnemyDeath(gameObject);
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button buttonA;
    [SerializeField] private Button buttonB;
    [SerializeField] private TextMeshProUGUI buttonAText;
    [SerializeField] private TextMeshProUGUI buttonBText;

    private NPCQuiz currentQuiz;
    private NPCQuizInteraction currentNPC;
    private PlayerActions actions;

    private int currentQuestionIndex = 0;
    private int correctCount = 0;
    private int wrongCount = 0;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        actions = new PlayerActions();
    }

    private void Start()
    {
        actions.Enable();

        actions.Dialogue.Interact.performed += ctx =>
        {
            if (currentNPC != null && !quizPanel.activeSelf)
            {
                ShowQuiz(currentNPC.GetQuiz());
            }
        };

        buttonA.onClick.AddListener(() => AnswerSelected(0));
        buttonB.onClick.AddListener(() => AnswerSelected(1));

        quizPanel.SetActive(false);
    }

    public void SetCurrentQuiz(NPCQuizInteraction npc)
    {
        currentNPC = npc;
    }

    public void ClearCurrentQuiz()
    {
        currentNPC = null;
        HideQuiz();
    }

    public void ShowQuiz(NPCQuiz quiz)
    {
        currentQuiz = quiz;
        currentQuestionIndex = 0;
        correctCount = 0;
        wrongCount = 0;

        quizPanel.SetActive(true);

        buttonA.gameObject.SetActive(false);
        buttonB.gameObject.SetActive(false);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(currentQuiz.introText, () =>
        {
            StartCoroutine(DelayBeforeNextQuestion(1.5f));
        }));
    }

    private IEnumerator DelayBeforeNextQuestion(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowCurrentQuestion();
    }


    private IEnumerator TypeText(string fullText, System.Action onComplete)
    {
        questionText.text = "";
        foreach (char c in fullText)
        {
            questionText.text += c;
            yield return new WaitForSeconds(0.05f);
        }

        onComplete?.Invoke();
    }

    private void ShowCurrentQuestion()
    {
        var question = currentQuiz.questions[currentQuestionIndex];

        buttonA.gameObject.SetActive(false);
        buttonB.gameObject.SetActive(false);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(question.question, () =>
        {
            buttonA.gameObject.SetActive(true);
            buttonB.gameObject.SetActive(true);
        }));

        buttonAText.text = question.optionA;
        buttonBText.text = question.optionB;
    }

    private void AnswerSelected(int index)
    {
        var question = currentQuiz.questions[currentQuestionIndex];

        if (index == question.correctAnswerIndex)
        {
            correctCount++;
            Debug.Log("Jawaban Benar");
        }
        else
        {
            wrongCount++;
            Debug.Log("Jawaban Salah");
        }

        currentQuestionIndex++;

        if (currentQuestionIndex < currentQuiz.questions.Length)
        {
            ShowCurrentQuestion();
        }
        else
        {
            ShowResult();
        }
    }

    private void ShowResult()
    {
        buttonA.gameObject.SetActive(false);
        buttonB.gameObject.SetActive(false);

        questionText.text = $"Quiz selesai!\n\n" +
                            $"Benar: {correctCount}\n" +
                            $"Salah: {wrongCount}";
    }

    public void HideQuiz()
    {
        quizPanel.SetActive(false);
        buttonA.gameObject.SetActive(true);
        buttonB.gameObject.SetActive(true);
    }
}

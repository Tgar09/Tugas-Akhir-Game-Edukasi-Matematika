using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class QuestionData3
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
public class QuestionApiResponse3
{
    public string status;
    public List<QuestionData3> data;
}

[Serializable]
public class AnswerData
{
    public int user_id;
    public int pre_test_id;
    public string selected_answer;
    public bool is_correct;
}

public class PreTestController : MonoBehaviour
{
    [Header("API")]
    public string questionsUrl = "https://www.matemakitagame.my.id/api/pre-tests";
    public string answerUrl = "https://www.matemakitagame.my.id/api/pre-test-results";

    [Header("UI")]
    public TMP_Text questionText;
    public TMP_Text buttonAText, buttonBText, buttonCText, buttonDText;
    public Button buttonA, buttonB, buttonC, buttonD;
    public GameObject resultPanel;
    public TMP_Text resultText;

    private List<QuestionData3> questions = new List<QuestionData3>();
    private QuestionData3 currentQuestion;
    private int currentIndex = 0;
    private int correctCount = 0;
    private int wrongCount = 0;
    private int userId;

    private void Start()
    {
        resultPanel.SetActive(false);
        userId = PlayerPrefs.GetInt("user_id", 0);
        if (userId == 0)
        {
            Debug.LogError("user_id tidak ditemukan di PlayerPrefs!");
            return;
        }

        StartCoroutine(LoadQuestions());
    }

    IEnumerator LoadQuestions()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(questionsUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Gagal mengambil soal: " + request.error);
                yield break;
            }

            QuestionApiResponse3 response = JsonUtility.FromJson<QuestionApiResponse3>(request.downloadHandler.text);
            questions = response.data;

            if (questions.Count == 0)
            {
                resultText.text = "Soal tidak tersedia.";
                resultPanel.SetActive(true);
                yield break;
            }

            currentIndex = 0;
            correctCount = 0;
            wrongCount = 0;

            ShowQuestion();
        }
    }

    void ShowQuestion()
    {
        if (currentIndex >= questions.Count)
        {
            ShowFinalResult();
            return;
        }

        currentQuestion = questions[currentIndex];
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

    void OnAnswerSelected(string selected)
    {
        bool isCorrect = selected == currentQuestion.correct_answer;

        if (isCorrect)
            correctCount++;
        else
            wrongCount++;

        StartCoroutine(SendAnswer(currentQuestion.id, selected, isCorrect));

        currentIndex++;
        ShowQuestion();
    }

    IEnumerator SendAnswer(int questionId, string selected, bool isCorrect)
    {
        AnswerData answer = new AnswerData
        {
            user_id = userId,
            pre_test_id = questionId,
            selected_answer = selected,
            is_correct = isCorrect
        };

        string jsonData = JsonUtility.ToJson(answer);

        using (UnityWebRequest request = new UnityWebRequest(answerUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Gagal mengirim jawaban: " + request.error);
            }
        }
    }

    void ShowFinalResult()
    {
        resultPanel.SetActive(true);
        resultText.text = $"Benar: {correctCount} | Salah: {wrongCount}";
        StartCoroutine(GoToSceneAfterDelay("SampleScene", 1f));
    }

    IEnumerator GoToSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}

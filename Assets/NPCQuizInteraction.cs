using UnityEngine;

public class NPCQuizInteraction : MonoBehaviour
{
    [SerializeField] private NPCQuiz quiz;
    [SerializeField] private GameObject interactionBox;

    public NPCQuiz GetQuiz() => quiz;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactionBox.SetActive(true);
            QuizManager.Instance.SetCurrentQuiz(this); // simpan referensi NPC
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactionBox.SetActive(false);
            QuizManager.Instance.ClearCurrentQuiz();
        }
    }
}

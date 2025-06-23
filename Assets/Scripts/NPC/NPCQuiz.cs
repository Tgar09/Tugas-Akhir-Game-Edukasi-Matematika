using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Quiz", menuName = "NPC/Quiz")]
public class NPCQuiz : ScriptableObject
{
    [TextArea]
    public string introText; // 🟢 Ini bisa diisi dari Inspector

    [System.Serializable]
    public class QuizQuestion
    {
        public string question;
        public string optionA;
        public string optionB;
        public int correctAnswerIndex; // 0 = A, 1 = B
    }

    public QuizQuestion[] questions;
}

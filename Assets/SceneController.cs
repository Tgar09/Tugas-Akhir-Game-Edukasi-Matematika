using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Nama scene tujuan (isi di Inspector)")]
    public string namaScene;

    // Panggil ini lewat tombol
    public void LoadSceneManual()
    {
        if (!string.IsNullOrEmpty(namaScene))
        {
            SceneManager.LoadScene(namaScene);
        }
        else
        {
            Debug.LogWarning("Nama scene belum diisi di Inspector!");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class RegisterForm : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public Button registerButton;

    public GameObject popupPanel;
    public TMP_Text popupMessageText;

    public string registerUrl = "http://192.168.1.6:8080/api/register";

    void Start()
    {
        registerButton.onClick.AddListener(OnRegister);
        popupPanel.SetActive(false);
    }

    void OnRegister()
    {
        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", nameInput.text);
        form.AddField("email", emailInput.text);
        form.AddField("password", passwordInput.text);
        form.AddField("password_confirmation", confirmPasswordInput.text);

        UnityWebRequest request = UnityWebRequest.Post(registerUrl, form);
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowPopup("Registrasi berhasil!");
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("Login"); // Ganti nama scene jika perlu
        }
        else
        {
            string errorMessage = "Registrasi gagal!";
            if (request.responseCode == 422)
            {
                try
                {
                    LaravelError errorObj = JsonUtility.FromJson<LaravelError>(request.downloadHandler.text);
                    if (errorObj.errors != null)
                    {
                        errorMessage = "";
                        foreach (var err in errorObj.errors.All())
                        {
                            errorMessage += "- " + err + "\n";
                        }
                    }
                }
                catch (Exception)
                {
                    errorMessage = "Terjadi kesalahan saat membaca pesan error.";
                }
            }
            ShowPopup(errorMessage);
        }
    }

    void ShowPopup(string message)
    {
        popupMessageText.text = message;
        popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}
[System.Serializable]
public class LaravelError
{
    public string message;
    public LaravelFieldErrors errors;

    [System.Serializable]
    public class LaravelFieldErrors
    {
        public string[] name;
        public string[] email;
        public string[] password;

        public string[] All()
        {
            var all = new System.Collections.Generic.List<string>();
            if (name != null) all.AddRange(name);
            if (email != null) all.AddRange(email);
            if (password != null) all.AddRange(password);
            return all.ToArray();
        }
    }
}

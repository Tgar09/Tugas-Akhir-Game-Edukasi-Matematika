using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class LoginForm : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;

    public GameObject popupPanel;
    public TMP_Text popupMessageText;

    public string loginUrl = "http://192.168.1.6:8080/api/login";

    void Start()
    {
        loginButton.onClick.AddListener(OnLogin);
        popupPanel.SetActive(false); // Sembunyikan popup saat awal
    }

    void OnLogin()
    {
        StartCoroutine(Login());
    }

    IEnumerator Login()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", emailInput.text);
        form.AddField("password", passwordInput.text);

        UnityWebRequest request = UnityWebRequest.Post(loginUrl, form);
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Login berhasil: " + request.downloadHandler.text);

            // Parse JSON
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

            if (response.status == "success")
            {
                // Simpan ke PlayerPrefs
                PlayerPrefs.SetInt("user_id", response.data.id);
                PlayerPrefs.SetString("user_name", response.data.name);
                PlayerPrefs.SetString("user_email", response.data.email);
                PlayerPrefs.SetString("user_role", response.data.role);
                PlayerPrefs.Save(); // Simpan permanen

                ShowPopup("Login berhasil!");
                yield return new WaitForSeconds(2f);
                SceneManager.LoadScene("MainMenu"); // Ganti nama scene tujuan
            }
            else
            {
                ShowPopup("Login gagal!");
            }
        }
        else
        {
            string errorMessage = "Login gagal!";
            if (request.responseCode == 422 || request.responseCode == 401)
            {
                try
                {
                    LaravelError2 errorObj = JsonUtility.FromJson<LaravelError2>(request.downloadHandler.text);
                    if (errorObj.errors != null)
                    {
                        errorMessage = "";
                        foreach (var err in errorObj.errors.All())
                        {
                            errorMessage += "- " + err + "\n";
                        }
                    }
                    else if (!string.IsNullOrEmpty(errorObj.message))
                    {
                        errorMessage = errorObj.message;
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
public class LaravelError2
{
    public string message;
    public LaravelFieldErrors errors;

    [System.Serializable]
    public class LaravelFieldErrors
    {
        public string[] email;
        public string[] password;

        public string[] All()
        {
            var all = new System.Collections.Generic.List<string>();
            if (email != null) all.AddRange(email);
            if (password != null) all.AddRange(password);
            return all.ToArray();
        }
    }
}
[System.Serializable]
public class LoginResponse
{
    public string status;
    public string message;
    public UserData data;

    [System.Serializable]
    public class UserData
    {
        public int id;
        public string name;
        public string email;
        public string role;
    }
}


using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_Text resultText;

    public string targetScene = "_MainScene";

    public void StartLogin()
    {
        StartCoroutine(LoginAndChangeScene());
    }

    IEnumerator LoginAndChangeScene()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", usernameField.text);
        form.AddField("password", passwordField.text);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/unity_api/login.php", form))
        {
            resultText.text = "Attempting to log in...";
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                resultText.text = "Error: " + www.error;
            }
            else
            {
                string responseText = www.downloadHandler.text;
                if (responseText.Contains("success"))
                {
                    resultText.text = "Login successful!";
                    yield return new WaitForSeconds(1f);
                    SceneManager.LoadScene(targetScene); 
                }
                else
                {
                    resultText.text = "Login failed!";
                }
            }
        }
    }
}

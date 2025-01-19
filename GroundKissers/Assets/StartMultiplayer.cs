using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMultiplayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        if (NetworkManager.Singleton !=null)
        {
            NetworkManager.Singleton.Shutdown(); // Desconecta al cliente
            Destroy(NetworkManager.Singleton.gameObject);
        }
      
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("_MainSceneNW");

        }
    }
}

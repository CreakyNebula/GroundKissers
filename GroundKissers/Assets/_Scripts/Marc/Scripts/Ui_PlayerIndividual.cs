using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ui_PlayerIndividual : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Image[] hearts; // Para inspección opcional
    [SerializeField] private Sprite deadHeart; // Para inspección opcional

    [SerializeField] public Local_Player_Script scriptPlayer;

    public bool test;
    public int vidas;
    public int myId;
    void Start()
    {
        hearts = GetComponentsInChildren<Image>();
        vidas=hearts.Length-1;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(test)
        {
            TakeDamage();
            test = false;
        }

    }
    public void TakeDamage()
    {

        hearts[vidas].sprite=deadHeart;
        vidas--;
    }

}

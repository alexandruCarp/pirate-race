using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectPlayers : MonoBehaviour
{
    
    [SerializeField] TMP_Text numJucatori;
    public int num = 2;
    // Start is called before the first frame update
    void Start()
    {
        numJucatori.text = num.ToString();
    }

    

    public void IncreaseVal()
    {
        if (num < 5)
        {
            num++;
            numJucatori.text = num.ToString();
            Debug.Log("Hello world!");
        }
    }

    public void DecreaseVal()
    {
        if (num > 2)
        {
            num--;
            numJucatori.text = num.ToString();
        }
        
    }

    public void NextScene()
    {
        NumarJucatori.NumJucatori = num.ToString();
        SceneManager.LoadScene("CodePage");
    }
}

public static class NumarJucatori
{
    public static string NumJucatori;
}

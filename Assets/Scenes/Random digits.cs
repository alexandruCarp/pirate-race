using UnityEngine;
using TMPro;  // Include the TextMeshPro namespace

public class RandomDigits : MonoBehaviour
{
    public TMP_Text tmpText;  // Reference to the TextMeshPro component

    void Start()
    {
        // Generate a random 6-digit number
        int randomNumber = Random.Range(100000, 1000000);
        // Set the text of the TextMeshPro component
        tmpText.text = randomNumber.ToString();
    }
}


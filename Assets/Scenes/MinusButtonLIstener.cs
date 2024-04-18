using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ClickExample : MonoBehaviour {
	public Button yourButton;

    // add reference to text component
    public TMP_Text text;

	void Start () {
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
		print("You have clicked the button!");
        // change text when button is clicked
        int nr_jucatori = text.text[0] - '0';
        text.text = (nr_jucatori - 1).ToString();
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundText : MonoBehaviour
{
    private Text _roundTextObj;
    // Start is called before the first frame update
    void Start()
    {
        _roundTextObj = transform.Find("Round_Text").GetComponent<Text>();
        gameObject.SetActive(false);
    }

    public void UpdateRoundText(int roundNumber)
    {
        gameObject.SetActive(true);
        _roundTextObj.text = $"{roundNumber}";
    }
}

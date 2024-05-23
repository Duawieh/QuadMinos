using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class S_UIScoreShower : MonoBehaviour
{
    private GameObject field;

    // Start is called before the first frame update
    void Start()
    {
        field = GameObject.FindGameObjectWithTag("Field");
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().text = field.GetComponent<S_Score>().SCORE.ToString();
    }
}

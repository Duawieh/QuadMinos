using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class S_ReviewController : MonoBehaviour
{
    private float[] paceChoices = { 1.0f, 1.25f, 1.5f, 2.0f, 5.0f, 0.25f, 0.5f };
    private int currentChoice = 0;
    private GameObject UI_PaceText;
    public GameObject UI_Background;
    public GameObject UI_EixtButton;
    public GameObject UI_PaceButton;

    // Start is called before the first frame update
    void Start()
    {
        if (!BattleInfo.ReviewMode) {
            UI_EixtButton.GetComponent<Image>().enabled = false;
            UI_PaceButton.GetComponent<Image>().enabled = false;
            UI_EixtButton.GetComponent<Button>().enabled = false;
            UI_PaceButton.GetComponent<Button>().enabled = false;
            UI_EixtButton.transform.GetChild(0).GetComponent<Text>().enabled = false;
            UI_PaceButton.transform.GetChild(0).GetComponent<Text>().enabled = false;
        }
        UI_PaceText = UI_PaceButton.transform.GetChild(0).gameObject;
        currentChoice = 0;
        return;
    }

    public void ButtonClick_Exit() {
        StartCoroutine(UI_Background.GetComponent<S_PlayfieldBackgroundMusic>().AudioStop());
        StartCoroutine(nameof(Anim_Exit));
        return;
    }

    public void ButtonClick_Pace() {
        currentChoice = (currentChoice + 1) % 7;
        BattleInfo.ReviewPace = paceChoices[currentChoice];
        string pace = paceChoices[currentChoice].ToString("f2");
        UI_PaceText.GetComponent<Text>().text = " x" + pace;
        Time.timeScale = paceChoices[currentChoice];
        return;
    }

    private IEnumerator Anim_Exit() {
        Time.timeScale = 1.0f;
        float t = 0.0f;

        GameObject screenMask = GameObject.Find("ScreenMask");
        screenMask.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        screenMask.GetComponent<Canvas>().sortingOrder = 100;

        while (t < 1.0f) {
            t += Time.deltaTime;
            float a = Functions.F_paraFadeinout(t, 1.0f, 0, 1);
            screenMask.GetComponent<Image>().color = new Color(0, 0, 0, a);
            yield return null;
        }
        screenMask.GetComponent<Image>().color = new Color(0, 0, 0, 1);

        LoadInfo.SceneName = "MainMenu";
        SceneManager.LoadScene(1);
        yield break;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_MainMenuButtons : MonoBehaviour
{
    public GameObject panel_solo;
    public GameObject panel_multy;
    public GameObject panel_config;
    public GameObject panel_copyright;

    private bool Quit = false;

    public void ClickButton_SOLO()
    {
        if (Quit) return;
        GameObject[] panels = GameObject.FindGameObjectsWithTag("ActUnit");
        foreach (GameObject panel in panels) { Destroy(panel); }
        GameObject _panel = Instantiate(panel_solo);
        _panel.transform.parent = transform.parent;
        _panel.transform.position = Vector3.zero;
        return;
    }

    public void ClickButton_MULTY()
    {
        if (Quit) return;
        GameObject[] panels = GameObject.FindGameObjectsWithTag("ActUnit");
        foreach (GameObject panel in panels) { Destroy(panel); }
        GameObject _panel = Instantiate(panel_multy);
        _panel.transform.parent = transform.parent;
        _panel.transform.position = Vector3.zero;
        return;
    }

    public void ClickButton_CONFIG() {
        if (Quit) return;
        GameObject[] panels = GameObject.FindGameObjectsWithTag("ActUnit");
        foreach (GameObject panel in panels) { Destroy(panel); }
        GameObject _panel = Instantiate(panel_config);
        _panel.transform.parent = transform.parent;
        _panel.transform.position = Vector3.zero;
        return;
    }

    public void ClickButton_COPYRIGHT()
    {
        if (Quit) return;
        GameObject[] panels = GameObject.FindGameObjectsWithTag("ActUnit");
        foreach (GameObject panel in panels) { Destroy(panel); }
        GameObject _panel = Instantiate(panel_copyright);
        _panel.transform.parent = transform.parent;
        _panel.transform.position = Vector3.zero;
        return;
    }

    public void ClickButton_EXIT()
    {
        GameObject[] panels = GameObject.FindGameObjectsWithTag("ActUnit");
        foreach (GameObject panel in panels) { Destroy(panel); }
        Quit = true;
        StartCoroutine(GetComponent<Anim_MainButtons>().ExitAnimation());
        return;
    }
}

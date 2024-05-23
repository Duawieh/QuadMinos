using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadInfo
{
    public static string SceneName;
}

public class LoadScene : MonoBehaviour
{
    public GameObject[] minos;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadNewScene(LoadInfo.SceneName));
        StartCoroutine(Anim_Loading());
    }

    private IEnumerator LoadNewScene(string sceneName)
    {
        AsyncOperation loadState = SceneManager.LoadSceneAsync(sceneName);
        loadState.allowSceneActivation = false;

        yield return new WaitForSeconds(0.2f);

        loadState.allowSceneActivation = true;

        yield break;
    }

    private IEnumerator Anim_Loading()
    {
        int minoIndex = 0;
        GameObject lstMino = null;
        while (true)
        {
            if (lstMino != null) Destroy(lstMino);
            GameObject mino = Instantiate(minos[minoIndex]);
            mino.transform.localScale = Vector3.one;
            mino.transform.position = Vector3.zero;
            mino.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 4) * 90);
            mino.transform.parent = transform.parent;
            mino.GetComponent<Mino_Active>().enabled = false;
            lstMino = mino;

            minoIndex++;
            if (minoIndex >= 7) minoIndex = 0;
            yield return new WaitForSeconds(0.1f);
        }
    }
}

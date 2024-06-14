using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

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

    private void LoadRecord(string _sceneName)
    {
        BattleRecords.Clear();

        if (_sceneName != "BattleScene") return;
        if (!BattleInfo.ReviewMode) return;

        RecordFileOperations.LoadRecords(BattleInfo.GameMode, BattleInfo.RecordName);
        return;
    }

    private void SaveRecord(string _sceneName)
    {
        if (_sceneName == "BattleScene") return;
        if (!BattleInfo.RecordMode) return;

        RecordFileOperations.SaveRecords(BattleInfo.GameMode);
        return;
    }

    private IEnumerator LoadNewScene(string sceneName)
    {
        AsyncOperation loadState = SceneManager.LoadSceneAsync(sceneName);
        loadState.allowSceneActivation = false;

        yield return new WaitForSecondsRealtime(0.1f);

        SaveRecord(sceneName);

        yield return new WaitForSecondsRealtime(0.1f);

        LoadRecord(sceneName);

        yield return new WaitForSecondsRealtime(0.1f);

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
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}

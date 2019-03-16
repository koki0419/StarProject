using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectSceneController : MonoBehaviour
{

    [SerializeField] FadeLayer fadeLayer;

    //フェード時表示用TEXT
    public GameObject fadeText;

    //フェード時表示用マスコットキャラ
    public GameObject fadeChara;

    bool isPlaying = false;

    IEnumerator Start()
    {
        isPlaying = false;
        yield return null;
        fadeText.SetActive(false);
        fadeChara.SetActive(false);
        yield return null;

        yield return fadeLayer.FadeInEnumerator(2);
        isPlaying = true;
    }

    //Start()より早く処理する
    private void Awake()
    {
        //初期化
        fadeLayer.ForceColor(Color.black);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                SceneManager.LoadScene("PrototypeScene");
            }
        }

    }
}

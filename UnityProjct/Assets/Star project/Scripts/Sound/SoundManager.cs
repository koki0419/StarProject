using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmAudio;
    [SerializeField] AudioSource playerSeAudio;
    [SerializeField] AudioSource playerLoopSeAudio;
    [SerializeField] AudioSource obstaclesSeAudio;

    [SerializeField] AudioClip normalBgm;
    [SerializeField] AudioClip bossBgm;
    [SerializeField] AudioClip jingleClear;
    [SerializeField] AudioClip jingleGameOver;
    [SerializeField] AudioClip[] se;

    /// <summary>
    /// BGM再生用
    /// BGMを通常とボス戦で変えられるようにclipをここでセットして再生
    /// </summary>
    /// <param name="playBjmName">再生したいBGMの種類を取得します</param>
    public void PlayBgm(string playBjmName)
    {
        if (bgmAudio.isPlaying)
        {
            bgmAudio.Stop();
        }
        if (playBjmName == "NormalBGM")
        {
            bgmAudio.clip = normalBgm;
        }
        else if (playBjmName == "BossBGM")
        {
            bgmAudio.clip = bossBgm;
        }
        bgmAudio.Play();
    }
    /// <summary>
    /// BGMをStopさせたいときに使用します
    /// </summary>
    public void StopBgm()
    {
        bgmAudio.Stop();
    }
    /// <summary>
    /// 全てのオーディオを停止します
    /// </summary>
    public void OllStopSound()
    {
        bgmAudio.Stop();
        playerSeAudio.Stop();
        obstaclesSeAudio.Stop();
    }
    /// <summary>
    /// BGMを途中から再生したいときに使用します
    /// 作ってみたかっただけなので使用しなくてもいいです
    /// BGM以外を流したいときは変えてください
    /// 途中から流すときは何秒後から再生かを指定する必要があります
    /// </summary>
    /// <param name="playTime">何秒後から再生させるか</param>
    public void PlayFromTheMiddle(float playTime)
    {
        bgmAudio.clip = normalBgm;
        bgmAudio.time = playTime;
        bgmAudio.Play();
    }
    /// <summary>
    /// クリア、ゲームオーバーのジングルを再生します
    /// GBMが流れていればStopさせてから再生させます
    /// </summary>
    /// <param name="playJingleName">ジングルの種類をstring型で取得します</param>
    public void PlayJingle(string playJingleName)
    {
        if (bgmAudio.isPlaying)
        {
            bgmAudio.Stop();
        }
        if (playJingleName == "GameClear")
        {
            bgmAudio.clip = jingleClear;
        }
        else if (playJingleName == "GameOver")
        {
            bgmAudio.clip = jingleGameOver;
        }
        bgmAudio.Play();
    }
    /// <summary>
    /// プレイヤーのSEを再生します
    /// </summary>
    /// <param name="playSeNum"></param>
    public void PlayPlayerSe(int playSeNum)
    {
        if (playerSeAudio.isPlaying && playerLoopSeAudio.isPlaying)
        {
            return;
        }
        else if (!playerSeAudio.isPlaying)
        {
            playerSeAudio.PlayOneShot(se[playSeNum]);
        }
    }
    public void PlayPlayerLoopSe(int playSeNum)
    {
        if (playerSeAudio.isPlaying && playerLoopSeAudio.isPlaying)
        {
            return;
        }
        else if (!playerLoopSeAudio.isPlaying)
        {
            playerLoopSeAudio.PlayOneShot(se[playSeNum]);
        }
    }

    public void StopPlayerSe()
    {
        playerSeAudio.Stop();
        playerLoopSeAudio.Stop();
    }

    public void PlayObstaclesSe(int playSeNum)
    {
        if (obstaclesSeAudio.isPlaying)
        {
            return;
        }
        else if (!obstaclesSeAudio.isPlaying)
        {
            obstaclesSeAudio.PlayOneShot(se[playSeNum]);
        }
    }
    public void StopObstaclesSe()
    {
        obstaclesSeAudio.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

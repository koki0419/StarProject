﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudio = null;
    [SerializeField] private AudioSource playerSeAudio = null;
    [SerializeField] private AudioSource playerLoopSeAudio = null;
    [SerializeField] private AudioSource obstaclesSeAudio = null;

    [SerializeField] private AudioClip normalBgm = null;
    [SerializeField] private AudioClip bossBgm = null;
    [SerializeField] private AudioClip jingleClear = null;
    [SerializeField] private AudioClip jingleGameOver = null;
    [SerializeField] private AudioClip[] se = null;

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
    /// <summary>
    /// プレイヤーのSEをループ再生します
    /// </summary>
    /// <param name="playSeNum"></param>
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
    /// <summary>
    /// プレイヤーSEの再生をすべてストップします
    /// </summary>
    public void StopPlayerSe()
    {
        playerSeAudio.Stop();
        playerLoopSeAudio.Stop();
    }
    /// <summary>
    /// 障害物、エネミーのSEを再生します
    /// SEが再生中は使用できません
    /// 多重再生予防
    /// </summary>
    /// <param name="playSeNum"></param>
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
    /// <summary>
    /// 障害物、エネミーのSEをストップさせます
    /// </summary>
    public void StopObstaclesSe()
    {
        obstaclesSeAudio.Stop();
    }
}

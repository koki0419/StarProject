using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    //完了した処理の削除用
    List<int> removeRes = new List<int>();

    //フェード処理を行うスプライトの情報
    class FadeSprite
    {
        public SpriteRenderer renderer;

        //初期Alpha値
        public float baseAlpha;
        //目標Alpha値
        public float targetAlpha;

        //フェード完了までのフレーム
        public int maxFrame;
        //現在フレーム
        public int frame;
    }

    List<FadeSprite> fadeSprites = new List<FadeSprite>();

    //フェード処理を行う画像の情報
    class FadeImage
    {
        public Image image;

        //初期Alpha値
        public float baseAlpha;
        //目標Alpha値
        public float targetAlpha;

        //フェード完了までのフレーム
        public int maxFrame;
        //現在フレーム
        public int frame;
    }

    List<FadeImage> fadeImages = new List<FadeImage>();

    //フェード処理を行うTransformの情報
    class FadeTransform
    {
        public Transform transform;

        //初期座標
        public Vector3 basePosition;
        //目標座標
        public Vector3 targetPosition;

        //初期スケール
        public Vector3 baseScale;
        //目標スケール
        public Vector3 targetScale;

        //フェード完了までのフレーム
        public int maxFrame;
        //現在フレーム
        public int frame;
    }

    List<FadeTransform> fadeTransforms = new List<FadeTransform>();

    //フェード処理を行うRectTransformの情報
    class FadeRect
    {
        public RectTransform rect;

        //初期座標
        public Vector3 basePosition;
        //目標座標
        public Vector3 targetPosition;

        //初期スケール
        public Vector3 baseScale;
        //目標スケール
        public Vector3 targetScale;

        //フェード完了までのフレーム
        public int maxFrame;
        //現在フレーム
        public int frame;
    }

    List<FadeRect> fadeRects = new List<FadeRect>();

    // Start is called before the first frame update
    void Start()
    {
        {
            //Debug.Log("testSprite is " + testSprite.GetType());
            //Debug.Log("testImage is " + testImage.GetType());
            //if(testSprite.GetType().ToString() == "UnityEngine.SpriteRenderer")
            //{
            //    Debug.Log("testSprite is SpriteRenderer");
            //}
            //if (testImage.GetType().ToString() == "UnityEngine.UI.Image")
            //{
            //    Debug.Log("testImage is Image");
            //}

        }
    }

    // Update is called once per frame
    void Update()
    {
        //スプライトのフェード処理
        if(fadeSprites.Count > 0)
        {
            removeRes.Clear();

            for(int i = 0; i < fadeSprites.Count;i++)
            {
                //設定するアルファ値を計算
                fadeSprites[i].frame++;
                float fadeAlpha = fadeSprites[i].baseAlpha + (fadeSprites[i].targetAlpha - fadeSprites[i].baseAlpha)
                                   * (fadeSprites[i].frame / (float)fadeSprites[i].maxFrame);
                //アルファ値を設定
                fadeSprites[i].renderer.color = new Color(fadeSprites[i].renderer.color.r,
                                                          fadeSprites[i].renderer.color.g,
                                                          fadeSprites[i].renderer.color.b,
                                                          fadeAlpha);

                //処理が完了していた場合要素の削除を予約
                if(fadeSprites[i].frame >= fadeSprites[i].maxFrame)
                {
                    fadeSprites[i].renderer.color = new Color(fadeSprites[i].renderer.color.r,
                                                          fadeSprites[i].renderer.color.g,
                                                          fadeSprites[i].renderer.color.b,
                                                          fadeSprites[i].targetAlpha);
                    removeRes.Add(i);
                }
            }

            //要素の削除
            if(removeRes.Count > 0)
            {
                for(int i = removeRes.Count - 1; i >= 0;i--)
                {
                    fadeSprites.RemoveAt(removeRes[i]);
                }
            }
        }

        //画像のフェード処理
        if (fadeImages.Count > 0)
        {
            removeRes.Clear();

            for (int i = 0; i < fadeImages.Count; i++)
            {
                //設定するアルファ値を計算
                fadeImages[i].frame++;
                float fadeAlpha = fadeImages[i].baseAlpha + (fadeImages[i].targetAlpha - fadeImages[i].baseAlpha)
                                   * (fadeImages[i].frame / (float)fadeImages[i].maxFrame);
                //アルファ値を設定
                fadeImages[i].image.color = new Color(fadeImages[i].image.color.r,
                                                          fadeImages[i].image.color.g,
                                                          fadeImages[i].image.color.b,
                                                          fadeAlpha);

                //処理が完了していた場合要素の削除を予約
                if (fadeImages[i].frame >= fadeImages[i].maxFrame)
                {
                    fadeImages[i].image.color = new Color(fadeImages[i].image.color.r,
                                                          fadeImages[i].image.color.g,
                                                          fadeImages[i].image.color.b,
                                                          fadeImages[i].targetAlpha);
                    removeRes.Add(i);
                }
            }

            //要素の削除
            if (removeRes.Count > 0)
            {
                for (int i = removeRes.Count - 1; i >= 0; i--)
                {
                    fadeImages.RemoveAt(removeRes[i]);
                }
            }
        }

        //Transformのフェード処理
        if (fadeTransforms.Count > 0)
        {
            removeRes.Clear();

            for (int i = 0; i < fadeTransforms.Count; i++)
            {
                fadeTransforms[i].frame++;
                float progress = fadeTransforms[i].frame / (float)fadeTransforms[i].maxFrame;
                //座標を移動
                fadeTransforms[i].transform.position = Vector3.Lerp(fadeTransforms[i].basePosition,
                                                                    fadeTransforms[i].targetPosition,
                                                                    progress);

                //スケールを変更
                fadeTransforms[i].transform.localScale = Vector3.Lerp(fadeTransforms[i].baseScale,
                                                                    fadeTransforms[i].targetScale,
                                                                    progress);

                //処理が完了していた場合要素の削除を予約
                if (fadeTransforms[i].frame >= fadeTransforms[i].maxFrame)
                {
                    fadeTransforms[i].transform.position = fadeTransforms[i].targetPosition;
                    fadeTransforms[i].transform.localScale = fadeTransforms[i].targetScale;

                    removeRes.Add(i);
                }
            }

            //要素の削除
            if (removeRes.Count > 0)
            {
                for (int i = removeRes.Count - 1; i >= 0; i--)
                {
                    fadeTransforms.RemoveAt(removeRes[i]);
                }
            }
        }

        //RectTransformのフェード処理
        if (fadeRects.Count > 0)
        {
            removeRes.Clear();

            for (int i = 0; i < fadeRects.Count; i++)
            {
                fadeRects[i].frame++;
                float progress = fadeRects[i].frame / (float)fadeRects[i].maxFrame;
                //座標を移動
                fadeRects[i].rect.localPosition = Vector3.Lerp(fadeRects[i].basePosition,
                                                                    fadeRects[i].targetPosition,
                                                                    progress);

                //スケールを変更
                fadeRects[i].rect.localScale = Vector3.Lerp(fadeRects[i].baseScale,
                                                                    fadeRects[i].targetScale,
                                                                    progress);

                //処理が完了していた場合要素の削除を予約
                if (fadeRects[i].frame >= fadeRects[i].maxFrame)
                {
                    fadeRects[i].rect.localPosition = fadeRects[i].targetPosition;
                    fadeRects[i].rect.localScale = fadeRects[i].targetScale;

                    removeRes.Add(i);
                }
            }

            //要素の削除
            if (removeRes.Count > 0)
            {
                for (int i = removeRes.Count - 1; i >= 0; i--)
                {
                    fadeRects.RemoveAt(removeRes[i]);
                }
            }
        }
    }

    /// <summary>
    /// フェード処理を行うSpriteを追加
    /// </summary>
    /// <param name="spriteRenderer">フェード対象のスプライト</param>
    /// <param name="baseAlpha">元Alpha値</param>
    /// <param name="targetAlpha">目標Alpha値</param>
    /// <param name="maxFrame">フェードを何フレームかけて行うか</param>
    public void AddFadeSprite(SpriteRenderer spriteRenderer,float baseAlpha,float targetAlpha,int maxFrame)
    {
        fadeSprites.Add(new FadeSprite());
        int index = fadeSprites.Count - 1;

        fadeSprites[index].renderer = spriteRenderer;
        fadeSprites[index].baseAlpha = baseAlpha;
        fadeSprites[index].targetAlpha = targetAlpha;
        fadeSprites[index].maxFrame = maxFrame;
        fadeSprites[index].frame = 0;
    }

    /// <summary>
    /// フェード処理を行うImageを追加
    /// </summary>
    /// <param name="image">フェード対象の画像</param>
    /// <param name="baseAlpha">元Alpha値</param>
    /// <param name="targetAlpha">目標Alpha値</param>
    /// <param name="maxFrame">フェードを何フレームかけて行うか</param>
    public void AddFadeImage(Image image, float baseAlpha, float targetAlpha, int maxFrame)
    {
        fadeImages.Add(new FadeImage());
        int index = fadeImages.Count - 1;

        fadeImages[index].image = image;
        fadeImages[index].baseAlpha = baseAlpha;
        fadeImages[index].targetAlpha = targetAlpha;
        fadeImages[index].maxFrame = maxFrame;
        fadeImages[index].frame = 0;
    }

    /// <summary>
    /// フェード処理を行うTransformを追加
    /// </summary>
    /// <param name="transform">フェード対象のTransform</param>
    /// <param name="basePosition">元座標</param>
    /// <param name="targetPosition">目標座標</param>
    /// <param name="baseScale">元スケール</param>
    /// <param name="targetScale">目標スケール</param>
    /// <param name="maxFrame">フェードを何フレームかけて行うか</param>
    public void AddFadeTransform(Transform transform,Vector3 basePosition,Vector3 targetPosition,Vector3 baseScale,Vector3 targetScale,int maxFrame)
    {
        fadeTransforms.Add(new FadeTransform());
        int index = fadeTransforms.Count - 1;

        fadeTransforms[index].transform = transform;
        fadeTransforms[index].basePosition = basePosition;
        fadeTransforms[index].targetPosition = targetPosition;
        fadeTransforms[index].baseScale = baseScale;
        fadeTransforms[index].targetScale = targetScale;
        fadeTransforms[index].maxFrame = maxFrame;
        fadeTransforms[index].frame = 0;
    }

    /// <summary>
    /// フェード処理を行うRectTransformを追加
    /// </summary>
    /// <param name="rect">フェード対象のRectTransform</param>
    /// <param name="basePosition">元座標</param>
    /// <param name="targetPosition">目標座標</param>
    /// <param name="baseScale">元スケール</param>
    /// <param name="targetScale">目標スケール</param>
    /// <param name="maxFrame">フェードを何フレームかけて行うか</param>
    public void AddFadeRectTransform(RectTransform rect, Vector3 basePosition, Vector3 targetPosition, Vector3 baseScale, Vector3 targetScale, int maxFrame)
    {
        fadeRects.Add(new FadeRect());
        int index = fadeRects.Count - 1;

        fadeRects[index].rect = rect;
        fadeRects[index].basePosition = basePosition;
        fadeRects[index].targetPosition = targetPosition;
        fadeRects[index].baseScale = baseScale;
        fadeRects[index].targetScale = targetScale;
        fadeRects[index].maxFrame = maxFrame;
        fadeRects[index].frame = 0;
    }
}

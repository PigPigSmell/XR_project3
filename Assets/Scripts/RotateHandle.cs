using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateHandle : MonoBehaviour
{
    public GameObject lightsObj;
    public float timeToAwakePrincess = 10f;
    public float initIntensity = 0.4f;
    public float finalIntensity = 4f;
    public GameObject princessSleep;
    public GameObject princessStand;
    public int winScore = 15;
    public RawImage winImage;
    public RawImage failImage;
    public float playMusicAfterJudgePrincessFate = 5f;

    [HideInInspector]
    public static bool isBackToInitIntensity = false;
    public static bool isWin;
    public static bool isPlayingMusic = false;
    public static int score;

    private int i;
    private float increaseIntensityPerFixedUpdate;
    private float decreaseIntensityPerFixedUpdate = 0.2f;
    private float currIntensity;
    private bool isMaxIntensity = false;
    private string scoreJsonPath = "./Assets/StreamingAssets/GameResult.json";
    private List<AudioSource> musicList;

    private void Awake()
    {
        currIntensity = initIntensity;
        increaseIntensityPerFixedUpdate = (finalIntensity - initIntensity) / timeToAwakePrincess * Time.fixedDeltaTime;
        SetLightsIntensity(initIntensity);
        princessStand.SetActive(false);
        score = GetScore();
        isWin = (score >= winScore);
        winImage.enabled = false;
        failImage.enabled = false;
        musicList = GetMusicList();
    }

    private void FixedUpdate()
    {
        if (HoldHandle.isRotate)
        {
            if (!(isPlayingMusic || isBackToInitIntensity))
            {
                StartCoroutine(PlayMusic(true));
            }

            if (!isMaxIntensity)
            {
                currIntensity += increaseIntensityPerFixedUpdate;
                SetLightsIntensity(currIntensity);
                if (currIntensity > finalIntensity)
                {
                    isMaxIntensity = true;
                    JudgePrincessFate();
                    StartCoroutine(BackToInitIntensity());
                }
            }
        }

        /*if (!isPlayingMusic && isBackToInitIntensity)
        {
            StartCoroutine(PlayMusic(false));
        }*/

    }

    private void SetLightsIntensity(float intensity)
    {
        Transform lightObj;
        Light light;
        for (i = 0; i < lightsObj.transform.childCount; i++)
        {
            lightObj = lightsObj.transform.GetChild(i);
            light = lightObj.GetComponent<Light>();
            light.intensity = intensity;
        }
    }


    private IEnumerator BackToInitIntensity()
    {
        while (currIntensity > initIntensity)
        {
            currIntensity -= decreaseIntensityPerFixedUpdate;
            SetLightsIntensity(currIntensity);
            yield return new WaitForSeconds(0.1f);
        }

        SetLightsIntensity(initIntensity);
        isBackToInitIntensity = true;
    }

    private int GetScore()
    {
        return ReadJSON.Read(scoreJsonPath).gameResult;
    }

    private void JudgePrincessFate()
    {
        if (isWin)
        {
            princessSleep.SetActive(false);
            princessStand.SetActive(true);
            winImage.enabled = true;
        }
        else
        {
            failImage.enabled = true;
        }
    }


    private List<AudioSource> GetMusicList()
    {
        if (isWin)
        {
            return GameObject.Find("BeautifulMusic").GetComponent<MusicList>().musicList;
        }
        else
        {
            return GameObject.Find("BadMusic").GetComponent<MusicList>().musicList;
        }
        
    }


    private IEnumerator PlayMusic(bool isFromRotateHandle)
    {
        int now = 0;

        isPlayingMusic = true;
        while (now < musicList.Count)
        {
            if (isBackToInitIntensity && isFromRotateHandle)
            {
                break;
            }
            
            if (HoldHandle.isRotate || !isFromRotateHandle)
            {
                musicList[now].Play();
                now++;
            }
            
            yield return new WaitForSeconds(0.5f);

        }

        yield return new WaitForSeconds(playMusicAfterJudgePrincessFate);
        isPlayingMusic = false;
    }

}

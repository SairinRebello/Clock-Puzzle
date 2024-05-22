using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    [SerializeField] private List<Transform> clockNeedles;
    [SerializeField] private List<TargetVariables> targets;
    [SerializeField] private Transform targetIndicator;
    [SerializeField] private GameObject gameWinPanel;

    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip rotationSound;
    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private AudioClip winSound;

    private int[] gameWinNumbers = new int[3];
    private int[] currentNumbers = new int[3];
    private int currentTarget;

    private const float rotationDegree = 30;

    private void Awake()
    {
        SetGameWinNumber(currentTarget);
    }

    public void RotateNeedle(int needleNo)
    {
        if (Time.timeScale == 0) return;
        _audio.clip = rotationSound;
        _audio.Play();
        Vector3 rotation = clockNeedles[needleNo].localEulerAngles;
        float rotationAngle = rotation.z - rotationDegree;   
        if (rotationAngle < 0) rotationAngle = 330f;
        float currentNo = rotationAngle / 30;
        currentNumbers[needleNo] = currentNo == 0 ? 12 : 12 - (int)currentNo;
        clockNeedles[needleNo].rotation = Quaternion.Euler(0, 0, rotationAngle);
        if (isWinOrNot())
        {
            Time.timeScale = 0;
            StartCoroutine(Win(1));
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }    

    private void SetGameWinNumber(int target)
    {
        for (int i = 0; i < gameWinNumbers.Length; i++)
        {
            gameWinNumbers[i] = int.Parse(targets[target].targetTexts[i].text);
        }
    }

    private bool isWinOrNot()
    {
        List<int> matchingNumbers = new List<int>();
        List<int> alreadyMathedNumebr = new List<int>();
        for (int i = 0; i < gameWinNumbers.Length; i++)
        {
            for (int j = 0; j < currentNumbers.Length; j++)
            {
                if (gameWinNumbers[i] == currentNumbers[j] && !matchingNumbers.Contains(j) && !alreadyMathedNumebr.Contains(i))
                {
                    matchingNumbers.Add(j);
                    alreadyMathedNumebr.Add(i);
                }
            }
        }
        return matchingNumbers.Count > 2 ? true : false;
    }


    private IEnumerator Win(float time)
    {
        _audio.clip = levelUpSound;
        _audio.Play();
        yield return new WaitForSecondsRealtime(time);
        NextTarget();
    }
    private void NextTarget()
    {
        currentTarget++;
        if (currentTarget > 2)
        {
            _audio.clip = winSound;
            _audio.Play();
            gameWinPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            SetGameWinNumber(currentTarget);
            targets[currentTarget].hiders.SetActive(false);
            targetIndicator.SetParent(targets[currentTarget].targetIndicatorTransform);
            targetIndicator.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }
    }

}

[Serializable] 
public class TargetVariables
{
    public GameObject hiders;
    public Transform targetIndicatorTransform;
    public List<Text> targetTexts;
}

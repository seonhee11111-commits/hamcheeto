using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("UI Settings")]
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI targetGoldText;

    [Header("Gold Settings")]
    public int currentGold = 0;
    public int targetGold = 10000;

    [Header("Time Settings")]
    public float gameTime = 120f;
    public bool isGameOver = false;

    [Header("Combo UI Settings")]
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private float floatSpeed = 50f;
    [SerializeField] private float floatDuration = 1f;
    private Vector2 comboTextOriginalPos;
    private RectTransform comboRect;

    [Header("HP Settings")]
    [SerializeField] private int playerHP = 3;
    [SerializeField] private GameObject HP1;
    [SerializeField] private GameObject HP2;
    [SerializeField] private GameObject HP3;

    public bool recentOrderSuccess = false;


    private void Awake()
    {
        if (instance == null) { instance = this; }
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateGoldUI();
        UpdatetargetGoldUI();
        playerHP = 3;

        if (comboText != null)
        {
            comboRect = comboText.GetComponent<RectTransform>();
            comboTextOriginalPos = comboRect.anchoredPosition;
            comboText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isGameOver) return;
        gameTime -= Time.deltaTime;
        if (gameTime<=0)
        {
            gameTime = 0;
            GameOver();
        }

        UpdateTimeUI();
    }

    public void AddGold(int amount)
    {
        if (isGameOver) return;

        currentGold += amount;
        //Debug.Log($"{amount}원 획득");
        //Debug.Log($"현재 골드 {currentGold}");
        UpdateGoldUI();
        CheckClearCondition();
    }

    public void HPDown()
    {
        playerHP--;

        if (playerHP == 2)
        { if (HP3 != null) HP3.SetActive(false); }
        if (playerHP == 1)
        { if (HP2 != null) HP2.SetActive(false); }
        if (playerHP == 0)
        {
            if (HP1 != null) HP1.SetActive(false);
            gameTime = 0;
            GameOver(); 
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = $"{currentGold}";
        }
    }

    private void UpdatetargetGoldUI()
    {
        if (targetGoldText != null)
        {
            targetGoldText.text = $"목표: {targetGold}원!";
        }
    }

    private void UpdateTimeUI()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void CheckClearCondition()
    {
        if (currentGold >= targetGold)
        {
            Debug.Log($"CLEAR ::: {targetGold}원 달성!");
            SoundManager.Instance.PlaySFX(SFXType.Clear);
            SceneManager.LoadScene("Clear");
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER");
        SoundManager.Instance.PlaySFX(SFXType.GameOver);
        SceneManager.LoadScene("GameOver");
    }

    public void ShowComboText()
    {
        if (comboText == null) return;

        StopCoroutine("FloatingComboRoutine");
        StartCoroutine("FloatingComboRoutine");
    }

    private System.Collections.IEnumerator FloatingComboRoutine()
    {
        comboText.gameObject.SetActive(true);
        comboRect.anchoredPosition = comboTextOriginalPos;

        Color textColor = comboText.color;
        textColor.a = 1f;
        comboText.color = textColor;
        float elapsed = 0f;

        while (elapsed < floatDuration)
        {
            elapsed += Time.deltaTime;
            comboRect.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;
            textColor.a = Mathf.Lerp(1f, 0f, elapsed / floatDuration);
            comboText.color = textColor;

            yield return null;
        }

        comboText.gameObject.SetActive(false);
        comboRect.anchoredPosition = comboTextOriginalPos;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager m_Instance { get; private set; }

    [SerializeField] private Text m_ScoreText;
    [SerializeField] private int m_MaxScore = 1000;
    private int m_Score;

    private void Start()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (m_Instance != null)
        {
            Destroy(this);
        }

        AddPoints(0);
    }

    public void AddPoints(int amount)
    {
        m_Score = Mathf.Clamp(m_Score + amount, 0, m_MaxScore);
        m_ScoreText.text = m_Score.ToString();
    }
}
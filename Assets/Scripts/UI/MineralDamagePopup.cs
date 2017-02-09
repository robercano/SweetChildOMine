using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineralDamagePopup : MonoBehaviour {

    public float Speed;
    public float Seconds;
    public Color Color;

    public int UnitsExtracted
    {
        get
        {
            return int.Parse(m_unitsExtractedText.text);
        }
        set
        {
            m_unitsExtractedText.text = value.ToString();
        }
    }

    private Text m_unitsExtractedText;
    private Rigidbody2D m_rigidBody;
    private float m_startTime;

    // Use this for initialization
    void Awake()
    {
        m_unitsExtractedText = gameObject.GetComponentInChildren<Text>();
        
        m_rigidBody = GetComponent<Rigidbody2D>();

        m_startTime = Time.time;
    }

    void Start()
    {
        m_unitsExtractedText.color = Color;
        m_rigidBody.velocity = new Vector2(0.0f, Speed);
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.time - m_startTime;

        if (elapsedTime > Seconds)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        m_unitsExtractedText.color = new Color(Color.r, Color.g, Color.g, 1.0f - elapsedTime / Seconds);
    }
}

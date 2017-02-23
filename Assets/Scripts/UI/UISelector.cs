using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UISelector : MonoBehaviour {

    private Transform[] m_corners;
    private Bounds m_bounds;

    // Use this for initialization
    void Awake()
    {
        m_corners = GetComponentsInChildren<Transform>();
        Assert.IsTrue(m_corners.Length == 5);
    }

    public void SetBounds(Bounds bounds)
    {
        m_bounds = bounds;

        m_corners[1].position = new Vector2(m_bounds.min.x, m_bounds.max.y);
        m_corners[2].position = new Vector2(m_bounds.max.x, m_bounds.max.y);
        m_corners[3].position = new Vector2(m_bounds.min.x, m_bounds.min.y);
        m_corners[4].position = new Vector2(m_bounds.max.x, m_bounds.min.y);
    }
}

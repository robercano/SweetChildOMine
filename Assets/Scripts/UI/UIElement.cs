using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MonoBehaviour {

	private GameObject m_followGameObject;
	private float m_yCoord;
    private RectTransform m_rectTransform;
    private GameObject m_mainUI;

    virtual protected void Awake () {
		m_followGameObject = null;
		m_yCoord = 0.0f;

		m_mainUI = GameObject.Find("MainUI");

        m_rectTransform = GetComponent<RectTransform>();
        m_rectTransform.pivot = new Vector2(0.5f, 0.0f);
        m_rectTransform.localScale = Vector3.one;

        SetParent(null);
	}

	virtual protected void Update () {
		if (m_followGameObject != null) {
			Vector2 objectPos = new Vector2 (m_followGameObject.transform.position.x, m_followGameObject.transform.position.y + m_yCoord);
			SetWorldPosition (objectPos);
		}
	}

	public void SetWorldPosition(Vector2 position)
	{
		transform.position = RectTransformUtility.WorldToScreenPoint (Camera.main, position);
	}

    public void SetPosition(Vector2 position)
    {
        m_rectTransform.anchoredPosition = position;
    }

    public void SetParent(Transform parent)
    {
        if (parent == null)
        {
            transform.SetParent(m_mainUI.transform, false);
        }
        else
        {
            transform.SetParent(parent);
        }
    }

    public void FollowGameObject(GameObject obj)
	{
		m_followGameObject = obj;

		m_yCoord = obj.GetComponent<SpriteRenderer>().bounds.size.y + UIGlobals.PegDistanceToObject;
	}
}

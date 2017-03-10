using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldPanel : MonoBehaviour {

	private GameObject m_followGameObject;
	private float m_yCoord;

	virtual protected void Awake () {
		m_followGameObject = null;
		m_yCoord = 0.0f;

		GameObject mainUI = GameObject.Find("MainUI");
		transform.SetParent(mainUI.transform);

		RectTransform rectTransform = GetComponent<RectTransform>();
		rectTransform.pivot = new Vector2(0.5f, 0.0f);
		rectTransform.localScale = Vector3.one;
	}

	virtual protected void Update () {
		if (m_followGameObject != null) {
			Vector2 objectPos = new Vector2 (m_followGameObject.transform.position.x, m_followGameObject.transform.position.y + m_yCoord);
			SetPosition (objectPos);
		}
	}

	public void SetPosition(Vector2 position)
	{
		transform.position = RectTransformUtility.WorldToScreenPoint (Camera.main, position);
	}

	public void FollowGameObject(GameObject obj)
	{
		m_followGameObject = obj;

		m_yCoord = obj.GetComponent<SpriteRenderer>().bounds.size.y + UIGlobals.PegDistanceToObject;
	}
}

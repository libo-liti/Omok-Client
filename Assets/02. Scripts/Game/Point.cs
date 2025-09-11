using UnityEngine;
using UnityEngine.EventSystems;

public class Point : MonoBehaviour
{
	[SerializeField] private SpriteRenderer markerSpriteRenderer;
	
	public delegate void OnPointClicked(int index);
	private OnPointClicked _onPointClicked;
	
	// 마커 타입
	public enum MarkerType { None, Black, White }
	
	// Point Index
	private int _pointIndex;
	
	// 1. 초기화
	public void InitMarker(int pointIndex, OnPointClicked onPointClicked)
	{
		_pointIndex = pointIndex;
		SetMarker(MarkerType.None);
		_onPointClicked = onPointClicked;
	}
	
	// 2. 마커 설정
	public void SetMarker(MarkerType markerType)
	{
		Color markerColor = markerSpriteRenderer.color;

		switch (markerType)
		{
			case MarkerType.None:
				markerColor = Color.black;
				markerColor.a = 0;
				break;
			case MarkerType.Black:
				markerColor = Color.black;
				markerColor.a = 1;
				break;
			case MarkerType.White:
				markerColor = Color.white;
				markerColor.a = 1;
				break;
		}

		markerSpriteRenderer.color = markerColor;

	}
	
	// 3. 블럭 터치 처리
	private void OnMouseUpAsButton()
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
        
		Debug.Log("Selected Point: " + _pointIndex);
        
		_onPointClicked?.Invoke(_pointIndex);
	}
}

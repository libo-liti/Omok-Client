using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Point : MonoBehaviour
{
	[SerializeField] private SpriteRenderer markerSpriteRenderer;
	[SerializeField] private SpriteRenderer borderSpriteRenderer;
	
	[SerializeField] private GameObject blackStone; // 흑돌
	[SerializeField] private GameObject whiteStone; // 백돌
	[SerializeField] private GameObject blackStoneTransparent; // 반투명 흑돌
	[SerializeField] private GameObject whiteStoneTransparent; // 반투명 백돌

	[SerializeField] private TMP_Text numberText; // 기보 숫자 텍스트
	
	public delegate void OnPointClicked(int index);
	private OnPointClicked _onPointClicked;
	
	public delegate void OnPointEnter(int index);
	private OnPointEnter _onPointEnter;
	
	public delegate void OnPointExit(int index);
	private OnPointExit _onPointExit;
	
	// 마커 타입
	public enum MarkerType { None, Black, White }
	
	// Point Index
	private int _pointIndex;
	
	// 초기화
	public void InitMarker(int pointIndex, OnPointClicked onPointClicked, OnPointEnter onPointEnter, OnPointExit onPointExit)
	{
		_pointIndex = pointIndex;
		SetMarker(MarkerType.None);
		ShowNumber(Constants.PlayerType.None, -1);
		_onPointClicked = onPointClicked;
		_onPointEnter = onPointEnter;
		_onPointExit = onPointExit;
	}
	
	// 마커 설정
	public void SetMarker(MarkerType markerType)
	{
		Color markerColor = Color.clear;
		Color borderColor = Color.clear;

		switch (markerType)
		{
			case MarkerType.None:
				markerColor = new Color(0, 0, 0, 0);
				borderColor = new Color(0, 0, 0, 0);
				break;
			case MarkerType.Black:
				markerColor = Color.black;
				borderColor = Color.yellow;
				
				blackStone.SetActive(true);
				break;
			case MarkerType.White:
				markerColor = Color.white;
				borderColor = Color.red;
				
				whiteStone.SetActive(true);

				break;
		}

		// markerSpriteRenderer.color = markerColor;
		
		if (borderSpriteRenderer != null)
		{
			borderSpriteRenderer.color = borderColor;
			borderSpriteRenderer.enabled = (markerType != MarkerType.None);
		
			// 원본은 0.9 사이즈
			// markerSpriteRenderer.transform.localScale = Vector3.one * 0.9f;
		
			// 뒤쪽 레이어 1.1 사이즈
			borderSpriteRenderer.transform.localScale = Vector3.one * 1.1f;
		}
	}
	
	public void HideBorder()
	{
		if (borderSpriteRenderer != null)
		{
			borderSpriteRenderer.enabled = false;
		}
	}
	
	// 미리보기용 반투명 돌 활성화 / 비활성화 
	public void Preview(MarkerType markerType, bool show)
	{
		switch (markerType)
		{
			case MarkerType.None:
				break;
			case MarkerType.Black:
				blackStoneTransparent.SetActive(show);
				break;
			case MarkerType.White:
				whiteStoneTransparent.SetActive(show);
				break;
		}
	}

	// 기보 숫자 보여주기
	public void ShowNumber(Constants.PlayerType playerType, int number)
	{
		switch (playerType)
		{
			case Constants.PlayerType.None:
				numberText.text = string.Empty;
				break;
			case Constants.PlayerType.PlayerA:
				numberText.color = Color.white;
				numberText.text = number.ToString();
				break;
			case Constants.PlayerType.PlayerB:
				numberText.color = Color.black;
				numberText.text = number.ToString();
				break;
		}
	}

	
	// 포인트 터치 처리
	private void OnMouseUpAsButton()
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
        
        
		_onPointClicked?.Invoke(_pointIndex);
	}
	
	// 포인트에 마우스가 들어왔을 때 처리
	private void OnMouseEnter()
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		
		_onPointEnter?.Invoke(_pointIndex);
	}
	
	// 포인트에서 마우스가 나갔을 때 처리
	private void OnMouseExit()
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		
		_onPointExit?.Invoke(_pointIndex);
	}
}

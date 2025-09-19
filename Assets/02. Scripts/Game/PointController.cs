using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PointController : MonoBehaviour
{
	private Point[] _points;
	private Point _lastPoint; //마지막 돌 
	
	public delegate void OnPointClicked(int row, int col);
	public OnPointClicked OnPointClickedDelegate;
	public delegate void OnPointEnter(int row, int col);
	public OnPointEnter OnPointEnterDelegate;
	public delegate void OnPointExit(int row, int col);
	public OnPointExit OnPointExitDelegate;
	
	// 1. 모든 Point를 초기화
	public void InitPoints()
	{
		_points = GetComponentsInChildren<Point>();
		
		for (int i = 0; i < _points.Length; i++)
		{
			_points[i].InitMarker(i, pointIndex =>
			{
				// 특정 Point가 클릭 된 상태에 대한 처리
				var row = pointIndex / Constants.BoardSize;
				var col = pointIndex % Constants.BoardSize;
				OnPointClickedDelegate?.Invoke(row, col);
			}, (pointIndex) =>
			{
				var row = pointIndex / Constants.BoardSize;
				var col = pointIndex % Constants.BoardSize;
				OnPointEnterDelegate?.Invoke(row, col);
			}, (pointIndex) =>
			{
				var row = pointIndex / Constants.BoardSize;
				var col = pointIndex % Constants.BoardSize;
				OnPointExitDelegate?.Invoke(row, col);
			});
		}
	}
	
	// 2. 특정 Point 마커 표시
	public void PlaceMaker(Point.MarkerType markerType, int row, int col)
	{
		// row, col >> index 변환
		var pointIndex = row * Constants.BoardSize + col;
		if (_lastPoint != null)
			_lastPoint.HideBorder();

		// 새돌 세팅
		_points[pointIndex].SetMarker(markerType);

		// 최신돌 저장
		_lastPoint = _points[pointIndex];
		
		//효과음 재생
		AudioManager.Instance.PlayStoneSE();
	}
	
	// 3. 바둑돌 놓을 위치 미리보기
	public void Preview(Point.MarkerType markerType, int row, int col, bool show)
	{
		// row, col >> index 변환
		var pointIndex = row * Constants.BoardSize + col;
	
		// 반투명 돌 보여주기
		_points[pointIndex].Preview(markerType, show);
	}
	
	// 4. 기보 보여주기
	public void ShowHistoryNumber(int row, int col, Constants.PlayerType playerType, int number)
	{
		// row, col >> index 변환
		var pointIndex = row * Constants.BoardSize + col;
	
		// 반투명 돌 보여주기
		_points[pointIndex].ShowNumber(playerType, number);
	}
	
	// // 투명도 변화
	// private Color ChangeAlpha(SpriteRenderer sprite, float a)
	// {
	// 	var color = sprite.color;
	// 	color.a = a;
	// 	sprite.color = color;
	// 	return color;
	// }
}
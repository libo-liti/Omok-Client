using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PointController : MonoBehaviour
{
	private Point[] _points;
	
	public delegate void OnPointClicked(int row, int col);
	public OnPointClicked OnPointClickedDelegate;
	
	// 1. 모든 Point를 초기화
	public void InitPoints()
	{
		_points = GetComponentsInChildren<Point>();
		
		for (int i = 0; i < _points.Length; i++)
		{
			_points[i].InitMarker(i, pointIndex =>
			{
				// 특정 Point가 클릭 된 상태에 대한 처리
				var row =  pointIndex / Constants.BoardSize;
				var col = pointIndex % Constants.BoardSize;
				OnPointClickedDelegate?.Invoke(row, col);
			});
		}
	}
	
	// 2. 특정 Point 마커 표시
	public void PlaceMaker(Point.MarkerType markerType, int row, int col)
	{
		// row, col >> index 변환
		var pointIndex = row * Constants.BoardSize + col;
		_points[pointIndex].SetMarker(markerType);
	}
	

	// 바둑돌 놓을 위치 미리보기
	// public void Preview(Vector2 pos)
	// {
	// 	// 바둑돌 생성
	// 	if (GameManager.Instance.currentTurn == GameManager.Turn.PlayerA)
	// 		Player.currentStone = Instantiate(blackStone, pos, Quaternion.identity);
	// 	else
	// 		Player.currentStone = Instantiate(whiteStone, pos, Quaternion.identity);
	//
	// 	// 바둑돌 투명하게
	// 	stoneSprite = Player.currentStone.GetComponent<SpriteRenderer>();
	// 	stoneSprite.color = ChangeAlpha(stoneSprite, 0.6f);
	// }
	
	// // 투명도 변화
	// private Color ChangeAlpha(SpriteRenderer sprite, float a)
	// {
	// 	var color = sprite.color;
	// 	color.a = a;
	// 	sprite.color = color;
	// 	return color;
	// }
}
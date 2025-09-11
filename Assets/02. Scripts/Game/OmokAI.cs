using UnityEngine;

public static class OmokAI
{
	// 우하, 우, 우상, 상
	private static int[] _dx = new int[4] { 1, 1, 1, 0 };
	private static int[] _dy = new int[4] { -1, 0, 1, 1 };

	public static (int row, int col)? GetBestMove(Constants.PlayerType[,] board)
	{
		(int row, int col) bestMove = new (-1, -1);
		int maxScore = int.MinValue;

		// 탐색 범위를 돌 주변으로 제한하여 효율성 향상
		// 여기서는 전체 보드를 탐색하지만, 더 최적화된 방법이 있음
		for (int i = 0; i < board.GetLength(0); i++)
		{
			for (int j = 0; j < board.GetLength(1); j++)
			{
				if (board[i, j] == Constants.PlayerType.None)
				{
					board[i, j] = Constants.PlayerType.PlayerB; // AI의 돌 (2)을 임시로 놓아봄

					// Minimax 알고리즘 (깊이 3으로 설정)
					int score = Minimax(board, 3, false, int.MinValue, int.MaxValue, i, j);

					board[i, j] = Constants.PlayerType.None; // 돌을 다시 제거

					if (score > maxScore)
					{
						maxScore = score;
						bestMove = (i, j);
					}
				}
			}
		}

		return bestMove;
	}

	private static int Minimax(Constants.PlayerType[,] board, int depth, bool isMaximizingPlayer, int alpha, int beta, int y, int x)
	{
		// 깊이 제한에 도달했거나 게임이 끝났을 때
		// CheckWin은 방금 돌을 놓은 플레이어(isMaximizingPlayer ? 1 : 2)의 승리 여부 판단
		if (CheckWin(board, isMaximizingPlayer ? Constants.PlayerType.PlayerB : Constants.PlayerType.PlayerA, y, x))
		{
			// 게임 승리: 매우 높은 점수 반환
			return isMaximizingPlayer ? int.MaxValue : int.MinValue;
		}

		// 무승부 판단
		if (IsBoardFull(board))
		{
			return 0;
		}

		// 깊이 제한에 도달했을 경우 보드 평가
		if (depth == 0)
		{
			return EvaluateBoard(board, isMaximizingPlayer ? Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB, y, x);
		}

		if (isMaximizingPlayer) // AI 턴
		{
			int maxEval = int.MinValue;
			for (int i = 0; i < board.GetLength(0); i++)
			{
				for (int j = 0; j < board.GetLength(1); j++)
				{
					if (board[i, j] == Constants.PlayerType.None)
					{
						board[i, j] = Constants.PlayerType.PlayerB;
						int evaluation = Minimax(board, depth - 1, false, alpha, beta, i, j);
						board[i, j] = Constants.PlayerType.None;
						maxEval = Mathf.Max(maxEval, evaluation);
						alpha = Mathf.Max(alpha, evaluation);
						if (beta <= alpha)
						{
							break;
						}
					}
				}

				if (beta <= alpha) break;
			}

			return maxEval;
		}
		else // 상대방 턴
		{
			int minEval = int.MaxValue;
			for (int i = 0; i < board.GetLength(0); i++)
			{
				for (int j = 0; j < board.GetLength(1); j++)
				{
					if (board[i, j] == 0)
					{
						board[i, j] = Constants.PlayerType.PlayerA;
						int evaluation = Minimax(board, depth - 1, true, alpha, beta, i, j);
						board[i, j] = Constants.PlayerType.None;
						minEval = Mathf.Min(minEval, evaluation);
						beta = Mathf.Min(beta, evaluation);
						if (beta <= alpha)
						{
							break;
						}
					}
				}

				if (beta <= alpha) break;
			}

			return minEval;
		}
	}

	private static int EvaluateBoard(Constants.PlayerType[,] board, Constants.PlayerType player, int y, int x)
	{
		int score = 0;
		Constants.PlayerType opponent = player == Constants.PlayerType.PlayerA ? Constants.PlayerType.PlayerB : Constants.PlayerType.PlayerA;

		// AI의 공격 점수 계산
		score += CalculatePatternScore(board, player, y, x);

		// 상대방의 공격 점수를 계산하여 방어 점수에 반영
		// (이 점수에 높은 가중치를 부여하여 방어의 중요성을 강조)
		score -= CalculatePatternScore(board, opponent, y, x) * 10;

		return score;
	}

	private static int CalculatePatternScore(Constants.PlayerType[,] board, Constants.PlayerType player, int y, int x)
	{
		int score = 0;
		for (int i = 0; i < 4; i++)
		{
			int count = 1;
			int open = 0;

			// 순방향
			for (int j = 1; j <= 4; j++)
			{
				int nx = x + _dx[i] * j;
				int ny = y + _dy[i] * j;

				if (nx < 0 || board.GetLength(0) <= nx || ny < 0 || board.GetLength(0) <= ny)
					break;
				if (board[ny, nx] == Constants.PlayerType.None)
				{
					open++;
					break;
				}

				if (board[ny, nx] != player)
					break;
				count++;
			}

			// 역방향
			for (int j = 1; j <= 4; j++)
			{
				int nx = x - _dx[i] * j;
				int ny = y - _dy[i] * j;

				if (nx < 0 || board.GetLength(0) <= nx || ny < 0 || board.GetLength(1) <= ny)
					break;
				if (board[ny, nx] == 0)
				{
					open++;
					break;
				}

				if (board[ny, nx] != player)
					break;
				count++;
			}

			// Mathf.Max 반환값을 score에 할당해야 함
			if (count == 5) score = Mathf.Max(score, 100000);
			if (count == 4 && open == 2) score = Mathf.Max(score, 50000);
			if (count == 3 && open == 2) score = Mathf.Max(score, 30000);
			if (count == 4 && open == 1) score = Mathf.Max(score, 5000);
			if (count == 2 && open == 2) score = Mathf.Max(score, 3000);
			if (count == 3 && open == 1) score = Mathf.Max(score, 2000);
		}

		return score;
	}

	public static bool CheckWin(Constants.PlayerType[,] board, Constants.PlayerType player, int y, int x)
	{
		for (int i = 0; i < 4; i++)
		{
			int count = board[y, x] == player ? 1 : 0;
			// 순방향
			for (int j = 1; j <= 5; j++)
			{
				int nx = x + _dx[i] * j;
				int ny = y + _dy[i] * j;

				if (nx < 0 || board.GetLength(0) <= nx || ny < 0 || board.GetLength(1) <= ny)
					break;
				if (board[ny, nx] != player)
					break;
				count++;
			}

			// 역방향
			for (int j = 1; j <= 5; j++)
			{
				int nx = x - _dx[i] * j;
				int ny = y - _dy[i] * j;

				if (nx < 0 || board.GetLength(0) <= nx || ny < 0 || board.GetLength(1) <= ny)
					break;
				if (board[ny, nx] != player)
					break;
				count++;
			}

			if (count >= 5) // 6목이 가능할 경우를 대비하여 >=로 변경
				return true;
		}

		return false;
	}

	public static bool IsBoardFull(Constants.PlayerType[,] board)
	{
		for (int i = 0; i < board.GetLength(0); i++)
		{
			for (int j = 0; j < board.GetLength(1); j++)
			{
				if (board[i, j] == Constants.PlayerType.None)
				{
					return false;
				}
			}
		}

		return true;
	}
}
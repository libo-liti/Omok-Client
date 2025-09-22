using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class OmokAI
{
	// 우하, 우, 우상, 상
	private static int[] _dx = { 1, 1, 1, 0 };
	private static int[] _dy = { -1, 0, 1, 1 };

	public static (int row, int col)? GetBestMove(Constants.PlayerType[,] board, out int bestScore)
	{
		int searchDepth = 3; // 몇 수 앞까지 내다보고 판단할 지 결정
		List<(int, int)> moves = GenerateCandidateMoves(board, 2);
		
		// 계산할 경우의 수가 많아지면 연산이 많아지므로 조정한다.
		if (moves.Count > 60) 
			moves = GenerateCandidateMoves(board, 1);
		if (moves.Count > 60)
			searchDepth = 2;
		
		bestScore = int.MinValue;
		(int row, int col) bestMove = (-1, -1);
		foreach (var m in moves)
		{
			board[m.Item1, m.Item2] = Constants.PlayerType.PlayerB; // AI의 돌을 임시로 놓아본다.
			int score = Minimax(board, searchDepth - 1, int.MinValue, int.MaxValue, false);
			board[m.Item1, m.Item2] = Constants.PlayerType.None; // 계산 후 AI의 돌을 다시 지운다.
			if (score > bestScore)
			{
				bestScore = score;
				bestMove = m;
			}
		}
		
		if (bestMove != (-1, -1))
		{
			return bestMove;
		}

		return null;
	}

	/// <summary>
	/// 알파-베타 가지치기를 활용한 미니맥스 알고리즘
	/// </summary>
	static int Minimax(Constants.PlayerType[,] board, int depth, int alpha, int beta, bool maximizingPlayer)
	{
		// 무승부 판단
		if (IsBoardFull(board))
		{
			return 0;
		}

		// 탐색 깊이 제한에 도달했을 때는 현재 점수 반환
		if (depth == 0)
		{
			int eval = EvaluateBoard(board);
			return eval;
		}

		List<(int, int)> moves = GenerateCandidateMoves(board, 1);

		if (maximizingPlayer) // AI의 턴
		{
			int value = int.MinValue;
			foreach (var m in moves)
			{
				board[m.Item1, m.Item2] = Constants.PlayerType.PlayerB;
				int child = Minimax(board, depth - 1, alpha, beta, false);
				board[m.Item1, m.Item2] = Constants.PlayerType.None;
				value = Math.Max(value, child);
				alpha = Math.Max(alpha, value);
				if (alpha >= beta) break; // beta cut-off
			}

			return value;
		}
		else // 플레이어의 턴
		{
			int value = int.MaxValue;
			foreach (var m in moves)
			{
				board[m.Item1, m.Item2] = Constants.PlayerType.PlayerA;
				int child = Minimax(board, depth - 1, alpha, beta, true);
				board[m.Item1, m.Item2] = Constants.PlayerType.None;
				value = Math.Min(value, child);
				beta = Math.Min(beta, value);
				if (alpha >= beta) break; // alpha cut-off
			}

			return value;
		}
	}

	/// <summary>
	/// 기존에 놓여있는 돌들에서 거리 2 이내의 빈 자리들을 반환
	/// </summary>
	static List<(int, int)> GenerateCandidateMoves(Constants.PlayerType[,] board, int radiusNeighbour)
	{
		List<(int, int)> result = new List<(int, int)>();
		bool[,] mark = new bool[Constants.BoardSize, Constants.BoardSize];

		int[] dx;
		int[] dy;

		if (radiusNeighbour == 2)
		{
			dx = new int[] { -2, -1, 0, 1, 2 };
			dy = new int[] { -2, -1, 0, 1, 2 };
		}
		else
		{
			dx = new int[] { -1, 0, 1 };
			dy = new int[] { -1, 0, 1 };
		}

		// 놓여있는 돌이 하나도 없을 경우 중앙 자리를 반환
		bool anyStone = false;
		for (int x = 0; x < Constants.BoardSize; x++)
		for (int y = 0; y < Constants.BoardSize; y++)
			if (board[x, y] != Constants.PlayerType.None)
				anyStone = true;

		if (!anyStone)
		{
			result.Add((Constants.BoardSize / 2, Constants.BoardSize / 2));
			return result;
		}

		for (int x = 0; x < Constants.BoardSize; x++)
		{
			for (int y = 0; y < Constants.BoardSize; y++)
			{
				if (board[x, y] != Constants.PlayerType.None)
				{
					foreach (int i in dx)
					foreach (int j in dy)
					{
						int nx = x + i;
						int ny = y + j;
						if (nx >= 0 && ny >= 0 && nx < Constants.BoardSize && ny < Constants.BoardSize)
						{
							if (board[nx, ny] == Constants.PlayerType.None && !mark[nx, ny])
							{
								mark[nx, ny] = true;
								result.Add((nx, ny));
							}
						}
					}
				}
			}
		}

		return result;
	}

	/// <summary>
	/// 형세를 판단하여 점수로 반환
	/// </summary>
	static int EvaluateBoard(Constants.PlayerType[,] board)
	{
		long scoreAI = CalculateScore(board, Constants.PlayerType.PlayerB);
		long scorePL = CalculateScore(board, Constants.PlayerType.PlayerA);

		// 공격/방어 성향 조정
		float defenseFactor = 1.1f;
		long total = scoreAI - (long)(scorePL * defenseFactor);

		// int 범위를 벗어나지 않도록 값 조정
		if (total > int.MaxValue) return int.MaxValue;
		if (total < int.MinValue) return int.MinValue;
		return (int)total;
	}

	/// <summary>
	/// 플레이어 별로 점수 계산
	/// </summary>
	static long CalculateScore(Constants.PlayerType[,] board, Constants.PlayerType player)
	{
		long score = 0;

		for (int x = 0; x < Constants.BoardSize; x++)
		{
			for (int y = 0; y < Constants.BoardSize; y++)
			{
				if (board[x, y] != player) continue;

				for (int d = 0; d < 4; d++)
				{
					int dx = _dx[d], dy = _dy[d];

					// 본인의 돌의 몇개 이어져있는지 숫자를 센다
					int cntPos = CountInDirection(board, x, y, dx, dy, player) - 1;
					int cntNeg = CountInDirection(board, x, y, -dx, -dy, player) - 1;
					int consecutive = 1 + cntPos + cntNeg;

					// 양쪽 끝이 열려있는지 막혀있는지 확인
					int frontX = x + dx * (cntPos + 1);
					int frontY = y + dy * (cntPos + 1);
					bool frontOpen = IsInBoard(frontX, frontY) && board[frontX, frontY] == Constants.PlayerType.None;

					int backX = x - dx * (cntNeg + 1);
					int backY = y - dy * (cntNeg + 1);
					bool backOpen = IsInBoard(backX, backY) && board[backX, backY] == Constants.PlayerType.None;

					int openEnds = (frontOpen ? 1 : 0) + (backOpen ? 1 : 0);

					// 위 정보들을 통해 점수를 계산
					score += PatternScore(consecutive, openEnds, player);
				}
			}
		}

		return score;
	}

	/// <summary>
	/// 한 쪽 방향으로 몇 개의 돌이 연속되어 있는지 확인하여 반환
	/// </summary>
	static int CountInDirection(Constants.PlayerType[,] board, int x, int y, int dx, int dy,
		Constants.PlayerType player)
	{
		int cnt = 0;
		int nx = x, ny = y;
		while (IsInBoard(nx, ny) && board[nx, ny] == player)
		{
			cnt++;
			nx += dx;
			ny += dy;
		}

		return cnt;
	}

	/// <summary>
	/// 전달받은 좌표가 오목판의 범위 안에 있는지 여부
	/// </summary>
	static bool IsInBoard(int x, int y) => x >= 0 && y >= 0 && x < Constants.BoardSize && y < Constants.BoardSize;

	/// <summary>
	/// 패턴에 맞게 점수를 반환
	/// </summary>
	static long PatternScore(int consecutive, int openEnds, Constants.PlayerType player)
	{
		if (player == Constants.PlayerType.PlayerB) // AI 점수 계산할 때에는 5 이상에 대해 높은 점수 부여
		{
			if (consecutive >= 5) return 20000000;
		}
		else if (player == Constants.PlayerType.PlayerA) // 플레이어 점수 계산할 때에는 정확히 5일 때에 높은 점수 부여
		{
			if (consecutive == 5) return 20000000;
		}
		
		if (consecutive == 4)
		{
			if (openEnds == 2) return 120000; // 열린 4
			if (openEnds == 1) return 12000; // 한쪽 막힌 4
		}

		if (consecutive == 3)
		{
			if (openEnds == 2) return 8000; // 열린 3
			if (openEnds == 1) return 800; // 한쪽 막힌 3
		}

		if (consecutive == 2)
		{
			if (openEnds == 2) return 300; // 열린 2
			if (openEnds == 1) return 30; // 한쪽 막힌 2
		}

		if (consecutive == 1)
		{
			if (openEnds == 2) return 10; // 열린 1
			if (openEnds == 1) return 1; // 한쪽 막힌 1
		}

		return 0;
	}

	/// <summary>
	/// 특정 플레이어가 승리했는지 확인
	/// </summary>
	public static bool CheckWin(Constants.PlayerType[,] board, Constants.PlayerType player, int row, int col)
	{
		if (board[row, col] != player)
			return false;

		for (int i = 0; i < 4; i++)
		{
			// 본인의 돌의 몇개 이어져있는지 숫자를 센다
			int dx = _dx[i];
			int dy = _dy[i];
			
			int cntPos = CountInDirection(board, row, col, dx, dy, player) - 1;
			int cntNeg = CountInDirection(board, row, col, -dx, -dy, player) - 1;
			int consecutive = 1 + cntPos + cntNeg;

			if (consecutive >= 5) // 6목이 가능할 경우를 대비하여 >=로 변경
				return true;
		}

		return false;
	}

	/// <summary>
	/// 모든 자리에 돌이 놓여있는지 확인
	/// </summary>
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

	#region 렌주룰
	
	/// <summary>
	/// 오목판에서 흑돌 금수 위치를 확인하여 반환
	/// </summary>
	public static bool[,] IsForbidden(Constants.PlayerType[,] board)
	{
		bool[,] b = new bool[Constants.BoardSize, Constants.BoardSize];
		
		for (int i = 0; i < board.GetLength(0); i++)
		{
			for (int j = 0; j < board.GetLength(1); j++)
			{
				if (board[i, j] != Constants.PlayerType.None)
					continue;
				
				int count_OpenThree = 0;
				int count_Four = 0;
				
				for (int k = 0; k < 4; k++)
				{
					string line = GetLine(board, i, j, _dx[k], _dy[k]);
					if (IsMoreThanFive(line))
					{
						b[i, j] = true;
						break;
					}
					
					if (IsOpenThree(line))	
						count_OpenThree++;
					
					if (IsFour(line))
						count_Four++;
				}

				if (count_OpenThree >= 2 || count_Four >= 2)
					b[i, j] = true;
			}
		}
	
		return b;
	}
	
	/// <summary>
	/// 어떤 위치에서 거리 5만큼 떨어진곳까지의 돌 배치를 파악하여 문자열로 반환
	/// O : 흑돌, X : 백돌, - : 빈 자리
	/// ex ) "-OOO-" : 열린 3,  "XOOOO-" : 막힌 4
	/// </summary>
	static string GetLine(Constants.PlayerType[,] board, int x, int y, int dx, int dy)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("O");
	
		// 전방 (한 방향)
		int nx = x + dx;
		int ny = y + dy;
		for (int i = 1; i < 6; i++)
		{
			if (IsInBoard(nx, ny) == false) break;
			
			if (board[nx, ny] == Constants.PlayerType.PlayerA)
			{
				sb.Append("O");
				nx += dx;
				ny += dy;
			}
			else if (board[nx, ny] == Constants.PlayerType.None)
			{
				sb.Append("-");
				nx += dx;
				ny += dy;
			}
			else
			{
				sb.Append("X");
				break;
			}
		}
	
		// 후방 (반대 방향)
		nx = x - dx;
		ny = y - dy;
	
		for (int i = 1; i < 6; i++)
		{
			if (IsInBoard(nx, ny) == false) break;

			if (board[nx, ny] == Constants.PlayerType.PlayerA)
			{
				sb.Insert(0, "O");
				nx -= dx;
				ny -= dy;
			}
			else if (board[nx, ny] == Constants.PlayerType.None)
			{
				sb.Insert(0, "-");
				nx -= dx;
				ny -= dy;
			}
			else
			{
				sb.Insert(0, "X");
				break;
			}
		}
	
		return sb.ToString();
	}

	/// <summary>
	/// 문자열이 열린 3을 의미하는지 확인
	/// </summary>
	static bool IsOpenThree(string line)
	{
		bool isOpenThree = line.Contains("-OOO-") || line.Contains("-O-OO-") || line.Contains("-OO-O-");
		
		return  isOpenThree && !IsFour(line);
	}
	
	/// <summary>
	/// 문자열이 4을 의미하는지 확인
	/// </summary>
	static bool IsFour(string line)
	{
		// 열린 4 또는 막힌 4인지 확인
		bool containFour = line.Contains("OOOO") || line.Contains("O-OOO") || line.Contains("OO-OO") ||
		                   line.Contains("OOO-O");
		
		// 5목 이상이면 제외
		bool notContainFive = line.Contains("OOOOO") == false;
		
		return containFour && notContainFive;
	}
	
	/// <summary>
	/// 문자열이 6목 이상을 의미하는지 확인
	/// </summary>
	static bool IsMoreThanFive(string line)
	{
		return line.Contains("OOOOOO");
	}
	
	#endregion
}
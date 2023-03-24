using GameAI.GamePlaying.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace GameAI.GamePlaying
{
    public class StudentAI : Behavior
    {
        public StudentAI()
        {
         
        }

        public ComputerMove Run(int color, Board board, int lookAheadDepth)
        {
            return GetMoveMinimax(color, board, 0, lookAheadDepth);
        }

        private ComputerMove GetMoveMinimax(int player, Board game_state, int currentDepth, int depthLimit)
        {
            ComputerMove best_move = null;
            Board new_state = new Board();
            List<ComputerMove> move_list = GenerateMoves(player, game_state);

            foreach (ComputerMove move in move_list)
            {
                new_state.Copy(game_state);
                new_state.MakeMove(player, move.row, move.column);

                if (new_state.IsTerminalState() || currentDepth == depthLimit)
                {
                    //move.rank = ExampleAI.MinimaxExample.EvaluateTest(new_state);
                    move.rank = Evaluate(new_state);
                }
                else
                {
                    move.rank = GetMoveMinimax(GetNextPlayer(player, new_state), new_state, currentDepth + 1, depthLimit).rank;
                }

                if (best_move == null || move.rank > best_move.rank)
                {
                    best_move = move;
                }
            }
            Console.WriteLine(best_move.row + " " + best_move.column + " " + best_move.rank);
            return best_move;
        }

        private List<ComputerMove> GenerateMoves(int color, Board board)
        {
            List<ComputerMove> moves = new List<ComputerMove>();

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    if (board.IsValidMove(color, row, column))
                    {
                        moves.Add(new ComputerMove(row, column));
                    }
                }
            }

            return moves;
        }

        
        private int Evaluate(Board board)
        {
            int value = 0;

            for (int row = 0; row < Board.Height; row++)
            {
                for (int col = 0; col < Board.Width; col++)
                {
                    int squareValue = 0;
                    int piece = board.GetTile(row, col);

                    if (piece == Board.White)
                    {
                        squareValue = 1;
                    }
                    else if (piece == Board.Black)
                    {
                        squareValue = -1;
                    }

                    if ((row == 0 || row == Board.Height - 1) && (col == 0 || col == Board.Width - 1))
                    {
                        squareValue *= 100;
                    }
                    else if (row == 0 || col == 0 || row == Board.Height - 1 || col == Board.Width - 1)
                    {
                        squareValue *= 10;
                    }

                    value += squareValue;
                }
            }

            if (board.IsTerminalState())
            {
                value += (value > 0 ? 10000 : -10000);
            }
            return value;
        }
        



        private int GetNextPlayer(int player, Board game_state)
        {
            if (game_state.HasAnyValidMove(-player))
            {
                return -player;
            }
            else
            {
                return player;
            }
        }
    }


}

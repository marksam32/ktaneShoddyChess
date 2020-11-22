using System.Collections.Generic;
using System.Linq;

namespace ShoddyChess
{
    public interface IShoddyChessPiece
    {
        List<Move> GetAvailableMoves(ChessPiece[,] board, int curX, int curY);
    }

    public abstract class Piece : IShoddyChessPiece
    {
        public abstract List<Move> GetAvailableMoves(ChessPiece[,] board, int curX, int curY);

        #region Scanners

        protected static List<Move> ScanX(ChessPiece[,] board, int curX, int curY, int maxInDirection = 8)
        {
            var moves = new List<Move>();
            var color = board[curY, curX].Color;
            var origX = curX;
            var origY = curY;

            //Move right
            for (var i = 0; i < maxInDirection; ++i)
            {
                curX += 1;
                var move = CheckSquare(curX, curY, board, origX, origY, color);
                if (move == null)
                {
                    break;
                }

                moves.Add(move);
                if (move.Capture)
                {
                    break;
                }
            }

            curX = origX;

            //Move left:
            for (var i = 0; i < maxInDirection; ++i)
            {
                curX -= 1;
                var move = CheckSquare(curX, curY, board, origX, origY, color);
                if (move == null)
                {
                    break;
                }

                moves.Add(move);
                if (move.Capture)
                {
                    break;
                }
            }

            return moves;
        }

        protected static List<Move> ScanY(ChessPiece[,] board, int curX, int curY, int maxInDirection = 8)
        {
            var moves = new List<Move>();
            var color = board[curY, curX].Color;
            var origX = curX;
            var origY = curY;

            //First try moving up:
            for (var i = 0; i < maxInDirection; ++i)
            {
                curY += 1;
                var move = CheckSquare(curX, curY, board, origX, origY, color);
                if (move == null)
                {
                    break;
                }

                moves.Add(move);
                if (move.Capture)
                {
                    break;
                }
            }

            curY = origY;
            //Move down:
            for (var i = 0; i < maxInDirection; ++i)
            {
                curY -= 1;
                var move = CheckSquare(curX, curY, board, origX, origY, color);
                if (move == null)
                {
                    break;
                }

                moves.Add(move);
                if (move.Capture)
                {
                    break;
                }
            }

            return moves;
        }

        protected static List<Move> ScanDiagonal(ChessPiece[,] board, int curX, int curY, int maxInDirection = 8)
        {
            var moves = new List<Move>();
            var color = board[curY, curX].Color;
            var origX = curX;
            var origY = curY;

            //Move up-right
            for (var i = 0; i < maxInDirection; ++i)
            {
                curX += 1;
                curY += 1;
                var move = CheckSquare(curX, curY, board, origX, origY, color);
                if (move == null)
                {
                    break;
                }

                moves.Add(move);
                if (move.Capture)
                {
                    break;
                }
            }

            curX = origX;
            curY = origY;

            //Move up-left
            for (var i = 0; i < maxInDirection; ++i)
            {
                curX -= 1;
                curY += 1;
                var move = CheckSquare(curX, curY, board, origX, origY, color);
                if (move == null)
                {
                    break;
                }

                moves.Add(move);
                if (move.Capture)
                {
                    break;
                }
            }

            curX = origX;
            curY = origY;

            //Move down-left
            for (var i = 0; i < maxInDirection; ++i)
            {
                curX -= 1;
                curY -= 1;
                var move = CheckSquare(curX, curY, board, origX, origY, color);
                if (move == null)
                {
                    break;
                }

                moves.Add(move);
                if (move.Capture)
                {
                    break;
                }
            }

            curX = origX;
            curY = origY;

            //Move down-right
            for (var i = 0; i < maxInDirection; ++i)
            {
                curX += 1;
                curY -= 1;
                var move = CheckSquare(curX, curY, board, origX, origY, color);
                if (move == null)
                {
                    break;
                }

                moves.Add(move);
                if (move.Capture)
                {
                    break;
                }
            }

            return moves;
        }

        protected static List<Move> ScanKnight(ChessPiece[,] board, int curX, int curY)
        {
            var yChanges = new List<int>{2,2,-2,-2,-1,1,-1,1};
            var xChanges = new List<int>{1,-1,-1,1,-2,-2,2,2};
            var moves = new List<Move>();
            var color = board[curY, curX].Color;
            var origX = curX;
            var origY = curY;
            
            
            for (int i = 0; i < 8; i++)
            {
                curY += yChanges[i];
                curX += xChanges[i];
                
                var move = CheckSquare(curX, curY, board, origX, origY, color);
                if (move != null)
                {
                    moves.Add(move);
                }

                curY = origY;
                curX = origX;
            }
            
            return moves;
        }

        protected static List<Move> ScanPawn(ChessPiece[,] board, int curX, int curY)
        {
            var moves = new List<Move>();
            var color = board[curY, curX].Color;
            var firstMove = board[curY, curX].PawnFirstMove;
            var origX = curX;
            var origY = curY;

            if (firstMove)
            {
                for (var i = 0; i < 2; ++i)
                {
                    if (color == PieceColor.White)
                    {
                        curY--;
                    }
                    else
                    {
                        curY++;
                    }
                    
                    if (curX > 7 || curY > 7 || curX < 0 || curY < 0)
                    {
                        //Out of bounds
                        break;
                    }

                    if (board[curY, curX] != null)
                    {
                        break;
                    }
                    
                    var move = new Move(new Pair<int, int>(origY, origX), new Pair<int, int>(curY, curX));
                    moves.Add(move);
                }
            }
            else
            {
                if (color == PieceColor.White)
                {
                    curY--;
                }
                else
                {
                    curY++;
                }
                if (!(curX > 7 || curY > 7 || curX < 0 || curY < 0))
                {
                    if (board[curY,curX] == null)
                    {
                        var move = new Move(new Pair<int, int>(origY, origX), new Pair<int, int>(curY, curX));
                        moves.Add(move);
                    }
                }
            }
            
            curX = origX;
            curY = origY;

            curX++;
            if (color == PieceColor.White)
            {
                curY--;
            }
            else
            {
                curY++;
            }

            if (!(curX > 7 || curY > 7 || curX < 0 || curY < 0))
            {
                if (board[curY,curX] != null)
                {
                    if (board[curY,curX].Color != color)
                    {
                        var move = new Move(new Pair<int, int>(origY, origX), new Pair<int, int>(curY, curX));
                        moves.Add(move);
                    }
                    
                }
            }
            
            curX = origX;
            curY = origY;

            curX--;
            if (color == PieceColor.White)
            {
                curY--;
            }
            else
            {
                curY++;
            }
            
            if (!(curX > 7 || curY > 7 || curX < 0 || curY < 0))
            {
                if (board[curY,curX] != null)
                {
                    if (board[curY,curX].Color != color)
                    {
                        var move = new Move(new Pair<int, int>(origY, origX), new Pair<int, int>(curY, curX));
                        moves.Add(move);
                    }
                }
            }
            
            return moves;
        }

        #endregion

        private static Move CheckSquare(int curX, int curY, ChessPiece[,] board, int origX, int origY, PieceColor color)
        {
            if (curX > 7 || curY > 7 || curX < 0 || curY < 0)
            {
                //Out of bounds
                return null;
            }
            else
            {
                if (board[curY, curX] == null)
                {
                    //Empty space
                    return new Move(new Pair<int, int>(origY, origX), new Pair<int, int>(curY, curX));
                }
                else if (board[curY, curX] != null && board[curY, curX].Color != color)
                {
                    //Capture piece
                    return new Move(new Pair<int, int>(origY, origX), new Pair<int, int>(curY, curX), true);
                }
                else
                {
                    //Same color
                    return null;
                }
            }
        }
    }

    public class Queen : Piece
    {
        public override List<Move> GetAvailableMoves(ChessPiece[,] board, int curX, int curY)
        {
            var moves = new List<Move>();

            moves.AddRange(ScanX(board, curX, curY));
            moves.AddRange(ScanY(board, curX, curY));
            moves.AddRange(ScanDiagonal(board, curX, curY));
            
            if (!moves.Any())
            {
                return null;
            }

            return moves;
        }
    }

    public class King : Piece
    {
        public override List<Move> GetAvailableMoves(ChessPiece[,] board, int curX, int curY)
        {
            var moves = new List<Move>();

            moves.AddRange(ScanX(board, curX, curY, 1));
            moves.AddRange(ScanY(board, curX, curY, 1));
            moves.AddRange(ScanDiagonal(board, curX, curY, 1));
            
            if (!moves.Any())
            {
                return null;
            }

            return moves;
        }
    }

    public class Bishop : Piece
    {
        public override List<Move> GetAvailableMoves(ChessPiece[,] board, int curX, int curY)
        {
            var moves = new List<Move>();
            
            
            moves.AddRange(ScanDiagonal(board,curX,curY));
            
            if (!moves.Any())
            {
                return null;
            }

            return moves;
        }
    }

    public class Knight : Piece
    {
        public override List<Move> GetAvailableMoves(ChessPiece[,] board, int curX, int curY)
        {
            var moves = ScanKnight(board, curX, curY);

            if (!moves.Any())
            {
                return null;
            }

            return moves;
        }
    }

    public class Rook : Piece
    {
        public override List<Move> GetAvailableMoves(ChessPiece[,] board, int curX, int curY)
        {
            var moves = new List<Move>();

            moves.AddRange(ScanX(board, curX, curY));
            moves.AddRange(ScanY(board, curX, curY));

            if (!moves.Any())
            {
                return null;
            }

            return moves;
        }
    }

    public class Pawn : Piece
    {
        public override List<Move> GetAvailableMoves(ChessPiece[,] board, int curX, int curY)
        {
            var moves = new List<Move>();

            moves.AddRange(ScanPawn(board,curX,curY));
            
            if (!moves.Any())
            {
                return null;
            }
            
            return moves;
        }
    }
}
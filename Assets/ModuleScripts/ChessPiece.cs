using System;
using System.Linq;

namespace ShoddyChess
{
    public class ChessPiece
    {
        private PieceType _pieceType { get; set; }
        public PieceIdentifier Id { get; private set; }
        public PieceColor Color { get; private set; }
        public IShoddyChessPiece Handler { get; private set; }
        public bool HasMoved { get; set; }
        
        public bool PawnFirstMove { get; set; }

        public PieceType PieceType
        {
            get
            {
                return _pieceType;
            }
            set
            {
                _pieceType = value;
                SetHandler(value);
            }
        }

        public override string ToString()
        {
            return string.Join("", new[]{Color.ToString().First().ToString(), Id.ToString()});
        }

        public ChessPiece(PieceColor color, PieceIdentifier id, PieceType type)
        {
            HasMoved = false;
            PieceType = type;
            Id = id;
            Color = color;
        }

        private void SetHandler(PieceType type)
        {
            switch (type)
            {
                case PieceType.Queen:
                    Handler = new Queen();
                    break;
                case PieceType.King:
                    Handler = new King();
                    break;
                case PieceType.Bishop:
                    Handler = new Bishop();
                    break;
                case PieceType.Knight:
                    Handler = new Knight();
                    break;
                case PieceType.Rook:
                    Handler = new Rook();
                    break;
                case PieceType.Pawn:
                    PawnFirstMove = true;
                    Handler = new Pawn();
                    break;
                default:
                    throw new InvalidOperationException("Invalid piece");
            }
        }
    }
}

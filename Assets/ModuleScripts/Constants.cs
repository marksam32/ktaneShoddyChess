using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace ShoddyChess
{
    public static class Constants
    {
        public static readonly List<List<PieceIdentifier>> _moveQueues = new List<List<PieceIdentifier>>
        {
            new List<PieceIdentifier>
            {
                PieceIdentifier.Q, PieceIdentifier.P5, PieceIdentifier.P7, PieceIdentifier.N1, PieceIdentifier.P6,
                PieceIdentifier.N2, PieceIdentifier.B2, PieceIdentifier.R2, PieceIdentifier.B1, PieceIdentifier.P3,
                PieceIdentifier.P4, PieceIdentifier.K, PieceIdentifier.P1, PieceIdentifier.P2, PieceIdentifier.R1,
                PieceIdentifier.P8
            },
            new List<PieceIdentifier>
            {
                PieceIdentifier.R2, PieceIdentifier.P2, PieceIdentifier.Q, PieceIdentifier.P1, PieceIdentifier.P4,
                PieceIdentifier.P5, PieceIdentifier.P7, PieceIdentifier.P6, PieceIdentifier.B1, PieceIdentifier.K,
                PieceIdentifier.N1, PieceIdentifier.B2, PieceIdentifier.P8, PieceIdentifier.N2, PieceIdentifier.R1,
                PieceIdentifier.P3
            },
            new List<PieceIdentifier>
            {
                PieceIdentifier.B2, PieceIdentifier.P1, PieceIdentifier.R1, PieceIdentifier.P6, PieceIdentifier.P4,
                PieceIdentifier.B1, PieceIdentifier.N1, PieceIdentifier.P7, PieceIdentifier.Q, PieceIdentifier.R2,
                PieceIdentifier.P8, PieceIdentifier.K, PieceIdentifier.P5, PieceIdentifier.P3, PieceIdentifier.P2,
                PieceIdentifier.N2
            },
            new List<PieceIdentifier>
            {
                PieceIdentifier.P1, PieceIdentifier.P7, PieceIdentifier.P3, PieceIdentifier.Q, PieceIdentifier.B1,
                PieceIdentifier.B2, PieceIdentifier.P5, PieceIdentifier.P8, PieceIdentifier.P6, PieceIdentifier.R2,
                PieceIdentifier.P4, PieceIdentifier.N1, PieceIdentifier.R1, PieceIdentifier.P2, PieceIdentifier.K,
                PieceIdentifier.N2
            },
            new List<PieceIdentifier>
            {
                PieceIdentifier.P8, PieceIdentifier.P5, PieceIdentifier.K, PieceIdentifier.R1, PieceIdentifier.P1,
                PieceIdentifier.P6, PieceIdentifier.P4, PieceIdentifier.Q, PieceIdentifier.P7, PieceIdentifier.R2,
                PieceIdentifier.N1, PieceIdentifier.B2, PieceIdentifier.N2, PieceIdentifier.P2, PieceIdentifier.P3,
                PieceIdentifier.B1
            },
            new List<PieceIdentifier>
            {
                PieceIdentifier.B1, PieceIdentifier.P7, PieceIdentifier.K, PieceIdentifier.P4, PieceIdentifier.B2,
                PieceIdentifier.P2, PieceIdentifier.P8, PieceIdentifier.N2, PieceIdentifier.P6, PieceIdentifier.P5,
                PieceIdentifier.R1, PieceIdentifier.P3, PieceIdentifier.N1, PieceIdentifier.R2, PieceIdentifier.P1,
                PieceIdentifier.Q
            },
            new List<PieceIdentifier>
            {
                PieceIdentifier.N1, PieceIdentifier.P5, PieceIdentifier.P2, PieceIdentifier.Q, PieceIdentifier.K,
                PieceIdentifier.P3, PieceIdentifier.P6, PieceIdentifier.R2, PieceIdentifier.N2, PieceIdentifier.P8,
                PieceIdentifier.R1, PieceIdentifier.P1, PieceIdentifier.B1, PieceIdentifier.P4, PieceIdentifier.P7,
                PieceIdentifier.B2
            },
            new List<PieceIdentifier>
            {
                PieceIdentifier.N2, PieceIdentifier.P4, PieceIdentifier.R1, PieceIdentifier.P8, PieceIdentifier.P1,
                PieceIdentifier.K, PieceIdentifier.B2, PieceIdentifier.P7, PieceIdentifier.P5, PieceIdentifier.Q,
                PieceIdentifier.R2, PieceIdentifier.P6, PieceIdentifier.N1, PieceIdentifier.P2, PieceIdentifier.B1,
                PieceIdentifier.P3
            },
            new List<PieceIdentifier>
            {
                PieceIdentifier.P7, PieceIdentifier.P3, PieceIdentifier.B2, PieceIdentifier.P2, PieceIdentifier.P4,
                PieceIdentifier.K, PieceIdentifier.R2, PieceIdentifier.P8, PieceIdentifier.R1, PieceIdentifier.B1,
                PieceIdentifier.Q, PieceIdentifier.N1, PieceIdentifier.N2, PieceIdentifier.P5, PieceIdentifier.P6,
                PieceIdentifier.P1
            },
            new List<PieceIdentifier>
            {
                PieceIdentifier.P6, PieceIdentifier.P5, PieceIdentifier.Q, PieceIdentifier.B1, PieceIdentifier.P7,
                PieceIdentifier.P2, PieceIdentifier.N1, PieceIdentifier.K, PieceIdentifier.N2, PieceIdentifier.R2,
                PieceIdentifier.R1, PieceIdentifier.B2, PieceIdentifier.P8, PieceIdentifier.P1, PieceIdentifier.P4,
                PieceIdentifier.P3
            }
        };

        public static ChessPiece[,] CreateBoard()
        {
            return new[,]
            {
                {
                    CreatePiece(PieceColor.Black, PieceIdentifier.R1, PieceType.Rook),
                    CreatePiece(PieceColor.Black, PieceIdentifier.N1, PieceType.Knight),
                    CreatePiece(PieceColor.Black, PieceIdentifier.B1, PieceType.Bishop),
                    CreatePiece(PieceColor.Black, PieceIdentifier.Q, PieceType.Queen),
                    CreatePiece(PieceColor.Black, PieceIdentifier.K, PieceType.King),
                    CreatePiece(PieceColor.Black, PieceIdentifier.B2, PieceType.Bishop),
                    CreatePiece(PieceColor.Black, PieceIdentifier.N2, PieceType.Knight),
                    CreatePiece(PieceColor.Black, PieceIdentifier.R2, PieceType.Rook)
                },
                {
                    CreatePiece(PieceColor.Black, PieceIdentifier.P1, PieceType.Pawn),
                    CreatePiece(PieceColor.Black, PieceIdentifier.P2, PieceType.Pawn),
                    CreatePiece(PieceColor.Black, PieceIdentifier.P3, PieceType.Pawn),
                    CreatePiece(PieceColor.Black, PieceIdentifier.P4, PieceType.Pawn),
                    CreatePiece(PieceColor.Black, PieceIdentifier.P5, PieceType.Pawn),
                    CreatePiece(PieceColor.Black, PieceIdentifier.P6, PieceType.Pawn),
                    CreatePiece(PieceColor.Black, PieceIdentifier.P7, PieceType.Pawn),
                    CreatePiece(PieceColor.Black, PieceIdentifier.P8, PieceType.Pawn)
                },
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {
                    CreatePiece(PieceColor.White, PieceIdentifier.P1, PieceType.Pawn),
                    CreatePiece(PieceColor.White, PieceIdentifier.P2, PieceType.Pawn),
                    CreatePiece(PieceColor.White, PieceIdentifier.P3, PieceType.Pawn),
                    CreatePiece(PieceColor.White, PieceIdentifier.P4, PieceType.Pawn),
                    CreatePiece(PieceColor.White, PieceIdentifier.P5, PieceType.Pawn),
                    CreatePiece(PieceColor.White, PieceIdentifier.P6, PieceType.Pawn),
                    CreatePiece(PieceColor.White, PieceIdentifier.P7, PieceType.Pawn),
                    CreatePiece(PieceColor.White, PieceIdentifier.P8, PieceType.Pawn)
                },
                {
                    CreatePiece(PieceColor.White, PieceIdentifier.R1, PieceType.Rook),
                    CreatePiece(PieceColor.White, PieceIdentifier.N1, PieceType.Knight),
                    CreatePiece(PieceColor.White, PieceIdentifier.B1, PieceType.Bishop),
                    CreatePiece(PieceColor.White, PieceIdentifier.Q, PieceType.Queen),
                    CreatePiece(PieceColor.White, PieceIdentifier.K, PieceType.King),
                    CreatePiece(PieceColor.White, PieceIdentifier.B2, PieceType.Bishop),
                    CreatePiece(PieceColor.White, PieceIdentifier.N2, PieceType.Knight),
                    CreatePiece(PieceColor.White, PieceIdentifier.R2, PieceType.Rook)
                }
            };
        }
        
        public const string _base36string = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static ChessPiece CreatePiece(PieceColor color, PieceIdentifier id, PieceType type)
        {
            return new ChessPiece(color, id, type);
        }
        
        public static readonly List<string> _symbols = new List<string>
            {"l", "w", "n1", "n2", "j1", "j2", "t1", "t2", "o1", "o2", "o3", "o4", "o5", "o6", "o7", "o8"};
        
        public static readonly Regex tpPlaceRegex =
            new Regex(
                "^place (([bw][qnrbp][12345678]|[abcdefgh][12345678])( ([bw][qnrbp][12345678]|[abcdefgh][12345678]))*)$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static readonly Regex tpClearRegex = new Regex("^clear (([abcdefgh][012345678])( [abcdefgh][012345678])*)$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static readonly int Close = Animator.StringToHash("Close");
        public static readonly int Open = Animator.StringToHash("Open");
        public static readonly int SmallOpen = Animator.StringToHash("SmallOpen");
        public static readonly int SmallClose = Animator.StringToHash("SmallClose");
    }
}
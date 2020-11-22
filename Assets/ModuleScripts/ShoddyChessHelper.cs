using System;
using System.Linq;

namespace ShoddyChess
{
    public static class ShoddyChessHelper
    {
        public static Pair<int, int> GetCoordinateFromIndex(int index)
        {
            var col = index;
            var row = 0;
            while (col > 7)
            {
                col -= 8;
                ++row;
            }

            return new Pair<int, int>(row, col);
        }

        public static string GetCoordinate(int r, int c)
        {
            var col = (char) (c + 65);
            var row = r + 1;

            return string.Join("", new[] {col.ToString(),  (9 - row).ToString()});
        }

        public static int GetIndexFromCoordinate(Pair<int, int> coordinates)
        {
            return (coordinates.Item1 * 8) + coordinates.Item2;
        }

        public static BoardItem GetPiece(string id)
        {
            var parts = id.ToArray();
            PieceIdentifier identifier;

            switch (parts[1])
            {
                case 'k':
                    identifier = PieceIdentifier.K;
                    break;
                case 'q':
                    identifier = PieceIdentifier.Q;
                    break;
                case 'b':
                    if (parts[2] != '1' && parts[2] != '2')
                    {
                        return null;
                    }

                    identifier = parts[2] == '1' ? PieceIdentifier.B1 : PieceIdentifier.B2;
                    break;
                case 'n':
                    if (parts[2] != '1' && parts[2] != '2')
                    {
                        return null;
                    }

                    identifier = parts[2] == '1' ? PieceIdentifier.N1 : PieceIdentifier.N2;
                    break;
                case 'r':
                    if (parts[2] != '1' && parts[2] != '2')
                    {
                        return null;
                    }

                    identifier = parts[2] == '1' ? PieceIdentifier.R1 : PieceIdentifier.R2;
                    break;
                case 'p':
                    identifier = (PieceIdentifier) int.Parse(parts[2].ToString()) + 7;
                    break;
                default:
                    throw new InvalidOperationException("Invalid piece, please contact Marksam on discord! GetPiece()");
            }

            return new BoardItem(identifier, parts[0] == 'w' ? PieceColor.White : PieceColor.Black, true);
        }
    }
}
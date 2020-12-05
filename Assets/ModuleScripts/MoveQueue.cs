using System.Collections.Generic;

namespace ShoddyChess
{
    public class MoveQueue
    {
        private int Position { get; set; }
        public PieceIdentifier CurrentPiece { get; private set; }
        private List<PieceIdentifier> Queue { get; set; }
        private int CurrentQueue { get; set; }

        public MoveQueue(int queue)
        {
            Position = 0;
            Queue = Constants._moveQueues[queue];
            CurrentQueue = queue;

            CurrentPiece = Queue[Position];
        }

        public void Advance()
        {
            if (Position == 15)
            {
                Position = 0;
                if (CurrentQueue == 9)
                {
                    CurrentQueue = 0;
                }
                else
                {
                    CurrentQueue++;
                }
                Queue = Constants._moveQueues[CurrentQueue];
            }
            else
            {
                Position++;
            }

            CurrentPiece = Queue[Position];
        }
    }
}

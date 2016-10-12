using System;

namespace BoardGames.Games.Chess
{
    internal class Rook : ChessPiece
    {
        private bool isMoved = false;
        public bool IsMoved
        {
            get { return isMoved; }
            set { isMoved = value;  }
        }

        public Rook(Player pOwner, PlayerBoardAlignment pBoardAlignment)
            : base(pOwner, pBoardAlignment)
        {

        }

        public override string PieceName
        {
            get { return "Rook"; }
        }

        public override string PieceNameOneLetter
        {
            get { return "R"; }
        }

        public override bool CanMove(GamePiece[,] table, Position from, Position to)
        {
            bool canMove = false;
            //IMPROVE: make a better algorithm
            if ((from.XAbsolute - to.XAbsolute != 0 && from.YAbsolute - to.YAbsolute != 0) ||
                (from.XAbsolute - to.XAbsolute == 0 && from.YAbsolute - to.YAbsolute == 0)
                ) return canMove;

            int xDirection = 0;
            int yDirection = 0;
            int distance = 0;
            if (from.XAbsolute - to.XAbsolute == 0)
            {
                distance = Math.Abs(from.YAbsolute - to.YAbsolute);
                if (from.YAbsolute - to.YAbsolute > 0)
                {
                    yDirection = -1;
                }
                else
                {
                    yDirection = 1;
                }
            }

            if (from.YAbsolute - to.YAbsolute == 0)
            {
                distance = Math.Abs(from.XAbsolute - to.XAbsolute);
                if (from.XAbsolute - to.XAbsolute > 0)
                {
                    xDirection = -1;
                }
                else
                {
                    xDirection = 1;
                }
            }

            int step = 1;
            int newX = from.XAbsolute + step * xDirection;
            int newY = from.YAbsolute + step * yDirection;
            //intermediate cells
            while (step <= distance)
            {
                newX = from.XAbsolute + step * xDirection;
                newY = from.YAbsolute + step * yDirection;
                if (table[newX, newY] == null)
                {
                    canMove = true;
                }
                else
                {
                    canMove = false;
                    return canMove;
                }

                step++;
            }
            //final place
            if (table[newX, newY] == null || table[newX, newY].Owner != this.Owner)
            {
                canMove = true;
            }

            return canMove;
        }
    }
}

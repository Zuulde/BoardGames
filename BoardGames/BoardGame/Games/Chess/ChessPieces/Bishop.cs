using System;

namespace BoardGames.Games.Chess
{
    internal class Bishop : ChessPiece
    {
        public Bishop(Player pOwner, PlayerBoardAlignment pBoardAlignment)
            : base(pOwner, pBoardAlignment)
        {

        }

        public override string PieceName
        {
            get { return "Bishop"; }
        }

        public override string PieceNameOneLetter
        {
            get { return "B"; }
        }

        public override bool CanMove(GamePiece[,] table, Position from, Position to)
        {
            bool canMove = false;
            //IMPROVE: make a better algorithm
            if (from.XAbsolute - to.XAbsolute == 0 || from.YAbsolute - to.YAbsolute == 0) return canMove;

            if (Math.Abs(from.XAbsolute - to.XAbsolute) == Math.Abs(from.YAbsolute - to.YAbsolute))
            {
                int xDirection = 1;
                if ( from.XAbsolute - to.XAbsolute > 0 )
                { 
                    xDirection = -1;
                }

                int yDirection = 1;
                if (from.YAbsolute - to.YAbsolute > 0)
                {
                    yDirection = -1;
                }

                int distance = Math.Abs(from.XAbsolute - to.XAbsolute);
                int step = 1;
                int newX = from.XAbsolute + step * xDirection;
                int newY = from.YAbsolute + step * yDirection;
                //intermediate cells
                while (step < distance)
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
            }

            return canMove;
        }
    }
}

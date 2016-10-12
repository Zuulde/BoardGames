using System;

namespace BoardGames.Games.Chess
{
    internal class Queen : ChessPiece
    {
        public Queen(Player pOwner, PlayerBoardAlignment pBoardAlignment)
            : base(pOwner, pBoardAlignment)
        {

        }

        public override string PieceName
        {
            get { return "Queen"; }
        }

        public override string PieceNameOneLetter
        {
            get { return "Q"; }
        }

        public override bool CanMove(GamePiece[,] table, Position from, Position to)
        {
            bool canMove = false;
            //IMPROVE: make a better algorithm
            if (from.XAbsolute - to.XAbsolute == 0 && from.YAbsolute - to.YAbsolute == 0) return canMove;

            int xDirection = 0;
            int yDirection = 0;
            int distance = 0;
            bool rookOrBishop = false;

            //Rook part
            if (Math.Abs(from.XAbsolute - to.XAbsolute) == 0 ||
                Math.Abs(from.YAbsolute - to.YAbsolute) == 0
                )
            {
                rookOrBishop = true;

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
            }
            //Bishop part
            else if (Math.Abs(from.XAbsolute - to.XAbsolute) == Math.Abs(from.YAbsolute - to.YAbsolute))
            {
                rookOrBishop = true;

                xDirection = 1;
                distance = Math.Abs(from.XAbsolute - to.XAbsolute);
                if (from.XAbsolute - to.XAbsolute > 0)
                {
                    xDirection = -1;
                }

                yDirection = 1;
                if (from.YAbsolute - to.YAbsolute > 0)
                {
                    yDirection = -1;
                }
            }

            if (!rookOrBishop) return canMove;

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


            return canMove;
        }
    }
}
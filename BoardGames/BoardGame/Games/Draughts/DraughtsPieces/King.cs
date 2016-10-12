using System;

namespace BoardGames.Games.Draughts
{
    class King : DraughtsPiece
    {
        public King(Player pOwner, PlayerBoardAlignment pBoardAlignment)
            : base(pOwner, pBoardAlignment)
        {
        }

        public override string PieceName
        {
            get { return "King"; }
        }

        public override string PieceNameOneLetter
        {
            get { return "K"; }
        }

        public override bool CanMove(GamePiece[,] table, Position from, Position to)
        {
            bool canMove = false;

            //move
            if (Math.Abs(from.XAbsolute - to.XAbsolute) == 1 && Math.Abs(from.YAbsolute - to.YAbsolute) == 1 &&
                table[to.XAbsolute, to.YAbsolute] == null
                )
            {
                canMove = true;
            }
            //attack
            else if (CanAttack(table, from, to))
            {
                canMove = true;
            }

            return canMove;
        }

        public override bool CanAttack(GamePiece[,] table, Position from, Position to)
        {
            bool canAttack = false;

            if (Math.Abs(from.XAbsolute - to.XAbsolute) == 2 && Math.Abs(from.YAbsolute - to.YAbsolute) == 2 &&
                table[to.XAbsolute, to.YAbsolute] == null
                )
            {
                int inBetweenX = (from.XAbsolute + to.XAbsolute) / 2;
                int inBetweenY = (from.YAbsolute + to.YAbsolute) / 2;
                if (table[inBetweenX, inBetweenY] != null && table[inBetweenX, inBetweenY].Owner != this.Owner)
                {
                    canAttack = true;
                }
            }

            return canAttack;
        }
    }
}

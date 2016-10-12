using System;

namespace BoardGames.Games.Chess
{
    internal class Knight : ChessPiece
    {
        public Knight(Player pOwner, PlayerBoardAlignment pBoardAlignment)
            : base(pOwner, pBoardAlignment)
        {

        }

        public override string PieceName
        {
            get { return "Knight"; }
        }

        public override string PieceNameOneLetter
        {
            get { return "N"; }
        }

        public override bool CanMove(GamePiece[,] table, Position from, Position to)
        {
            bool canMove = false;

            if ( (table[to.XAbsolute, to.YAbsolute] == null || table[to.XAbsolute, to.YAbsolute].Owner != this.Owner) &&
                 ( (Math.Abs(from.XAbsolute - to.XAbsolute) == 2 && Math.Abs(from.YAbsolute - to.YAbsolute) == 1) || // wides moves
                 (Math.Abs(from.XAbsolute - to.XAbsolute) == 1 && Math.Abs(from.YAbsolute - to.YAbsolute) == 2) )// higher moves
                )
            {
                canMove = true;
            }

            return canMove;
        }
    }
}

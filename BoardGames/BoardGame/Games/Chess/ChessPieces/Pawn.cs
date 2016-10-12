using System;

namespace BoardGames.Games.Chess
{
    internal class Pawn : ChessPiece
    {
        public Pawn(Player pOwner, PlayerBoardAlignment pBoardAlignment)
            : base(pOwner, pBoardAlignment)
        {

        }

        public override string PieceName
        {
            get { return "Pawn"; }
        }

        public override string PieceNameOneLetter
        {
            get { return "P"; }
        }

        public override bool CanMove(GamePiece[,] table, Position from, Position to)
        {
            bool canMove = false;

            int rows = table.GetLength(1);
            int columns = table.GetLength(0);
            int direction = 1;
            int startRow = 1;
            if (table[from.XAbsolute, from.YAbsolute].BoardAlignment == PlayerBoardAlignment.highindex)
            {
                direction = -1;
                startRow = rows - 2;
            }

            //move forward 1 tile
            if (from.XAbsolute + direction == to.XAbsolute &&
                from.YAbsolute == to.YAbsolute &&
                table[to.XAbsolute, to.YAbsolute] == null
                )
            {
                canMove = true;
            }
            //Jump 2 tiles from starting line
            else if ((from.XAbsolute + 2 * direction == to.XAbsolute && from.XAbsolute == startRow) &&
                (from.YAbsolute == to.YAbsolute) &&
                table[to.XAbsolute, to.YAbsolute] == null && table[to.XAbsolute + direction, from.YAbsolute] == null
                )
            {
                canMove = true;
            }
            //attack
            else if (from.XAbsolute + direction == to.XAbsolute &&
                Math.Abs(from.YAbsolute - to.YAbsolute) == 1 &&
                table[to.XAbsolute, to.YAbsolute] != null && table[to.XAbsolute, to.YAbsolute].Owner != this.Owner
                )
            {
                canMove = true;
            }
            //en Passant

            return canMove;
        }

        public bool IsFinalLine(GamePiece[,] table, Position myPosition)
        {
            bool isFinalLine = false;

            int rows = table.GetLength(1);
            int finalRow = rows - 1;
            if (table[myPosition.XAbsolute, myPosition.YAbsolute].BoardAlignment == PlayerBoardAlignment.highindex)
            {
                finalRow = 0;
            }

            if (myPosition.XAbsolute == finalRow)
            {
                isFinalLine = true;
            }

            return isFinalLine;
        }

    }
}

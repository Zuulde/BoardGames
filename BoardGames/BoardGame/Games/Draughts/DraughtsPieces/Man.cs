using System;

namespace BoardGames.Games.Draughts
{
    class Man : DraughtsPiece
    {
        public Man(Player pOwner, PlayerBoardAlignment pBoardAlignment)
            : base(pOwner, pBoardAlignment)
        {
        }

        public override string PieceName
        {
            get { return "Man"; }
        }

        public override string PieceNameOneLetter
        {
            get { return "M"; }
        }

        public override bool CanMove(GamePiece[,] table, Position from, Position to)
        {
            bool canMove = false;

            int rows = table.GetLength(1);
            int columns = table.GetLength(0);
            int direction = 1;
            if (table[from.XAbsolute, from.YAbsolute].BoardAlignment == PlayerBoardAlignment.highindex)
            {
                direction = -1;
            }

            //move
            if (from.XAbsolute + direction == to.XAbsolute && Math.Abs(from.YAbsolute - to.YAbsolute) == 1 &&
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

            int rows = table.GetLength(1);
            int columns = table.GetLength(0);
            int direction = 1;
            int chagePieceRow = rows - 1;
            if (table[from.XAbsolute, from.YAbsolute].BoardAlignment == PlayerBoardAlignment.highindex)
            {
                direction = -1;
                chagePieceRow = 0;
            }

            if (from.XAbsolute + direction * 2 == to.XAbsolute && Math.Abs(from.YAbsolute - to.YAbsolute) == 2 &&
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

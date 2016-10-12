using System;

namespace BoardGames.Games.Chess
{
    internal class King : ChessPiece
    {
        private bool castlingAllowed = true;
        public bool CastlingAllowed
        {
            get { return castlingAllowed; }
            set { castlingAllowed = value; }
        }

        private bool isInCheck = false;
        public bool IsInCheck
        {
            get { return isInCheck; }
            set { isInCheck = value; }
        }

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

            int rows = table.GetLength(1);
            int startRow = 0;
            if (table[from.XAbsolute, from.YAbsolute].BoardAlignment == PlayerBoardAlignment.highindex)
            {
                startRow = rows - 1;
            }


            if (from.XAbsolute - to.XAbsolute == 0 && from.YAbsolute - to.YAbsolute == 0) return canMove;

            if ((from.XAbsolute - to.XAbsolute == 0 || Math.Abs(from.XAbsolute - to.XAbsolute) == 1) &&
                (from.YAbsolute - to.YAbsolute == 0 || Math.Abs(from.YAbsolute - to.YAbsolute) == 1) &&
                (table[to.XAbsolute, to.YAbsolute] == null || table[to.XAbsolute, to.YAbsolute].Owner != this.Owner)
                )
            {
                canMove = true;
            }

            //IMPROVE code maybe could be optimized a bit. Regarding the checking here and outside
            #region castling
            //we are at initial position, etc
            if (CastlingAllowed && !IsInCheck && (from.XAbsolute == startRow && to.XAbsolute == startRow) &&
                from.YAbsolute == 4 && Math.Abs(to.YAbsolute - from.YAbsolute) == 2
                )
            {
                int horizontalMove = to.YAbsolute - from.YAbsolute;
                int hRookPos = -10;
                //is not at check?
                if (horizontalMove == 2) // king side
                {
                    hRookPos = 7;
                }
                else // queen side
                {
                    hRookPos = 0;
                }

                //in between places are empty
                if ((table[startRow, from.YAbsolute + horizontalMove] == null &&
                    table[startRow, from.YAbsolute + (horizontalMove / 2)] == null) &&
                    //The player's rook is in position
                    table[startRow, hRookPos] != null && table[startRow, hRookPos].Owner == this.Owner &&
                    (table[startRow, hRookPos] is Rook) && (table[startRow, hRookPos] as Rook).IsMoved == false
                    )
                {
                    // empty places and check between
                    canMove = true;
                }
            }
            #endregion

            return canMove;
        }
    }
}
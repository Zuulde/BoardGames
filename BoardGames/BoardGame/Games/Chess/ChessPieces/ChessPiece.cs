using System;

namespace BoardGames.Games.Chess
{
    internal abstract class ChessPiece : GamePiece
    {
        public ChessPiece(Player pOwner, PlayerBoardAlignment pBoardAlignment)
            : base(pOwner, pBoardAlignment) { }
    }
}

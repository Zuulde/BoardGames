using System;

namespace BoardGames.Games.Draughts
{
    internal abstract class DraughtsPiece : GamePiece
    {
        public DraughtsPiece(Player pOwner, PlayerBoardAlignment pBoardAlignment)
            : base(pOwner, pBoardAlignment) { }

        public abstract bool CanAttack(GamePiece[,] table, Position from, Position to);
    }
}

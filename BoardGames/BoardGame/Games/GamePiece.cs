using System;

namespace BoardGames
{
    /// <summary>
    /// Represents a game piece/figure on a given board game.
    /// </summary>
    public abstract class GamePiece
    {
        public abstract String PieceName { get; }
        /// <summary>
        /// Returns the first letter of the piece's name. Should be unique.
        /// </summary>
        /// <example>
        /// Can be used to identify the piece.
        /// </example>
        public abstract String PieceNameOneLetter { get; }

        private PlayerBoardAlignment boardAlignment;
        public PlayerBoardAlignment BoardAlignment
        {
            get { return boardAlignment; }
        }

        private Player owner;
        public Player Owner
        {
            get { return owner; }
        }

        public String OwnerNameOneLetter
        {
            get { return Owner.NameOneLetter; }
        }

        internal GamePiece(Player pOwner, PlayerBoardAlignment pBoardAlignment)
        {
            owner = pOwner;
            boardAlignment = pBoardAlignment;
        }

        /// <summary>
        /// Implements the logic where a given piece should be able to move.
        /// </summary>
        /// <remarks>
        /// This may define if the piece can attack the destination position.
        /// </remarks>
        /// <param name="table">The board where the game goes on</param>
        /// <param name="from">Source position of movement</param>
        /// <param name="to">Destination position of movement</param>
        /// <returns>Boolean indication if the piece can move to destination position</returns>
        public abstract bool CanMove(GamePiece[,] table, Position from, Position to);
    }
}

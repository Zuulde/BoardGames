namespace BoardGames
{
    /// <summary>
    /// Represents a user interface that handles user inputs and game outputs.
    /// </summary>
    /// <remarks>
    /// This must be implemented to have a valid running game. Not all of them are necessary.
    /// Internal structure of the game is hidden from the outside world.
    /// </remarks>
    /// <example>
    /// Console interface that reads/write from/to console. ConsoleUI project is a good example.
    /// </example>
    public interface IUserInterface
    {
        /// <summary>
        /// Visualize/handle message received from the game.
        /// </summary>
        /// <param name="msg">The exact message sent by the game</param>
        /// <remarks>Required</remarks>
        void ShowMessage(string msg);

        /// <summary>
        /// Reads input string message that is to be processed by the game
        /// </summary>
        /// <returns>String message returned to the game</returns>
        /// <remarks>Required</remarks>
        string ReadInput();

        /// <summary>
        /// Visualize/handle the game area where the game goes on.
        /// </summary>
        /// <param name="pTable">Current game area</param>
        /// <remarks>Required</remarks>
        void ShowBoard(GamePiece[,] pTable);

        /// <summary>
        /// Helper function to provide a method to prepare input data for boardgame
        /// </summary>
        /// <remarks>Optional</remarks>
        void SetInputReady(string input, bool isInputReady);
    }
}

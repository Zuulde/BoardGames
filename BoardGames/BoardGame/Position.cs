namespace BoardGames
{
    /// <summary>
    /// Indicates a position on the game board
    /// </summary>
    public class Position
    {
        int xAbsolute;
        public int XAbsolute
        {
            get { return xAbsolute; }
            set { xAbsolute = value; }
        }

        int yAbsolute;
        public int YAbsolute
        {
            get { return yAbsolute; }
            set { yAbsolute = value; }
        }

        string xTextual;
        /// <summary>
        /// Show the current X position in a textual format.
        /// </summary>
        /// <example>
        /// In chess columns are identified by letters
        /// </example>
        public string XTextual
        {
            get { return (xAbsolute + 1).ToString(); }
        }

        string ytextual;
        /// <summary>
        /// Show the current Y position in a textual format.
        /// </summary>
        /// <example>
        /// In chess columns are identified by letters
        /// </example>
        public string Ytextual
        {
            get { return ((char)('A' + yAbsolute)).ToString(); }
        }

        internal Position(int x, int y)
        {
            xAbsolute = x;
            yAbsolute = y;
        }
    }
}

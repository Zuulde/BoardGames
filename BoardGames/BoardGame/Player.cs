using System;
using System.Drawing;

namespace BoardGames
{
    /// <summary>
    /// Class to represent a player in a given game.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Right now this is the unique id
        /// </summary>
        private static int IDCOUNTER = 0;

        public int ID;
        String name;
        public String Name
        {
            get { return name; }
        }

        /// <summary>
        /// First letter of player's name.
        /// </summary>
        /// <example>
        /// Console interface could use this with the piece's name to make a piece 'composition'
        /// </example>
        public String NameOneLetter
        {
            get { return name[0].ToString(); }
        }

        Color playerColor;

        int wins;
        public int Wins
        {
            get { return wins; }
        }
        int losses;
        public int Losses
        {
            get { return losses; }
        }
        int draws;
        public int Draws
        {
            get { return draws; }
        }
        PlayerBoardAlignment boardAlignment;
        public PlayerBoardAlignment BoardAlignment
        {
            get { return boardAlignment; }
            set { boardAlignment = value; }
        }

        public int TotalGame
        {
            get { return wins + losses + draws; }
        }

        public decimal WinRate
        {
            get
            {
                Decimal divider = (losses + draws);
                if (divider == 0) return 100m;
                return (decimal)(wins / divider);
            }
        }

        internal Player(String pName)
        {
            ID = ++IDCOUNTER;
            name = pName;
        }

        internal Player(Boolean nonUser)
        {
            ID = 0;
            name = "none";
        }

        void AddWin()
        {
            wins++;
        }

        void AddLose()
        {
            losses++;
        }

        void AddDraw()
        {
            draws++;
        }
    }
}

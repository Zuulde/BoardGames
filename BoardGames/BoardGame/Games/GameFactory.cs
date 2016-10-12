using System;
using BoardGames.Resources;
using System.Linq;
using System.Collections.Generic;

namespace BoardGames
{
    /// <summary>
    /// Creates a board game based on user input.
    /// </summary>
    public static class GameFactory
    {
        /// <summary>
        /// Creates a board game based on user input and make initial configurations.
        /// </summary>
        /// <param name="ui">The interface that shall show the game to the user</param>
        /// <returns>Selected game</returns>
        public static BoardGame CreateGame(IUserInterface ui)
        {
            BoardGame selectedGame = null;
            Type[] gameTypes = typeof(BoardGame).Assembly.GetTypes().
                Where(type => type.IsSubclassOf(typeof(BoardGame))).ToArray();
            List<Tuple<String, BoardGame>> games = new List<Tuple<String, BoardGame>>(); //UNSURE is tuple necessary?

            //IMPROVE: create a solution that is general, automatic, (encapsulated) more efficient, and elegant
            for (int index = 0; index < gameTypes.Length; index++)
            {
                BoardGame currentGame = null;
                try
                {
                    currentGame = Activator.CreateInstance(gameTypes[index]) as BoardGame;
                }
                catch { }

                if (currentGame != null && currentGame.IsGamePlayable)
                {
                    //you may add licensing or whatever you desire.
                    games.Add(new Tuple<String, BoardGame>(currentGame.GameName, currentGame));
                }
            }
            ui.ShowMessage(Strings.game_choose);
            ui.ShowMessage(String.Format(Strings.games_list, String.Join(", ",
                games.Select(game => game.Item1).ToArray<String>())));
            ui.ShowMessage(Strings.factory_commands);
            while (selectedGame == null)
            {
                var input = ui.ReadInput();
                if (input == "none" || input == "quit" || input == "exit")
                {
                    ui.ShowMessage(Strings.factory_exit);
                    break;
                }
                foreach(var game in games)
                {
                    if (String.Equals(input, game.Item1, StringComparison.InvariantCultureIgnoreCase))
                    {
                        selectedGame = game.Item2;
                        selectedGame.UI = ui;
                        break;
                    }
                }
            }

            return selectedGame;
        }
    }
}

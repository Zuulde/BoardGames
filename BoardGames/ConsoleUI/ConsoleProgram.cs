using BoardGames;

namespace ConsoleUI
{
    class ConsoleProgram
    {
        static void Main(string[] args)
        {
            IUserInterface consoleUI = new ConsoleInterface();
            BoardGame game = null;
            do {
                game = GameFactory.CreateGame(consoleUI);
                if (game != null) game.Play();
            } while (game != null);
            consoleUI.ReadInput();
        }
    }
}

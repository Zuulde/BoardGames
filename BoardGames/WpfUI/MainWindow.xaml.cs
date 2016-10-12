using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoardGames;
using WpfUI.Resources;

namespace WpfUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IUserInterface ui;

        protected Boolean isUserInputReady = false;
        public BoardGame currentGame;
        protected String currentSkinPath; //IMPROVE right now default skin is redundant as it is copied from skin dir
        Thread boardGameThread;

        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
            boardGameThread = new Thread(InitiateBoardGame);
            boardGameThread.Start();
        }

        public void InitiateBoardGame()
        {
            ui = new WpfInterface(this); //arbitrary. e.g.: new ConsoleInterface
            currentGame = null;
            do
            {
                currentGame = GameFactory.CreateGame(ui);

                #region skin section - only for WPF right now
                if (ui is WpfInterface)
                {
                    currentSkinPath = Constants.RESOURCES_DIR + currentGame.GameName + Path.DirectorySeparatorChar;
                    String[] skins = Directory.GetDirectories(currentSkinPath);
                    for (int index = 0; index < skins.Length; index++) skins[index] = Path.GetFileName(skins[index]);

                    if (skins.Length > 0)
                    {
                        ui.ShowMessage(String.Format(StringsWPF.game_skins_available, String.Join(", ", skins)));
                        Boolean isSkinSelected = false;
                        while (!isSkinSelected)
                        {
                            String userInput = ui.ReadInput();
                            foreach (var skin in skins)
                            {
                                if (String.Equals(userInput, skin, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    isSkinSelected = true;
                                    currentSkinPath += skin + Path.DirectorySeparatorChar;
                                }
                                else if (String.IsNullOrWhiteSpace(userInput))
                                {
                                    isSkinSelected = true;
                                    break;
                                }
                            }
                        }

                    }
                }
                   

                #endregion

                if (currentGame != null) currentGame.Play();
            } while (currentGame != null);
        }

        private void CreateGameArea(BoardGame pGame)
        {
            BoardUniformGrid.Rows = pGame.TableHeight + 2;
            BoardUniformGrid.Columns = pGame.TableWidth + 2;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserInputEntered();
        }

        private void TextBox_userInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            e.Handled = true;
            UserInputEntered();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            boardGameThread.Abort();
        }

        private void TextBox_MessagaArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox_MessagaArea.ScrollToEnd();
        }
    }
}

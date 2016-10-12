using System;
using System.Threading;
using BoardGames;

namespace WpfUI
{
    //UNSURE: there might be a way to solve threading in a more elegant manner.
    //Use delegates or events ? is this version proper enough?
    class WpfInterface : IUserInterface
    {
        protected MainWindow hostWindow;
        protected bool isInputReady = false;
        protected string inputMessage = String.Empty;

        public WpfInterface(MainWindow pHostWindow)
        {
            hostWindow = pHostWindow;
        }

        public void SetInputReady(string pInputMessage, bool pIsInputReady)
        {
            inputMessage = pInputMessage;
            isInputReady = pIsInputReady;
        }

        public void ShowMessage(string msg)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    hostWindow.UpdateInfoArea(msg);
                });
        }

        public string ReadInput()
        {
            while (!isInputReady)
            {
                if (!isInputReady) Thread.Sleep(Constants.INPUT_WAIT_SLEEP_TIME);
            }
            isInputReady = !isInputReady;
            string retVal = inputMessage;
            inputMessage = String.Empty;

            ShowMessage(retVal);

            return retVal;
        }

        public void ShowBoard(GamePiece[,] pTable)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    hostWindow.DrawBoard(pTable);
                });
        }
    }
}

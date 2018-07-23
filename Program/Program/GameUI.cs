namespace B18_Ex02
{
    using System;

    public class GameUI
    {
        private static readonly char sr_QuitInput = 'Q';
        private readonly string r_GetInputNameFromPlayerMessage = string.Format("Please enter your name ({0} letters top without spaces): ", Player.ValidNameSize);
        private readonly string r_InvalidInputNameMessage = "Invalid name. ";
        private readonly string r_GetBoardSizeFromPlayerMessage = "Please enter the board size (6 / 8 / 10): ";
        private readonly string r_AnotherRoundMessage = "Do you want another round? please enter y/n ";
        private readonly string r_DrawMessage = "It's a draw!";
        private readonly string r_PlayerCanQuitMessage = string.Format("To quit press {0}", sr_QuitInput);
        private readonly string r_InvalidInputBoardSizeMessage = "Invalid size. ";
        private readonly string r_WinnerDeclarationMessage = "The winner is: ";
        private readonly string r_GetPlayer2TypeFromPlayerMessage = "Please enter '1' for vs. PC or '2' for Two players : ";
        private readonly string r_InvalidInputPlayer2SizeMessage = "Invalid Choice. ";
        private readonly uint r_HumanChoice = 1;
        private readonly uint r_CpuChoice = 2;
        private readonly char r_CapitalLetter = 'A';
        private readonly char r_AnotherRoundInput = 'y';
        private readonly char r_QuitGameInput = 'n';
        private readonly string r_InvalidInputForAnotherRoundInputMessage = "Invalid input. please enter y/n ";
        private readonly char r_SmallLetter = 'a';
        private readonly char r_Space = ' ';
        private readonly char r_RowSeparator = '=';
        private readonly char r_ColSeparator = '|';
        private readonly int r_BoardCellSize = 3;
        private readonly string r_GetInputMoveFromPlayerMessage = "Please enter your move (COLrow>COLrow): ";
        private readonly string r_InvalidInputMoveFromPlayerMessage = "Invalid move. ";

        public void StartGame()
        {
            bool doesPlayer1IntendToQuit = false;
            bool doesPlayer2IntendToQuit = false;
            uint boardSize;
            bool doesUserWantToPlay = true;
            bool isDraw = false;
            Player winner = null;
            Player player1;
            Player player2;
            Board board = null;
            GameManager gameManager = new GameManager();

            initializeGame(out player1, out player2, out boardSize, out board);

            while (doesUserWantToPlay)
            {
                Console.Clear();
                board.InitializeBoard();
                printGameState(board);
                gameManager.ResetGame(player1, player2);
                while (!gameManager.IsGameOver)
                {
                    if (gameManager.CurrentTurn == gameManager.Player1Turn)
                    {
                        playTurn(board, player1, player2, gameManager, out doesPlayer1IntendToQuit);
                    }
                    else
                    {
                        playTurn(board, player2, player1, gameManager, out doesPlayer2IntendToQuit);
                    }

                    gameManager.CheckAndUpdateGameOver(doesPlayer1IntendToQuit, doesPlayer2IntendToQuit, board, player1, player2, out winner, out isDraw);
                    gameManager.ChangeTurn();
                }

                printEndGame(player1, player2, isDraw, winner);
                doesUserWantToPlay = doesUserWantAnotherRound();
            }
        }

        // $G$ CSS-013 (-5) Bad variable name (should be in the form of i_PascalCase).
        private void printEndGame(Player io_Player1, Player io_Player2, bool isDraw, Player io_Winner)
        {
            if (isDraw)
            {
                Console.WriteLine(r_DrawMessage);
            }
            else
            {
                Console.WriteLine("{0}{1}", r_WinnerDeclarationMessage, io_Winner.Name);
            }

            Console.WriteLine("{0}'s score is: {1}, {2}'s score is: {3} ", io_Player1.Name, io_Player1.Score, io_Player2.Name, io_Player2.Score);
        }

        private bool doesUserWantAnotherRound()
        {
            bool doesUserWantAnotherRound = false;
            bool isLegalInputEntered = false;
            string userInput;
            Console.WriteLine(r_AnotherRoundMessage);

            while (isLegalInputEntered == false)
            {
                userInput = Console.ReadLine();
                if (userInput == r_AnotherRoundInput.ToString())
                {
                    doesUserWantAnotherRound = true;
                    isLegalInputEntered = true;
                }
                else if (userInput == r_QuitGameInput.ToString())
                {
                    doesUserWantAnotherRound = false;
                    isLegalInputEntered = true;
                }
                else
                {
                    Console.WriteLine(r_InvalidInputForAnotherRoundInputMessage);
                }
            }

            return doesUserWantAnotherRound;
        }

        private void playTurn(Board io_Board, Player io_ActivePlayer, Player io_PassivePlayer, GameManager io_GameManager, out bool o_DoesPlayerIntendToQuit)
        {
            o_DoesPlayerIntendToQuit = false;
            bool isPlayerHasChainEatingMoves = true;
            Move playerMove = null;
            bool isPawnTypeChanged;

            while (isPlayerHasChainEatingMoves)
            {
                printCurrentHumanTurnMessage(io_ActivePlayer);
                io_ActivePlayer.CheckIfPlayerHasEatingMoves(io_Board);
                playerMove = getMoveFromActivePlayer(io_Board, io_ActivePlayer, io_PassivePlayer, io_GameManager);
                if (playerMove == null)
                {
                    o_DoesPlayerIntendToQuit = true;
                    break;
                }

                io_ActivePlayer.MakeMove(io_Board, playerMove, out isPawnTypeChanged);
                if (!io_ActivePlayer.IsHasChainEatingMove)
                {
                    isPlayerHasChainEatingMoves = false;
                }

                Console.Clear();
                printGameState(io_Board);

                if (io_GameManager.CurrentTurn == io_GameManager.Player1Turn)
                {
                    printLastMoveMessage(io_Board, io_ActivePlayer, playerMove, isPawnTypeChanged);
                }
                else
                {
                    printLastMoveMessage(io_Board, io_ActivePlayer, playerMove, isPawnTypeChanged);
                }
            }

            io_ActivePlayer.IsHasEatingMoves = false;
        }

        // $G$ CSS-013 (-5) Bad variable name (should be in the form of io_PascalCase).
        private void initializeGame(out Player o_Player1, out Player o_Player2, out uint o_BoardSize, out Board o_Board)
        {
            string playerName;
            uint player2Type;

            playerName = getPlayerName();
            o_Player1 = new Player(Player.HumanPlayer1Type, playerName);
            o_BoardSize = getBoardSizeFromPlayer();
            o_Board = new Board(o_BoardSize);
            player2Type = getPlayer2TypeFromPlayer();

            if (player2Type == Player.HumanPlayer2Type)
            {
                playerName = getPlayerName();
                o_Player2 = new Player(Player.HumanPlayer2Type, playerName);
            }
            else
            {
                o_Player2 = new Player(Player.CpuType);
            }
        }

        private string getPlayerName()
        {
            string playerName = null;
            bool isNameValid = false;

            while (!isNameValid)
            {
                Console.Write(r_GetInputNameFromPlayerMessage);
                playerName = Console.ReadLine();

                if (Player.IsInputNameValid(playerName))
                {
                    isNameValid = true;
                }
                else
                {
                    Console.Write(r_InvalidInputNameMessage);
                }
            }

            return playerName;
        }

        private uint getBoardSizeFromPlayer()
        {
            uint boardSize = 0;
            bool isBoardSizeValid = false;

            while (!isBoardSizeValid)
            {
                Console.Write(r_GetBoardSizeFromPlayerMessage);

                if (uint.TryParse(Console.ReadLine(), out boardSize))
                {
                    if (Board.IsInputSizeValid(boardSize))
                    {
                        isBoardSizeValid = true;
                    }
                }

                if (!isBoardSizeValid)
                {
                    Console.Write(r_InvalidInputBoardSizeMessage);
                }
            }

            return boardSize;
        }

        private uint getPlayer2TypeFromPlayer()
        {
            uint player2Type = Player.HumanPlayer2Type;
            uint playerChoice;
            bool isTypeValid = false;

            while (!isTypeValid)
            {
                Console.Write(r_GetPlayer2TypeFromPlayerMessage);

                if (uint.TryParse(Console.ReadLine(), out playerChoice))
                {
                    if (isInputChoiceValid(playerChoice))
                    {
                        player2Type = playerChoice;
                        isTypeValid = true;
                    }
                }

                if (!isTypeValid)
                {
                    Console.Write(r_InvalidInputPlayer2SizeMessage);
                }
            }

            return player2Type;
        }

        private bool isInputChoiceValid(uint i_InputChoice)
        {
            bool isInputChoiceValid = false;

            if (i_InputChoice == r_HumanChoice || i_InputChoice == r_CpuChoice)
            {
                isInputChoiceValid = true;
            }

            return isInputChoiceValid;
        }

        private void printGameState(Board io_Board)
        {
            printColsLetters(io_Board);
            printRowsSeparators(io_Board);
            printInnerBoard(io_Board);
        }

        private void printColsLetters(Board io_Board)
        {
            char colLetter = r_CapitalLetter;

            printSpaces(r_BoardCellSize);
            for (int i = 0; i < io_Board.Size; i++)
            {
                Console.Write(colLetter.ToString());
                colLetter++;

                printSpaces(r_BoardCellSize);
            }

            Console.Write(Environment.NewLine);
        }

        private void printRowsSeparators(Board io_Board)
        {
            Console.Write(r_Space.ToString());
            Console.Write(r_RowSeparator.ToString());
            for (int i = 0; i < io_Board.Size; i++)
            {
                printRowSeparators(r_BoardCellSize);
                Console.Write(r_RowSeparator.ToString());
            }

            Console.Write(Environment.NewLine);
        }

        private void printInnerBoard(Board io_Board)
        {
            char rowLetter = r_SmallLetter;
            Pawn pawnInCell;

            for (int i = 0; i < io_Board.Size; i++)
            {
                Console.Write(rowLetter.ToString());
                Console.Write(r_ColSeparator.ToString());

                for (int j = 0; j < io_Board.Size; j++)
                {
                    pawnInCell = io_Board[i, j];

                    if (pawnInCell == null)
                    {
                        printSpaces(r_BoardCellSize);
                    }
                    else
                    {
                        Console.Write(r_Space.ToString());
                        Console.Write(pawnInCell.Type.ToString());
                        Console.Write(r_Space.ToString());
                    }

                    Console.Write(r_ColSeparator.ToString());
                }

                rowLetter++;
                Console.Write(Environment.NewLine);
                printRowsSeparators(io_Board);
            }
        }

        private void printSpaces(int i_NumOfSpaces)
        {
            for (int i = 0; i < i_NumOfSpaces; i++)
            {
                Console.Write(r_Space.ToString());
            }
        }

        private void printRowSeparators(int i_NumOfSeparators)
        {
            for (int i = 0; i < i_NumOfSeparators; i++)
            {
                Console.Write(r_RowSeparator.ToString());
            }
        }

        private void printCurrentHumanTurnMessage(Player io_Player)
        {
            char pawnType;

            if (io_Player.Type != Player.CpuType)
            {
                if (io_Player.Type == Player.HumanPlayer1Type)
                {
                    pawnType = Pawn.Player1Type;
                }
                else
                {
                    pawnType = Pawn.Player2Type;
                }

                Console.Write(string.Format("{0}'s Turn ({1}):{2}", io_Player.Name, pawnType, Environment.NewLine));
            }
        }

        private Move getMoveFromActivePlayer(Board io_Board, Player io_ActivePlayer, Player io_PassivePlayer, GameManager io_GameManager)
        {
            Move move = null;

            if (io_ActivePlayer.Type != Player.CpuType)
            {
                move = getMoveFromHumanPlayer(io_Board, io_ActivePlayer, io_PassivePlayer, io_GameManager);
            }
            else
            {
                move = io_ActivePlayer.GetRandomMoveForPlayer(io_Board);
            }

            return move;
        }

        private Move getMoveFromHumanPlayer(Board io_Board, Player io_ActivePlayer, Player io_PassivePlayer, GameManager io_GameManager)
        {
            string inputMoveStr;
            Move inputMove = null;
            bool isMoveLegal = false;
            bool isCurrentPlayerInBadSituation = false;

            while (!isMoveLegal)
            {
                Console.Write(r_GetInputMoveFromPlayerMessage);
                isCurrentPlayerInBadSituation = io_GameManager.IsCurrentPlayerInBadSituation(io_Board, io_ActivePlayer, io_PassivePlayer);
                if (isCurrentPlayerInBadSituation)
                {
                    Console.WriteLine(r_PlayerCanQuitMessage);
                }

                inputMoveStr = Console.ReadLine();
                if (inputMoveStr == sr_QuitInput.ToString() && isCurrentPlayerInBadSituation)
                {
                    isMoveLegal = true;
                    inputMove = null;
                }
                else
                {
                    inputMove = new Move(inputMoveStr, io_Board, io_ActivePlayer);

                    if (inputMove.Source != null && inputMove.Destination != null)
                    {
                        isMoveLegal = true;
                    }
                    else
                    {
                        Console.Write(r_InvalidInputMoveFromPlayerMessage);
                    }
                }
            }

            return inputMove;
        }

        private void printLastMoveMessage(Board io_Board, Player io_CurrentPlayer, Move i_PlayerMove, bool i_IsPawnTypeChanged)
        {
            char pawnType = io_Board[i_PlayerMove.Destination.Row, i_PlayerMove.Destination.Col].Type;

            if (i_IsPawnTypeChanged)
            {
                if (io_CurrentPlayer.Type == Player.HumanPlayer1Type)
                {
                    pawnType = Pawn.Player1Type;
                }
                else
                {
                    pawnType = Pawn.Player2Type;
                }
            }

            Console.Write("{0}'s move was ({1}) : {2}>{3}{4}", io_CurrentPlayer.Name, pawnType, i_PlayerMove.Source.ToString(), i_PlayerMove.Destination.ToString(), Environment.NewLine);
        }
    }
}
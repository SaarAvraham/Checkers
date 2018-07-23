namespace B18_Ex02
{
    using System.Collections.Generic;

    public class GameManager
    {
        private readonly uint r_ScoreForKingPawn = 4;
        private readonly uint r_ScoreForRegularPawn = 1;
        private uint m_CurrentTurn = (uint)eTurn.Player1;
        private bool m_IsGameOver = false;

        private enum eTurn
        {
            Player1 = 1,
            Player2
        }

        public uint Player1Turn
        {
            get
            {
                return (uint)eTurn.Player1;
            }
        }

        public bool IsGameOver
        {
            get
            {
                return m_IsGameOver;
            }

            set
            {
                m_IsGameOver = value;
            }
        }

        public uint Player2Turn
        {
            get
            {
                return (uint)eTurn.Player2;
            }
        }

        public uint CurrentTurn
        {
            get
            {
                return m_CurrentTurn;
            }
        }

        public void ResetGame(Player io_Player1, Player io_Player2)
        {
            m_CurrentTurn = (uint)eTurn.Player1;
            m_IsGameOver = false;

            io_Player1.IsHasChainEatingMove = false;
            io_Player1.IsHasEatingMoves = false;
            io_Player2.IsHasChainEatingMove = false;
            io_Player2.IsHasEatingMoves = false;
            io_Player1.LastMoveDestination = null;
            io_Player2.LastMoveDestination = null;
        }

        public void ChangeTurn()
        {
            if (m_CurrentTurn == (uint)eTurn.Player1)
            {
                m_CurrentTurn = (uint)eTurn.Player2;
            }
            else
            {
                m_CurrentTurn = (uint)eTurn.Player1;
            }
        }

        public void CheckAndUpdateGameOver(
            bool doesPlayer1WantToQuit,
            bool doesPlayer2WantToQuit,
            Board io_Board,
            Player io_Player1,
            Player io_Player2,
            out Player o_Winner,
            out bool o_IsDraw)
        {
            List<Move> allPossibleMovesOfPlayer1;
            List<Move> allPossibleMovesOfPlayer2;
            o_IsDraw = false;
            o_Winner = null;

            allPossibleMovesOfPlayer1 = io_Player1.GetAllPossibleMoves(io_Board);
            allPossibleMovesOfPlayer2 = io_Player2.GetAllPossibleMoves(io_Board);

            if (allPossibleMovesOfPlayer1.Count == 0 && allPossibleMovesOfPlayer2.Count == 0)
            {
                o_IsDraw = true;
                m_IsGameOver = true;
            }
            else if (allPossibleMovesOfPlayer1.Count > 0 && (allPossibleMovesOfPlayer2.Count == 0 || doesPlayer2WantToQuit))
            {
                o_Winner = io_Player1;
                m_IsGameOver = true;
                io_Player1.Score += GetCalculatedScore(io_Board, io_Player1, io_Player2);
            }
            else if ((allPossibleMovesOfPlayer1.Count == 0 || doesPlayer1WantToQuit) && allPossibleMovesOfPlayer2.Count > 0)
            {
                o_Winner = io_Player2;
                m_IsGameOver = true;
                io_Player2.Score += GetCalculatedScore(io_Board, io_Player2, io_Player1);
            }
        }

        public int GetCalculatedScore(Board io_Board, Player io_WinnerPlayer, Player io_LoserPlayer)
        {
            int score;
            uint numberOfRegularPawnsOfWinner;
            uint numberOfKingPawnsOfWinner;
            uint numberOfRegularPawnsOfLoser;
            uint numberOfKingPawnsOfLoser;

            io_WinnerPlayer.GetQuantityPawnTypes(io_Board, out numberOfRegularPawnsOfWinner, out numberOfKingPawnsOfWinner);
            io_LoserPlayer.GetQuantityPawnTypes(io_Board, out numberOfRegularPawnsOfLoser, out numberOfKingPawnsOfLoser);
            score = (int)((r_ScoreForRegularPawn * (numberOfRegularPawnsOfWinner - numberOfRegularPawnsOfLoser)) +
                (r_ScoreForKingPawn * (numberOfKingPawnsOfWinner - numberOfKingPawnsOfLoser)));

            return score;
        }

        public bool IsCurrentPlayerInBadSituation(Board io_Board, Player io_CurrentPlayer, Player io_OtherPlayer)
        {
            bool isCurrentPlayerInBadSituation = false;
            int currentPlayerScore;
            int otherPlayerScore;

            currentPlayerScore = GetCalculatedScore(io_Board, io_CurrentPlayer, io_OtherPlayer);
            otherPlayerScore = GetCalculatedScore(io_Board, io_OtherPlayer, io_CurrentPlayer);
            if (currentPlayerScore < otherPlayerScore)
            {
                isCurrentPlayerInBadSituation = true;
            }

            return isCurrentPlayerInBadSituation;
        }
    }
}
namespace B18_Ex02
{
    using System;

    public class Board
    {
        private uint m_Size;
        private Pawn[,] m_BoardState;

        public Board(uint i_Size)
        {
            m_Size = i_Size;
            m_BoardState = new Pawn[m_Size, m_Size];
            InitializeBoard();
        }

        private enum eSize
        {
            Small = 6,
            Medium = 8,
            Large = 10
        }

        public uint Size
        {
            get
            {
                return m_Size;
            }
        }

        public Pawn this[int i_Row, int i_Col]
        {
            get
            {
                return (Pawn)m_BoardState.GetValue(i_Row, i_Col);
            }

            set
            {
                m_BoardState.SetValue(value, i_Row, i_Col);
            }
        }

        public static bool IsInputSizeValid(uint i_BoardSize)
        {
            bool isInputSizeValid = false;

            if (Enum.IsDefined(typeof(eSize), (int)i_BoardSize))
            {
                isInputSizeValid = true;
            }

            return isInputSizeValid;
        }

        public void UpdateBoard(Player io_Player, Move i_Move)
        {
            m_BoardState[i_Move.Destination.Row, i_Move.Destination.Col] = m_BoardState[i_Move.Source.Row, i_Move.Source.Col];
            m_BoardState[i_Move.Source.Row, i_Move.Source.Col] = null;

            if (i_Move.Destination.Row == m_Size - 1 && io_Player.Type == Player.HumanPlayer1Type)
            {
                m_BoardState[i_Move.Destination.Row, i_Move.Destination.Col].Type = Pawn.Player1KingType;
            }
            else if (i_Move.Destination.Row == 0 && (io_Player.Type == Player.HumanPlayer2Type || io_Player.Type == Player.CpuType))
            {
                m_BoardState[i_Move.Destination.Row, i_Move.Destination.Col].Type = Pawn.Player2KingType;
            }

            if (i_Move.IsEatingMove)
            {
                m_BoardState[i_Move.EatenPawn.Row, i_Move.EatenPawn.Col] = null;
            }
        }

        public void InitializeBoard()
        {
            nullifyBoardCells();
            insertPlayer1PawnsIntoBoard();
            insertPlayer2PawnsIntoBoard();
        }

        private void nullifyBoardCells()
        {
            for (int row = 0; row < m_Size; row++)
            {
                for (int col = 0; col < m_Size; col++)
                {
                    m_BoardState[row, col] = null;
                }
            }
        }

        private void insertPlayer1PawnsIntoBoard()
        {
            char player1PawnType = Pawn.Player1Type;
            uint numOfRowsWithPawns = (m_Size / 2) - 1;
            bool isRowWithPawnInFirstCell = true;

            for (int row = 0; row < numOfRowsWithPawns; row++)
            {
                for (int col = 0; col < m_Size; col++)
                {
                    if (isRowWithPawnInFirstCell)
                    {
                        if (col % 2 != 0)
                        {
                            m_BoardState[row, col] = new Pawn(player1PawnType);
                        }
                    }
                    else
                    {
                        if (col % 2 == 0)
                        {
                            m_BoardState[row, col] = new Pawn(player1PawnType);
                        }
                    }
                }

                isRowWithPawnInFirstCell = !isRowWithPawnInFirstCell;
            }
        }

        private void insertPlayer2PawnsIntoBoard()
        {
            char player2PawnType = Pawn.Player2Type;
            int rowToStartFrom = (int)((m_Size / 2) + 1);
            bool isRowWithPawnInFirstCell;

            if (rowToStartFrom % 2 == 0)
            {
                isRowWithPawnInFirstCell = false;
            }
            else
            {
                isRowWithPawnInFirstCell = true;
            }

            for (int row = rowToStartFrom; row < m_Size; row++)
            {
                for (int col = 0; col < m_Size; col++)
                {
                    if (isRowWithPawnInFirstCell)
                    {
                        if (col % 2 == 0)
                        {
                            m_BoardState[row, col] = new Pawn(player2PawnType);
                        }
                    }
                    else
                    {
                        if (col % 2 != 0)
                        {
                            m_BoardState[row, col] = new Pawn(player2PawnType);
                        }
                    }
                }

                isRowWithPawnInFirstCell = !isRowWithPawnInFirstCell;
            }
        }
    }
}
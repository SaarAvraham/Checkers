namespace B18_Ex02
{
    using System;
    using System.Collections.Generic;

    public class Player
    {
        private const string k_PCName = "CPU";
        private static readonly uint sr_ValidNameSize = 20;
        private static readonly char r_Space = ' ';
        private string m_Name;
        private uint m_Type;
        // $G$ DSN-999 (-3) This Collection should be readonly.
        private List<char> m_PawnTypes = new List<char>();
        private bool m_IsHasEatingMoves = false;
        private bool m_IsHasChainEatingMove = false;
        private Point m_LastMoveDestination = null;
        private int m_Score = 0;

        private enum eType
        {
            CPU = 1,
            HumanPlayer2,
            HumanPlayer1
        }

        public Player(uint i_Type, string i_Name = k_PCName)
        {
            m_Name = i_Name;
            m_Type = i_Type;

            if (m_Type == (uint)eType.CPU || m_Type == (uint)eType.HumanPlayer2)
            {
                m_PawnTypes.Add(Pawn.Player2Type);
                m_PawnTypes.Add(Pawn.Player2KingType);
            }
            else
            {
                m_PawnTypes.Add(Pawn.Player1Type);
                m_PawnTypes.Add(Pawn.Player1KingType);
            }
        }

        public static uint HumanPlayer1Type
        {
            get
            {
                return (uint)eType.HumanPlayer1;
            }
        }

        public static uint HumanPlayer2Type
        {
            get
            {
                return (uint)eType.HumanPlayer2;
            }
        }

        public static uint CpuType
        {
            get
            {
                return (uint)eType.CPU;
            }
        }

        public static uint ValidNameSize
        {
            get
            {
                return sr_ValidNameSize;
            }
        }

        public int Score
        {
            get
            {
                return m_Score;
            }

            set
            {
                m_Score = value;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public uint Type
        {
            get
            {
                return m_Type;
            }
        }

        public bool IsHasEatingMoves
        {
            get
            {
                return m_IsHasEatingMoves;
            }

            set
            {
                m_IsHasEatingMoves = value;
            }
        }

        public bool IsHasChainEatingMove
        {
            get
            {
                return m_IsHasChainEatingMove;
            }

            set
            {
                m_IsHasChainEatingMove = value;
            }
        }

        public Point LastMoveDestination
        {
            get
            {
                return m_LastMoveDestination;
            }

            set
            {
                m_LastMoveDestination = value;
            }
        }

        public static bool IsInputNameValid(string i_InputName)
        {
            bool isInputNameValid = false;

            if (isInputNameInLegalSize(i_InputName))
            {
                if (isInputNameWithoutSpaces(i_InputName))
                {
                    isInputNameValid = true;
                }
            }

            return isInputNameValid;
        }

        public bool IsPlayerPawn(Pawn i_Pawn)
        {
            bool isPlayerPawn = false;

            if (i_Pawn != null)
            {
                if (m_PawnTypes.Contains(i_Pawn.Type))
                {
                    isPlayerPawn = true;
                }
            }

            return isPlayerPawn;
        }

        public void CheckIfPlayerHasEatingMoves(Board io_Board)
        {
            List<Move> eatingMoves = getAllPlayerEatingMoves(io_Board);

            if (eatingMoves.Count > 0)
            {
                m_IsHasEatingMoves = true;
            }
            else
            {
                m_IsHasEatingMoves = false;
            }
        }

        public void GetQuantityPawnTypes(Board io_Board, out uint o_NumOfRegularPawns, out uint o_NumOfKingPawns)
        {
            Pawn pawn;
            o_NumOfRegularPawns = 0;
            o_NumOfKingPawns = 0;

            for (int row = 0; row < io_Board.Size; row++)
            {
                for (int col = 0; col < io_Board.Size; col++)
                {
                    pawn = io_Board[row, col];
                    if (pawn != null)
                    {
                        if (pawn.Type == Pawn.Player1KingType && m_Type == HumanPlayer1Type)
                        {
                            o_NumOfKingPawns++;
                        }
                        else if (pawn.Type == Pawn.Player1Type && m_Type == HumanPlayer1Type)
                        {
                            o_NumOfRegularPawns++;
                        }
                        else if (pawn.Type == Pawn.Player2KingType && m_Type != HumanPlayer1Type)
                        {
                            o_NumOfKingPawns++;
                        }
                        else if (pawn.Type == Pawn.Player2Type && m_Type != HumanPlayer1Type)
                        {
                            o_NumOfRegularPawns++;
                        }
                    }
                }
            }
        }

        public void MakeMove(Board io_Board, Move io_PlayerMove, out bool o_IsPawnTypeChanged)
        {
            List<Move> chainEatingMoves;
            char pawnTypeBeforeMove = io_Board[io_PlayerMove.Source.Row, io_PlayerMove.Source.Col].Type;
            char pawnTypeAfterMove;

            io_Board.UpdateBoard(this, io_PlayerMove);
            pawnTypeAfterMove = io_Board[io_PlayerMove.Destination.Row, io_PlayerMove.Destination.Col].Type;

            o_IsPawnTypeChanged = Pawn.IsPawnTypeChanged(pawnTypeBeforeMove, pawnTypeAfterMove);
            if (!o_IsPawnTypeChanged)
            {
                if (m_IsHasEatingMoves)
                {
                    chainEatingMoves = new List<Move>();
                    getAllPlayerEatingMovesFromSource(io_Board, io_PlayerMove.Destination, chainEatingMoves);

                    if (chainEatingMoves.Count > 0)
                    {
                        m_IsHasChainEatingMove = true;
                        m_LastMoveDestination = io_PlayerMove.Destination;
                    }
                    else
                    {
                        m_IsHasChainEatingMove = false;
                        m_LastMoveDestination = null;
                    }
                }
                else
                {
                    m_IsHasChainEatingMove = false;
                    m_LastMoveDestination = null;
                }
            }
            else
            {
                m_IsHasChainEatingMove = false;
                m_LastMoveDestination = null;
            }
        }

        private static bool isInputNameInLegalSize(string i_InputName)
        {
            bool isInputNameInLegalSize = false;
            int inputNameSize = i_InputName.Length;

            if (inputNameSize <= sr_ValidNameSize && inputNameSize > 0)
            {
                isInputNameInLegalSize = true;
            }

            return isInputNameInLegalSize;
        }

        private static bool isInputNameWithoutSpaces(string i_InputName)
        {
            bool isInputNameWithoutSpaces = true;

            foreach (char sign in i_InputName)
            {
                if (sign == r_Space)
                {
                    isInputNameWithoutSpaces = false;
                    break;
                }
            }

            return isInputNameWithoutSpaces;
        }

        private List<Move> getAllPlayerEatingMoves(Board io_Board)
        {
            List<Move> eatingMoves = new List<Move>();
            Point source;

            for (int row = 0; row < io_Board.Size; row++)
            {
                for (int col = 0; col < io_Board.Size; col++)
                {
                    if (IsPlayerPawn(io_Board[row, col]))
                    {
                        source = new Point(row, col);
                        getAllPlayerEatingMovesFromSource(io_Board, source, eatingMoves);
                    }
                }
            }

            return eatingMoves;
        }

        private void getAllPlayerEatingMovesFromSource(Board io_Board, Point i_Source, List<Move> io_EatingMoves)
        {
            Point destination;

            destination = new Point(i_Source.Row + 2, i_Source.Col + 2);
            addLegalMoveToEatingMovesList(io_Board, io_EatingMoves, i_Source, destination);
            destination = new Point(i_Source.Row + 2, i_Source.Col - 2);
            addLegalMoveToEatingMovesList(io_Board, io_EatingMoves, i_Source, destination);
            destination = new Point(i_Source.Row - 2, i_Source.Col - 2);
            addLegalMoveToEatingMovesList(io_Board, io_EatingMoves, i_Source, destination);
            destination = new Point(i_Source.Row - 2, i_Source.Col + 2);
            addLegalMoveToEatingMovesList(io_Board, io_EatingMoves, i_Source, destination);
        }

        private void getAllAdjacentMovesFromSource(Board io_Board, Point i_Source, List<Move> io_AdjacentMoves)
        {
            Point destination;

            destination = new Point(i_Source.Row + 1, i_Source.Col + 1);
            addLegalMoveToAdjacentMovesList(io_Board, io_AdjacentMoves, i_Source, destination);
            destination = new Point(i_Source.Row + 1, i_Source.Col - 1);
            addLegalMoveToAdjacentMovesList(io_Board, io_AdjacentMoves, i_Source, destination);
            destination = new Point(i_Source.Row - 1, i_Source.Col - 1);
            addLegalMoveToAdjacentMovesList(io_Board, io_AdjacentMoves, i_Source, destination);
            destination = new Point(i_Source.Row - 1, i_Source.Col + 1);
            addLegalMoveToAdjacentMovesList(io_Board, io_AdjacentMoves, i_Source, destination);
        }

        private void addLegalMoveToEatingMovesList(Board io_Board, List<Move> io_EatMoves, Point i_Source, Point i_Destination)
        {
            Move move = new Move(i_Source, i_Destination);

            if (move.isMoveLegal(io_Board, this))
            {
                if (move.IsEatingMove)
                {
                    io_EatMoves.Add(move);
                }
            }
        }

        private void addLegalMoveToAdjacentMovesList(Board io_Board, List<Move> io_EatMoves, Point i_Source, Point i_Destination)
        {
            Move move = new Move(i_Source, i_Destination);

            if (move.isMoveLegal(io_Board, this))
            {
                if (!move.IsEatingMove)
                {
                    io_EatMoves.Add(move);
                }
            }
        }

        private List<Move> getAllAdjacentMoves(Board io_Board)
        {
            List<Move> adjacentMoves = new List<Move>();
            Point source;

            for (int row = 0; row < io_Board.Size; row++)
            {
                for (int col = 0; col < io_Board.Size; col++)
                {
                    source = new Point(row, col);
                    getAllAdjacentMovesFromSource(io_Board, source, adjacentMoves);
                }
            }

            return adjacentMoves;
        }

        public List<Move> GetAllPossibleMoves(Board io_Board)
        {
            List<Move> allPossibleMoves = null;

            allPossibleMoves = getAllAdjacentMoves(io_Board);
            allPossibleMoves.AddRange(getAllPlayerEatingMoves(io_Board));

            return allPossibleMoves;
        }

        public Move GetRandomMoveForPlayer(Board io_Board)
        {
            Move moveToMake = null;
            List<Move> eatingChainMoves = new List<Move>();
            List<Move> eatingMoves;
            List<Move> adjacentMoves;

            if (m_IsHasChainEatingMove)
            {
                getAllPlayerEatingMovesFromSource(io_Board, m_LastMoveDestination, eatingChainMoves);

                // There could be multiple choices for continuous eating move.
                if (eatingChainMoves.Count >= 1)
                {
                    moveToMake = chooseRandomMoveFromList(eatingChainMoves);
                }
            }
            else if (m_IsHasEatingMoves)
            {
                eatingMoves = getAllPlayerEatingMoves(io_Board);

                if (eatingMoves.Count >= 1)
                {
                    moveToMake = chooseRandomMoveFromList(eatingMoves);
                }
            }
            else
            {
                adjacentMoves = getAllAdjacentMoves(io_Board);

                if (adjacentMoves.Count >= 1)
                {
                    moveToMake = chooseRandomMoveFromList(adjacentMoves);
                }
            }

            return moveToMake;
        }

        private Move chooseRandomMoveFromList(List<Move> io_Moves)
        {
            Random randomMoveNumberGenerator = new Random();
            int randomMoveResult;

            randomMoveResult = randomMoveNumberGenerator.Next(io_Moves.Count);

            return io_Moves[randomMoveResult];
        }
    }
}
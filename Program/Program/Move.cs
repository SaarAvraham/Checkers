namespace B18_Ex02
{
    public class Move
    {
        private static readonly char sr_InputMoveSign = '>';
        private static readonly uint sr_InputMoveValidLength = 5;
        private Point m_Source;
        private Point m_Destination;
        private bool m_IsEatingMove = false;
        private Point m_EatenPawn = null;

        private enum eDirection
        {
            BottomLeft = 1,
            BottomRight,
            UpperLeft,
            UpperRight
        }

        public Move(string i_InputMove, Board io_Board, Player io_Player)
        {
            if (!isInputMoveValid(i_InputMove, io_Board, io_Player))
            {
                m_Source = null;
                m_Destination = null;
            }
        }

        public Move(Point i_Source, Point i_Destination)
        {
            m_Source = i_Source;
            m_Destination = i_Destination;
        }

        public Point Source
        {
            get
            {
                return m_Source;
            }
        }

        public Point Destination
        {
            get
            {
                return m_Destination;
            }
        }

        public bool IsEatingMove
        {
            get
            {
                return m_IsEatingMove;
            }
        }

        public Point EatenPawn
        {
            get
            {
                return m_EatenPawn;
            }
        }

        private bool isInputMoveValid(string i_InputMove, Board io_Board, Player io_Player)
        {
            string inputMoveSource;
            string inputMoveDestination;
            bool isInputMoveValid = false;

            if (isInputMoveSyntactic(i_InputMove, io_Board.Size))
            {
                inputMoveSource = i_InputMove.Substring(0, 2);
                inputMoveDestination = i_InputMove.Substring(3, 2);

                m_Source = new Point(inputMoveSource);
                m_Destination = new Point(inputMoveDestination);

                if (isMoveLegal(io_Board, io_Player))
                {
                    isInputMoveValid = true;
                }
            }

            return isInputMoveValid;
        }

        private bool isInputMoveSyntactic(string i_inputMove, uint i_BoardSize)
        {
            bool isInputMoveSyntactic = false;

            if (i_inputMove.Length == sr_InputMoveValidLength)
            {
                if (i_inputMove[2] == sr_InputMoveSign)
                {
                    if (isSubInputMoveSyntactic(i_inputMove.Substring(0, 2)) && isSubInputMoveSyntactic(i_inputMove.Substring(3, 2)))
                    {
                        isInputMoveSyntactic = true;
                    }
                }
            }

            return isInputMoveSyntactic;
        }

        private bool isSubInputMoveSyntactic(string i_SubInputMove)
        {
            bool isSubInputMoveSyntactic = false;

            if (char.IsUpper(i_SubInputMove[0]) && char.IsLower(i_SubInputMove[1]))
            {
                isSubInputMoveSyntactic = true;
            }

            return isSubInputMoveSyntactic;
        }

        public bool isMoveLegal(Board io_Board, Player io_Player)
        {
            bool isMoveLegal = false;
            Pawn sourcePawn;
            Pawn destinationPawn;

            if (isMoveInRange(io_Board.Size))
            {
                sourcePawn = io_Board[m_Source.Row, m_Source.Col];
                destinationPawn = io_Board[m_Destination.Row, m_Destination.Col];
                if (io_Player.IsPlayerPawn(sourcePawn) && destinationPawn == null && isMoveInDirection(sourcePawn))
                {
                    if (isEatingMove(sourcePawn, io_Player, io_Board))
                    {
                        if (io_Player.IsHasChainEatingMove)
                        {
                            if (m_Source.Row == io_Player.LastMoveDestination.Row && m_Source.Col == io_Player.LastMoveDestination.Col)
                            {
                                isMoveLegal = true;
                            }
                        }
                        else
                        {
                            isMoveLegal = true;
                        }
                    }
                    else if (isMoveAdjacent(sourcePawn))
                    {
                        if (!io_Player.IsHasEatingMoves)
                        {
                            isMoveLegal = true;
                        }
                    }
                }
            }

            return isMoveLegal;
        }

        private bool isMoveInRange(uint i_BoardSize)
        {
            bool isMoveInRange = false;

            if (m_Source.Row < i_BoardSize && m_Source.Row >= 0)
            {
                if (m_Source.Col < i_BoardSize && m_Source.Col >= 0)
                {
                    if (m_Destination.Row < i_BoardSize && m_Destination.Row >= 0)
                    {
                        if (m_Destination.Col < i_BoardSize && m_Destination.Col >= 0)
                        {
                            isMoveInRange = true;
                        }
                    }
                }
            }

            return isMoveInRange;
        }

        private bool isMoveInDirection(Pawn i_Pawn)
        {
            bool isMoveInDirection = false;

            if (i_Pawn.Type == Pawn.Player1Type)
            {
                if (m_Destination.Row > m_Source.Row)
                {
                    isMoveInDirection = true;
                }
            }
            else if (i_Pawn.Type == Pawn.Player2Type)
            {
                if (m_Destination.Row < m_Source.Row)
                {
                    isMoveInDirection = true;
                }
            }
            else
            {
                isMoveInDirection = true;
            }

            return isMoveInDirection;
        }

        private bool isEatingMove(Pawn i_Pawn, Player io_Player, Board io_Board)
        {
            bool isEatingMove = false;
            Point adjacentPointInDirection;

            if (i_Pawn.Type == Pawn.Player1Type || i_Pawn.Type == Pawn.Player1KingType || i_Pawn.Type == Pawn.Player2KingType)
            {
                if (isEatingMoveInDirectionValid(io_Player, io_Board, eDirection.BottomLeft))
                {
                    adjacentPointInDirection = getPointInDirection(eDirection.BottomLeft, 1);
                    m_EatenPawn = adjacentPointInDirection;
                    isEatingMove = true;
                }
                else if (isEatingMoveInDirectionValid(io_Player, io_Board, eDirection.BottomRight))
                {
                    adjacentPointInDirection = getPointInDirection(eDirection.BottomRight, 1);
                    m_EatenPawn = adjacentPointInDirection;
                    isEatingMove = true;
                }
            }

            if (i_Pawn.Type == Pawn.Player2Type || i_Pawn.Type == Pawn.Player1KingType || i_Pawn.Type == Pawn.Player2KingType)
            {
                if (isEatingMoveInDirectionValid(io_Player, io_Board, eDirection.UpperLeft))
                {
                    adjacentPointInDirection = getPointInDirection(eDirection.UpperLeft, 1);
                    m_EatenPawn = adjacentPointInDirection;
                    isEatingMove = true;
                }
                else if (isEatingMoveInDirectionValid(io_Player, io_Board, eDirection.UpperRight))
                {
                    adjacentPointInDirection = getPointInDirection(eDirection.UpperRight, 1);
                    m_EatenPawn = adjacentPointInDirection;
                    isEatingMove = true;
                }
            }

            m_IsEatingMove = isEatingMove;

            return isEatingMove;
        }

        private bool isEatingMoveInDirectionValid(Player io_Player, Board io_Board, eDirection i_Direction)
        {
            bool isEatingMoveInDirectionValid = false;
            Point eatingPointInDirection = getPointInDirection(i_Direction, 2);
            Point adjacentPointInDirection = getPointInDirection(i_Direction, 1);
            Pawn ediblePawn;

            if (m_Destination.Row == eatingPointInDirection.Row && m_Destination.Col == eatingPointInDirection.Col)
            {
                ediblePawn = io_Board[adjacentPointInDirection.Row, adjacentPointInDirection.Col];

                if (!io_Player.IsPlayerPawn(ediblePawn) && ediblePawn != null)
                {
                    isEatingMoveInDirectionValid = true;
                }
            }

            return isEatingMoveInDirectionValid;
        }

        private Point getPointInDirection(eDirection i_Direction, int i_Distance)
        {
            Point pointInDirection = null;

            if (i_Direction == eDirection.BottomLeft)
            {
                pointInDirection = new Point(m_Source.Row + i_Distance, m_Source.Col - i_Distance);
            }
            else if (i_Direction == eDirection.BottomRight)
            {
                pointInDirection = new Point(m_Source.Row + i_Distance, m_Source.Col + i_Distance);
            }
            else if (i_Direction == eDirection.UpperLeft)
            {
                pointInDirection = new Point(m_Source.Row - i_Distance, m_Source.Col - i_Distance);
            }
            else if (i_Direction == eDirection.UpperRight)
            {
                pointInDirection = new Point(m_Source.Row - i_Distance, m_Source.Col + i_Distance);
            }

            return pointInDirection;
        }

        private bool isMoveAdjacent(Pawn i_Pawn)
        {
            bool isMoveAdjacent = false;

            if (i_Pawn.Type == Pawn.Player1Type || i_Pawn.Type == Pawn.Player1KingType || i_Pawn.Type == Pawn.Player2KingType)
            {
                if (m_Destination.Row == m_Source.Row + 1)
                {
                    if (m_Destination.Col == m_Source.Col - 1 || m_Destination.Col == m_Source.Col + 1)
                    {
                        isMoveAdjacent = true;
                    }
                }
            }

            if (i_Pawn.Type == Pawn.Player2Type || i_Pawn.Type == Pawn.Player1KingType || i_Pawn.Type == Pawn.Player2KingType)
            {
                if (m_Destination.Row == m_Source.Row - 1)
                {
                    if (m_Destination.Col == m_Source.Col - 1 || m_Destination.Col == m_Source.Col + 1)
                    {
                        isMoveAdjacent = true;
                    }
                }
            }

            return isMoveAdjacent;
        }
    }
}
namespace B18_Ex02
{
    public class Pawn
    {
        private char m_Type;

        private enum eType
        {
            Player1 = 'O',
            Player2 = 'X',
            Player1King = 'K',
            Player2King = 'U'
        }

        public Pawn(char i_PawnType)
        {
            m_Type = i_PawnType;
        }

        public static char Player1Type
        {
            get
            {
                return (char)eType.Player1;
            }
        }

        public static char Player2Type
        {
            get
            {
                return (char)eType.Player2;
            }
        }

        public static char Player1KingType
        {
            get
            {
                return (char)eType.Player1King;
            }
        }

        public static char Player2KingType
        {
            get
            {
                return (char)eType.Player2King;
            }
        }

        public char Type
        {
            get
            {
                return m_Type;
            }

            set
            {
                m_Type = value;
            }
        }

        public static bool IsPawnTypeChanged(char i_PawnTypeBeforeMove, char i_PawnTypeAfterMove)
        {
            bool isPawnTypeChanged = false;

            if (i_PawnTypeBeforeMove != i_PawnTypeAfterMove)
            {
                isPawnTypeChanged = true;
            }

            return isPawnTypeChanged;
        }
    }
}
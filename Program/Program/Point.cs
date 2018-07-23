namespace B18_Ex02
{
    using System.Text;

    public class Point
    {
        private readonly char r_FirstColLetter = 'A';
        private readonly char r_FirstRowLetter = 'a';

        private int m_Row;
        private int m_Col;

        public Point(int i_Row, int i_Col)
        {
            m_Row = i_Row;
            m_Col = i_Col;
        }

        public Point(string i_Point)
        {
            m_Col = i_Point[0] - r_FirstColLetter;
            m_Row = i_Point[1] - r_FirstRowLetter;
        }

        public override string ToString()
        {
            StringBuilder resultString = new StringBuilder();
            char column;
            char row;

            column = (char)(m_Col + r_FirstColLetter);
            resultString.Append(column);
            row = (char)(m_Row + r_FirstRowLetter);
            resultString.Append(row);

            return resultString.ToString();
        }

        public int Row
        {
            get
            {
                return m_Row;
            }

            set
            {
                m_Row = value;
            }
        }

        public int Col
        {
            get
            {
                return m_Col;
            }

            set
            {
                m_Col = value;
            }
        }
    }
}

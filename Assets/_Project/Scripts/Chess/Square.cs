using System;

namespace WizardChess.Chess
{
    public readonly struct Square
    {
        private readonly byte _indexPlusOne;
        public static Square Invalid => default;
        public bool IsValid => _indexPlusOne != 0;
        public int Index
        {
            get
            {
                if (!IsValid)
                    throw new InvalidOperationException("invalid square의 index를 읽을 수 없습니다.");
                return _indexPlusOne - 1;
            }
        }
        public int File => Index % 8;
        public int Rank => Index / 8;

        public Square(int file, int rank)
        {
            if (!IsInRange(file))
            {
                throw new System.ArgumentOutOfRangeException(nameof(file));
            }
            
            if (!IsInRange(rank))
            {
                throw new System.ArgumentOutOfRangeException(nameof(rank));
            }
            
            _indexPlusOne = (byte)(rank * 8 + file + 1);
        }
        
        public override string ToString()
        {
            return IsValid? $"{(char)('a' + File)}{Rank + 1}" : "-";
        }

        public static Square FromIndex(int index)
        {
            if (index < 0 || index > 63)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index));
            }
            
            int rank = index / 8;
            int file = index % 8;
            return new Square(file, rank);
        }

        public static bool TryParse(string algebraic, out Square square)
        {
            square = Invalid;

            if (algebraic == null || algebraic.Length != 2)
            {
                return false;
            }
            
            int file = algebraic[0] - 'a';
            int rank = algebraic[1] - '1';

            if (!IsInside(file, rank))
            {
                return false;
            }
            
            square = new Square(file, rank);
            return true;
        }
        
        public static Square FromAlgebraic(string algebraic)
        {
            if (algebraic == null)
            {
                throw new ArgumentNullException(nameof(algebraic));
            }

            Square square;
            if (!TryParse(algebraic, out square))
            {
                throw new ArgumentException($"'{algebraic}'은 유효하지 않은 칸입니다.", nameof(algebraic));
            }
            return square;
        }

        public bool TryOffset(int fileDelta, int rankDelta, out Square result)
        {
            int newFile = File + fileDelta;
            int newRank = Rank + rankDelta;
            
            if (!IsInside(newFile, newRank))
            {
                result = default(Square);
                return false;
            }
            
            result = new Square(newFile, newRank);
            return true;
        }

        private static bool IsInRange(int coordinate)
        {
            return coordinate >= 0 && coordinate <= 7;
        }
        private static bool IsInside(int file, int rank)
        {
            return IsInRange(file) && IsInRange(rank);
        }
    }
}

using System;
using NUnit.Framework;
using WizardChess.Chess;

namespace WizardChess.Tests.EditMode
{
    public class SquareTests
    {
        [Test]
        public void Constructor_E4_HasExpectedIndex()
        {
            // Arrange & Act
            Square square = new Square(4, 3);
            
            // Assert
            Assert.AreEqual(28, square.Index);
        }

        [TestCase(8, 0)]
        [TestCase(-1, 0)]
        public void Constructor_FileOutOfRange_Throws(int file, int rank)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Square(file, rank));
            Assert.AreEqual("file", ex.ParamName);
        }
        
        [TestCase(0, 8)]
        [TestCase(0, -1)]
        public void Constructor_RankOutOfRange_Throws(int file, int rank)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Square(file, rank));
            Assert.AreEqual("rank", ex.ParamName);
        }

        [TestCase("a1", 0)]
        [TestCase("h1", 7)]
        [TestCase("e4", 28)]
        [TestCase("h8", 63)]
        public void FromAlgebraic_ValidText_ReturnsExpectedIndex(string text, int expectedIndex)
        {
            Assert.AreEqual(expectedIndex, Square.FromAlgebraic(text).Index);
        }
        
        [TestCase(0, "a1")]
        [TestCase(7, "h1")]
        [TestCase(28, "e4")]
        [TestCase(63, "h8")]
        public void FromIndex_ValidIndex_ReturnsExpectedSquare(int index, string expected)
        {
            Assert.AreEqual(expected, Square.FromIndex(index).ToString());
        }

        [TestCase(-1)]
        [TestCase(64)]
        public void FromIndex_OutOfRange_Throws(int index)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Square.FromIndex(index));
            Assert.AreEqual("index", ex.ParamName);
        }

        [Test]
        public void Constructor_FileAndRank_AreReadBack()
        {
            Square square = new Square(4, 3);

            Assert.AreEqual(4, square.File);
            Assert.AreEqual(3, square.Rank);
            Assert.IsTrue(square.IsValid);
        }

        [Test]
        public void ToString_E4_ReturnsAlgebraic()
        {
            Assert.AreEqual("e4", new Square(4, 3).ToString());
        }

        [Test]
        public void ToString_Invalid_ReturnsDash()
        {
            Assert.AreEqual("-", Square.Invalid.ToString());
        }

        [Test]
        public void RoundTrip_AllSquares_IndexSurvivesTextConversion()
        {
            // Index -> text -> index must be identity for every square on the board.
            for (int index = 0; index < 64; index++)
            {
                string text = Square.FromIndex(index).ToString();
                Assert.AreEqual(index, Square.FromAlgebraic(text).Index, $"Round trip failed for {text}");
            }
        }

        [TestCase("i4")]
        [TestCase("e9")]
        [TestCase("e0")]
        [TestCase("e")]
        [TestCase("e44")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("E4")]
        public void TryParse_InvalidText_ReturnsFalseAndInvalid(string text)
        {
            Square square;
            bool parsed = Square.TryParse(text, out square);

            Assert.IsFalse(parsed);
            Assert.IsFalse(square.IsValid);
        }

        [Test]
        public void TryParse_ValidText_ReturnsTrueAndSquare()
        {
            Square square;
            bool parsed = Square.TryParse("e4", out square);

            Assert.IsTrue(parsed);
            Assert.AreEqual(28, square.Index);
        }

        [TestCase("i4")]
        [TestCase("")]
        public void FromAlgebraic_InvalidText_ThrowsArgumentException(string text)
        {
            var ex = Assert.Throws<ArgumentException>(() => Square.FromAlgebraic(text));
            Assert.AreEqual("algebraic", ex.ParamName);
        }

        [Test]
        public void FromAlgebraic_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Square.FromAlgebraic(null));
        }

        [TestCase("e4", 1, 2, "f6")]
        [TestCase("e4", -1, -2, "d2")]
        [TestCase("a1", 7, 7, "h8")]
        [TestCase("h8", -7, -7, "a1")]
        public void TryOffset_StaysOnBoard_ReturnsTrueAndTarget(string from, int fileDelta, int rankDelta, string expected)
        {
            Square result;
            bool moved = Square.FromAlgebraic(from).TryOffset(fileDelta, rankDelta, out result);

            Assert.IsTrue(moved);
            Assert.AreEqual(expected, result.ToString());
        }

        [TestCase("h1", 1, 0)]  // would wrap to a2 with naive index + 1
        [TestCase("a1", 0, -1)]
        [TestCase("a1", -1, 0)]
        [TestCase("h8", 0, 1)]
        [TestCase("b1", -2, 1)] // knight jump off the a-file
        public void TryOffset_LeavesBoard_ReturnsFalseAndInvalid(string from, int fileDelta, int rankDelta)
        {
            Square result;
            bool moved = Square.FromAlgebraic(from).TryOffset(fileDelta, rankDelta, out result);

            Assert.IsFalse(moved);
            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void Default_IsInvalid()
        {
            Assert.IsFalse(default(Square).IsValid);
            Assert.IsFalse(Square.Invalid.IsValid);
        }

        [Test]
        public void Invalid_ReadingIndexFileOrRank_Throws()
        {
            Square invalid = Square.Invalid;

            Assert.Throws<InvalidOperationException>(() => { int unused = invalid.Index; });
            Assert.Throws<InvalidOperationException>(() => { int unused = invalid.File; });
            Assert.Throws<InvalidOperationException>(() => { int unused = invalid.Rank; });
        }

        [Test]
        public void Invalid_TryOffset_Throws()
        {
            Square result;
            Assert.Throws<InvalidOperationException>(() => Square.Invalid.TryOffset(1, 0, out result));
        }
    }
}
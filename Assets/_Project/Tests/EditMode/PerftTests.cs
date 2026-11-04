using System.Linq;
using NUnit.Framework;
using UnityChess;

namespace WizardChess.Tests.EditMode
{
    /// <summary>
    /// Perft (performance test) for the vendored chess rules library.
    /// Counts every leaf node reachable at a given depth and compares it with the
    /// well-known reference counts. Any mismatch means a move generation bug.
    /// </summary>
    public class PerftTests
    {
        private const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        // Dense with castling, en passant, promotion and pins.
        private const string KiwipeteFen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
        // En passant that would expose the king along a rank.
        private const string Position3Fen = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1";
        // Promotions, under-promotions and castling out of check.
        private const string Position4Fen = "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1";
        private const string Position5Fen = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";

        [TestCase(StartFen, 1, 20L)]
        [TestCase(StartFen, 2, 400L)]
        [TestCase(StartFen, 3, 8902L)]
        [TestCase(KiwipeteFen, 1, 48L)]
        [TestCase(KiwipeteFen, 2, 2039L)]
        [TestCase(KiwipeteFen, 3, 97862L)]
        [TestCase(Position3Fen, 1, 14L)]
        [TestCase(Position3Fen, 2, 191L)]
        [TestCase(Position3Fen, 3, 2812L)]
        [TestCase(Position3Fen, 4, 43238L)]
        [TestCase(Position4Fen, 1, 6L)]
        [TestCase(Position4Fen, 2, 264L)]
        [TestCase(Position4Fen, 3, 9467L)]
        [TestCase(Position5Fen, 1, 44L)]
        [TestCase(Position5Fen, 2, 1486L)]
        [TestCase(Position5Fen, 3, 62379L)]
        public void Perft_MatchesReferenceCount(string fen, int depth, long expected)
        {
            Game game = new FENSerializer().Deserialize(fen);
            Assert.AreEqual(expected, CountLeafNodes(game, depth));
        }

        private static long CountLeafNodes(Game game, int depth)
        {
            game.LegalMovesTimeline.TryGetCurrent(out var legalMovesByPiece);
            if (legalMovesByPiece == null)
            {
                return 0;
            }

            var moves = legalMovesByPiece.Values.SelectMany(byPiece => byPiece.Values).ToList();
            game.ConditionsTimeline.TryGetCurrent(out GameConditions conditions);
            Side side = conditions.SideToMove;

            // The library stores one PromotionMove per square pair; perft counts each promotion piece separately.
            if (depth == 1)
            {
                return moves.Sum(move => move is PromotionMove ? 4L : 1L);
            }

            long nodes = 0;
            int halfMoveIndex = game.HalfMoveTimeline.HeadIndex;
            foreach (Movement move in moves)
            {
                Piece[] promotionChoices = move is PromotionMove
                    ? new Piece[] { new Queen(side), new Rook(side), new Bishop(side), new Knight(side) }
                    : new Piece[] { null };

                foreach (Piece promotionPiece in promotionChoices)
                {
                    if (move is PromotionMove promotionMove)
                    {
                        promotionMove.SetPromotionPiece(promotionPiece);
                    }

                    Assert.IsTrue(game.TryExecuteMove(move), $"Library rejected its own legal move {move.Start}->{move.End}");
                    nodes += CountLeafNodes(game, depth - 1);
                    game.ResetGameToHalfMoveIndex(halfMoveIndex);
                }
            }

            return nodes;
        }
    }
}

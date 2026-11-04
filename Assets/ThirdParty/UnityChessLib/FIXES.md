# UnityChessLib (vendored)

Source: https://github.com/ErkrodC/UnityChessLib (MIT, `development` branch, last upstream commit 2021-12-28).
Copied from the repository's `src/` folder. Upstream tests were not included.

## Local fixes (found with perft, see `Assets/_Project/Tests/EditMode/PerftTests.cs`)

1. `Pieces/Pawn.cs` - en passant: only a pawn on an adjacent file may capture en passant.
   Upstream allowed any pawn on the 5th/4th rank to capture onto the en passant square.
2. `Base/Rules.cs` - `MoveObeysRules`: simulate en passant moves with their captured pawn removed.
   Upstream ignored the captured pawn, so an en passant that exposes the king was treated as legal.
3. `GameSerialization/GameConditions.cs` - queenside castling rights were derived from the kingside flags
   (copy-paste error), so moving the kingside rook also removed queenside castling.

After these fixes the library matches the standard perft counts
(start position to depth 4, Kiwipete to depth 4, positions 3/4/5 to depth 4-5).
Do not update this folder from upstream without re-running the perft tests.

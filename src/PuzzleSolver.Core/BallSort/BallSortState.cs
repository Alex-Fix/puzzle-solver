using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.BallSort;

public sealed class BallSortState : IState<BallSortState, BallSortMove, BallSortOptions>
{
    private const byte Empty = byte.MaxValue;
    
    private readonly int _flasksCount;
    private readonly int _flaskCapacity;
    private readonly byte[] _layout;
    
    public BallSortState(int flasksCount, int flaskCapacity, byte[][] layout)
    {
        _flasksCount = flasksCount;
        _flaskCapacity = flaskCapacity;
        _layout = new byte[flasksCount * flaskCapacity];

        Span<byte> layoutSpan = _layout.AsSpan();
        layoutSpan.Fill(Empty);

        // Append balls to the end of each flask
        for (int flaskIndex = 0; flaskIndex < flasksCount; ++flaskIndex)
        {
            int ballsCount = layout[flaskIndex].Length;
            int offset = flaskCapacity * flaskIndex + (flaskCapacity - ballsCount);
            layout[flaskIndex].AsSpan().CopyTo(layoutSpan[offset..]);
        }
    }

    private BallSortState(int flasksCount, int flaskCapacity, byte[] layout)
    {
        _flasksCount = flasksCount;
        _flaskCapacity = flaskCapacity;
        _layout = layout;
    }
    
    public IEnumerable<BallSortMove> GetValidMoves()
    {
        for (int fromFlaskIndex = 0; fromFlaskIndex < _flasksCount; ++fromFlaskIndex)
        {
            GetTopBall(_layout, fromFlaskIndex, out _, out byte fromBall, false);
            if (fromBall == Empty)
                continue;
            
            for (int toFlaskIndex = 0; toFlaskIndex < _flasksCount; ++toFlaskIndex)
            {
                // Dont more balls withing the same flask
                if (fromFlaskIndex == toFlaskIndex)
                    continue;

                GetTopBall(_layout, toFlaskIndex, out int toBallIndex, out byte toBall, false);
                
                // Move ball to empty flask or to flask where exist slot and the same ball color below this slot
                if(toBall == Empty || toBallIndex > 0 && fromBall == toBall)
                    yield return new BallSortMove(fromFlaskIndex, toFlaskIndex);
            }
        }
    }

    public BallSortState Apply(BallSortMove move)
    {
        byte[] newLayout = (byte[])_layout.Clone();
        
        // Pop ball
        GetTopBall(newLayout, move.From, out _, out byte ball, true);
        // Push ball
        SetTopBall(newLayout, move.To, ball);        
        
        return new BallSortState(_flasksCount, _flaskCapacity, newLayout);
    }

    public bool IsSolved()
    {
        for (int flaskIndex = 0; flaskIndex < _flasksCount; ++flaskIndex)
        {
            ReadOnlySpan<byte> flask = GetFlask(_layout, flaskIndex);
            byte topBall = flask[0];

            // Flask is not ordered if all balls are not the same color or empty
            foreach(ref readonly byte ball in flask)
                if (topBall != ball)
                    return false;
        }

        return true;
    }
    
    public double GetHeuristic(BallSortOptions options)
    {
        double score = 0;

        for (int flaskIndex = 0; flaskIndex < _flasksCount; ++flaskIndex)
        {
            GetTopBall(_layout, flaskIndex, out int topBallIndex, out byte topBall, false);
            if (topBall == Empty)
                continue;
            
            ReadOnlySpan<byte> flask = GetFlask(_layout, flaskIndex);
            for (; topBallIndex < _flaskCapacity; ++topBallIndex)
            {
                byte ball = flask[topBallIndex];
                if (ball == topBall)
                    continue;
                
                // Add penalty if not all balls are the same for flask
                ++score;
            }
        }

        return score;
    }

    public int GetStateHash()
    {
        var hashCode = new HashCode();
        hashCode.AddBytes(_layout.AsSpan());
        return hashCode.ToHashCode();
    }
 
    internal IEnumerable<BallSortMove> GetSortingMoves()
    {
        byte expectedBall = 0;
        var moves = new List<BallSortMove>();

        for (int fromFlaskIndex = 0; fromFlaskIndex < _flasksCount; ++fromFlaskIndex)
        {
            GetTopBall(_layout, fromFlaskIndex, out _, out byte fromTopBall, false);
            // Flask is already ordered
            if (fromTopBall == expectedBall)
                continue;

            // Move balls to empty flask
            FillSortingMoves(Empty, fromFlaskIndex, false, ref moves);

            // Try to move balls with previous weight to current flask 
            bool flaskFound = FillSortingMoves(expectedBall, fromFlaskIndex, true, ref moves);
            if (flaskFound)
                continue;

            // Try to move balls with next weight to current flask
            ++expectedBall;
            FillSortingMoves(expectedBall, fromFlaskIndex, true, ref moves);
        }

        return moves;
    }
    
    private bool FillSortingMoves(int expectedBall, int fromFlaskIndex, bool reverse, ref List<BallSortMove> moves)
    {
        for (int toFlaskIndex = fromFlaskIndex; toFlaskIndex < _flasksCount; ++toFlaskIndex)
        {
            GetTopBall(_layout, toFlaskIndex, out _, out byte toTopBall, false);
            if (toTopBall != expectedBall)
                continue;

            for (int ballIndex = 0; ballIndex < _flaskCapacity; ++ballIndex)
            {
                GetTopBall(_layout, reverse ? toFlaskIndex : fromFlaskIndex, out _, out byte ball, true);
                SetTopBall(_layout, reverse ? fromFlaskIndex : toFlaskIndex, ball);
                moves.Add(reverse ? new BallSortMove(toFlaskIndex, fromFlaskIndex) : new BallSortMove(fromFlaskIndex, toFlaskIndex));
            }

            return true;
        }

        return false;
    }
    
    private Span<byte> GetFlask(byte[] layout, int flaskIndex)
        => layout.AsSpan(_flaskCapacity * flaskIndex, _flaskCapacity);
    
    private void GetTopBall(byte[] layout, int flaskIndex, out int ballIndex, out byte ball, bool remove)
    {
        Span<byte> flask = GetFlask(layout, flaskIndex);
        for (ballIndex = 0; ballIndex < _flaskCapacity; ++ballIndex)
        {
            ball = flask[ballIndex];

            if (ball == Empty)
                continue;

            if (remove)
                flask[ballIndex] = Empty;

            return;
        }

        ballIndex = -1;
        ball = Empty;
    }

    private void SetTopBall(byte[] layout, int flaskIndex, byte ball)
    {
        Span<byte> flask = GetFlask(layout, flaskIndex);
        for (int ballIndex = _flaskCapacity - 1; ballIndex >= 0; --ballIndex)
        {
            if (flask[ballIndex] != Empty)
                continue;

            flask[ballIndex] = ball;
            return;
        }
    }
}

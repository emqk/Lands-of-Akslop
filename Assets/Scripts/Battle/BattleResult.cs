public struct BattleResult
{
    public bool isFinished;
    public bool isStarted;
    public bool victory;

    public BattleResult(bool isVictory, bool _isStarted, bool _isFinished)
    {
        victory = isVictory;
        isStarted = _isStarted;
        isFinished = _isFinished;
    }

    public void Reset()
    {
        isFinished = false;
        isStarted = false;
        victory = false;
    }
}

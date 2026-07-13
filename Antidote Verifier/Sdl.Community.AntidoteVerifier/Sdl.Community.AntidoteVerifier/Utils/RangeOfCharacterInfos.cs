namespace Sdl.Community.AntidoteVerifier
{
    /// <summary>A range of locked characters inside a segment's collected text.</summary>
    public readonly struct RangeOfCharacterInfos
    {
        public RangeOfCharacterInfos(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public int Start { get; }
        public int Length { get; }

        public int End => Start + Length;
    }
}

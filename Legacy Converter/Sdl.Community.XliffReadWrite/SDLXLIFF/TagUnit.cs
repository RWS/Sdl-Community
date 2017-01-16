namespace Sdl.Community.XliffReadWrite.SDLXLIFF
{

    public class TagUnit
    {
        public enum TagUnitState
        {
            IsOpening = 0,
            IsClosing,
            IsEmpty
        }

        public enum TagUnitType
        {
            IsTag = 0,
            IsPlaceholder,
            IsLockedContent
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public TagUnitState State { get; set; }
        public TagUnitType Type { get; set; }


        public TagUnit(string id, string name, string content, TagUnitState state, TagUnitType type)
        {
            Id = id;
            Name = name;
            Content = content;
            State = state;
            Type = type;
        }
    }
}

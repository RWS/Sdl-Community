namespace Sdl.Community.TM.Database
{
    public abstract class ItemReflection
    {

    }
    public class ItemReflection<T> : ItemReflection where T : class
    {
        public enum State
        {
            created,
            updated,
            deleted
        }
        public T item { get; set; }
        public State itemState { get; set; }


        public ItemReflection(T t, State state)
        {
            item = t;
            itemState = state;
        }
    }
}

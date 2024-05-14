namespace InterpretBank.Helpers
{
    public class ActionResult<T>(bool success, T result, string message) : ActionResult(success, message)
    {
        public T Result { get; set; } = result;

        public void Deconstruct(out bool success, out T result, out string message)
        {
            success = Success;
            result = Result;
            message = Message;
        }
    }

    public class ActionResult(bool success, string message)
    {
        public string Message { get; set; } = message;
        public bool Success { get; set; } = success;

        public void Deconstruct(out bool success, out string message)
        {
            success = Success;
            message = Message;
        }
    }
}
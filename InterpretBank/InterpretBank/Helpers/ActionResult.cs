namespace InterpretBank.Helpers
{
    public class ActionResult<T>
    {
        public ActionResult(bool success, T result, string message)
        {
            Success = success;
            Result = result;
            Message = message;
        }

        public string Message { get; set; }
        public T Result { get; set; }
        public bool Success { get; set; }

        public void Deconstruct(out bool success, out T result, out string message)
        {
            success = Success;
            result = Result;
            message = Message;
        }
    }
}
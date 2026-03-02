using Sdl.ProjectAutomation.Core;
using Task = System.Threading.Tasks.Task;

namespace VerifyFilesAuditReport.BatchTasks
{
    public class Signal
    {
        public static bool Finished { get; set; } = false;
        public static ExecutionMessage Message { get; set; }
        public static bool VerificationRunning { get; set; }

        public static ExecutionMessage GetMessage()
        {
            while (Message is null)
            {
                if (Finished)
                    return null;
                Task.Delay(500);
            }

            var message = Message;
            Message = null;
            return message;
        }

        public static void Reset()
        {
            Finished = false;
            VerificationRunning = false;
        }

        public static void SendMessage(ExecutionMessage argsMessage)
        {
            while (Message is not null)
                Task.Delay(500);
            Message = argsMessage;
        }
    }
}
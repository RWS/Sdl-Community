using System.Collections.Generic;

namespace InterpretBank.Model
{
    public class DatabaseList
    {
        public string LastUsed { get; set; }
        public List<string> List { get; set; } = new();
    }
}
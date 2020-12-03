using System.Collections.Generic;
using System.Linq;
using Iridio.Binding.Model;
using Iridio.Common;

namespace Editor.Wpf
{
    public class ValidationResult
    {
        public IEnumerable<string> Messages { get; }

        public ValidationResult(Script unit)
        {
            Messages = new[] { "Compile operation successful" };
        }

        public ValidationResult(Errors messages)
        {
            Messages = messages.Select(e => e.ErrorKind + ": " + e.AdditionalData);
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace Gemmob.EditorTools {
	public class BuildException : Exception {
		public BuildException() { }

		public BuildException(string message) : base(message) { }

		protected BuildException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		public BuildException(string message, Exception innerException) : base(message, innerException) { }
	}
}
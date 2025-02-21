﻿using System;
using System.Runtime.Serialization;

namespace Trados.Transcreate.Service.ProgressDialog
{
	[Serializable]
	internal class ProgressDialogCancellationExcpetion : Exception
	{
		public ProgressDialogCancellationExcpetion()
			: base()
		{
		}

		public ProgressDialogCancellationExcpetion(string message)
			: base(message)
		{
		}

		public ProgressDialogCancellationExcpetion(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public ProgressDialogCancellationExcpetion(string format, params string[] arg)
			: base(string.Format(format, arg))
		{
		}

		protected ProgressDialogCancellationExcpetion(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

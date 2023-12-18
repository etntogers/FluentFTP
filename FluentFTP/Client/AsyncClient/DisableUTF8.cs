﻿using FluentFTP.Exceptions;

using System.Text;

namespace FluentFTP {
	public partial class AsyncFtpClient {

		/// <summary>
		/// Disables UTF8 support and changes the Encoding property
		/// back to ASCII. If the server returns an error when trying
		/// to turn UTF8 off a FtpCommandException will be thrown.
		/// </summary>
		public void DisableUTF8() {
			FtpReply reply;

			if (!(reply = ((IInternalFtpClient)this).ExecuteInternal("OPTS UTF8 OFF")).Success) {
				throw new FtpCommandException(reply);
			}

			m_textEncoding = Encoding.ASCII;
			m_textEncodingAutoUTF = false;

		}
	}
}

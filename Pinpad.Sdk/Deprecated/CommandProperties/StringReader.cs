﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinPadSDK.Property {
    /// <summary>
    /// Controller for reading a string containing parameters
    /// </summary>
    internal class StringReader {
        /// <summary>
        /// Original string
        /// </summary>
        internal string Value { get; set; }

        /// <summary>
        /// The last read performed
        /// </summary>
        internal string LastReadString { get; private set; }

        /// <summary>
        /// Current parameter offset
        /// </summary>
        internal int Offset { get; set; }

        /// <summary>
        /// Remaining characters in the string
        /// </summary>
        internal int Remaining { get { return this.Value.Length - this.Offset; } }

        /// <summary>
        /// True if the string was fully read
        /// </summary>
        internal bool IsOver { get { return Remaining == 0; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">string to read</param>
        internal StringReader(string value) {
            this.Value = value;
            this.LastReadString = String.Empty;
            this.Offset = 0;
        }

        /// <summary>
        /// Reads a substring
        /// </summary>
        /// <param name="length">length to read</param>
        /// <returns>string</returns>
        internal string ReadString(int length) {
            this.LastReadString = String.Empty;
            string value = this.Value.Substring(this.Offset, length);
            this.Jump(length);
            this.LastReadString = value;
            return value;
        }

        /// <summary>
        /// Reads a substring without changing the offset
        /// </summary>
        /// <param name="length">length to read</param>
        /// <returns>string</returns>
        internal string PeekString(int length) {
            this.LastReadString = String.Empty;
            string value = this.Value.Substring(this.Offset, length);
            this.LastReadString = value;
            return value;
        }

        /// <summary>
        /// Reads a long integer
        /// </summary>
        /// <param name="length">length to read</param>
        /// <returns>long</returns>
        internal long ReadLong(int length) {
            string substring = this.ReadString(length);
            long value = Convert.ToInt64(substring);
            return value;
        }

        /// <summary>
        /// Reads a long integer without changing the offset
        /// </summary>
        /// <param name="length">length to read</param>
        /// <returns>long</returns>
        internal long PeekLong(int length) {
            string substring = this.PeekString(length);
            long value = Convert.ToInt64(substring);
            return value;
        }

        /// <summary>
        /// Reads a integer
        /// </summary>
        /// <param name="length">length to read</param>
        /// <returns>integer</returns>
        internal int ReadInt(int length) {
            string substring = this.ReadString(length);
            int value = Convert.ToInt32(substring);
            return value;
        }

        /// <summary>
        /// Reads a integer without changing the offset
        /// </summary>
        /// <param name="length">length to read</param>
        /// <returns>integer</returns>
        internal int PeekInt(int length) {
            string substring = this.PeekString(length);
            int value = Convert.ToInt32(substring);
            return value;
        }

        /// <summary>
        /// Reads a boolean
        /// </summary>
        /// <returns>boolean</returns>
        internal bool ReadBool() {
            int value = this.ReadInt(1);

            return value != 0;
        }

        /// <summary>
        /// Reads a boolean without changing the offset
        /// </summary>
        /// <returns>boolean</returns>
        internal bool PeekBool() {
            int value = this.PeekInt(1);

            return value != 0;
        }

        /// <summary>
        /// Skips the specified length or return if length is negative
        /// </summary>
        /// <param name="length">length to skip or return if negative</param>
        internal void Jump(int length) {
            this.Offset += length;
        }
    }
}
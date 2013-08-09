#region Usings

using System;
using System.Text;

#endregion

namespace IHI.Server.Network
{
    public interface IInternalOutgoingMessage
    {
        #region Properties

        /// <summary>
        ///   Gets the ID of this message as an unsigned 32 bit integer.
        /// </summary>
        uint Id { get; }

        /// <summary>
        ///   Gets the header of this message, by Base64 encoding the message ID to a 2 byte string.
        /// </summary>
        string Header { get; }

        /// <summary>
        ///   Gets the length of the content in this message.
        /// </summary>
        int ContentLength { get; }

        /// <summary>
        ///   Returns the total content of this message as a string.
        /// </summary>
        /// <returns>String</returns>
        string ContentString { get; }

        #endregion

        #region Methods

        /// <summary>
        ///   Clears all the content in the message and sets the message ID.
        /// </summary>
        /// <param name = "id">The ID of this message as an unsigned 32 bit integer.</param>
        IInternalOutgoingMessage Initialize(uint id);

        /// <summary>
        ///   Clears the message content.
        /// </summary>
        IInternalOutgoingMessage Clear();
        
        /// <summary>
        ///   Appends a single byte to the message content.
        /// </summary>
        /// <param name = "b">The byte to append.</param>
        IInternalOutgoingMessage Append(byte b);

        /// <summary>
        ///   Appends a byte array to the message content.
        /// </summary>
        /// <param name = "bzData">The byte array to append.</param>
        IInternalOutgoingMessage Append(byte[] bzData);

        /// <summary>
        ///   Encodes a string with the environment's default text encoding and appends it to the message content.
        /// </summary>
        /// <param name = "s">The string to append.</param>
        IInternalOutgoingMessage Append(string s);

        /// <summary>
        ///   Encodes a string with a given text encoding and appends it to the message content.
        /// </summary>
        /// <param name = "s">The string to append.</param>
        /// <param name = "encoding">A System.Text.Encoding to use for encoding the string.</param>
        IInternalOutgoingMessage Append(string s, Encoding encoding);

        /// <summary>
        ///   Appends a 32 bit integer in it's string representation to the message content.
        /// </summary>
        /// <param name = "i">The 32 bit integer to append.</param>
        IInternalOutgoingMessage Append(Int32 i);

        /// <summary>
        ///   Appends a 32 bit unsigned integer in it's string representation to the message content.
        /// </summary>
        /// <param name = "i">The 32 bit unsigned integer to append.</param>
        IInternalOutgoingMessage Append(uint i);

        /// <summary>
        ///   Appends a wire encoded boolean to the message content.
        /// </summary>
        /// <param name = "b">The boolean to encode and append.</param>
        IInternalOutgoingMessage AppendBoolean(bool b);

        /// <summary>
        ///   Appends a wire encoded 32 bit integer to the message content.
        /// </summary>
        /// <param name = "i">The 32 bit integer to encode and append.</param>
        IInternalOutgoingMessage AppendInt32(Int32 i);

        /// <summary>
        ///   Appends a wire encoded 32 bit unsigned integer to the message content.
        /// </summary>
        /// <param name = "i">The 32 bit unsigned integer to encode and append.</param>
        /// <seealso>AppendInt32</seealso>
        IInternalOutgoingMessage AppendUInt32(uint i);

        /// <summary>
        ///   Appends a string with the default string breaker byte to the message content.
        /// </summary>
        /// <param name = "s">The string to append.</param>
        IInternalOutgoingMessage AppendString(string s);

        /// <summary>
        ///   Appends a string with a given string breaker byte to the message content.
        /// </summary>
        /// <param name = "s">The string to append.</param>
        /// <param name = "breaker">The byte used to mark the end of the string.</param>
        IInternalOutgoingMessage AppendString(string s, byte breaker);

        byte[] GetBytes();

        #endregion
    }
}
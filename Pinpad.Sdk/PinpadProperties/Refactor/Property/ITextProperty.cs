﻿namespace Pinpad.Sdk.PinpadProperties.Refactor.Property
{
    /// <summary>
    /// A property whose data can only be represented in text.
    /// </summary>
    public interface ITextProperty : IProperty
    {
        // Methods
        /// <summary>
        /// Gets the value of the property as a String, throws UnsetPropertyException if the value is null
        /// </summary>
        /// <returns>Value of the property as string</returns>
        string GetString();
        /// <summary>
        /// Parses a string into the property Value
        /// </summary>
        /// <param name="reader">string reader</param>
        void ParseString(StringReader reader);
    }
}

﻿using PinPadSDK.Enums;
using PinPadSDK.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PinPadSDK.Property {
    /// <summary>
    /// Controller for PinPad command properties
    /// </summary>
    internal class SimpleProperty<type> : IProperty {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="isOptional">Indicates if this property must exist in the command string or not</param>
        /// <param name="stringFormatter">String formatter to use</param>
        /// <param name="stringParser">String parser to use</param>
        /// <param name="defaultStringValue">Default string value for Optional properties with default value</param>
        /// <param name="value">Initial Value for the property</param>
        internal SimpleProperty(string name, bool isOptional = false, Func<type, string> stringFormatter = null, Func<StringReader, type> stringParser = null, string defaultStringValue = null, type value = default(type))
        {
            this.Name = name;
            this.IsOptional = isOptional;
            this.stringFormatter = stringFormatter;
            this.stringParser = stringParser;
            this.DefaultStringValue = defaultStringValue;
            this._Value = value;
        }

        /// <summary>
        /// Gets the value of the property, throws UnsetPropertyException if the value is null
        /// </summary>
        /// <returns>Value of the property</returns>
        internal type GetValue() {
            if (this.HasValue == false && this.IsOptional == false) {
                throw new UnsetPropertyException(this.Name);
            }
            return Value;
        }

        /// <summary>
        /// Gets the value of the property, throws UnsetPropertyException if the value is null
        /// </summary>
        /// <returns>Value of the property</returns>
        internal propertyType GetValueAs<propertyType>() where propertyType : class {
            return this.GetValue() as propertyType;
        }

        /// <summary>
        /// Gets the value of the property as a String, throws UnsetPropertyException if the value is null
        /// </summary>
        /// <returns>Value of the property as string</returns>
        internal virtual string GetString() {
            type obj = this.GetValue();
            if (obj == null || obj.Equals(default(type))) {
                return this.DefaultStringValue;
            }

            string value = this.stringFormatter(obj);
            return value;
        }

        /// <summary>
        /// Parses a string into the property Value
        /// </summary>
        /// <param name="reader">string reader</param>
        internal virtual void ParseString(StringReader reader) {
            if (reader.IsOver == true && this.IsOptional == true) {
                this.Value = default(type);
            }
            else {
                type value = this.stringParser(reader);
                this.Value = value;
            }
        }

        /// <summary>
        /// Indicates if the property is null or not
        /// </summary>
        internal bool HasValue {
            get {
                return this.Value != null && this.Value.Equals(default(type)) == false;
            }
        }

        /// <summary>
        /// Indicates if this property must exist in the command string or not
        /// </summary>
        internal bool IsOptional {
            get;
            private set;
        }

        /// <summary>
        /// Name of this property
        /// </summary>
        internal string Name {
            get;
            private set;
        }

        /// <summary>
        /// Default string value for when the actual value is the default of it's type
        /// </summary>
        protected string DefaultStringValue {
            get;
            set;
        }

        private type _Value {
            get;
            set;
        }

        /// <summary>
        /// Value of the property
        /// </summary>
        internal virtual type Value {
            get {
                return this._Value;
            }
            set {
                type oldValue = this._Value;
                this._Value = value;
                try {
                    GetString();
                }
                catch (Exception ex) {
                    this._Value = oldValue;
                    if (value == null) {
                        throw new InvalidValueException(this.Name + " : Null value was not supported.", ex);
                    }
                    else {
                        throw new InvalidValueException(this.Name + " : Value \"" + value.ToString() + "\" was not supported.", ex);
                    }
                }
            }
        }

        private Func<type, string> stringFormatter;

        private Func<StringReader, type> stringParser;
    }
}

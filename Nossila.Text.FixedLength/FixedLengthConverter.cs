using System;
using System.Linq;

namespace Nossila.Text.FixedLength
{
    public class FixedLengthConverter
    {
        public static void Parse(string model, Action<FixedLengthParser> handler)
        {
            handler(new FixedLengthParser(model));
        }

        public static string Serialize(Action<FixedLengthSerializer> handler)
        {
            var serializer = new FixedLengthSerializer();
            handler(serializer);
            return serializer.Model;
        }
    }

    public enum PadTypes
    {
        Left,
        Right
    }

    public class FixedLengthParser
    {
        #region Properties and fields

        /// <summary>
        /// The untouched serialized model.
        /// </summary>
        public string RawModel { get; }

        /// <summary>
        /// The serialized model.
        /// </summary>
        public string Model { get; private set; }

        /// <summary>
        /// A flag to represent if model has characters left.
        /// </summary>
        public bool HasData => Model.Length > 0;

        /// <summary>
        /// A flag to support automatic text trim during cuts.
        /// </summary>
        public bool AutoTrim { get; set; } = true;

        #endregion

        #region Constructors

        public FixedLengthParser(string model)
        {
            Model = RawModel = model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the string value by cuting model data based on length parameter.
        /// </summary>
        /// <returns>The</returns>
        public string Cut(int length)
        {
            if (!HasData)
                throw new Exception("Data has already been parsed.");

            var value = Model.Substring(0, length);
            Model = Model.Remove(0, length);

            if (AutoTrim)
                value = value.Trim();

            return value;
        }

        /// <summary>
        /// Remove all line breaks from model
        /// </summary>
        public void RemoveLineBreaks()
        {
            Model = Model.Replace("\r\n", string.Empty);
        }

        #endregion
    }

    public class FixedLengthSerializer
    {
        /// <summary>
        /// The serialized model.
        /// </summary>
        public string Model { get; private set; }

        /// <summary>
        /// Validates the model length based on specified length parameter.
        /// </summary>
        /// <param name="modelLength">The expected model length parameter to be used during validation.</param>
        /// <param name="ignoreLineBreaks">The flag to ignore line breaks in characters count during validation.</param>
        public FixedLengthSerializer Validate(int modelLength, bool ignoreLineBreaks = false)
        {
            var offset = 0;

            if (ignoreLineBreaks)
                offset = Model.Split("\r\n".ToArray()).Where(a => !string.IsNullOrWhiteSpace(a)).Count() * 2;

            if (Model.Length != modelLength + offset)
                throw new Exception("Model length is invalid.");

            return this;
        }

        public FixedLengthSerializer Concat(object value, int length, char paddingChar = ' ', PadTypes padType = PadTypes.Right)
        {
            var _value = value is null ? string.Empty : value.ToString();
            return Concat(_value, length, paddingChar, padType);
        }

        public FixedLengthSerializer Concat(decimal value, int length)
        {
            return ConcatNumber(value, length);
        }

        public FixedLengthSerializer Concat(double value, int length)
        {
            return ConcatNumber(value, length);
        }

        public FixedLengthSerializer Concat(long value, int length)
        {
            return ConcatNumber(value, length);
        }

        public FixedLengthSerializer Concat(int value, int length)
        {
            return ConcatNumber(value, length);
        }

        private FixedLengthSerializer ConcatNumber(object value, int length)
        {
            var _value = value.ToString();
            return Concat(_value, length, '0', PadTypes.Left);
        }

        /// <summary>
        /// Concatenates the specified string into model data.
        /// </summary>
        public FixedLengthSerializer Concat(string value, int length, char paddingChar = ' ', PadTypes padType = PadTypes.Right)
        {
            if (value is null)
            {
                Model += string.Empty.PadLeft(length, paddingChar);
                return this;
            }

            var _value = value.ToString().Trim();
            Model += padType == PadTypes.Left ? _value.PadLeft(length, paddingChar) : _value.PadRight(length, paddingChar);
            return this;
        }

        /// <summary>
        /// Adds a new line in model with 2 chars
        /// </summary>
        public void ConcatLineBreak()
        {
            Model += "\r\n";
        }
    }
}

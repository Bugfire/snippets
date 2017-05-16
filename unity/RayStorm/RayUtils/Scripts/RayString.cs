// RayUtils
// https://github.com/Bugfire

namespace RayStorm
{
    /// <summary>
    /// Ray string no dynamic allocation string (except capacity resizing)
    /// </summary>
    sealed public class RayString
    {
        public int Capacity {
            get {
                return _capacity;
            }
        }

        public int Length {
            get {
                return _length;
            }
        }

        public char[] Buffer {
            get {
                return _buffer;
            }
        }

        int _capacity;
        int _length;
        char[] _buffer;

        public RayString (int capacity)
        {
            _capacity = capacity;
            _buffer = new char[capacity];
            _length = 0;
        }

        public RayString Clear ()
        {
            _length = 0;
            return this;
        }

        public RayString ClearSecure ()
        {
            _length = 0;
            System.Array.Clear (_buffer, 0, _capacity);
            return this;
        }

        /// <summary>
        /// Adds the int value.
        /// If length != 0, formatter requires [ minLength + value.ToString ().Length ] work area.
        /// </summary>
        /// <returns>The int.</returns>
        /// <param name="value">Value.</param>
        /// <param name="length">Format length.</param>
        public RayString AddInt (int value, int minLength = 0)
        {
            if (minLength < 0) {
                minLength = 0;
            }
            if (_capacity < _length + minLength + 16) { // MaxWidth = 11 ("-2147483648")
                Resize (minLength + 256);
            }

            if (minLength == 0) {
                _length = IntToCharArray (value, _length, ref _buffer);
            } else {
                var buffer = _buffer;
                var length = _length;

                var workOffset = length + minLength;
                var workTail = IntToCharArray (value, workOffset, ref buffer);
                var outputLength = workTail - workOffset;
                var paddingLength = minLength - outputLength;
                if (paddingLength > 0) {
                    var fillTail = length + paddingLength;
                    while (length < fillTail) {
                        buffer [length++] = ' ';
                    }
                }
                // Array.Copy supports src and dst are the same array.
                // Document says:
                //  - If sourceArray and destinationArray overlap, this method behaves as
                //  - if the original values of sourceArray were preserved in a temporary
                //  - location before destinationArray is overwritten.
                // What is temporary location? 
                var copyTail = length + outputLength;
                while (length < copyTail) {
                    buffer [length++] = buffer [workOffset++];
                }

                _buffer = buffer;
                _length = length;
            }
            return this;
        }

        public RayString AddFloat (float value, int integerPartLength, int fractionalPartLength)
        {
            var maxLength = 32;
            if (fractionalPartLength > 0) {
                maxLength += fractionalPartLength;
            }
            if (_capacity < _length + maxLength) {
                Resize (maxLength + 256);
            }

            _length = FloatToCharArray (value, integerPartLength, fractionalPartLength, _length, ref _buffer);
            return this;
        }

        public RayString AddStr (string value, int minLength = 0)
        {
            if (minLength < 0) {
                minLength = 0;
            }
            var outputLength = value.Length;
            if (minLength < outputLength) {
                minLength = outputLength;
            }
            if (_capacity < _length + minLength) { 
                Resize (minLength + 256);
            }
            if (value == null) {
                value = string.Empty;
            }

            var buffer = _buffer;
            var length = _length;

            for (var i = 0; i < outputLength; ++i) {
                Buffer [length++] = value [i];
            }

            var paddingLength = minLength - outputLength;
            if (paddingLength > 0) {
                var fillTail = length + paddingLength;
                while (length < fillTail) {
                    buffer [length++] = ' ';
                }
            }

            _buffer = buffer;
            _length = length;
            return this;
        }

        static public int IntToCharArray (long value, int bufferIndex, ref char[] buffer)
        {
            if (value < 0) {
                buffer [bufferIndex++] = '-';
                value = -value;
            }
            var t = 1L;
            while (value >= t * 10) {
                t *= 10;
            }
            while (t != 0) {
                var p = value / t;
                buffer [bufferIndex++] = (char)('0' + p);
                value -= p * t;
                t /= 10;
            }
            return bufferIndex;
        }

        // supports very small range of float
        static public int FloatToCharArray (float value, int integerPartLength, int fractionalPartLength, int bufferIndex, ref char[] buffer)
        {
            var t = 1L;
            for (var i = 0; i < fractionalPartLength; ++i) {
                t = t * 10;
            }
            var iv = (long)System.Math.Round (value * t);
            if (iv < 0) {
                buffer [bufferIndex++] = '-';
                iv = -iv;
                integerPartLength--;
            }
            {
                if (integerPartLength < 0) {
                    integerPartLength = 0;
                }
                var intV = iv / t;
                var workOffset = bufferIndex + integerPartLength;
                var workTail = IntToCharArray (intV, workOffset, ref buffer);
                var outputLength = workTail - workOffset;
                var paddingLength = integerPartLength - outputLength;
                if (paddingLength > 0) {
                    var fillTail = bufferIndex + paddingLength;
                    while (bufferIndex < fillTail) {
                        buffer [bufferIndex++] = ' ';
                    }
                }
                var copyTail = bufferIndex + outputLength;
                while (bufferIndex < copyTail) {
                    buffer [bufferIndex++] = buffer [workOffset++];
                }
            }
            if (fractionalPartLength > 0) {
                buffer [bufferIndex++] = '.';

                var fracV = iv % t;
                var workOffset = bufferIndex + fractionalPartLength;
                var workTail = IntToCharArray (fracV, workOffset, ref buffer);
                var outputLength = workTail - workOffset;
                var paddingLength = fractionalPartLength - outputLength;
                if (paddingLength > 0) {
                    var fillTail = bufferIndex + paddingLength;
                    while (bufferIndex < fillTail) {
                        buffer [bufferIndex++] = '0';
                    }
                }
                var copyTail = bufferIndex + outputLength;
                while (bufferIndex < copyTail) {
                    buffer [bufferIndex++] = buffer [workOffset++];
                }
            }
            return bufferIndex;
        }

        public override string ToString ()
        {
            return new string (_buffer, 0, _length);
        }

        void Resize (int size)
        {
            var newBuffer = new char[_capacity + size];
            System.Array.Copy (_buffer, newBuffer, _length);
            _buffer = newBuffer;
        }
    }

}
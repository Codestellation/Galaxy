using System;
using System.IO;

namespace Codestellation.Quarks.IO
{
    internal sealed class ReusableStream : Stream
    {
        private byte[] _internalBuffer;
        private long _position;

        public ReusableStream(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            ChangeBuffer(buffer);
        }

        public void ChangeBuffer(byte[] buffer)
        {
            _internalBuffer = buffer;
            _position = 0;
            //TODO: other cleanup;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return _internalBuffer.LongLength; }
        }

        public override long Position
        {
            get { return _position; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Position must be non-negative value");
                }

                if (value > Length)
                {
                    var message = string.Format("Value must be lower than buffer length {0}", _internalBuffer.Length);
                    throw new ArgumentOutOfRangeException("value", message);
                }
                _position = value;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    SeekInternal(offset);
                    break;
                case SeekOrigin.Current:
                    SeekInternal(_position + offset);
                    break;

                case SeekOrigin.End:
                    SeekInternal(_internalBuffer.Length + offset);
                    break;
                default:
                    throw new ArgumentException("Invalid SeekOrigin", "origin");
            }
            return _position;
        }

        private void SeekInternal(long position)
        {
            if (position < 0)
            {
                throw new IOException("Could not seek before origin");
            }
            if (position >= _internalBuffer.Length)
            {
                throw new IOException("Could not seek beyond stream length");
            }
            _position = position;
        }

        public override int ReadByte()
        {
            if (_position >= _internalBuffer.Length)
            {
                return -1;
            }

            return _internalBuffer[_position++];
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateParameters(buffer, offset, count);

            var maxBytesToRead = _internalBuffer.Length - (int)_position;

            var bytesToRead = maxBytesToRead < count ? maxBytesToRead : count;

            if (bytesToRead <= 0)
            {
                return 0;
            }

            Array.Copy(_internalBuffer, _position, buffer, offset, bytesToRead);

            _position += bytesToRead;

            return bytesToRead;
        }

        public override void WriteByte(byte value)
        {
            if (_position >= _internalBuffer.Length)
            {
                throw new IOException("Could not write beyond buffer length");
            }
            _internalBuffer[_position++] = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateParameters(buffer, offset, count);

            int bytesToWrite = count;

            Array.Copy(buffer, offset, _internalBuffer, _position, count);

            _position += bytesToWrite;
        }

        private static void ValidateParameters(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer", "Buffer cannot be null");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", "Offset should be non negative number");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Offset should be non negative number");
            }
            //if (buffer.Length - offset < count)
            //{
            //    throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
            //}
        }

        public override void Close()
        {
            //Do nothing, really. 
        }

        public override void Flush()
        {
            //Do nothing really
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }
}
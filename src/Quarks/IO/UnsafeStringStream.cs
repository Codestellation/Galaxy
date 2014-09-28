using System;
using System.IO;

namespace Codestellation.Quarks.IO
{
    internal sealed unsafe class UnsafeStringStream : Stream
    {
        private readonly byte* _origin;
        private byte* _current;
        private bool _endOfString; 

        public UnsafeStringStream(byte* origin)
        {
            _origin = origin;
            _current = _origin;
            _endOfString = false;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    return (long) (_current = _origin + offset);
                case SeekOrigin.Current:
                    return (long) (_current + offset);
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            for (int index = offset; index < offset + count; index++)
            {
                int item = ReadByte();

                if (_endOfString) // end of string
                {
                    return index - offset;
                }

                buffer[index] = (byte)item;
            }
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
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
            get { return false; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { return _current - _origin;  }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Value must be non-negative", "value");
                }
                
                _current =  value + _origin;
            }
        }

        public override int ReadByte()
        {
            byte result = *(_current);

            if (result == 0)
            {
                _endOfString = true;
                return -1;
            }

            _current++;
            return result;
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }
    }
}
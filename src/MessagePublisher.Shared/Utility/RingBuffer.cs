using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Shared.Utility
{
    public class RingBuffer<T>
    {
        private int _bufferPos;
        private int _currentIndex;
        private int _size;
        private T[] _buffer;

        public RingBuffer(int size)
        {
            _currentIndex = -1;
            _size = size;
            _buffer = new T[size];
        }

        public void Add(T entry)
        {
            _buffer[_bufferPos] = entry;
            _bufferPos++;
            if(_bufferPos == _size)
            {
                _bufferPos = 0;
            }
            _currentIndex++;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new Exception("Does not allow negative index.");
                }
                if (index > _currentIndex)
                {
                    throw new Exception("Request index larger than the number of entries stored in the buffer.");
                }
                if (index <= _currentIndex - _size)
                {
                    throw new Exception("The requested entry is already overwritten.");
                }
                int pos = GetRelativePosition(index);
                return _buffer[pos];
            }
        }

        public T[] GetBufferStartWith(int index)
        {
            if (index < 0)
            {
                throw new Exception("Does not allow negative index.");
            }
            if (index < _currentIndex - _size)
            {
                throw new Exception("The requested entry is already overwritten.");
            }
            if(index > _currentIndex)
            {
                return new T[0];
            }
            int pos = GetRelativePosition(index);
            T[] output = new T[_currentIndex - index + 1];
            if (pos <= _bufferPos)
            {
                for(int i = pos; i <= _bufferPos; i++)
                {
                    output[i - pos] = _buffer[i];
                }
            }
            else
            {
                for(int i = pos; i < _size; i++)
                {
                    output[i - pos] = _buffer[i];
                }
                for(int i = 0; i <= _bufferPos; i++)
                {
                    output[i + _size - pos] = _buffer[i];
                }
            }
            return output;
        }

        private int GetRelativePosition(int index)
        {
            int pos = _currentIndex - index + _bufferPos;
            if (pos >= _size)
            {
                pos -= _size;
            }
            return pos;
        }
    }
}

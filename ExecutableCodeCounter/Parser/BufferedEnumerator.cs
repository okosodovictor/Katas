using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ExecutableCodeCounter.Parser
{
    public class BufferedEnumerator<T> : IEnumerator<T>
    {
        private readonly List<T> _buffer = new List<T>();

        private readonly IEnumerator<T> _source;

        /// <summary>
        /// Represents the current position within the enumerator
        /// </summary>
        public int Cursor { get; private set; }

        /// <summary>
        /// Current value pointed to in the enumerator
        /// </summary>
        public T Current => Cursor < 0 || Cursor >= _buffer.Count
            ? default(T) :
            _buffer[Cursor];

        /// <summary>
        /// Current value pointed to in the enumerator
        /// </summary>
        object IEnumerator.Current => Current;

        /// <summary>
        /// Create a new enumerator
        /// </summary>
        /// <param name="enumerator"></param>
        public BufferedEnumerator(IEnumerator<T> enumerator)
        {
            _source = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            Cursor = -1;
        }

        /// <summary>
        /// Dispose the enumerator
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
                _source.Dispose();
        }

        /// <summary>
        /// Move to the next value (if available) and return true: return false otherwise
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (Cursor < _buffer.Count)
                Cursor++;

            if (Cursor == _buffer.Count)
            {
                if (!_source.MoveNext())
                    return false;

                _buffer.Add(_source.Current);
            }

            return true;
        }

        /// <summary>
        /// Try to move to, and return the next value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryNext(out T value)
        {
            if (MoveNext())
            {
                value = Current;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Reset the enumerator to its original position (Cusor = -1)
        /// </summary>
        /// <returns></returns>
        public BufferedEnumerator<T> Reset() => Reset(-1);

        /// <summary>
        /// Reset the cusor to the given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public BufferedEnumerator<T> Reset(int position)
        {
            if (position > Cursor || position < -1)
                throw new Exception("Invalid reset position");

            else
                Cursor = position;

            return this;
        }

        /// <summary>
        /// Reset the cursor one step behind its current position unless it is already at the begining.
        /// </summary>
        /// <returns>The current buffer</returns>
        public BufferedEnumerator<T> Back()
        {
            if (Cursor > -1)
                return Reset(Cursor - 1);

            return this;
        }

        public BufferedEnumerator<T> Back(uint steps)
        {
            while (Cursor > -1 && steps > 0)
            {
                Back();
                steps--;
            }

            return this;
        }

        /// <summary>
        /// Reset the enumerator to its original position (Cusor = -1)
        /// </summary>
        void IEnumerator.Reset() => Reset();
    }
}

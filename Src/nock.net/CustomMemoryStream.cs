using System;
using System.IO;

namespace Nock.net
{
    internal class CustomMemoryStream : MemoryStream
    {
        private readonly Action<string> _setBodyAction;
        private bool _closed;

        public CustomMemoryStream(Action<string> setBodyAction)
        {
            _setBodyAction = setBodyAction;
        }

        public override void Close()
        {
            if (_closed)
                return;

            Position = 0;

            var reader = new StreamReader(this, true);
            var body = reader.ReadToEnd();
            _setBodyAction(body);

            base.Close();
            _closed = true;
        }
    }
}

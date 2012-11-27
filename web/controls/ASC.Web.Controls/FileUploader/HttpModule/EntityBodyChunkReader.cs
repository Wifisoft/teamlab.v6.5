using System.Text;

namespace ASC.Web.Controls.FileUploader.HttpModule
{
    internal class EntityBodyChunkReader {

        private int index;
        private int size;
        internal char Current;
        private byte[] buffer;

        private Encoding encoding = System.Text.Encoding.UTF8;

        internal EntityBodyChunkReader(byte[] buffer, int offset) {
            index = offset - 1;
            this.buffer = buffer;
            size = buffer.Length;
            Current = '0';
        }

        internal bool MoveTo(int pos) {
            if (size == 0 || (pos >= size && pos != 0))
                return false;

            index = pos;
            Current = encoding.GetChars(buffer, index, 1)[0];
            return true;
        }

        internal bool Read() {
            return MoveTo(++index);
        }

        public int Index {
            get { return index; }
        }
    }
}
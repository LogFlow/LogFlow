using System.IO;

namespace LogFlow.Builtins.Components
{
	internal class SplitIntoLines
    {
        /// <summary>
        /// Reads a line of characters from the current stream at the current position and returns the data as a string.
        /// </summary>
        /// <returns>The next line from the input stream, or null if the end of the input stream is reached</returns>
        public string ReadLine(StreamReader stream)
        {
            if (stream.BaseStream.Position == stream.BaseStream.Length)
                return null;

            string line = "";
            int nextChar = stream.Read();
            while (nextChar != -1)
            {
                char current = (char)nextChar;
                if (current.Equals('\n'))
                    break;
                else if (current.Equals('\r'))
                {
                    int pickChar = stream.Peek();
                    if (pickChar != -1 && ((char)pickChar).Equals('\n'))
                        nextChar = stream.Read();
                    break;
                }
                else
                    line += current;
                nextChar = stream.Read();
            }
            return line;
        }
    }
}

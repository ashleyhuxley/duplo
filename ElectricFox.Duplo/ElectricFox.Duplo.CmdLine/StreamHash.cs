﻿using System.Security.Cryptography;
using ElectricFox.Duplo.CmdLine.Extensions;

namespace ElectricFox.Duplo.CmdLine
{
    public class StreamHash
    {
        public string ComputeHash(Stream input)
        {
            input.Seek(0, SeekOrigin.Begin);

            byte[] bytes = new byte[input.Length];

            int numBytesToRead = (int)input.Length;
            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = input.Read(bytes, numBytesRead, numBytesToRead);

                // The end of the file is reached.
                if (n == 0)
                {
                    break;
                }

                numBytesRead += n;
                numBytesToRead -= n;
            }

            // Start at 0, hash bytesToHash elements
            var md5 = MD5.Create();

            var result = md5.ComputeHash(bytes, 0, numBytesRead);
            return result.ToHex(true);
        }
    }

}

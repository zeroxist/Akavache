﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Akavache
{
    public static class Encryption
    {
        public static Task<byte[]> EncryptBlock(byte[] block)
        {
            var tcs = new TaskCompletionSource<byte[]>();
            try {
#if SILVERLIGHT
                tcs.TrySetResult(ProtectedData.Protect(block, null));
#else
                tcs.TrySetResult(ProtectedData.Protect(block, null, DataProtectionScope.CurrentUser));
#endif
            } catch (Exception ex) {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

        public static Task<byte[]> DecryptBlock(byte[] block)
        {
            var tcs = new TaskCompletionSource<byte[]>();
            try {
#if SILVERLIGHT
                tcs.TrySetResult(ProtectedData.Unprotect(block, null));
#else
                tcs.TrySetResult(ProtectedData.Unprotect(block, null, DataProtectionScope.CurrentUser));
#endif
            } catch (Exception ex) {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }
    }
}
//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************




using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Diagnostics;
using System.Security.Cryptography;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Text;

namespace CryptoUtility
{
    [ComVisible(false)]
    internal abstract class SymmetricCryptographer
    {
        static SymmetricCryptographer()
        {
            Initiaize();
        }

        private static RNGCryptoServiceProvider _rng = null;
        private static SymmetricAlgorithm _alg = null;
        private static Boolean _hasBeenInitialized = false;

        private const int IV_SIZE = 16;
        private const int KEY_SIZE_BYTES = 32;

        internal static void Initiaize(){
            if (_hasBeenInitialized) {
                return;
            }

            _hasBeenInitialized = false;

            _rng = new RNGCryptoServiceProvider();

            _alg = RijndaelManaged.Create();

            _alg.KeySize = KEY_SIZE_BYTES * 8;

            AppDomain.CurrentDomain.DomainUnload += new System.EventHandler(SymmetricCryptographer.Unload);
            AppDomain.CurrentDomain.ProcessExit += new System.EventHandler(SymmetricCryptographer.Unload);

            _hasBeenInitialized = true;
        }

        internal static string EncryptString(string clenrString, byte[] key){
            if (!_hasBeenInitialized) {
                throw new ApplicationException("Symmetric cryptographer has not been initialized.");
            }

            byte[] clearText = Encoding.Unicode.GetBytes(clenrString);
            byte[] data = null;
            byte[] output = null;
            byte[] newIV = null;

            //newIV = new byte(IV_SIZE - 1);
            lock(_rng){
                _rng.GetBytes(newIV);
            }
            ICryptoTransform trans = _alg.CreateEncryptor(key, newIV);
            try
            {
                MemoryStream ms = new MemoryStream();
                try
                {
                    CryptoStream cs = new CryptoStream(ms, trans, CryptoStreamMode.Write);
                    try
                    {
                        cs.Write(clearText, 0, clearText.Length);
                        cs.FlushFinalBlock();
                        data = ms.ToArray();
                    }
                    finally
                    {
                        cs.Close();
                    }
                }
                finally
                {
                    ms.Close();
                }
            }
            finally {
                trans.Dispose();
            }

            output = new byte[IV_SIZE + data.Length - 1];
            Buffer.BlockCopy(newIV, 0, output, 0, newIV.Length);
            Buffer.BlockCopy(data, 0, output, IV_SIZE, data.Length);

            return Convert.ToBase64String(output);
        }

        internal static string DecryptString(string cipherString, byte[] key) {
            if (!_hasBeenInitialized) { 
                throw new ApplicationException("Symmetric cryptographer has not been initialized.");
            }
            byte[] cipherBlob = Convert.FromBase64String(cipherString);
            byte[] cipherText = new byte[cipherBlob.Length - IV_SIZE];
            byte[] data = null;
            byte[] iv = new byte[IV_SIZE];

            int blockSize = _alg.BlockSize / 8;

            if (cipherBlob.Length < blockSize) {
                throw new ArgumentException("Data length must be greater than block size.");
            }
            Buffer.BlockCopy(cipherBlob, 0, iv, 0, IV_SIZE);
            Buffer.BlockCopy(cipherBlob, IV_SIZE, cipherText, 0, cipherBlob.Length - IV_SIZE);

            ICryptoTransform trans = _alg.CreateDecryptor(key, iv);
            try
            {
                MemoryStream ms = new MemoryStream();
                try
                {
                    CryptoStream cs = new CryptoStream(ms, trans, CryptoStreamMode.Write);
                    try
                    {
                        cs.Write(cipherText, 0, cipherText.Length);
                        cs.FlushFinalBlock();
                        data = ms.ToArray();
                    }
                    finally {
                        cs.Close();
                    }
                }
                finally {
                    ms.Close();
                }
            }
            finally {
                trans.Dispose();
            }

            return Encoding.Unicode.GetString(data);
        }

        
        internal static void Unload(object sender, EventArgs e) {
            if (!_hasBeenInitialized) {
                return;
            }
            ((IDisposable)_alg).Dispose();

            _hasBeenInitialized = false;
            
        }

    }
}
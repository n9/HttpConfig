using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HttpConfig
{
    public class CertUtil
    {
        private CertUtil() {}

        #region Methods
        [DllImport("Cryptui.dll", SetLastError=true, CharSet=CharSet.Unicode, EntryPoint="CryptUIDlgSelectCertificateW")]
        public unsafe static extern IntPtr CryptUIDlgSelectCertificate(
            [In] IntPtr pcsc); // PCCRYPTUI_SELECTCERTIFICATE_STRUCT

        [DllImport("Cryptui.dll", SetLastError=true, CharSet=CharSet.Unicode)]
        public static extern bool CryptUIDlgViewContext(
            ContextType  dwContextType,
            IntPtr       pvContext,
            IntPtr       hwnd,
            string       pwszTitle,
            int          dwFlags,
            IntPtr       pvReserved);

        [DllImport("Crypt32.dll")]
        public static extern bool CertCloseStore(
            IntPtr hCertStore,
            int    dwFlags);

        [DllImport("Crypt32.dll", SetLastError=true)]
        public static extern bool CertFreeCertificateContext(
            IntPtr pCertContext);

        [DllImport("Crypt32.dll", SetLastError=true)]
        public static extern bool CertGetCertificateContextProperty(
            IntPtr pCertContext,
            int dwPropId,
            IntPtr pvData,
            ref int pcbData);

        [DllImport("Crypt32.dll", CharSet=CharSet.Ansi)]	
        public static extern IntPtr CertOpenStore(
            IntPtr lpszStoreProvider,
            int dwMsgAndCertEncodingType,
            int hCryptProv,
            int dwFlags,
            string pvPara);

        [DllImport("Crypt32.dll", SetLastError=true, EntryPoint="CertGetNameStringW")]
        public static extern int CertGetNameString(
            IntPtr        pCertContext,
            CertNameType  dwType,
            int           dwFlags,
            IntPtr        pvTypePara,
            IntPtr        pszNameString,
            int           cchNameString);

        [DllImport("Crypt32.dll", SetLastError=true)]
        public static extern IntPtr CertFindCertificateInStore(
            IntPtr         hCertStore,
            int            dwCertEncodingType,
            int            dwFindFlags,
            int            dwFindType,
            IntPtr         pvFindPara,
            IntPtr         pPrevCertContext);

        [DllImport("Kernel32.dll", EntryPoint="RtlZeroMemory")]
        public static extern void ZeroMemory(IntPtr dest, int size);
        #endregion Methods

/*
DWORD dwSize;
HWND hwndParent;
DWORD dwFlags;
LPCTSTR szTitle;
DWORD dwDontUseColumn;
LPCTSTR szDisplayString;
PFNCFILTERPROC pFilterCallback;
PFNCCERTDISPLAYPROC pDisplayCallback;
void* pvCallbackData;
DWORD cDisplayStores;
HCERTSTORE* rghDisplayStores;
DWORD cStores;
HCERTSTORE* rghStores;
DWORD cPropSheetPages;
LPCPROPSHEETPAGE rgPropSheetPages;
HCERTSTORE hSelectedCertStore;
*/
        #region Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPTUI_SELECTCERTIFICATE_STRUCT 
        {
            public uint   dwSize;
            public IntPtr hwndParent;
            public uint   dwFlags;

            [MarshalAs(UnmanagedType.LPWStr)]
			public string szTitle;

            public uint dwDontUseColumn;

            [MarshalAs(UnmanagedType.LPWStr)]
			public string szDisplayString;

            public IntPtr pFilterCallback; // PFNCFILTERPROC
            public IntPtr pDisplayCallback; // PFNCCERTDISPLAYPROC
            public IntPtr pvCallbackData; // void*
            public uint   cDisplayStores;
            public IntPtr rghDisplayStores; // HCERTSTORE*
            public uint   cStores;
            public IntPtr rghStores; //HCERTSTORE*
            public uint   cPropSheetPages;
            public IntPtr rgPropSheetPages; // LPCPROPSHEETPAGE
            public IntPtr hSelectedCertStore; // HCERTSTORE
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPTOAPI_BLOB 
        {
            public int cbData;
            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CERT_CONTEXT 
        {
            public int       dwCertEncodingType;
            public IntPtr    pbCertEncoded;
            public int       cbCertEncoded;
            public IntPtr    pCertInfo;         //PCERT_INFO
            public IntPtr    hCertStore;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CERT_INFO 
        {
            public int                         dwVersion;
            public CRYPTOAPI_BLOB              SerialNumber;
            public CRYPT_ALGORITHM_IDENTIFIER  SignatureAlgorithm;
            public CRYPTOAPI_BLOB              Issuer;
            public FILETIME                    NotBefore;
            public FILETIME                    NotAfter;
            public CRYPTOAPI_BLOB              Subject;
            public CERT_PUBLIC_KEY_INFO        SubjectPublicKeyInfo;
            public CRYPTOAPI_BLOB              IssuerUniqueId;
            public CRYPTOAPI_BLOB              SubjectUniqueId;
            public int                         cExtension;
            public IntPtr                      rgExtension; // PCERT_EXTENSION
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPT_ALGORITHM_IDENTIFIER
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pszObjId;
            public CRYPTOAPI_BLOB Parameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CERT_PUBLIC_KEY_INFO 
        {
            public CRYPT_ALGORITHM_IDENTIFIER  Algorithm;
            public CRYPTOAPI_BLOB              PublicKey;
        }
        #endregion Structures

        #region Enumerations
        public enum ContextType : int
        {
            Certificate = 1,
            Crl         = 2,
            Ctl         = 3
        }

        public enum CertStoreLocation : int
        {
            CurrentUser = 0x00010000,
            LocalMachine = 0x00020000
        }

        public enum CryptEncoding : int
        {
            CRYPT_ASN_ENCODING  = 0x00000001,
            CRYPT_NDR_ENCODING  = 0x00000002,
            X509_ASN_ENCODING   = 0x00000001,
            X509_NDR_ENCODING   = 0x00000002,
            PKCS_7_ASN_ENCODING = 0x00010000,
            PKCS_7_NDR_ENCODING = 0x00020000
        }

        public enum CertNameType : int
        {
            CERT_NAME_EMAIL_TYPE            = 1,
            CERT_NAME_RDN_TYPE              = 2,
            CERT_NAME_ATTR_TYPE             = 3,
            CERT_NAME_SIMPLE_DISPLAY_TYPE   = 4,
            CERT_NAME_FRIENDLY_DISPLAY_TYPE = 5,
            CERT_NAME_DNS_TYPE              = 6,
            CERT_NAME_URL_TYPE              = 7,
            CERT_NAME_UPN_TYPE              = 8
        }
        #endregion Enumerations

        public static readonly IntPtr X509_UNICODE_NAME        = new IntPtr(20);
        public static readonly int    CERT_SHA1_HASH_PROP_ID   = 3;
        public static readonly IntPtr CERT_STORE_PROV_SYSTEM_A = new IntPtr(9);
		public static readonly IntPtr CERT_STORE_PROV_SYSTEM_W = new IntPtr(10);
        public static readonly int    CERT_FIND_HASH           = 0x00010000;
        public static readonly string UNKNOWN_CERT_NAME        = "<UNKNOWN>";

        public static byte[] GetCertHash(IntPtr pCert)
        {
            IntPtr pHash      = IntPtr.Zero;
            int    hashLength = 0;

            try
            {
                if(!CertGetCertificateContextProperty(pCert, CERT_SHA1_HASH_PROP_ID, pHash, ref hashLength))
                    throw new Exception("CertGetCertificateContextProperty failed.  Error = " + Marshal.GetLastWin32Error());

                pHash = Marshal.AllocHGlobal(hashLength);

                if(!CertGetCertificateContextProperty(pCert, CERT_SHA1_HASH_PROP_ID, pHash, ref hashLength))
                    throw new Exception("CertGetCertificateContextProperty failed.  Error = " + Marshal.GetLastWin32Error());

                byte[] hash = new byte[hashLength];

                Marshal.Copy(pHash, hash, 0, hashLength);

                return hash;
            }
            finally
            {
                if(pHash != IntPtr.Zero)
                    Marshal.FreeHGlobal(pHash);
            }
        }

        public static string BytesToHex(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);

            foreach(byte b in bytes)
                hex.AppendFormat("{0:X2}", b);

            return hex.ToString();
        }

        public static string GetCertNameAttribute(IntPtr pCert, CertNameType nameType)
        {
            IntPtr pName  = IntPtr.Zero;
            int    length = 0;

            length = CertGetNameString(pCert, nameType, 0, IntPtr.Zero, pName, length);            
            if(length == 1)
                return "";

            pName = Marshal.AllocHGlobal(length * 2);

            length = CertGetNameString(pCert, nameType, 0, IntPtr.Zero, pName, length);

            string name = Marshal.PtrToStringUni(pName, length - 1);

            Marshal.FreeHGlobal(pName);

            return name;
        }

        public static string GetCertNameFromStoreAndHash(string storeName, byte[] certHash)
        {
            IntPtr hCertStore = IntPtr.Zero;
            IntPtr hCert      = IntPtr.Zero;
            IntPtr pBlob      = IntPtr.Zero;
            IntPtr pHash      = IntPtr.Zero;

            string name = UNKNOWN_CERT_NAME;

            try
            {
                hCertStore = CertOpenStore(CERT_STORE_PROV_SYSTEM_A, 0, 0, (int)CertStoreLocation.LocalMachine, storeName);
                if(hCertStore == IntPtr.Zero)
                    return name;

                pHash = Marshal.AllocHGlobal(certHash.Length);

                Marshal.Copy(certHash, 0, pHash, certHash.Length);

                CRYPTOAPI_BLOB hashBlob = new CRYPTOAPI_BLOB();

                hashBlob.cbData = certHash.Length;
                hashBlob.pbData = pHash;

                pBlob = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CRYPTOAPI_BLOB)));

                Marshal.StructureToPtr(hashBlob, pBlob, false);

                hCert = CertFindCertificateInStore(hCertStore, 0, 0, CERT_FIND_HASH, pBlob, IntPtr.Zero);
                if(hCert == IntPtr.Zero)
                    return name;

                name = GetCertNameAttribute(hCert, CertNameType.CERT_NAME_FRIENDLY_DISPLAY_TYPE);

                return name;
            }
            finally
            {
                if(pBlob != IntPtr.Zero)
                    Marshal.FreeHGlobal(pBlob);

                if(pHash != IntPtr.Zero)
                    Marshal.FreeHGlobal(pHash);

                if(hCert != IntPtr.Zero)
                    CertFreeCertificateContext(hCert);

                if(hCertStore != IntPtr.Zero)
                    CertCloseStore(hCertStore, 0);
            }
        }

        public static void ViewCertificate(string storeName, byte[] certHash, IntPtr hwnd)
        {
            IntPtr hCertStore = IntPtr.Zero;
            IntPtr hCert      = IntPtr.Zero;
            IntPtr pBlob      = IntPtr.Zero;
            IntPtr pHash      = IntPtr.Zero;

            try
            {
                hCertStore = CertOpenStore(CERT_STORE_PROV_SYSTEM_A, 0, 0, (int)CertStoreLocation.LocalMachine, storeName);
                if(hCertStore == IntPtr.Zero)
                    throw new Exception("CertOpenStore failed.  Error = " + Marshal.GetLastWin32Error().ToString());

                pHash = Marshal.AllocHGlobal(certHash.Length);

                Marshal.Copy(certHash, 0, pHash, certHash.Length);

                CRYPTOAPI_BLOB hashBlob = new CRYPTOAPI_BLOB();

                hashBlob.cbData = certHash.Length;
                hashBlob.pbData = pHash;

                pBlob = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CRYPTOAPI_BLOB)));

                Marshal.StructureToPtr(hashBlob, pBlob, false);

                hCert = CertFindCertificateInStore(hCertStore, 0, 0, CERT_FIND_HASH, pBlob, IntPtr.Zero);
                if(hCert == IntPtr.Zero)
                    throw new Exception("CertFindCertificateInStore failed.  Error = " + Marshal.GetLastWin32Error().ToString());

                string name = GetCertNameAttribute(hCert, CertNameType.CERT_NAME_FRIENDLY_DISPLAY_TYPE);

                CryptUIDlgViewContext(ContextType.Certificate, hCert, hwnd, name, 0, IntPtr.Zero);
            }
            finally
            {
                if(pBlob != IntPtr.Zero)
                    Marshal.FreeHGlobal(pBlob);

                if(pHash != IntPtr.Zero)
                    Marshal.FreeHGlobal(pHash);

                if(hCert != IntPtr.Zero)
                    CertFreeCertificateContext(hCert);

                if(hCertStore != IntPtr.Zero)
                    CertCloseStore(hCertStore, 0);
            }
        }
    }
}

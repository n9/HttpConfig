using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace HttpConfig
{
	public class HttpApi
	{
		private HttpApi() {}

		#region Methods
        [DllImport("Httpapi.dll")]
        public static extern HttpApi.Error HttpInitialize(
	        HTTPAPI_VERSION version,
	        InitFlag flags,
	        IntPtr reserved);

        [DllImport("Httpapi.dll")]
        public static extern HttpApi.Error HttpQueryServiceConfiguration(
            IntPtr                  ServiceHandle,
            HTTP_SERVICE_CONFIG_ID  ConfigId,
            IntPtr                  pInputConfigInfo,
            int                     InputConfigInfoLength,
            IntPtr                  pOutputConfigInfo,
            int                     OutputConfigInfoLength,
            out int                 pReturnLength,
            IntPtr                  pOverlapped);

        [DllImport("Httpapi.dll")]
        public static extern HttpApi.Error HttpSetServiceConfiguration(
            IntPtr                  ServiceHandle,
            HTTP_SERVICE_CONFIG_ID  ConfigId,
            IntPtr                  pConfigInformation,
            int                     ConfigInformationLength,
            IntPtr                  pOverlapped);

        [DllImport("Httpapi.dll")]
        public static extern HttpApi.Error HttpDeleteServiceConfiguration(
            IntPtr                  ServiceHandle,
            HTTP_SERVICE_CONFIG_ID  ConfigId,
            IntPtr                  pConfigInformation,
            int                     ConfigInformationLength,
            IntPtr                  pOverlapped);

        [DllImport("Httpapi.dll")]
        public static extern HttpApi.Error HttpTerminate(
	        InitFlag flags,
	        IntPtr reserved);
        
        [DllImport("Kernel32.dll", EntryPoint="RtlZeroMemory")]
        public static extern void ZeroMemory(
            IntPtr dest,
            int size);
		#endregion Methods

		#region Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct HTTPAPI_VERSION
        {
	        public HTTPAPI_VERSION(ushort majorVersion, ushort minorVersion)
	        {
		        major = majorVersion;
		        minor = minorVersion;
	        }

	        public ushort major;
	        public ushort minor;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SOCKADDR_STORAGE
        {
            public short sa_family;
            
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType=UnmanagedType.U1, SizeConst=6)]
            public byte[] __ss_pad1;

            public long __ss_align;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType=UnmanagedType.U1, SizeConst=112)]
            public byte[] __ss_pad2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HTTP_SERVICE_CONFIG_IP_LISTEN_QUERY
        {
            public int                AddrCount;
            public SOCKADDR_STORAGE   AddrList; // [ANYSIZE_ARRAY];
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HTTP_SERVICE_CONFIG_IP_LISTEN_PARAM
        {
            public short    AddrLength;
            public IntPtr   pAddress; // PSOCKADDR
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HTTP_SERVICE_CONFIG_SSL_KEY
        {
            public IntPtr pIpPort; // PSOCKADDR
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HTTP_SERVICE_CONFIG_SSL_QUERY
        {
            public HTTP_SERVICE_CONFIG_QUERY_TYPE  QueryDesc;
            public HTTP_SERVICE_CONFIG_SSL_KEY     KeyDesc;
            public int                             dwToken;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HTTP_SERVICE_CONFIG_URLACL_KEY
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pUrlPrefix;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct sockaddr 
        {
            public short sa_family;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst=14, ArraySubType=UnmanagedType.U1)]
            char[]  sa_data;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct HTTP_SERVICE_CONFIG_URLACL_QUERY
        {
            public HTTP_SERVICE_CONFIG_QUERY_TYPE  QueryDesc;
            public HTTP_SERVICE_CONFIG_URLACL_KEY  KeyDesc;
            public int                             dwToken;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HTTP_SERVICE_CONFIG_SSL_PARAM 
        {
            public int SslHashLength;
            
            public IntPtr pSslHash;
            
            public Guid AppId;
            
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pSslCertStoreName;
            
            public ClientCertCheckMode CertCheckMode;
            
            public int RevocationFreshnessTime;
            
            public int RevocationUrlRetrievalTimeout;
            
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pSslCtlIdentifier;
            
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pSslCtlStoreName;
            
            public SslConfigFlag Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HTTP_SERVICE_CONFIG_SSL_SET 
        {
            public HTTP_SERVICE_CONFIG_SSL_KEY     KeyDesc;
            public HTTP_SERVICE_CONFIG_SSL_PARAM   ParamDesc;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HTTP_SERVICE_CONFIG_URLACL_PARAM 
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pStringSecurityDescriptor;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HTTP_SERVICE_CONFIG_URLACL_SET 
        {
            public HTTP_SERVICE_CONFIG_URLACL_KEY     KeyDesc;
            public HTTP_SERVICE_CONFIG_URLACL_PARAM   ParamDesc;
        }
        #endregion Structures

		#region Enumerations
		public enum InitFlag : int
		{
			HTTP_INITIALIZE_SERVER = 1,
			HTTP_INITIALIZE_CONFIG = 2
		}

		public enum HTTP_SERVICE_CONFIG_ID : int
		{
			HttpServiceConfigIPListenList = 0,
			HttpServiceConfigSSLCertInfo  = 1,
			HttpServiceConfigUrlAclInfo   = 2
		}

        public enum HTTP_SERVICE_CONFIG_QUERY_TYPE : int
        {
	        HttpServiceConfigQueryExact = 0,
	        HttpServiceConfigQueryNext  = 1
        }

        public enum Error : int
        {
	        NO_ERROR				  = 0,
	        ERROR_FILE_NOT_FOUND	  = 2,
	        ERROR_INVALID_DATA		  = 13,
	        ERROR_HANDLE_EOF		  = 38,
	        ERROR_INVALID_PARAMETER   = 87,
            ERROR_INSUFFICIENT_BUFFER = 122,
            ERROR_NO_MORE_ITEMS       = 259,
	        ERROR_INVALID_DLL		  = 1154,
            ERROR_NOT_FOUND           = 1168
        }

		public enum ClientCertCheckMode
		{
			NoVerifyRevocation         = 1,
			CachedRevocationOnly       = 2,
			UseRevocationFreshnessTime = 4,
			NoUsageCheck               = 0x10000,
		}

        public enum SslConfigFlag : int
        {
            UseDSMapper                 = 1,
			NegotiateClientCertificates = 2,
			DoNotRouteToRawIsapiFilters = 4,
        }
		#endregion Enumerations

        public static IntPtr IncIntPtr(IntPtr ptr, int count)
        {
            return (IntPtr)((int)ptr + count);
        }

        public static IntPtr BuildSockaddr(short family, ushort port, IPAddress address)
        {
            int sockaddrSize = Marshal.SizeOf(typeof(HttpApi.sockaddr));

            IntPtr pSockaddr = Marshal.AllocHGlobal(sockaddrSize);

            HttpApi.ZeroMemory(pSockaddr, sockaddrSize);

            Marshal.WriteInt16(pSockaddr, family);

            ushort p = (ushort)IPAddress.HostToNetworkOrder((short)port);

            Marshal.WriteInt16(pSockaddr, 2, (short)p);

            byte[] addr = address.GetAddressBytes();

            IntPtr pAddr = HttpApi.IncIntPtr(pSockaddr, 4);

            Marshal.Copy(addr, 0, pAddr, addr.Length);

            return pSockaddr;
        }
	}

    public class HttpApiException : Exception
    {
	    public readonly HttpApi.Error HttpApiErrorCode;
    	
	    public HttpApiException(HttpApi.Error error) : base()
	    {
		    HttpApiErrorCode = error;
	    }

	    public HttpApiException(HttpApi.Error error, string message) : base(message)
	    {
		    HttpApiErrorCode = error;
	    }

	    public HttpApiException(HttpApi.Error error, string message, Exception innerException) : base(message, innerException)
	    {
		    HttpApiErrorCode = error;
	    }
    }
}

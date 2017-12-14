using System;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace HttpConfig
{
	public class SslConfigItem : ConfigItem
	{
        private IPAddress _address = new IPAddress(0);
        private ushort    _port;

        private byte[] _hash;
        private Guid   _appId;
        private string _certStoreName;        
        private int    _revocationFreshnessTime;
        private int    _revocationUrlRetrievalTimeout;
        private string _sslCtlIdentifier;
        private string _sslCtlStoreName;

		private HttpApi.ClientCertCheckMode _certCheckMode;
        private HttpApi.SslConfigFlag       _flags;

        public SslConfigItem() { }

        #region Properties
        public IPAddress Address
        {
            get { return _address;  }
            set
            {
                if(value == null)
                    throw new ArgumentNullException("Address", "Address cannot be null");

                _address = value;

                SetItems();
            }
        }

        public ushort Port
        {
            get { return _port;  }
            set
            {
                _port = value;
                SetItems();
            }
        }

        public byte[] Hash
        {
            get { return _hash;  }
            set
            {
                _hash = value;
                SetItems();
            }
        }

        public Guid AppId
        {
            get { return _appId;  }
            set
            {
                _appId = value;
                SetItems();
            }
        }

        public string CertStoreName
        {
            get { return _certStoreName;  }
            set
            {
                _certStoreName = value;
                SetItems();
            }
        }

        public HttpApi.ClientCertCheckMode CertCheckMode
        {
            get { return _certCheckMode;  }
            set
            {
                _certCheckMode = value;
                SetItems();
            }
        }

        public int RevocationFreshnessTime
        {
            get { return _revocationFreshnessTime;  }
            set
            {
                _revocationFreshnessTime = value;
                SetItems();
            }
        }

        public int RevocationUrlRetrievalTimeout
        {
            get { return _revocationUrlRetrievalTimeout;  }
            set
            {
                _revocationUrlRetrievalTimeout = value;
                SetItems();
            }
        }

        public string SslCtlIdentifier
        {
            get { return _sslCtlIdentifier;  }
            set
            {
                _sslCtlIdentifier = value;
                SetItems();
            }
        }

        public string SslCtlStoreName
        {
            get { return _sslCtlStoreName;  }
            set
            {
                _sslCtlStoreName = value;
                SetItems();
            }
        }

        public HttpApi.SslConfigFlag Flags
        {
            get { return _flags;  }
            set
            {
                _flags= value;
                SetItems();
            }
        }

        public override string Key
        {
            get { return _address.ToString() + ":" + _port.ToString(); }
        }
        #endregion Properties

        public override void ApplyConfig()
        {
            IntPtr pStruct = IntPtr.Zero;

            HttpApi.HTTP_SERVICE_CONFIG_SSL_SET setStruct = ToStruct();

            try
            {
                pStruct = Marshal.AllocHGlobal(Marshal.SizeOf(setStruct));

                Marshal.StructureToPtr(setStruct, pStruct, false);

                if((Status == ModifiedStatus.Modified) || (Status == ModifiedStatus.Removed))
                {
                    HttpApi.Error error = HttpApi.HttpDeleteServiceConfiguration(
                        IntPtr.Zero,
                        HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigSSLCertInfo,
                        pStruct,
                        Marshal.SizeOf(setStruct),
                        IntPtr.Zero);
                        
                    if(error != HttpApi.Error.NO_ERROR)
						throw new HttpApiException(error, "HttpDeleteServiceConfiguration (SSL) failed.  Error = " + error);
                }

                if((Status == ModifiedStatus.Modified) || (Status == ModifiedStatus.Added))
                {
                    HttpApi.Error error = HttpApi.HttpSetServiceConfiguration(
                        IntPtr.Zero,
                        HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigSSLCertInfo,
                        pStruct,
                        Marshal.SizeOf(setStruct),
                        IntPtr.Zero);

                    if(error != HttpApi.Error.NO_ERROR)
						throw new HttpApiException(error, "HttpSetServiceConfiguration (SSL) failed.  Error = " + error);
                }
            }
            finally
            {
                if(pStruct != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pStruct, typeof(HttpApi.HTTP_SERVICE_CONFIG_SSL_SET));
                    Marshal.FreeHGlobal(pStruct);
                }

                FreeStruct(setStruct);
            }
        }

        public static Hashtable QueryConfig()
        {
            Hashtable items = new Hashtable();

            HttpApi.HTTP_SERVICE_CONFIG_SSL_QUERY query = new HttpApi.HTTP_SERVICE_CONFIG_SSL_QUERY();

            query.QueryDesc = HttpApi.HTTP_SERVICE_CONFIG_QUERY_TYPE.HttpServiceConfigQueryNext;

            IntPtr pInput = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(HttpApi.HTTP_SERVICE_CONFIG_SSL_QUERY)));

            try
            {
                for(query.dwToken = 0; ; query.dwToken++)
                {
                    Marshal.StructureToPtr(query, pInput, false);

                    int requiredBufferLength = 0;
                    
                    HttpApi.Error error = QueryServiceConfig(pInput, IntPtr.Zero, 0, out requiredBufferLength);

                    if(error == HttpApi.Error.ERROR_NO_MORE_ITEMS)
                        break;
                    else if(error != HttpApi.Error.ERROR_INSUFFICIENT_BUFFER)
						throw new HttpApiException(error, "HttpQueryServiceConfiguration (SSL) failed.  Error = " + error);

                    IntPtr pOutput = Marshal.AllocHGlobal(requiredBufferLength);

                    try
                    {
                        HttpApi.ZeroMemory(pOutput, requiredBufferLength);

                        error = QueryServiceConfig(pInput, pOutput, requiredBufferLength, out requiredBufferLength);

                        if(error != HttpApi.Error.NO_ERROR)
							throw new HttpApiException(error, "HttpQueryServiceConfiguration (SSL) failed.  Error = " + error);

                        SslConfigItem item = Deserialize(pOutput);

                        items.Add(item.Key, item);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(pOutput);
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pInput);
            }

            return items;
        }

        private HttpApi.HTTP_SERVICE_CONFIG_SSL_SET ToStruct()
        {
            HttpApi.HTTP_SERVICE_CONFIG_SSL_SET sslStruct = new HttpApi.HTTP_SERVICE_CONFIG_SSL_SET();

            sslStruct.KeyDesc.pIpPort = HttpApi.BuildSockaddr(2, _port, _address);

            if(_hash != null)
            {
                sslStruct.ParamDesc.pSslHash = Marshal.AllocHGlobal(_hash.Length);

                Marshal.Copy(_hash, 0, sslStruct.ParamDesc.pSslHash, _hash.Length);

                sslStruct.ParamDesc.SslHashLength = _hash.Length;
            }

            sslStruct.ParamDesc.AppId                         = _appId;
            sslStruct.ParamDesc.pSslCertStoreName             = _certStoreName;            
            sslStruct.ParamDesc.RevocationFreshnessTime       = _revocationFreshnessTime;
            sslStruct.ParamDesc.RevocationUrlRetrievalTimeout = _revocationUrlRetrievalTimeout;
            sslStruct.ParamDesc.pSslCtlIdentifier             = _sslCtlIdentifier;
            sslStruct.ParamDesc.pSslCtlStoreName              = _sslCtlStoreName;
            sslStruct.ParamDesc.Flags                         = _flags;

			sslStruct.ParamDesc.CertCheckMode = _certCheckMode;

            return sslStruct;
        }

        private static void FreeStruct(HttpApi.HTTP_SERVICE_CONFIG_SSL_SET sslStruct)
        {
            Marshal.FreeHGlobal(sslStruct.KeyDesc.pIpPort);
            Marshal.FreeHGlobal(sslStruct.ParamDesc.pSslHash);
        }

        private static SslConfigItem Deserialize(IntPtr pSslConfigSetStruct)
        {
            SslConfigItem item = new SslConfigItem();

            HttpApi.HTTP_SERVICE_CONFIG_SSL_SET sslStruct =
                (HttpApi.HTTP_SERVICE_CONFIG_SSL_SET)Marshal.PtrToStructure(pSslConfigSetStruct, typeof(HttpApi.HTTP_SERVICE_CONFIG_SSL_SET));

            item._port                          = (ushort)IPAddress.NetworkToHostOrder(Marshal.ReadInt16(sslStruct.KeyDesc.pIpPort, 2));
            item._address                       = new IPAddress((long)Marshal.ReadInt32(sslStruct.KeyDesc.pIpPort, 4) & 0x00000000ffffffff);
            item._appId                         = sslStruct.ParamDesc.AppId;
            item._certStoreName                 = sslStruct.ParamDesc.pSslCertStoreName;
            item._certCheckMode                 = sslStruct.ParamDesc.CertCheckMode;
            item._revocationFreshnessTime       = sslStruct.ParamDesc.RevocationFreshnessTime;
            item._revocationUrlRetrievalTimeout = sslStruct.ParamDesc.RevocationUrlRetrievalTimeout;
            item._sslCtlIdentifier              = sslStruct.ParamDesc.pSslCtlIdentifier;
            item._sslCtlStoreName               = sslStruct.ParamDesc.pSslCtlStoreName;
            item._flags                         = sslStruct.ParamDesc.Flags;
            item.Status                         = ModifiedStatus.Unmodified;
			item._hash                          = new byte[sslStruct.ParamDesc.SslHashLength];

			Marshal.Copy(sslStruct.ParamDesc.pSslHash, item._hash, 0, item._hash.Length);

            item.SetItems();

            return item;
        }

        private static HttpApi.Error QueryServiceConfig(IntPtr pInput, IntPtr pOutput, int outputLength, out int requiredBufferLength)
        {
            return HttpApi.HttpQueryServiceConfiguration(
                IntPtr.Zero,
                HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigSSLCertInfo,
                pInput,
                Marshal.SizeOf(typeof(HttpApi.HTTP_SERVICE_CONFIG_SSL_QUERY)),
                pOutput,
                outputLength,
                out requiredBufferLength,
                IntPtr.Zero);
        }

        private void SetItems()
        {
            SubItems.Clear();

            Text = Key;

            string storeName = _certStoreName == null ? "MY" : _certStoreName;

            string certName = "";
            
            if((_hash != null) && (_hash.Length > 0))
                certName = CertUtil.GetCertNameFromStoreAndHash(storeName, _hash);

            SubItems.Add(certName);

            SubItems.Add(storeName);
        }
	}
}

using System;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;

namespace HttpConfig
{
	public class IpListenConfigItem : ConfigItem
	{
        private IPAddress _address;

		public IpListenConfigItem() { }

        public IPAddress Address
        {
            get { return _address;  }
            set
            {
                _address = value;

                Text = _address.ToString();
            }
        }

        public override string Key
        {
            get { return _address.ToString(); }
        }

        public override void ApplyConfig()
        {
            IntPtr pStruct = IntPtr.Zero;

            HttpApi.HTTP_SERVICE_CONFIG_IP_LISTEN_PARAM setStruct = new HttpApi.HTTP_SERVICE_CONFIG_IP_LISTEN_PARAM();

            try
            {                
                setStruct.pAddress = HttpApi.BuildSockaddr(2, 0, _address);

                setStruct.AddrLength = (short)Marshal.SizeOf(typeof(HttpApi.sockaddr));

                pStruct = Marshal.AllocHGlobal(Marshal.SizeOf(setStruct));

                Marshal.StructureToPtr(setStruct, pStruct, false);

                if((Status == ModifiedStatus.Modified) || (Status == ModifiedStatus.Removed))
                {
                    HttpApi.Error error = HttpApi.HttpDeleteServiceConfiguration(
                        IntPtr.Zero,
                        HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigIPListenList,
                        pStruct,
                        Marshal.SizeOf(setStruct),
                        IntPtr.Zero);
                        
                    if(error != HttpApi.Error.NO_ERROR)
						throw new HttpApiException(error, "HttpDeleteServiceConfiguration (IPLISTEN) failed.  Error = " + error);
                }

                if((Status == ModifiedStatus.Modified) || (Status == ModifiedStatus.Added))
                {
                    HttpApi.Error error = HttpApi.HttpSetServiceConfiguration(
                        IntPtr.Zero,
                        HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigIPListenList,
                        pStruct,
                        Marshal.SizeOf(setStruct),
                        IntPtr.Zero);

                    if(error != HttpApi.Error.NO_ERROR)
                        throw new HttpApiException(error, "HttpSetServiceConfiguration (IPLISTEN) failed.  Error = " + error);
                }
            }
            finally
            {
                if(setStruct.pAddress != IntPtr.Zero)
                    Marshal.FreeHGlobal(setStruct.pAddress);

                if(pStruct != IntPtr.Zero)
                    Marshal.FreeHGlobal(pStruct);
            }
        }

        public static Hashtable QueryConfig()
        {
            Hashtable items = new Hashtable();

            int requiredBufferLength = 0;

            HttpApi.Error error = QueryServiceConfig(IntPtr.Zero, 0, out requiredBufferLength);

            if((error != HttpApi.Error.ERROR_FILE_NOT_FOUND) && (error != HttpApi.Error.ERROR_NOT_FOUND))
            {
                if(error != HttpApi.Error.ERROR_INSUFFICIENT_BUFFER)
                    throw new HttpApiException(error, "HttpQueryServiceConfiguration (IPLISTEN) failed.  Error = " + error);

                IntPtr pOutput = Marshal.AllocHGlobal(requiredBufferLength);

                try
                {
                    HttpApi.ZeroMemory(pOutput, requiredBufferLength);

                    error = QueryServiceConfig(pOutput, requiredBufferLength, out requiredBufferLength);

                    if(error != HttpApi.Error.NO_ERROR)
                        throw new HttpApiException(error, "HttpQueryServiceConfiguration (IPLISTEN) failed.  Error = " + error);

                    HttpApi.HTTP_SERVICE_CONFIG_IP_LISTEN_QUERY output =
                        (HttpApi.HTTP_SERVICE_CONFIG_IP_LISTEN_QUERY)Marshal.PtrToStructure(pOutput, typeof(HttpApi.HTTP_SERVICE_CONFIG_IP_LISTEN_QUERY));

                    int addrSize = Marshal.SizeOf(typeof(HttpApi.SOCKADDR_STORAGE));

                    IntPtr pAddress = HttpApi.IncIntPtr(pOutput, 8); // Increment past the addrcount member and padding

                    for(int i = 0; i < output.AddrCount; i++)
                    {
                        IpListenConfigItem item = new IpListenConfigItem();

                        item.Address = new IPAddress((long)Marshal.ReadInt32(pAddress, 4) & 0x00000000ffffffff);

                        item.Status = ModifiedStatus.Unmodified;

                        items.Add(item.Key, item);

                        pAddress = HttpApi.IncIntPtr(pAddress, addrSize); // Increment to the next IP address
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(pOutput);
                }
            }

            return items;
        }

        private static HttpApi.Error QueryServiceConfig(IntPtr pOutput, int outputLength, out int requiredBufferLength)
        {
            return HttpApi.HttpQueryServiceConfiguration(
                IntPtr.Zero,
                HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigIPListenList,
                IntPtr.Zero,
                0,
                pOutput,
                outputLength,
                out requiredBufferLength,
                IntPtr.Zero);
        }
	}
}

using System;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HttpConfig
{
    public class UrlAclConfigItem : ConfigItem
    {
        private string _url;
        private Acl    _acl;

        public UrlAclConfigItem() { }

        public string Url
        {
            get { return _url;  }
            set
            {
                _url = value;
                SetItems();
            }
        }

        public Acl Dacl
        {
            get { return _acl;  }
            set
            {
                _acl = value;
                SetItems();
            }
        }

        public override string Key
        {
            get { return _url; }
        }

        public override string ToString()
        {
            return _url;
        }

        public override void ApplyConfig()
        {
            IntPtr pStruct = IntPtr.Zero;

            HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET setStruct = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET();

            setStruct.KeyDesc.pUrlPrefix = _url;

            setStruct.ParamDesc.pStringSecurityDescriptor = _acl.ToSddl();

            try
            {
                pStruct = Marshal.AllocHGlobal(Marshal.SizeOf(setStruct));

                Marshal.StructureToPtr(setStruct, pStruct, false);

                if((Status == ModifiedStatus.Modified) || (Status == ModifiedStatus.Removed))
                {
                    HttpApi.Error error = HttpApi.HttpDeleteServiceConfiguration(
                        IntPtr.Zero,
                        HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
                        pStruct,
                        Marshal.SizeOf(setStruct),
                        IntPtr.Zero);
                        
                    if(error != HttpApi.Error.NO_ERROR)
						throw new HttpApiException(error, "HttpDeleteServiceConfiguration (URLACL) failed.  Error = " + error);
                }

                if((Status == ModifiedStatus.Modified) || (Status == ModifiedStatus.Added))
                {
                    HttpApi.Error error = HttpApi.HttpSetServiceConfiguration(
                        IntPtr.Zero,
                        HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
                        pStruct,
                        Marshal.SizeOf(setStruct),
                        IntPtr.Zero);

                    if(error != HttpApi.Error.NO_ERROR)
						throw new HttpApiException(error, "HttpSetServiceConfiguration (URLACL) failed.  Error = " + error);
                }
            }
            finally
            {
                if(pStruct != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pStruct, typeof(HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET));
                    Marshal.FreeHGlobal(pStruct);
                }
            }
        }

        public static Hashtable QueryConfig()
        {
            Hashtable items = new Hashtable();

            HttpApi.HTTP_SERVICE_CONFIG_URLACL_QUERY query = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_QUERY();

            query.QueryDesc = HttpApi.HTTP_SERVICE_CONFIG_QUERY_TYPE.HttpServiceConfigQueryNext;
            
            HttpApi.Error error = HttpApi.Error.NO_ERROR;

            IntPtr pInput = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(HttpApi.HTTP_SERVICE_CONFIG_URLACL_QUERY)));

            try
            {
                for(query.dwToken = 0; ; query.dwToken++)
                {
                    Marshal.StructureToPtr(query, pInput, false);

                    int requiredBufferLength = 0;
                    
                    error = QueryServiceConfig(pInput, IntPtr.Zero, 0, out requiredBufferLength);

                    if(error == HttpApi.Error.ERROR_NO_MORE_ITEMS)
                        break;
                    else if(error != HttpApi.Error.ERROR_INSUFFICIENT_BUFFER)
						throw new HttpApiException(error, "HttpQueryServiceConfiguration (URLACL) failed.  Error = " + error);

                    IntPtr pOutput = Marshal.AllocHGlobal(requiredBufferLength);

                    try
                    {
                        HttpApi.ZeroMemory(pOutput, requiredBufferLength);

                        error = QueryServiceConfig(pInput, pOutput, requiredBufferLength, out requiredBufferLength);

                        if(error != HttpApi.Error.NO_ERROR)
							throw new HttpApiException(error, "HttpQueryServiceConfiguration (URLACL) failed.  Error = " + error);
                        
                        UrlAclConfigItem item = Deserialize(pOutput);

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

        private static UrlAclConfigItem Deserialize(IntPtr pUrlAclConfigSetStruct)
        {
            UrlAclConfigItem item = new UrlAclConfigItem();

            HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET aclStruct =
                (HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET)Marshal.PtrToStructure(pUrlAclConfigSetStruct, typeof(HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET));

            item.Url = aclStruct.KeyDesc.pUrlPrefix;

            item.Dacl = Acl.FromSddl(aclStruct.ParamDesc.pStringSecurityDescriptor);

            item.Status = ModifiedStatus.Unmodified;

            return item;
        }

        private static HttpApi.Error QueryServiceConfig(IntPtr pInput, IntPtr pOutput, int outputLength, out int requiredBufferLength)
        {
            return HttpApi.HttpQueryServiceConfiguration(
                IntPtr.Zero,
                HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
                pInput,
                Marshal.SizeOf(typeof(HttpApi.HTTP_SERVICE_CONFIG_URLACL_QUERY)),
                pOutput,
                outputLength,
                out requiredBufferLength,
                IntPtr.Zero);
        }

        private void SetItems()
        {
            SubItems.Clear();
            
            if(_url != null)
                Text = _url;

            if(_acl != null)
                SubItems.Add(_acl.FriendlyString);
        }
    }
}

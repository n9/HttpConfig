using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace HttpConfig
{
	public class SecurityEditor :IDisposable
	{
		#region P/Invoke

		[DllImport("aclui", SetLastError=true)]
		private static extern bool EditSecurity(
			IntPtr hwndOwner,
			IntPtr psi); // LPSECURITYINFO

		[DllImport("Advapi32", SetLastError=true)]
		private static extern bool MakeSelfRelativeSD(
			IntPtr pAbsoluteSD,
			IntPtr pSelfRelativeSD,
			ref uint lpdwBufferLength);

		private enum SI_PAGE_TYPE
		{
			SI_PAGE_PERM=0,
			SI_PAGE_ADVPERM=1,
			SI_PAGE_AUDIT=2,
			SI_PAGE_OWNER=3,
			SI_PAGE_EFFECTIVE=4,
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SI_INHERIT_TYPE
		{
			public IntPtr pguid; // GUID *

			public uint dwFlags;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszName;            // may be resource ID
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SI_OBJECT_INFO
		{
			public uint dwFlags;

			public IntPtr hInstance;          // resources (e.g. strings) reside here

			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszServerName;      // must be present

			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszObjectName;      // must be present

			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszPageTitle;       // only valid if SI_PAGE_TITLE is set

			public Guid guidObjectType;     // only valid if SI_OBJECT_GUID is set
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SI_ACCESS
		{
			public IntPtr pguid; // GUID *

			public uint mask;  // ACCESS_MASK

			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszName;            // may be resource ID

			public uint dwFlags;
		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
		private struct SiAccess
		{
			public SiAccess(IntPtr pguid, uint mask, string name, SiAccessFlags flags)
			{
				PGuid = pguid;
				Mask  = mask;
				Name  = name;
				Flags = flags;
			}

			public IntPtr PGuid; // Guid*
			public uint Mask;
			public string Name;
			public SiAccessFlags Flags;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct GENERIC_MAPPING
		{
			public int GenericRead;
			public int GenericWrite;
			public int GenericExecute;
			public int GenericAll;
		}

		private enum SiAccessFlags
		{
			Specific  = 0x10000,
			General   = 0x20000,
			Container = 0x40000,
			Property  = 0x80000,
		}

		private const uint S_OK                      = 0;
		private const uint E_FAIL                    = 0x80004005;
		private const uint E_NOINTERFACE             = 0x80004002;
		private const uint ERROR_INSUFFICIENT_BUFFER = 122;
		private const uint GENERIC_READ              = 0x80000000;
		private const uint GENERIC_WRITE             = 0x40000000;
		private const uint GENERIC_EXECUTE           = 0x20000000;
		private const uint GENERIC_ALL               = 0x10000000;
		private const uint SI_EDIT_PERMS             = 0x00000000;
		private const uint SI_EDIT_OWNER             = 0x00000001;
		private const uint SI_EDIT_AUDITS            = 0x00000002;
		private const uint SI_ADVANCED               = 0x00000010;

		private const uint SI_EDIT_ALL = (SI_EDIT_PERMS | SI_EDIT_OWNER | SI_EDIT_AUDITS);

		#endregion P/Invoke

		delegate uint QueryInterfaceDelegate(IntPtr pThis, [In] ref Guid riid, out IntPtr ppvObj);
		delegate uint AddRefDelegate(IntPtr pThis);
		delegate uint ReleaseDelegate(IntPtr pThis);
		delegate uint GetObjectInformationDelegate(IntPtr pThis, ref SI_OBJECT_INFO pObjectInfo);
		delegate uint GetSecurityDelegate(IntPtr pThis, [In] uint RequestedInformation, ref IntPtr ppSecurityDescriptor, [In] bool fDefault);		
		delegate uint SetSecurityDelegate(IntPtr pThis, [In] uint SecurityInformation, [In] IntPtr pSecurityDescriptor);
		delegate uint GetInheritTypesDelegate(IntPtr pThis, ref SI_INHERIT_TYPE ppInheritTypes, ref uint pcInheritTypes);
		delegate uint PropertySheetPageCallbackDelegate(IntPtr pThis, [In] IntPtr hwnd, [In] uint uMsg, [In] SI_PAGE_TYPE uPage);
		
		unsafe delegate uint GetAccessRightsDelegate(IntPtr pThis, [In] Guid *pguidObjectType, [In] uint dwFlags, ref IntPtr ppAccess, ref uint pcAccesses, ref uint piDefaultAccess);
		unsafe delegate uint MapGenericDelegate(IntPtr pThis, [In] Guid *pguidObjectType, [In] ref byte pAceFlags, [In] ref uint pMask);

		[StructLayout(LayoutKind.Sequential)]
		private struct ISI_Struct
		{
			public IntPtr pTable;
			public IntPtr QueryInterface;
			public IntPtr AddRef;
			public IntPtr Release;
			public IntPtr GetObjectInformation;
			public IntPtr GetSecurity;
			public IntPtr SetSecurity;
			public IntPtr GetAccessRights;
			public IntPtr MapGeneric;
			public IntPtr GetInheritTypes;
			public IntPtr PropertySheetPageCallback;
		}

		private static readonly SiAccess[] __rights = new SiAccess[] {
			new SiAccess(IntPtr.Zero, GENERIC_EXECUTE, "Register", SiAccessFlags.General | SiAccessFlags.Specific), // Default right...must be at index 0
			new SiAccess(IntPtr.Zero, GENERIC_WRITE, "Delegate", SiAccessFlags.General | SiAccessFlags.Specific)
		};

		private static readonly Guid IID_ISecurityInformation = new Guid("965fc360-16ff-11d0-91cb-00aa00bbb723");

		private string   _sddl;
		private string   _objectName;
		private IntPtr   _pSecurityInterface;
		private IntPtr   _pAccessRights;
		private Delegate _queryInterfaceDelegate;
		private Delegate _addRefDelegate;
		private Delegate _releaseDelegate;
		private Delegate _getObjectInformationDelegate;
		private Delegate _getSecurityDelegate;
		private Delegate _setSecurityDelegate;
		private Delegate _getAccessRightsDelegate;
		private Delegate _mapGenericDelegate;
		private Delegate _getInheritTypesDelegate;
		private Delegate _propertySheetPageCallbackDelegate;

		private SecurityEditor(string objectName, string sddl)
		{
			_objectName = objectName;
			_sddl       = sddl;

			_queryInterfaceDelegate            = new QueryInterfaceDelegate(QueryInterface);
			_addRefDelegate                    = new AddRefDelegate(AddRef);
			_releaseDelegate                   = new ReleaseDelegate(Release);
			_getObjectInformationDelegate      = new GetObjectInformationDelegate(GetObjectInformation);
			_getSecurityDelegate               = new GetSecurityDelegate(GetSecurity);
			_setSecurityDelegate               = new SetSecurityDelegate(SetSecurity);
			_getInheritTypesDelegate           = new GetInheritTypesDelegate(GetInheritTypes);
			_propertySheetPageCallbackDelegate = new PropertySheetPageCallbackDelegate(PropertySheetPageCallback);

			unsafe
			{
				_getAccessRightsDelegate = new GetAccessRightsDelegate(GetAccessRights);
				_mapGenericDelegate      = new MapGenericDelegate(MapGeneric);
			}

			ISI_Struct isi = new ISI_Struct();

			_pSecurityInterface = Marshal.AllocHGlobal(Marshal.SizeOf(isi));
			
			isi.pTable = (IntPtr)((long)_pSecurityInterface + (long)Marshal.SizeOf(typeof(IntPtr)));

			isi.QueryInterface            = Marshal.GetFunctionPointerForDelegate(_queryInterfaceDelegate);
			isi.AddRef                    = Marshal.GetFunctionPointerForDelegate(_addRefDelegate);
			isi.Release                   = Marshal.GetFunctionPointerForDelegate(_releaseDelegate);
			isi.GetObjectInformation      = Marshal.GetFunctionPointerForDelegate(_getObjectInformationDelegate);
			isi.GetSecurity               = Marshal.GetFunctionPointerForDelegate(_getSecurityDelegate);
			isi.SetSecurity               = Marshal.GetFunctionPointerForDelegate(_setSecurityDelegate);
			isi.GetAccessRights           = Marshal.GetFunctionPointerForDelegate(_getAccessRightsDelegate);
			isi.MapGeneric                = Marshal.GetFunctionPointerForDelegate(_mapGenericDelegate);
			isi.GetInheritTypes           = Marshal.GetFunctionPointerForDelegate(_getInheritTypesDelegate);
			isi.PropertySheetPageCallback = Marshal.GetFunctionPointerForDelegate(_propertySheetPageCallbackDelegate);

			Marshal.StructureToPtr(isi, _pSecurityInterface, false);
		}

		public static string EditSecurity(IntPtr hwndOwner, string objectName, string sddl)
		{
			using(SecurityEditor editor = new SecurityEditor(objectName, sddl))
			{
				if(!EditSecurity(hwndOwner, editor._pSecurityInterface))
				{
					int err = Marshal.GetLastWin32Error();

					throw new System.ComponentModel.Win32Exception(err, "EditSecurity failed.  Error = " + err);
				}

				return editor._sddl;
			}
		}

		public void Dispose()
		{
			if(_pAccessRights != IntPtr.Zero)
				Marshal.FreeHGlobal(_pAccessRights);

			if(_pSecurityInterface != IntPtr.Zero)
				Marshal.FreeHGlobal(_pSecurityInterface);
		}

		private uint QueryInterface(IntPtr pThis, ref Guid riid, out IntPtr ppvObj)
		{
			if(riid == IID_ISecurityInformation)
			{
				ppvObj = _pSecurityInterface;
				return S_OK;
			}
			else
			{
				ppvObj = IntPtr.Zero;
				return E_NOINTERFACE;
			}
		}

		private uint AddRef(IntPtr pThis)
		{
			return S_OK;
		}

		private uint Release(IntPtr pThis)
		{
			return S_OK;
		}

		private uint GetObjectInformation(IntPtr pThis, ref SI_OBJECT_INFO pObjectInfo)
		{
			pObjectInfo.dwFlags       = SI_EDIT_ALL | SI_ADVANCED;
			pObjectInfo.pszObjectName = _objectName;

			return S_OK;
		}

		private uint GetSecurity(IntPtr pThis, uint RequestedInformation, ref IntPtr ppSecurityDescriptor, bool fDefault)
		{
			try
			{
				FileSecurity fs = new FileSecurity();

				fs.SetSecurityDescriptorSddlForm(_sddl);

				byte[] sd = fs.GetSecurityDescriptorBinaryForm();

				ppSecurityDescriptor = Marshal.AllocHGlobal(sd.Length);

				Marshal.Copy(sd, 0, ppSecurityDescriptor, sd.Length);

				fDefault = false;

				return S_OK;
			}
			catch
			{
				return E_FAIL;
			}
		}

		private uint SetSecurity(IntPtr pThis, uint SecurityInformation, IntPtr pSecurityDescriptor)
		{
			IntPtr psd = IntPtr.Zero;
			uint   len = 0;

			try
			{
				if(!MakeSelfRelativeSD(pSecurityDescriptor, psd, ref len))
				{
					int err = Marshal.GetLastWin32Error();

					if(err != ERROR_INSUFFICIENT_BUFFER)
						throw new System.ComponentModel.Win32Exception(err, "MakeSelfRelativeSD failed.  Error = " + err);
				}

				psd = Marshal.AllocHGlobal((int)len);

				if(!MakeSelfRelativeSD(pSecurityDescriptor, psd, ref len))
				{
					int err = Marshal.GetLastWin32Error();

					throw new System.ComponentModel.Win32Exception(err, "MakeSelfRelativeSD failed.  Error = " + err);
				}

				byte[] sd = new byte[len];

				Marshal.Copy(psd, sd, 0, (int)len);

				FileSecurity fs = new FileSecurity();

				fs.SetSecurityDescriptorBinaryForm(sd);

				_sddl = fs.GetSecurityDescriptorSddlForm(AccessControlSections.All);

				return S_OK;
			}
			catch
			{
				return E_FAIL;
			}
			finally
			{
				if(psd != IntPtr.Zero)
					Marshal.FreeHGlobal(psd);
			}
		}

		private unsafe uint GetAccessRights(IntPtr pThis, Guid *pguidObjectType, uint dwFlags, ref IntPtr ppAccess, ref uint pcAccesses, ref uint piDefaultAccess)
		{
			try
			{
				if(_pAccessRights == IntPtr.Zero)
				{
					_pAccessRights = Marshal.AllocHGlobal(__rights.Length * Marshal.SizeOf(__rights[0]));

					for(int i = 0; i < __rights.Length; i++)
						Marshal.StructureToPtr(__rights[i], (IntPtr)((int)_pAccessRights + (i * Marshal.SizeOf(__rights[0]))), false);
				}

				ppAccess        = _pAccessRights;
				pcAccesses      = (uint)__rights.Length;
				piDefaultAccess = 0;

				return S_OK;
			}
			catch
			{
				return E_FAIL;
			}
		}

		private unsafe uint MapGeneric(IntPtr pThis, Guid *pguidObjectType, ref byte pAceFlags, ref uint pMask)
		{
			return S_OK;
		}

		private uint GetInheritTypes(IntPtr pThis, ref SI_INHERIT_TYPE ppInheritTypes, ref uint pcInheritTypes)
		{
			pcInheritTypes = 0;

			return S_OK;
		}

		private uint PropertySheetPageCallback(IntPtr pThis, IntPtr hwnd, uint uMsg, SI_PAGE_TYPE uPage)
		{
			return S_OK;
		}
	}
}

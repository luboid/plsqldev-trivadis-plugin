using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TrivadisPLSQLCop
{
    delegate void IdeCreateWindow(int windowType, byte[] text, [MarshalAs(UnmanagedType.Bool)] bool execute);

    [return: MarshalAs(UnmanagedType.Bool)]
    delegate bool IdeSetText(byte[] text);
    delegate IntPtr IdeGetText();

    [return: MarshalAs(UnmanagedType.Bool)]
    delegate bool IdeSaveFile();
    delegate void IdeSetFilename(byte[] text);
    delegate IntPtr IdeFileName();
    delegate IntPtr IdeGetFileData();

    [return: MarshalAs(UnmanagedType.Bool)]
    delegate bool IdeShowHTML(byte[] url, byte[] hash, byte[] title, byte[] id);

    delegate IntPtr IdeGetPrefAsString(int plugInId, byte[] prefSet, byte[] name, byte[] @default);
    [return: MarshalAs(UnmanagedType.Bool)]
    delegate bool IdeSetPrefAsString(int plugInId, byte[] prefSet, byte[] name, byte[] value);

    [return: MarshalAs(UnmanagedType.Bool)]
    delegate bool IdeGetPrefAsBool(int plugInId, byte[] prefSet, byte[] name, [MarshalAs(UnmanagedType.Bool)] bool @default); //214
    [return: MarshalAs(UnmanagedType.Bool)]
    delegate bool IdeSetPrefAsBool(int plugInId, byte[] prefSet, byte[] name, [MarshalAs(UnmanagedType.Bool)] bool value); //217

    [return: MarshalAs(UnmanagedType.Bool)]
    delegate bool IdePlugInSetting(int plugInId, byte[] setting, byte[] value); //219

    delegate void IdeCreatePopupItem(int plugInId, int index, byte[] name, byte[] objectType); //69

    delegate void IdeGetPopupObject(out IntPtr objectType, out IntPtr objectOwner, out IntPtr objectName, out IntPtr subObject); //74

    [return: MarshalAs(UnmanagedType.Bool)]
    delegate bool IdeGetWindowObject(out IntPtr objectType, out IntPtr objectOwner, out IntPtr objectName, out IntPtr subObject); //110

    delegate IntPtr IdeGetObjectSource(byte[] objectType, byte[] objectOwner, byte[] objectName); //79



    public class Callbacks
    {
        const int CREATE_WINDOW_CALLBACK = 20;
        const int SET_TEXT_CALLBACK = 34;
        const int GET_TEXT_CALLBACK = 30;
        const int SAVE_FILE_CALLBACK = 22;
        const int FILE_NAME_CALLBACK = 23;
        const int SET_FILE_NAME_CALLBACK = 29;
        const int GET_FILE_DATA_CALLBACK = 105;
        const int SHOW_HTML_CALLBACK = 107;
        const int GET_PREF_STRING_CALLBACK = 212;
        const int SET_PREF_STRING_CALLBACK = 215;
        const int GET_PREF_BOOL_CALLBACK = 214;
        const int SET_PREF_BOOL_CALLBACK = 217;
        const int PLUGIN_SETTING_CALLBACK = 219;
        const int CREATE_POPUP_ITEM_CALLBACK = 69;
        const int GET_POPUP_OBJECT_CALLBACK = 74;
        const int GET_OBJECT_SOURCE_CALLBACK = 79;
        const int GET_WINDOW_OBJECT_CALLBACK = 110;
        //IDE_GetFileData
        static IdeCreateWindow createWindowCallback;
        static IdeSetText setTextCallback;
        static IdeGetText getTextCallback;
        static IdeFileName fileNameCallback;
        static IdeSaveFile saveFileCallback;
        static IdeSetFilename setFilenameCallback;
        static IdeGetFileData getFileDataCallback;
        static IdeShowHTML showHtmlCallback;
        static IdeGetPrefAsString getPrefAsStringCallback;
        static IdeSetPrefAsString setPrefAsStringCallback;
        static IdeGetPrefAsBool getPrefAsBoolCallback;
        static IdeSetPrefAsBool setPrefAsBoolCallback;
        static IdePlugInSetting plugInSettingCallback;
        static IdeCreatePopupItem createPopupItemCallback;
        static IdeGetPopupObject getPopupObjectCallback;
        static IdeGetObjectSource getObjectSourceCallback;
        static IdeGetWindowObject getWindowObjectCallback;

        public static string GetObjectSource(string objectType, string objectOwner, string objectName)
        {
            return getObjectSourceCallback(
                objectType.ToUTF8ByteArray(),
                objectOwner.ToUTF8ByteArray(),
                objectName.ToUTF8ByteArray()).PtrUTF8StrToString();
        }

        public static bool GetWindowObject(out string objectType, out string objectOwner, out string objectName, out string subObject)
        {
            IntPtr objectTypePtr, objectOwnerPtr, objectNamePtr, subObjectPtr;

            var  result = getWindowObjectCallback(out objectTypePtr, out objectOwnerPtr, out objectNamePtr, out subObjectPtr);

            objectType = objectTypePtr.PtrUTF8StrToString();
            objectOwner = objectOwnerPtr.PtrUTF8StrToString();
            objectName = objectNamePtr.PtrUTF8StrToString();
            subObject = subObjectPtr.PtrUTF8StrToString();

            return result;
        }

        public static void GetPopupObject(out string objectType, out string objectOwner, out string objectName, out string subObject)
        {
            IntPtr objectTypePtr, objectOwnerPtr, objectNamePtr, subObjectPtr;

            getPopupObjectCallback(out objectTypePtr, out objectOwnerPtr, out objectNamePtr, out subObjectPtr);

            objectType = objectTypePtr.PtrUTF8StrToString();
            objectOwner = objectOwnerPtr.PtrUTF8StrToString();
            objectName = objectNamePtr.PtrUTF8StrToString();
            subObject = subObjectPtr.PtrUTF8StrToString();
        }

        public static void CreatePopupItem(int plugInId, int index, string name, string objectType)
        {
            createPopupItemCallback(plugInId, index, name.ToUTF8ByteArray(), objectType.ToUTF8ByteArray());
        }

        // can change char paarameters encoding to ANSI/UTF8/UTF8BOM
        // CHARMODE ANSI|UTF8|UTF8BOM
        // NOFILEDATECHECK TRUE|FALSE
        public static bool PlugInSetting(int plugInId, string setting, string value)
        {
            return plugInSettingCallback(plugInId,
                setting.ToUTF8ByteArray(),
                value.ToUTF8ByteArray());
        }

        public static bool GetPrefAsBool(int plugInId, string prefSet, string name, bool @default)
        {
            return getPrefAsBoolCallback(plugInId, prefSet.ToUTF8ByteArray(), name.ToUTF8ByteArray(), @default);
        }

        public static bool SetPrefAsBool(int plugInId, string prefSet, string name, bool value)
        {
            return setPrefAsBoolCallback(plugInId, prefSet.ToUTF8ByteArray(), name.ToUTF8ByteArray(), value);
        }

        public static string GetPrefAsString(int plugInId, string prefSet, string name, string @default)
        {
            return getPrefAsStringCallback(plugInId,
                prefSet.ToUTF8ByteArray(),
                name.ToUTF8ByteArray(),
                @default.ToUTF8ByteArray()).PtrUTF8StrToString();
        }

        public static bool SetPrefAsString(int plugInId, string prefSet, string name, string value)
        {
            return setPrefAsStringCallback(plugInId,
                prefSet.ToUTF8ByteArray(),
                name.ToUTF8ByteArray(),
                value.ToUTF8ByteArray());
        }

        public static bool ShowHtml(string url, string title = "", string hash = "", string id = "")
        {
            return showHtmlCallback(
                url.ToUTF8ByteArray(),
                hash.ToUTF8ByteArray(),
                title.ToUTF8ByteArray(),
                id.ToUTF8ByteArray());
        }

        public static void CreateWindow(int windowType, string text, bool execute)
        {
            createWindowCallback(windowType, text.ToUTF8ByteArray(), execute);
        }

        public static string GetText()
        {
            return getTextCallback().PtrUTF8StrToString();
        }

        public static bool SetText(string text)
        {
            return setTextCallback(text.ToUTF8ByteArray());
        }

        public static string FileName()
        {
            return fileNameCallback().PtrUTF8StrToString();
        }

        public static bool SaveFile()
        {
            return saveFileCallback();
        }

        public static void SetFilename(string name)
        {
            setFilenameCallback(name.ToUTF8ByteArray());
        }

        public static string GetFileData()
        {
            return getFileDataCallback().PtrUTF8StrToString();
        }

        public static void RegisterCallback(int index, IntPtr function)
        {
            switch (index)
            {
                case CREATE_WINDOW_CALLBACK:
                    createWindowCallback = Marshal.GetDelegateForFunctionPointer<IdeCreateWindow>(function);
                    break;
                case SET_TEXT_CALLBACK:
                    setTextCallback = Marshal.GetDelegateForFunctionPointer<IdeSetText>(function);
                    break;
                case GET_TEXT_CALLBACK:
                    getTextCallback = Marshal.GetDelegateForFunctionPointer<IdeGetText>(function);
                    break;
                case FILE_NAME_CALLBACK:
                    fileNameCallback = Marshal.GetDelegateForFunctionPointer<IdeFileName>(function);
                    break;
                case SAVE_FILE_CALLBACK:
                    saveFileCallback = Marshal.GetDelegateForFunctionPointer<IdeSaveFile>(function);
                    break;
                case SET_FILE_NAME_CALLBACK:
                    setFilenameCallback = Marshal.GetDelegateForFunctionPointer<IdeSetFilename>(function);
                    break;
                case GET_FILE_DATA_CALLBACK:
                    getFileDataCallback = Marshal.GetDelegateForFunctionPointer<IdeGetFileData>(function);
                    break;
                case SHOW_HTML_CALLBACK:
                    showHtmlCallback = Marshal.GetDelegateForFunctionPointer<IdeShowHTML>(function);
                    break;
                case GET_PREF_STRING_CALLBACK:
                    getPrefAsStringCallback = Marshal.GetDelegateForFunctionPointer<IdeGetPrefAsString>(function);
                    break;
                case SET_PREF_STRING_CALLBACK:
                    setPrefAsStringCallback = Marshal.GetDelegateForFunctionPointer<IdeSetPrefAsString>(function);
                    break;
                case SET_PREF_BOOL_CALLBACK:
                    setPrefAsBoolCallback = Marshal.GetDelegateForFunctionPointer<IdeSetPrefAsBool>(function);
                    break;
                case GET_PREF_BOOL_CALLBACK:
                    getPrefAsBoolCallback = Marshal.GetDelegateForFunctionPointer<IdeGetPrefAsBool>(function);
                    break;
                case PLUGIN_SETTING_CALLBACK:
                    plugInSettingCallback = Marshal.GetDelegateForFunctionPointer<IdePlugInSetting>(function);
                    break;
                case CREATE_POPUP_ITEM_CALLBACK:
                    createPopupItemCallback = Marshal.GetDelegateForFunctionPointer<IdeCreatePopupItem>(function);
                    break;
                case GET_POPUP_OBJECT_CALLBACK:
                    getPopupObjectCallback = Marshal.GetDelegateForFunctionPointer<IdeGetPopupObject>(function);
                    break;
                case GET_OBJECT_SOURCE_CALLBACK:
                    getObjectSourceCallback = Marshal.GetDelegateForFunctionPointer<IdeGetObjectSource>(function);
                    break;
                case GET_WINDOW_OBJECT_CALLBACK:
                    getWindowObjectCallback = Marshal.GetDelegateForFunctionPointer<IdeGetWindowObject>(function);
                    break;

            }
        }
    }
}

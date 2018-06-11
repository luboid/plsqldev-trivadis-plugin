using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using RGiesecke.DllExport;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace TrivadisPLSQLCop
{

    public class PlugIn
    {
        private const string PLUGIN_NAME = "Trivadis PL/SQL Cop";
        private const int PLUGIN_MENU_PREFERENCES_INDEX = 1;
        private const int PLUGIN_POPUPMENU_INDEX = 3;
        private const int PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX = 5;
        private const int PLUGIN_POPUPMENU_OBJECT_BROWSER_BOTH_INDEX = 6;

        public static int Id { get; set; }

        [DllExport("IdentifyPlugIn", CallingConvention = CallingConvention.Cdecl)]
        public static string IdentifyPlugIn(int id)
        {
            PlugIn.Id = id;
            return PLUGIN_NAME;
        }

        [DllExport("RegisterCallback", CallingConvention = CallingConvention.Cdecl)]
        public static void RegisterCallback(int index, IntPtr function)
        {
            Callbacks.RegisterCallback(index, function);
        }


        [DllExport("OnCreate", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static void OnCreate()
        {
        }

        [DllExport("OnActivate", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static void OnActivate()
        {
            Callbacks.PlugInSetting(Id, "CHARMODE", "UTF8");

            string popupMenuName = "Run " + PLUGIN_NAME;
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_INDEX - 1, "-", "PROGRAMWINDOW");//separator
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_INDEX, popupMenuName, "PROGRAMWINDOW");
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX - 1, "-", "PACKAGE");//separator
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX, popupMenuName, "PACKAGE");
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_BOTH_INDEX, popupMenuName + " (spec && body)", "PACKAGE");
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX - 1, "-", "PACKAGE BODY");//separator
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX, popupMenuName, "PACKAGE BODY");
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX - 1, "-", "FUNCTION");//separator
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX, popupMenuName, "FUNCTION");
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX - 1, "-", "PROCEDURE");//separator
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX, popupMenuName, "PROCEDURE");
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX - 1, "-", "TRIGGER");//separator
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX, popupMenuName, "TRIGGER");
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX - 1, "-", "TYPE");//separator
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX, popupMenuName, "TYPE");
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_BOTH_INDEX, popupMenuName + " (spec && body)", "TYPE");
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX - 1, "-", "TYPE BODY");//separator
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX, popupMenuName, "TYPE BODY");
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX - 1, "-", "VIEW");//separator
            Callbacks.CreatePopupItem(Id, PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX, popupMenuName, "VIEW");
        }

        static string TempDirectory
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetTempPath(), "tvdcc");
            }
        }

        static string ContentDirectory
        {
            get
            {
                return System.IO.Path.Combine(TempDirectory, "content");
            }
        }

        static void ClearDirectories()
        {
            string path = TempDirectory;
            string content = ContentDirectory;
            if (!System.IO.Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (System.IO.Directory.Exists(content))
            {
                foreach (var fx in System.IO.Directory.GetFiles(content))
                {
                    File.Delete(fx);
                }
            }
            else
            {
                Directory.CreateDirectory(content);
            }
        }
        public static void RunPLSQLCop(string title)
        {
            string html = Path.Combine(TempDirectory, "result.html");
            string check = SettingsDialog.GetTrivadisCheck(Id);
            string skip = SettingsDialog.GetTrivadisSkip(Id);

            System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo();
            p.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            p.FileName = SettingsDialog.GetTrivadisLocation(Id);
            p.Arguments = string.Format("path=\"{0}\" html=true output=\"{1}\" filter=\"({2})$\" check=\"{3}\" skip=\"{4}\"",
                ContentDirectory, html, "sql", check, skip);

            using (var f = System.Diagnostics.Process.Start(p))
            {
                f.WaitForExit();
                if (f.ExitCode == 0)
                {
                    Callbacks.ShowHtml("file:///" + html, title);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(string.Format("tvdcc exit with : {0}", f.ExitCode));
                }
            }
        }

        public static void RunCurrenWindowPLSQLCop()
        {
            try
            {
                string objectType, objectOwner, objectName, subObject;
                if (!Callbacks.GetWindowObject(out objectType, out objectOwner, out objectName, out subObject))
                {
                    return;
                }

                var items = new string[] { "PACKAGE", "PACKAGE BODY", "FUNCTION", "PROCEDURE", "TRIGGER", "TYPE", "TYPE BODY" };
                if (Array.IndexOf<string>(items, objectType) == -1)
                {
                    return;
                }

                string fileNameWithPath = null;
                string fileName = null;
                string fileNameExtension = null;

                bool saved = Callbacks.SaveFile();
                if (saved)
                {
                    fileNameWithPath = Callbacks.FileName();
                    saved = !string.IsNullOrWhiteSpace(fileNameWithPath);
                    if (saved)
                    {
                        fileName = Path.GetFileName(fileNameWithPath);
                        fileNameExtension = Path.GetExtension(fileName);
                    }
                }

                if (!saved)
                {
                    System.Windows.Forms.MessageBox.Show("Can't save content.");
                    return;
                }

                ClearDirectories();

                string copyTo = Path.Combine(ContentDirectory,
                    Path.ChangeExtension(fileName, ".sql"));

                Utils.CopyTo(fileNameWithPath, Path.Combine(ContentDirectory, copyTo), Encoding.Default /*new System.Text.UTF8Encoding(false)*/);

                RunPLSQLCop(Path.GetFileNameWithoutExtension(copyTo));

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public static void RunContextMenuPLSQLCop(int context)
        {
            try
            {
                string objectType = null, objectOwner = null, objectName = null, subObject = null;
                Callbacks.GetPopupObject(out objectType, out objectOwner, out objectName, out subObject);

                var builder = new StringBuilder();
                builder.Append(Callbacks.GetObjectSource(objectType, objectOwner, objectName).Trim());
                builder.AppendLine();
                builder.AppendLine("/");
                if (context == PLUGIN_POPUPMENU_OBJECT_BROWSER_BOTH_INDEX)
                {
                    builder.Append(Callbacks.GetObjectSource(objectType + " BODY", objectOwner, objectName).Trim());
                    builder.AppendLine();
                    builder.AppendLine("/");
                }

                ClearDirectories();

                string fileName = Path.Combine(ContentDirectory, objectName + ".sql");

                File.WriteAllText(fileName, builder.ToString(), Encoding.Default /*new System.Text.UTF8Encoding(false)*/);

                RunPLSQLCop(objectName);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        [DllExport("AfterExecuteWindow", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static void AfterExecuteWindow(int windowType, int result)
        {
            if (result != 2 || windowType != 3 || !SettingsDialog.GetTrivadisRunAfterCompile(Id))
            {
                return;
            }

            RunCurrenWindowPLSQLCop();
        }

        [DllExport("OnMenuClick", CallingConvention = CallingConvention.Cdecl)]
        public static void OnMenuClick(int index)
        {
            switch (index)
            {
                case PLUGIN_MENU_PREFERENCES_INDEX:
                    (new SettingsDialog(Id, PLUGIN_NAME)).ShowDialog();
                    break;
                case PLUGIN_POPUPMENU_INDEX:
                    RunCurrenWindowPLSQLCop();
                    break;
                case PLUGIN_POPUPMENU_OBJECT_BROWSER_INDEX:
                case PLUGIN_POPUPMENU_OBJECT_BROWSER_BOTH_INDEX:
                    RunContextMenuPLSQLCop(index);
                    break;
            }
        }

        [DllExport("CreateMenuItem", CallingConvention = CallingConvention.Cdecl)]
        public static string CreateMenuItem(int index)
        {
            if (index == PLUGIN_MENU_PREFERENCES_INDEX)
            {
                return "Tools / " + PLUGIN_NAME + " / Preferences";
            }
            else
            {
                return "";
            }
        }

        [DllExport("About", CallingConvention = CallingConvention.Cdecl)]
        public static string About()
        {
            return PLUGIN_NAME + ".";
        }

        public string Name
        {
            get
            {
                return PLUGIN_NAME;
            }

        }
    }
}

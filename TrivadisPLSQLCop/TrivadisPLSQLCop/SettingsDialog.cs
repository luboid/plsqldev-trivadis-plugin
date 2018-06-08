using System;
using System.Windows.Forms;

namespace TrivadisPLSQLCop
{
    public partial class SettingsDialog : Form
    {
        private int id;

        public SettingsDialog(int id, string name)
        {
            InitializeComponent();
            Text = name;
            this.id = id;
        }

        public static string GetTrivadisLocation(int id)
        {
            return Callbacks.GetPrefAsString(id, "", "TrivadisLocation", @"C:\tvdcc\tvdcc.cmd");
        }

        public static string GetTrivadisExtensionMap(int id)
        {
            return Callbacks.GetPrefAsString(id, "", "TrivadisExtensionMap", "pck|pks,bdy|pkb,spc|pks");
        }

        public static string GetTrivadisCheck(int id)
        {
            return Callbacks.GetPrefAsString(id, "", "TrivadisCheck", "");
        }

        public static string GetTrivadisSkip(int id)
        {
            return Callbacks.GetPrefAsString(id, "", "TrivadisSkip", "");
        }

        public static bool GetTrivadisRunAfterCompile(int id)
        {
            return Callbacks.GetPrefAsBool(id, "", "TrivadisRunAfterCompile", true);
        }

        public new bool ShowDialog()
        {
            textBox1.Text = GetTrivadisLocation(id);
            textBox2.Text = GetTrivadisExtensionMap(id);
            textBox3.Text = GetTrivadisCheck(id);
            textBox4.Text = GetTrivadisSkip(id);
            checkBox1.Checked = GetTrivadisRunAfterCompile(id);

            if (base.ShowDialog() == DialogResult.OK)
            {
                Callbacks.SetPrefAsString(id, "", "TrivadisLocation", textBox1.Text);
                Callbacks.SetPrefAsString(id, "", "TrivadisExtensionMap", textBox2.Text);
                Callbacks.SetPrefAsString(id, "", "TrivadisCheck", textBox3.Text);
                Callbacks.SetPrefAsString(id, "", "TrivadisSkip", textBox4.Text);
                Callbacks.SetPrefAsBool(id, "", "TrivadisRunAfterCompile", checkBox1.Checked);
                return true;
            }
            return false;
        }

        private void SettingsDialog_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = @"C:\tvdcc\tvdcc.cmd";
            textBox2.Text = "pck|pks,bdy|pkb,spc|pks";
            textBox3.Text = "";
            textBox4.Text = "";
            checkBox1.Checked = true;

        }
    }
}

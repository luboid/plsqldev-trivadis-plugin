using System;
using System.Windows.Forms;

namespace TrivadisPLSQLCop
{
    public partial class SettingsDialog : Form
    {
        private PlugIn plugIn;

        public SettingsDialog(PlugIn plugIn)
        {
            this.plugIn = plugIn;
            InitializeComponent();
            Text = plugIn.Name;
        }

        public new bool ShowDialog()
        {
            textBox1.Text = Callbacks.GetPrefAsString(plugIn.Id, "", "TrivadisLocation", "");
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                Callbacks.SetPrefAsString(plugIn.Id, "", "TrivadisLocation", @"C:\tvdcc\tvdcc.cmd");
                textBox1.Text = Callbacks.GetPrefAsString(plugIn.Id, "", "TrivadisLocation", "");
            }
            textBox2.Text = Callbacks.GetPrefAsString(plugIn.Id, "", "TrivadisExtensionMap", "");
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                Callbacks.SetPrefAsString(plugIn.Id, "", "TrivadisExtensionMap", "pck|pks,bdy|pkb,spc|pks");
                textBox2.Text = Callbacks.GetPrefAsString(plugIn.Id, "", "TrivadisExtensionMap", "");
            }
            textBox3.Text = Callbacks.GetPrefAsString(plugIn.Id, "", "TrivadisCheck", "");
            textBox4.Text = Callbacks.GetPrefAsString(plugIn.Id, "", "TrivadisSkip", "");
            checkBox1.Checked = Callbacks.GetPrefAsBool(plugIn.Id, "", "TrivadisRunAfterCompile", true);

            if (base.ShowDialog() == DialogResult.OK)
            {
                Callbacks.SetPrefAsString(plugIn.Id, "", "TrivadisLocation", textBox1.Text);
                Callbacks.SetPrefAsString(plugIn.Id, "", "TrivadisExtensionMap", textBox2.Text);
                Callbacks.SetPrefAsString(plugIn.Id, "", "TrivadisCheck", textBox3.Text);
                Callbacks.SetPrefAsString(plugIn.Id, "", "TrivadisSkip", textBox4.Text);
                Callbacks.SetPrefAsBool(plugIn.Id, "", "TrivadisRunAfterCompile", checkBox1.Checked);
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

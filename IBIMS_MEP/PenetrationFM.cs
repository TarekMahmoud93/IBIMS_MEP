using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IBIMS_MEP
{
    public partial class PenetrationFM : Form
    {
        public List<string> linksnames = new List<string>();
        public List<int> linksinds = new List<int>();
        public bool sel;
        public PenetrationFM()
        {
            InitializeComponent();
        }

        private void Form9_Load(object sender, EventArgs e)
        {
            foreach (string s in linksnames)
            {
                checkedListBox1.Items.Add(s, true);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (int i in checkedListBox1.CheckedIndices)
            {
                linksinds.Add(i);
            }
            sel = checkBox1.Checked;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            sel=checkBox1.Checked;
        }
    }
}

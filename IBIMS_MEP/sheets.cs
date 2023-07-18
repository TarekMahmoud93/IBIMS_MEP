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
    public partial class sheets : Form
    {
        public List<string> shets= new List<string>();
        public int i;
        public sheets()
        {
            InitializeComponent();
        }

        private void sheets_Load(object sender, EventArgs e)
        {
            foreach(string s in shets) { comboBox1.Items.Add(s); }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            i= comboBox1.SelectedIndex;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

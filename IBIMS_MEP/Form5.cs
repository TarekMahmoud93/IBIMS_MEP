using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IBimsAddins
{
    public partial class Form5 : Form
    {
        public List<string> parents, templates,viewports,titleblocks;
        public int np, sc,perntid,templtid,tbid;
        public string pnam, pfx,lvlcod,syscod,ss,RN,D;

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        public bool sheets, templ;
        public List<int> inds=new List<int>();
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) { comboBox1.Enabled = true; templ = true; comboBox1.SelectedItem = templates[0]; comboBox1.SelectedIndex = 0; }
            else { comboBox1.Enabled = false; templ = false; comboBox1.SelectedItem = ""; }
        }


        public Form5()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            np = Convert.ToInt32(domainUpDown1.SelectedItem);
            try
            {
                sc = Convert.ToInt32(textBox5.Text);
            }
            catch { sc = 50; }
            if (textBox6.Text != null) { pnam = textBox6.Text; }
            else { pnam = "Part"; }
            if (templ) { templtid = comboBox1.SelectedIndex; }
            if (sheets)
            {
                tbid = comboBox3.SelectedIndex;
                perntid = comboBox2.SelectedIndex;
                pfx = textBox1.Text;
                lvlcod = textBox2.Text;
                syscod = textBox4.Text;
                ss = textBox3.Text;
                RN = textBox7.Text;
                D = textBox8.Text;
                foreach (int i in checkedListBox1.CheckedIndices)
                {
                    inds.Add(i);
                }
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) { panel1.Enabled = true; sheets = true; }
            else { panel1.Enabled = false; sheets = false; }
        }
        private void Form5_Load(object sender, EventArgs e)
        {
            foreach (string p in parents)
            {
                comboBox2.Items.Add(p);
            }
            foreach (string p in templates)
            {
                comboBox1.Items.Add(p);
            }
            foreach (string p in viewports)
            {
                checkedListBox1.Items.Add(p);
            }
            foreach (string p in titleblocks)
            {
                comboBox3.Items.Add(p);
            }
            for (int i = 2; i < 7; i++)
            {
                domainUpDown1.Items.Add(i);
            }
            domainUpDown1.SelectedItem = 4;sheets = true;
            comboBox1.Enabled = false;
            comboBox2.SelectedItem = parents[0]; comboBox2.SelectedIndex = 0;
            comboBox3.SelectedItem = titleblocks[0]; comboBox3.SelectedIndex = 0;
            textBox8.Text = DateTime.Now.ToString("dd/MM/yy");
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

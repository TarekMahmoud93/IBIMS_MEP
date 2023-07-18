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
    public partial class Pdfrnm : Form
    {

        public List<string> sheets = new List<string>(); 
        public List<int> inds = new List<int>();
        public Pdfrnm()
        {
            InitializeComponent();
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            button3.DialogResult = DialogResult.Cancel;
            foreach (string v in sheets)
            {
                checkedListBox1.Items.Add(v);
            }

            button2.DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            foreach (int i in checkedListBox1.CheckedIndices)
            {
                inds.Add(i);
            }

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        private void button3_Click(object sender, EventArgs e)
        {
            button3.DialogResult = DialogResult.Cancel;
        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            

        }
    }
}
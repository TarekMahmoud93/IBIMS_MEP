using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;

namespace IBIMS_MEP
{
    public partial class sheetcopyform : Form
    {
        public IList<Document> docs = new List<Document>();
        public List<string> elementsnames = new List<string>() {"Sheets Parameters","Detail Lines","Text Notes","Schaduals","Legends"};
        
        public sheetcopyform()
        {
            InitializeComponent();
        }

        private void sheetcopyform_Load(object sender, EventArgs e)
        {
            foreach(string s in elementsnames) { elements.Items.Add(s,true); }
            comboBox1.SelectedIndex = 0; comboBox1.Text = docs[0].Title;
        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
                
        }

        private void button3_Click(object sender, EventArgs e) // check all sheets
        {
            for(int i=0;i<sheets.Items.Count;i++)
            {
                sheets.SetItemChecked(i, true);
            }
        }

        private void button4_Click(object sender, EventArgs e)// check none sheets
        {
            for (int i = 0; i < sheets.Items.Count; i++)
            {
                sheets.SetItemChecked(i, false);
            }
        }

        private void button5_Click(object sender, EventArgs e)// check all elements
        {
            for (int i = 0; i < elements.Items.Count; i++)
            {
                elements.SetItemChecked(i, true);
            }
        }

        private void button6_Click(object sender, EventArgs e) // check none elements
        {
            for (int i = 0; i < elements.Items.Count; i++)
            {
                elements.SetItemChecked(i, false);
            }
        }
    }
}

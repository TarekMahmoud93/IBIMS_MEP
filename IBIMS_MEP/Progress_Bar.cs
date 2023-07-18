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
    public partial class Fpb : Form
    {
        public int ipb = 0;
        public Fpb()
        {
            InitializeComponent();
        }

        private void Progress_Bar_Load(object sender, EventArgs e)
        {
            pbr.Value = ipb;
            
        }
    }
}

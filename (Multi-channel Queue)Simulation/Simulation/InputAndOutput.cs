using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulation
{
    public partial class InputAndOutput : Form
    {
        public InputAndOutput()
        {
            InitializeComponent();
        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void RUN_Click(object sender, EventArgs e)
        {
            try
            {
                Input();//Fill 3 input tables and be ready for output
                Output();//Fill output table ,result ang graph
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }
    }
}

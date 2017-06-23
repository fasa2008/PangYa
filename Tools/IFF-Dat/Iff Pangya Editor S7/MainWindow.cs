using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Iff_Pangya_Editor
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new LangageEditor().Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new Desc_Editor().Show();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Arya.HelperClasses;

namespace Arya
{
    public partial class FrmIntersection : Form
    {

        private TreeNode TopNode;
        public FrmIntersection()
        {
            InitializeComponent();
            listView1.Columns.Add("Tree Level",150);
            TopNode = new TreeNode(AryaTools.Instance.InstanceData.CurrentProject.ToString());
            //InitData();
        }

       

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Add(comboBoxField.Text);
        }

        private void FrmIntersection_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TreeNode Node;


            //Node.Nodes.Add(

           
        }



            //if (listView1.Items[0].Text == "Taxonomy")
            //{
            //    TreeNode TopNode = new TreeNode(AryaTools.Instance.InstanceData.CurrentProject.ToString());

            //    var nodes = AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.Where(a=> a.SkuInfos.Any(b=>b.Active)).ToList();

            //    TopNode.Nodes.Add(

            //    nodes.ForEach(n => treeView.Nodes.Add(n.ToString()+"("+n.SkuCount+")"));


            //}
        }
    }


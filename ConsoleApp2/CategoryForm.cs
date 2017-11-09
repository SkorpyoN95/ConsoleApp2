using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp2
{
    public partial class CategoryForm : Form
    {
        public ProdContext bContext;
        private string user;
        public Dictionary<Product, int> bucket = new Dictionary<Product, int>();
        decimal total;
        public CategoryForm()
        {
            InitializeComponent();
        }

        public void Load()
        {
            bContext = new ProdContext();
            bContext.Categories.Load();
            this.categoryBindingSource1.DataSource = bContext.Categories.Local.ToBindingList();
        }

        private void categoryDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int categoryID = getSelectedCategoryID(e.RowIndex);
            
            List<Product> products = bContext.Products.Where(product => product.CategoryID == categoryID).ToList();


            var bl = new BindingList<Product>(products);
            this.productsDataGridView1.DataSource = bl;
            this.productsDataGridView1.Refresh();
        }

        private void bindingNavigatorCountItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            bContext.SaveChanges();
            this.categoryDataGridView1.Refresh();
        }

        private int getSelectedCategoryID(int categoryRow)
        {
            DataGridViewRow row = categoryDataGridView1.Rows[categoryRow];
            int categoryID = Int32.Parse(row.Cells[0].Value.ToString());
            return categoryID;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var query = from customer in bContext.Customers
                        where customer.CompanyName == textBox1.Text
                        select customer.CompanyName;
            if(query.Count() == 1)
            {
                panel3.Hide();
                panel4.Show();
                user = textBox1.Text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Customer customer = new Customer { CompanyName = textBox2.Text };
            bContext.Customers.Add(customer);
            try
            {
                bContext.SaveChanges();
            }
            catch
            {
                MessageBox.Show("You are already registered!", "Register error", MessageBoxButtons.OK);
            }
        }

        private void productsDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                DataGridViewRow row = productsDataGridView1.Rows[e.RowIndex];
                int id = Int32.Parse(row.Cells["dataGridViewTextBoxColumn7"].Value.ToString());
                Product product = (from prod in bContext.Products
                            where prod.ProductId == id
                            select prod).FirstOrDefault();

                if (!bucket.ContainsKey(product))
                {
                    bucket[product] = 0;
                }

                bucket[product]++;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var bl = new BindingList<Product>(bucket.Keys.ToList());
            this.productsDataGridView2.DataSource = bl;
            this.productsDataGridView2.Refresh();
            panel4.Hide();
            panel5.Show();
            total = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool valid = true;
            foreach(Product prod in bucket.Keys.ToList())
            {
                if (prod.UnitsInStock < bucket[prod]) valid = false;
            }

            if (valid)
            {
                foreach (Product prod in bucket.Keys.ToList())
                {
                    bContext.Orders.Add(new Order
                    {
                        CustomerID = this.user,
                        ProductID = prod.ProductId,
                        Quantity = bucket[prod]
                    });
                    prod.UnitsInStock -= bucket[prod];
                }
                bContext.SaveChanges();
                MessageBox.Show("Your order has been completed!", "Success!", MessageBoxButtons.OK);
            }
            else
                MessageBox.Show("Your order cannot be completed! Some products are not availaible!", "Failure!", MessageBoxButtons.OK);

            panel5.Hide();
            panel4.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel5.Hide();
            panel4.Show();
            bucket = new Dictionary<Product, int>();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            panel5.Hide();
            panel4.Show();
        }

        private void productsDataGridView2_OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            if (e.ColumnIndex == productsDataGridView2.Columns["quantityColumn"].Index)
            {
                DataGridViewRow row = productsDataGridView2.Rows[e.RowIndex];
                if (row.Cells["dataGridViewTextBoxColumn12"].Value != null)
                { 
                    int id = Int32.Parse(row.Cells["dataGridViewTextBoxColumn12"].Value.ToString());
                    Product product = (from prod in bContext.Products
                                       where prod.ProductId == id
                                       select prod).FirstOrDefault();
                    e.Value = string.Format("{0}", bucket[product]);
                    e.FormattingApplied = true;
                    if (product.UnitsInStock < bucket[product])
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }

            if (e.ColumnIndex == productsDataGridView2.Columns["priceColumn"].Index)
            {
                DataGridViewRow row = productsDataGridView2.Rows[e.RowIndex];
                if (row.Cells["dataGridViewTextBoxColumn12"].Value != null)
                {
                    int id = Int32.Parse(row.Cells["dataGridViewTextBoxColumn12"].Value.ToString());
                    Product product = (from prod in bContext.Products
                                       where prod.ProductId == id
                                       select prod).FirstOrDefault();
                    e.Value = string.Format("{0}", product.Unitprice * bucket[product]);
                    e.FormattingApplied = true;
                    total += product.Unitprice * bucket[product];
                    textBox3.Text = total.ToString();
                }
            }
        }

    }
}

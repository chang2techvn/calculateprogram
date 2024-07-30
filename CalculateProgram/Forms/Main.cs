using System;
using System.Drawing;
using System.Windows.Forms;
using CalculateProgram.Modules;

namespace CalculateProgram
{
    public partial class Main : Form
    {
        // Constructor to initialize the form and context menu
        public Main()
        {
            InitializeComponent();
            CreateHeadCol();


            // Add sorting options to the context menu
            contextMenuStrip1.Items.Add("Name (A-Z)");
            contextMenuStrip1.Items.Add("Consumption (Low to High)");
            contextMenuStrip1.Items.Add("Consumption (High to Low)");
            contextMenuStrip1.Items.Add("Total Bill (Low to High)");
            contextMenuStrip1.Items.Add("Total Bill (High to Low)");
        }

        // Create columns for the ListView
        private void CreateHeadCol()
        {
            lvResult.View = View.Details;
            lvResult.Columns.Add("No.", 50);
            lvResult.Columns.Add("ID", 50);
            lvResult.Columns.Add("Name", 50);
            lvResult.Columns.Add("Last Month", 70);
            lvResult.Columns.Add("This Month", 70);
            lvResult.Columns.Add("Consumption", 80);
            lvResult.Columns.Add("Price", 70);
            lvResult.Columns.Add("Total Bill", 100);
        }

        // Calculate bill and display results in ListView and Showbill form
        private void CalculateBtt_Click(object sender, EventArgs e)
        {
            string id = IdCusTxtb.Text;
            string name = NameCusTxtb.Text;

            if (id == "PE12345678901,...")
            {
                MessageBox.Show("Please enter your ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (name == "Charles Darwin,...")
            {
                MessageBox.Show("Please enter your Name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (TypeOfCustomerTxtb.Text == "Household customer, Administrative agency, public services, ")
            {
                MessageBox.Show("Please choose a type of customer!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Parse input values
            if (int.TryParse(LMonthTxtb.Text, out int lastMonth) && int.TryParse(ThisMonthtxtb.Text, out int thisMonth))
            {
                if (thisMonth <= lastMonth)
                {
                    MessageBox.Show("You have entered an invalid amount of water consumed.");
                    return;
                }

                int consumption = thisMonth - lastMonth;
                string customerType = TypeOfCustomerTxtb.SelectedItem.ToString();
                double price;
                double totalBill;

                // Calculate price and total bill based on customer type
                if (customerType == "Household customer")
                {
                    if (int.TryParse(numberPeoplerTxtb.Text, out int numberOfPeople))
                    {
                        (price, totalBill) = CalculationModule.CalculateBill(customerType, consumption, numberOfPeople);
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid number for the number of people.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    (price, totalBill) = CalculationModule.CalculateBill(customerType, consumption, 1);
                }

                // Add results to ListView
                lvResult.Items.Clear();
                int index = lvResult.Items.Count + 1;
                ListViewItem item = new ListViewItem(index.ToString())
                {
                    SubItems =
                    {
                        new ListViewItem.ListViewSubItem(null, id),
                        new ListViewItem.ListViewSubItem(null, name),
                        new ListViewItem.ListViewSubItem(null, lastMonth.ToString() + " m3"),
                        new ListViewItem.ListViewSubItem(null, thisMonth.ToString() + " m3"),
                        new ListViewItem.ListViewSubItem(null, consumption.ToString() + " m3"),
                        new ListViewItem.ListViewSubItem(null, price.ToString("0.00") + " đ/m3"),
                        new ListViewItem.ListViewSubItem(null, totalBill.ToString("0.00") + " đ")
                    }
                };
                lvResult.Items.Add(item);

                // Resize columns
                for (int i = 0; i < lvResult.Columns.Count; i++)
                {
                    lvResult.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                    lvResult.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                // Save results to file
                string filePath = "Results.txt";
                FileModule.SaveResultsToFile(filePath, lvResult);

                // Show detailed billing information in a separate form
                Showbill showbillForm = new Showbill(
                    id,
                    name,
                    lastMonth.ToString() + " m3",
                    thisMonth.ToString() + " m3",
                    consumption.ToString() + " m3",
                    price.ToString("0.00") + " đ/m3",
                    totalBill.ToString("0.00") + " đ"
                );
                showbillForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please enter valid numbers for last month and this month readings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // Search for results in ListView and file
        private void searchBtt_Click(object sender, EventArgs e)
        {
            string searchTerm = SearchTxtbox.Text;
            string filePath = "Results.txt";

            SearchModule.SearchAndDisplayResults(lvResult, searchTerm, filePath);
        }

        // Handle context menu item clicks for sorting
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int columnIndex = -1;
            bool ascending = true;

            string selectedItem = e.ClickedItem.Text;

            // Determine column index and sort order based on selected menu item
            if (selectedItem == "Name (A-Z)")
            {
                columnIndex = 2;
                ascending = true;
            }
            else if (selectedItem == "Consumption (Low to High)")
            {
                columnIndex = 5;
                ascending = true;
            }
            else if (selectedItem == "Consumption (High to Low)")
            {
                columnIndex = 5;
                ascending = false;
            }
            else if (selectedItem == "Total Bill (Low to High)")
            {
                columnIndex = 7;
                ascending = true;
            }
            else if (selectedItem == "Total Bill (High to Low)")
            {
                columnIndex = 7;
                ascending = false;
            }

            if (columnIndex != -1)
            {
                SortingModule.SortListView(lvResult, columnIndex, ascending);
            }
        }

        // Show context menu on PictureBox click
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(pictureBox1, new Point(0, pictureBox1.Height));
        }

        //Clear Button
        private void ClearBtt_Click(object sender, EventArgs e)
        {
            IdCusTxtb.Clear();
            LMonthTxtb.Clear();
            ThisMonthtxtb.Clear();
            NameCusTxtb.Clear();
            numberPeoplerTxtb.Clear();
            TypeOfCustomerTxtb.SelectedIndex = -1;

            SetPlaceholders();
        }

        // Set placeholder text for input fields
        private void SetPlaceholders()
        {
            IdCusTxtb.Text = "PE12345678901,...";
            IdCusTxtb.ForeColor = Color.Silver;

            LMonthTxtb.Text = " 10, 20, 30, 40, 50,...";
            LMonthTxtb.ForeColor = Color.Silver;

            ThisMonthtxtb.Text = " 10, 20, 30, 40, 50,...";
            ThisMonthtxtb.ForeColor = Color.Silver;

            NameCusTxtb.Text = "Charles Darwin,...";
            NameCusTxtb.ForeColor = Color.Silver;

            TypeOfCustomerTxtb.Text = "Household customer, Administrative agency, public services, ";
            TypeOfCustomerTxtb.ForeColor = Color.Silver;

            numberPeoplerTxtb.Text = "0, 1, 2, 3,...(If Household Customer)";
            numberPeoplerTxtb.ForeColor = Color.Silver;
        }



        // Handle placeholder text for textboxes
        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (IdCusTxtb.Text == "PE12345678901,...")
            {
                IdCusTxtb.Text = "";
                IdCusTxtb.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (IdCusTxtb.Text == "")
            {
                IdCusTxtb.Text = "PE12345678901,...";
                IdCusTxtb.ForeColor = Color.Silver;
            }
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (NameCusTxtb.Text == "Charles Darwin,...")
            {
                NameCusTxtb.Text = "";
                NameCusTxtb.ForeColor = Color.Black;
            }
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (NameCusTxtb.Text == "")
            {
                NameCusTxtb.Text = "Charles Darwin,...";
                NameCusTxtb.ForeColor = Color.Silver;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (LMonthTxtb.Text == " 10, 20, 30, 40, 50,...")
            {
                LMonthTxtb.Text = "";
                LMonthTxtb.ForeColor = Color.Black;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (LMonthTxtb.Text == "")
            {
                LMonthTxtb.Text = " 10, 20, 30, 40, 50,...";
                LMonthTxtb.ForeColor = Color.Silver;
            }
        }

        private void thisMonthtxtb_Enter(object sender, EventArgs e)
        {
            if (ThisMonthtxtb.Text == " 10, 20, 30, 40, 50,...")
            {
                ThisMonthtxtb.Text = "";
                ThisMonthtxtb.ForeColor = Color.Black;
            }
        }

        private void thisMonthtxtb_Leave(object sender, EventArgs e)
        {
            if (ThisMonthtxtb.Text == "")
            {
                ThisMonthtxtb.Text = " 10, 20, 30, 40, 50,...";
                ThisMonthtxtb.ForeColor = Color.Silver;
            }
        }

        private void typeOfCustomerTxtb_Enter(object sender, EventArgs e)
        {
            if (TypeOfCustomerTxtb.Text == "Household customer, Administrative agency, public services, ")
            {
                TypeOfCustomerTxtb.Text = "";
                TypeOfCustomerTxtb.ForeColor = Color.Black;
            }
        }

        private void typeOfCustomerTxtb_Leave(object sender, EventArgs e)
        {
            if (TypeOfCustomerTxtb.Text == "")
            {
                TypeOfCustomerTxtb.Text = "Household customer, Administrative agency, public services, ";
                TypeOfCustomerTxtb.ForeColor = Color.Silver;
            }
        }

        private void numberPeoplerTxtb_Enter(object sender, EventArgs e)
        {
            if (numberPeoplerTxtb.Text == "0, 1, 2, 3,...(If Household Customer)")
            {
                numberPeoplerTxtb.Text = "";
                numberPeoplerTxtb.ForeColor = Color.Black;
            }
        }

        private void numberPeoplerTxtb_Leave(object sender, EventArgs e)
        {
            if (numberPeoplerTxtb.Text == "")
            {
                numberPeoplerTxtb.Text = "0, 1, 2, 3,...(If Household Customer)";
                numberPeoplerTxtb.ForeColor = Color.Silver;
            }
        }

        private void SearchTxtbox_Enter(object sender, EventArgs e)
        {
            if (SearchTxtbox.Text == "Pe12345, John, 10 m3,...")
            {
                SearchTxtbox.Text = "";
                SearchTxtbox.ForeColor = Color.Black;
            }
        }

        private void SearchTxtbox_Leave(object sender, EventArgs e)
        {
            if (SearchTxtbox.Text == "")
            {
                SearchTxtbox.Text = "Pe12345, John, 10 m3,...";
                SearchTxtbox.ForeColor = Color.Silver;
            }
        }
    }
}





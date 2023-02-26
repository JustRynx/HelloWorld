using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MySql.Data.MySqlClient;
using System.IO;
using System.Security.Cryptography;


namespace remakeITS
{
    public partial class Main : Form
    {
        Login loginform; KitchenDP kd;

        MySqlConnection cn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["remakeITS.Properties.Settings.ordersysConnectionString"].ConnectionString);
        //This will allow the connection string to read the data from the database even if you re-run the application, which I manually added/stored in the client's application settings. 
        //(Note that there are also other ways to connect this application to the database.)

        MySqlCommand cmd = new MySqlCommand();

        string currentImage = @"..\\Icon\\no-image-box.png";

        string datetimeHMS;

        // ------------------------------
        string oldproduct, olddesc, oldcateg, olduprice;
        // ------------------------------
        string oldcategmain = "";
        // ------------------------------
        string oldcategforstatus = "";
        // ------------------------------
        string oldfirstname, oldlastname, oldaddress, oldphonenumber, oldusername, oldpassword, oldusertype, olduserid;

        double grosssales = 0, netsales = 0;
       
        public Main()
        {

            InitializeComponent();
            system_timer.Start(); //Start system timer.
           
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.ContainerControl |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.SupportsTransparentBackColor
                          , true);

            kd = new KitchenDP(); //Call KD Form at the start of the application.

            // - Setting Dates --------------------------
            DateTime dateTime = DateTime.Now;
            this.dtp_auditStart.Value = dateTime; holder_dtpauditStart.Text = dateTime.ToString("MMM dd, yyyy");
            this.dtp_auditEnd.Value = dateTime; holder_dtpauditEnd.Text = dateTime.ToString("MMM dd, yyyy");

            this.dtp_orderhStart.Value = dateTime; holder_dtpOHStart.Text = dateTime.ToString("MMM dd, yyyy");
            this.dtp_orderhEnd.Value = dateTime; holder_dtpOHEnd.Text = dateTime.ToString("MMM dd, yyyy");

            this.dtp_salesStart.Value = dateTime; holder_dtpsalesStart.Text = dateTime.ToString("MMM dd, yyyy");
            this.dtp_salesEnd.Value = dateTime; holder_dtpsalesEnd.Text = dateTime.ToString("MMM dd, yyyy");

            txtgrossSales.Text = "Total Gross Sales"; txtnetSales.Text = "Total Net Sales";
            // ----------------------------------------------------------

            dgv_orderList.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv_orderList.Columns[2].HeaderCell.Style.Padding = new Padding(0, 0, 2, 0);
            dgv_orderList.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv_orderList.Columns[3].HeaderCell.Style.Padding = new Padding(0, 0, 2, 0);

            
            
            
            //dgv_orderList.Columns[2].HeaderCell.Style.Padding.Right = 5;
           // receiptBindingSource1.DataSource = new List<Receipt>();

            //pa_order.Visible = true;

            //pa_addprod.Visible = false;
           // pa_users.Visible = false;
           // pa_categ.Visible = false;
           // pa_cstmDscnt.Visible = false;
          //  pa_viewprod.Visible = false;
          //  pa_viewuser.Visible = false;
          //  pa_viewcateg.Visible = false;
          //  pa_audittrail.Visible = false;
         //   pa_backrestore.Visible = false;
          //  pa_orderhistory.Visible = false;
          //  pa_sales.Visible = false;

            //Try to check the Microsoft's rules for your program  if its valid. You can check it at the client's properties then Code Analysis. 
            //Also check if its gonna manually check your entire code or will just say the valid rules of a good application.

           
        }

        private void system_timer_Tick(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.lbl_dateOrder.Text = dateTime.ToString("yyyy-MM-dd"); //Current Time Order
            this.lb_dateDisplay.Text = dateTime.ToString(); //Current System Date
            this.datetimeHMS = dateTime.ToString("HH:mm:ss"); //Current System Date Time
        }

        private void Main_Load(object sender, EventArgs e)
        {
            fillcategcmbbox();

            viewProductList();
            viewCategList();
            viewUserList();
            viewAudittrail();
            viewOrderhistory();
            viewSales();

            lbl_orderNo.Text = GetRandomKey(5);

            orderIDIncre();
            prodIDIncre();
            categIDIncre();
            userIDIncre();

            getcustomDiscount();

            viewgrossrevenue();
            viewtoptimeorder();
            viewtopdaysorder();
            viewtopproduct();

      
            //Create a timeline base of deleting all or atleast delete becomes available for each item from the ordered database (ex. delete becomes available when the item/s is/are 10 years old from the time it created)

            receiptBindingSource.DataSource = new List<Receipt>(); //- Init Empty List --

            if (lb_userlvl.Text == "Cashier Name :")
            {
                addMenuItem.Visible = false;
                categMenuItem.Visible = false;
                userMenuItem.Visible = false;

                view_Strip.Enabled = false;
                report_Strin.Enabled = false;
                tool_Strip.Enabled = false;

                pa_order.Visible = true;
                pa_addprod.Visible = false;
                pa_users.Visible = false;
                pa_categ.Visible = false;
                pa_viewprod.Visible = false;
                pa_viewcateg.Visible = false;
                pa_viewuser.Visible = false;
                pa_sales.Visible = false;
                pa_orderhistory.Visible = false;
                pa_backrestore.Visible = false;
                pa_audittrail.Visible = false;

            }
            else if (lb_userlvl.Text == "Admin Name :" && pa_order.Enabled == true)
            {
                menu_Strip.Enabled = true;

                //UserType
                cb_usertype.Items.Add("Admin");
                cb_usertype.Items.Add("Cashier");

                backupToolStripMenuItem.Enabled = false;
                auditTrailToolStripMenuItem.Enabled = false;
            }
            else if (lb_userlvl.Text == "Owner Name :" && pa_order.Enabled == true)
            {
                menu_Strip.Enabled = true;

                //UserType
                cb_usertype.Items.Add("Owner");
                cb_usertype.Items.Add("Admin");
                cb_usertype.Items.Add("Cashier");
                
            }
        
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (pa_order.Visible == true && pa_order.Enabled == true)
            {
                if (e.KeyCode == Keys.F1)
                {
                    ProcessOrder();
                }
                else if (e.KeyCode == Keys.F3)
                {
                    removefrmCart();
                }
                else if (e.KeyCode == Keys.F4)
                {
                    CancelOrder();
                }
                else if (e.KeyCode == Keys.F5)
                {
                    applyDiscount();
                }
                else if (e.KeyCode == Keys.F6)
                {
                    UndoOrder();   
                }
             
            }
        }

        public void viewgrossrevenue()
        {
            //Opens the connection between application and the database, so that I can modify the data in and out.
            cn.Open();
            cmd.Connection = cn;

            cmd.CommandText = "SELECT date_format(orderdate, '%M/%d/%Y') AS Date, sum(totalAmt) AS TotalAmount, sum(discount) AS Discount FROM ordersys.tbl_ordermain Group by orderdate;";
            
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);

            BindingSource bSource = new BindingSource();
            bSource.DataSource = dt;

            chart2.DataSource = bSource;

            chart2.Series[0].XValueMember = "Date"; //key
            chart2.Series[0].YValueMembers = "TotalAmount"; //value

            chart2.Series[1].XValueMember = "Date"; //key
            chart2.Series[1].YValueMembers = "Discount"; //value

            txtlbtotalgross.Text = grosssales.ToString("₱ #,###,##0.00");
            txtlbtotalnet.Text = netsales.ToString("₱ #,###,##0.00");

            da.Update(dt);
            cn.Close();
            
        }

        public void viewtopdaysorder()
        {
            //Opens the connection between application and the database, so that I can modify the data in and out.
            cn.Open();
            cmd.Connection = cn;

            cmd.CommandText = "SELECT Count(*) AS Numoforders, date_format(orderdate, '%a') AS Days FROM ordersys.tbl_ordermain GROUP BY Days ORDER BY date_format(orderdate, '%w');";

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);

            BindingSource bSource = new BindingSource();
            bSource.DataSource = dt;

            chart1.DataSource = bSource;


            chart1.Series[0].XValueMember = "Days"; //key
            chart1.Series[0].YValueMembers = "Numoforders"; //value

            //chart1.Series[0]["PixelPointWidth"] = "150";
        
            da.Update(dt);
            cn.Close();

        }

        public void viewtoptimeorder()
        {
            //Opens the connection between application and the database, so that I can modify the data in and out.
            cn.Open();
            cmd.Connection = cn;

            cmd.CommandText = "SELECT COUNT(*) AS Numoforders, time_format(ordertime, '%h %p') AS Time, orderno, orderid FROM ordersys.tbl_ordermain GROUP BY time_format(ordertime, '%h %p') ORDER BY ordertime;";

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);

            BindingSource bSource = new BindingSource();
            bSource.DataSource = dt;

            chart4.DataSource = bSource;


            chart4.Series[0].XValueMember = "Time"; //key
            chart4.Series[0].YValueMembers = "Numoforders"; //value

            //chart1.Series[0]["PixelPointWidth"] = "150";

            da.Update(dt);
            cn.Close();

        }

        public void viewtopproduct()
        {
            //Opens the connection between application and the database, so that I can modify the data in and out.
            cn.Open();
            cmd.Connection = cn;

            cmd.CommandText = "SELECT A.prod_name AS 'Product', A.prod_ID, SUM(C.Quantity) AS Quantity, sum(C.subtotal) AS 'Sub Total' FROM tbl_product A INNER JOIN tbl_category D ON A.prod_categID = D.prod_categID INNER JOIN tbl_orderdetails C ON A.prod_ID = C.prod_ID INNER JOIN tbl_ordermain B ON B.orderID = C.orderID group by prod_name order by Quantity Desc Limit 5;";

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);

            BindingSource bSource = new BindingSource();
            bSource.DataSource = dt;

            chart3.DataSource = bSource;

            chart3.Series[0].XValueMember = "Product"; //key
            chart3.Series[0].YValueMembers = "Quantity"; //value
    
            da.Update(dt);
            cn.Close();
        }

        public void viewProductList()
        {

            //Opens the connection between application and the database, so that I can modify the data in and out.
            cn.Open();
            cmd.Connection = cn;

            cmd.CommandText = "SELECT CONCAT('P-', A.prod_ID), A.prod_name, A.prod_desc, B.prod_categname, A.unit_price, A.status, A.prod_image FROM tbl_product A INNER JOIN tbl_category B ON A.prod_categID = B.prod_categID ORDER BY prod_ID ASC;";

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);

            // - Show Products to Ordering List ----------------------------------------------------

            BindingSource bSource = new BindingSource();
            bSource.DataSource = dt;

            dgv_prodlist.DataSource = bSource;

            foreach (DataGridViewColumn column in dgv_prodlist.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                //Disabled each column sort.

                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            DataGridViewImageColumn image = new DataGridViewImageColumn();
            image = (DataGridViewImageColumn)dgv_prodlist.Columns[6];
            image.ImageLayout = DataGridViewImageCellLayout.Stretch;

            dgv_prodlist.Columns[0].Visible = false;

            dgv_prodlist.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv_prodlist.Columns[1].HeaderText = "Product Name";
            dgv_prodlist.Columns[1].Width = 200;

            dgv_prodlist.Columns[2].HeaderText = "Description";
            dgv_prodlist.Columns[2].Width = 170;

            dgv_prodlist.Columns[3].HeaderText = "Category";

            dgv_prodlist.Columns[4].HeaderText = "Unit Price";
            dgv_prodlist.Columns[4].DefaultCellStyle.Format = "N2";

            dgv_prodlist.Columns[5].HeaderText = "Status";
            dgv_prodlist.Columns[6].HeaderText = "Product Image";

            dgv_prodlist.ClearSelection();

            // - View Product Main ---------------------------------------------

            BindingSource bSource2 = new BindingSource();
            bSource2.DataSource = dt;

            dgv_prodmain.DataSource = bSource2;

            foreach (DataGridViewColumn column in dgv_prodmain.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                //Disabled each column sort.
            }

            DataGridViewImageColumn image2 = new DataGridViewImageColumn();
            image2 = (DataGridViewImageColumn)dgv_prodmain.Columns[6];
            image2.ImageLayout = DataGridViewImageCellLayout.Stretch;

            dgv_prodmain.Columns[0].HeaderText = "PID";
            dgv_prodmain.Columns[0].Width = 130;

            dgv_prodmain.Columns[1].HeaderText = "Product Name";
            dgv_prodmain.Columns[1].Width = 320;

            dgv_prodmain.Columns[2].HeaderText = "Description";
            dgv_prodmain.Columns[2].Width = 230;

            dgv_prodmain.Columns[3].HeaderText = "Category";

            dgv_prodmain.Columns[4].HeaderText = "Unit Price";
            dgv_prodmain.Columns[4].DefaultCellStyle.Format = "N2";

            dgv_prodmain.Columns[5].HeaderText = "Status";
            dgv_prodmain.Columns[6].HeaderText = "Product Image";
            dgv_prodmain.Columns[6].Width = 160;

            dgv_prodmain.ClearSelection();

            // - View Product ----------------------------------------------------

            BindingSource bSource3 = new BindingSource();
            bSource3.DataSource = dt;

            dgv_prodview.DataSource = bSource3;

            foreach (DataGridViewColumn column in dgv_prodview.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                //Disabled each column sort.
            }

            DataGridViewImageColumn image3 = new DataGridViewImageColumn();
            image3 = (DataGridViewImageColumn)dgv_prodview.Columns[6];
            image3.ImageLayout = DataGridViewImageCellLayout.Stretch;

            dgv_prodview.Columns[0].HeaderText = "PID";
            dgv_prodview.Columns[0].Width = 130;

            dgv_prodview.Columns[1].HeaderText = "Product Name";
            dgv_prodview.Columns[1].Width = 320;

            dgv_prodview.Columns[2].HeaderText = "Description";
            dgv_prodview.Columns[2].Width = 230;

            dgv_prodview.Columns[3].HeaderText = "Category";

            dgv_prodview.Columns[4].HeaderText = "Unit Price";
            dgv_prodview.Columns[4].DefaultCellStyle.Format = "N2";

            dgv_prodview.Columns[5].HeaderText = "Status";
            dgv_prodview.Columns[6].HeaderText = "Product Image";
            dgv_prodview.Columns[6].Width = 160;

            dgv_prodview.ClearSelection();

            da.Update(dt);
            cn.Close();

        }

        public void viewCategList()
        {

            cn.Open();
            cmd.Connection = cn;

            cmd.CommandText = "SELECT CONCAT('C-', prod_categID), prod_categname, status FROM tbl_category;";

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);

            ////////////////////////////////////////////////

            BindingSource bSource = new BindingSource();
            bSource.DataSource = dt;

            dgv_viewcateg.DataSource = bSource;

            foreach (DataGridViewColumn column in dgv_viewcateg.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                //Disabled each column sort.

            }
            
            dgv_viewcateg.Columns[0].HeaderText = "CID";
            dgv_viewcateg.Columns[0].Width = 130;

            dgv_viewcateg.Columns[1].HeaderText = "Category";
            dgv_viewcateg.Columns[2].HeaderText = "Status";

            dgv_viewcateg.ClearSelection();

            ////////////////////////////////////////

            BindingSource bSource1 = new BindingSource();
            bSource1.DataSource = dt;

            dgv_categlist.DataSource = bSource1;


            foreach (DataGridViewColumn column in dgv_categlist.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                //Disabled each column sort.

            }

            dgv_categlist.Columns[0].HeaderText = "CID";
            dgv_categlist.Columns[0].Width = 130;

            dgv_categlist.Columns[1].HeaderText = "Category";
            dgv_categlist.Columns[2].HeaderText = "Status";

            dgv_categlist.ClearSelection();

            ////////////////////////////////////////

            da.Update(dt);
            cn.Close();

        }

        public void viewUserList()
        {

            cn.Open();
            cmd.Connection = cn;

            cmd.CommandText = "SELECT CONCAT('U-', UserID), FirstName, LastName, Address, PhoneNumber, SHA1(Username), SHA1(Password), UserType FROM tbl_login;";

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);

            // - View User ---------------------------------------------------

            BindingSource bSource = new BindingSource();
            bSource.DataSource = dt;

            dgv_viewuser.DataSource = bSource;

            foreach (DataGridViewColumn column in dgv_viewuser.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                //Disabled each column sort.
            }

            dgv_viewuser.Columns[0].HeaderText = "UID";
            dgv_viewuser.Columns[0].Width = 130;

            dgv_viewuser.Columns[1].HeaderText = "First Name";
            dgv_viewuser.Columns[2].HeaderText = "Last Name";
            dgv_viewuser.Columns[3].HeaderText = "Address";
            dgv_viewuser.Columns[4].HeaderText = "Phone Number";
            dgv_viewuser.Columns[5].HeaderText = "Username";
            dgv_viewuser.Columns[6].HeaderText = "Password";
            dgv_viewuser.Columns[7].HeaderText = "User Type";

            dgv_viewuser.ClearSelection();

            // - View User Main ---------------------------------------

            BindingSource bSource1 = new BindingSource();
            bSource1.DataSource = dt;

            dgv_userlist.DataSource = bSource1;

            foreach (DataGridViewColumn column in dgv_userlist.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                //Disabled each column sort.
            }

            dgv_userlist.Columns[0].HeaderText = "UID";
            dgv_userlist.Columns[0].Width = 130;

            dgv_userlist.Columns[1].HeaderText = "First Name";
            dgv_userlist.Columns[2].HeaderText = "Last Name";
            dgv_userlist.Columns[3].HeaderText = "Address";
            dgv_userlist.Columns[4].HeaderText = "Phone Number";
            dgv_userlist.Columns[5].HeaderText = "Username";
            dgv_userlist.Columns[6].HeaderText = "Password";
            dgv_userlist.Columns[7].HeaderText = "User Type";

            dgv_userlist.ClearSelection();

            ////////////////////////////////////////

            da.Update(dt);
            cn.Close();

        }

        public void viewAudittrail()
        {

            cn.Open();
            cmd.Connection = cn;

            //Try to limit the data that can load to the dgv (Ex. Dgv can hold up to 20 data per load, then load the next data when scrolled.) (Not Done)
            cmd.CommandText = "SELECT User, Activity, Action, Time_Format(Time, '%r') As Time, Date FROM tbl_audittrail ORDER BY auditID DESC;";

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);

            ////////////////////////////////////////////////

            BindingSource bSource = new BindingSource();
            bSource.DataSource = dt;

            dgv_audit.DataSource = bSource;

            foreach (DataGridViewColumn column in dgv_audit.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                //Disabled each column sort.
            }

            dgv_audit.Columns[0].Width = 230;
            dgv_audit.Columns[1].Width = 700;
     
            dgv_audit.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv_audit.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv_audit.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgv_audit.ClearSelection();

            ////////////////////////////////////////

            da.Update(dt);
            cn.Close();

        }

        public void viewOrderhistory()
        {

            cn.Open();
            cmd.Connection = cn;

            //Try to limit the data that can load to the dgv (Ex. Dgv can hold up to 20 data per load, then load the next data when scrolled.) (Not Done)
            cmd.CommandText = "SELECT B.orderNo, A.prod_name AS 'Product Name', D.prod_categname AS 'Category', C.Quantity, C.unitprice AS 'Unit Price', C.subtotal AS 'Sub Total', Time_Format(B.ordertime, '%r') AS 'Order Time', B.orderdate AS 'Order Date' FROM tbl_product A INNER JOIN tbl_category D ON A.prod_categID = D.prod_categID INNER JOIN tbl_orderdetails C ON A.prod_ID = C.prod_ID INNER JOIN tbl_ordermain B ON B.orderID = C.orderID ORDER BY recNo DESC;";

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);

            ////////////////////////////////////////////////

            BindingSource bSource = new BindingSource();
            bSource.DataSource = dt;

            dgv_orderhistor.DataSource = bSource;

            foreach (DataGridViewColumn column in dgv_orderhistor.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                //Disabled each column sort.
            }

            dgv_orderhistor.Columns[0].HeaderText = "Order No.";
            dgv_orderhistor.Columns[4].DefaultCellStyle.Format = "N2";
            dgv_orderhistor.Columns[5].DefaultCellStyle.Format = "N2";


            dgv_orderhistor.ClearSelection();

            ////////////////////////////////////////

            da.Update(dt);
            cn.Close();

        }

        public void viewSales()
        {

            cn.Open();
            cmd.Connection = cn;

            //Try to limit the data that can load to the dgv (Ex. Dgv can hold up to 20 data per load, then load the next data when scrolled.) (Not Done)
            cmd.CommandText = "SELECT orderno AS 'Order Number', discount AS 'Discount', totalAmt AS 'Total Amount', Time_Format(ordertime, '%r') AS 'Order Time', orderdate AS 'Order Date' FROM tbl_ordermain ORDER BY orderID DESC;";

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);

            ////////////////////////////////////////////////

            BindingSource bSource = new BindingSource();
            bSource.DataSource = dt;

            dgv_salesmain.DataSource = bSource;

            foreach (DataGridViewColumn column in dgv_salesmain.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                //Disabled each column sort.
            }
            
            dgv_salesmain.Columns[1].DefaultCellStyle.Format = "N2";
            dgv_salesmain.Columns[2].DefaultCellStyle.Format = "N2";

            double calculatesales = 0;

            for (int o = 0; o < dgv_salesmain.RowCount; o++)
            {
                double totamt = calculatesales + Double.Parse(dgv_salesmain.Rows[o].Cells[2].Value.ToString());
                netsales = totamt;

                calculatesales = netsales;
                lb_netsales.Text = netsales.ToString("₱ #,###,##0.00");

                lbtxt_numberoford.Text = Convert.ToString(o + 1);
            }

            for (int o = 0; o < dgv_salesmain.RowCount; o++)
            {
                double totamt = calculatesales + Double.Parse(dgv_salesmain.Rows[o].Cells[1].Value.ToString());
                grosssales = totamt;

                calculatesales = grosssales;
                lb_grosssales.Text = grosssales.ToString("₱ #,###,##0.00");
            }

            dgv_salesmain.ClearSelection();

            ////////////////////////////////////////

            da.Update(dt);
            cn.Close();

        }

        public void insertOrderDtls()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.Parameters.Clear();

                for (int i = 0; i < dgv_orderList.RowCount; i++)
                {
                    cmd.CommandText = "INSERT INTO tbl_orderdetails (orderID,prod_ID,Quantity,unitprice,subtotal) VALUES (@orderID,@prod_ID,@Quantity,@unitprice,@subtotal);";

                    cmd.Parameters.AddWithValue("@orderID", orderIDMax);
                    cmd.Parameters.AddWithValue("@prod_ID", dgv_orderList.Rows[i].Cells[4].Value.ToString().Replace("P-", ""));
                    cmd.Parameters.AddWithValue("@Quantity", dgv_orderList.Rows[i].Cells[0].Value);
                    cmd.Parameters.AddWithValue("@unitprice", Convert.ToDouble(dgv_orderList.Rows[i].Cells[2].Value.ToString().Replace("₱ ", "")));
                    cmd.Parameters.AddWithValue("@subtotal", Convert.ToDouble(dgv_orderList.Rows[i].Cells[3].Value.ToString().Replace("₱ ", "")));

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }  
            finally { cn.Close(); }
        }

        public void updProdtoActive()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                int RowsAffected = 0;

                for (int i = 0; i < dgv_orderList.RowCount; i++)
                {
                    cmd.CommandText = "UPDATE tbl_product SET status = 'Active' WHERE prod_ID = '" + dgv_orderList.Rows[i].Cells[4].Value + "';";

                    RowsAffected = cmd.ExecuteNonQuery();

                    if (RowsAffected > 0) { }
                    else
                    {
                        MessageBox.Show("No ID Found", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally { cn.Close(); }
            
            viewProductList();
        }

        List<string> prodstatInactive = new List<string>();

        public void updProdtoInactive()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                int RowsAffected = 0;

                for (int i = 0; i < prodstatInactive.Count; i++)
                {
                    cmd.CommandText = "UPDATE tbl_product SET status = 'Inactive' WHERE prod_ID = '" + prodstatInactive[i] + "';";
                    RowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally { cn.Close(); }

            prodstatInactive.Clear();

            viewProductList();
        }

        public void updCategtoActive()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                int RowsAffected = 0;

                cmd.CommandText = "UPDATE tbl_category SET status = 'Active' WHERE prod_categID = '" + cb_categ.SelectedValue + "';";

                RowsAffected = cmd.ExecuteNonQuery();

                if (RowsAffected > 0)
                {

                }
                else
                {
                    MessageBox.Show("No ID Found", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
            finally
            {
                cn.Close();
            }

            viewCategList();
        }

        public void updCategtoInactive()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = "SELECT Count(1) FROM tbl_product WHERE prod_categID = '" + oldcategforstatus + "' ; ";

                int RowsAffected = Convert.ToInt32(cmd.ExecuteScalar());
                if (RowsAffected == 0)
                {
                    try
                    {
                        int RowsAffected2 = 0;

                        cmd.CommandText = "UPDATE tbl_category SET status = 'Inactive' WHERE prod_categID = '" + oldcategforstatus + "';";

                        RowsAffected2 = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
            finally
            {
                cn.Close();
            }
        
            viewCategList();

        }

        public void getcustomDiscount()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = "Select * from cstmdiscount";

                MySqlDataReader myReader;
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    lb_dispDiscount.Text = myReader[1].ToString() + " %";
                    customdisc = myReader[1].ToString() + " %";
                }

                myReader.Close();
                nud_setdiscount.Text = "0";
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }         
            finally { cn.Close(); }
            
        }

        private string GetRandomKey(int len)
        {
            int maxSize = len;
            char[] chars = new char[30];
            string a;
            a = "1234567890";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[7];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);

            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        private string orderIDMax;
        public void orderIDIncre()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = "SELECT orderID FROM tbl_ordermain";

                MySqlDataReader Read;
                Read = cmd.ExecuteReader();

                while (Read.Read())
                {
                    int increID = int.Parse(Read[0].ToString()) + 1;
                    orderIDMax = increID.ToString();
                }
            }

            catch (Exception)
            {
                throw;
            }
            finally { cn.Close(); }

        }

        public void prodIDIncre()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = "SELECT prod_ID FROM tbl_product ORDER BY prod_ID ASC";

                MySqlDataReader Read;
                Read = cmd.ExecuteReader();

                while (Read.Read())
                {
                    int maxID = int.Parse(Read[0].ToString()) + 1;
                    txtprodID.Text = "P-" + maxID.ToString();          
                }

            }

            catch (Exception)
            {
                throw;
            }
            finally
            {
                cn.Close();
            }
        }

        public void categIDIncre()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = "SELECT prod_categID FROM tbl_category ORDER BY prod_categID ASC";

                MySqlDataReader Read;
                Read = cmd.ExecuteReader();

                while (Read.Read())
                {
                    int maxID = int.Parse(Read[0].ToString()) + 1;
                    txtcategID.Text = "C-" + maxID.ToString();
                }

            }

            catch (Exception)
            {
                throw;
            }
            finally
            {
                cn.Close();
            }

        }

        public void userIDIncre()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = "SELECT UserID FROM tbl_login ORDER BY UserID ASC";

                MySqlDataReader Read;
                Read = cmd.ExecuteReader();

                while (Read.Read())
                {
                    int maxID = int.Parse(Read[0].ToString()) + 1;
                    txtUserID.Text = "U-" + maxID.ToString();
                }

            }

            catch (Exception)
            {
                throw;
            }
            finally
            {
                cn.Close();
            }
        }

        public void fillcategcmbbox()
        {
            cn.Open();
            cmd.Connection = cn;

            cmd.CommandText = "Select prod_categID, prod_categname from tbl_category;";

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);

            BindingSource bSource = new BindingSource();
            bSource.DataSource = dt;
            cb_categ.DataSource = bSource;

            cb_categ.DisplayMember = "prod_categname";
            cb_categ.ValueMember = "prod_categID";

            cb_categ.SelectedIndex = -1;

            // -----------------------------------------

            BindingSource bSource2 = new BindingSource();
            bSource2.DataSource = dt;
            cb_prodfilter.DataSource = bSource2;

            cb_prodfilter.DisplayMember = "prod_categname";
            cb_prodfilter.ValueMember = "prod_categID";

            cb_prodfilter.SelectedIndex = -1;

            // -----------------------------------------

            BindingSource bSource3 = new BindingSource();
            bSource3.DataSource = dt;
            cb_prodfilter2.DataSource = bSource3;

            cb_prodfilter2.DisplayMember = "prod_categname";
            cb_prodfilter2.ValueMember = "prod_categID";

            cb_prodfilter2.SelectedIndex = -1;

            da.Update(dt);
            cn.Close();

        }

        public void fillprodfilter()
        {
             try
             {
                 cn.Open();
                 cmd.Connection = cn;

                 cmd.CommandText = ("SELECT CONCAT('P-', A.prod_ID), A.prod_name, A.prod_desc, B.prod_categname, A.unit_price, A.status, A.prod_image FROM tbl_product A INNER JOIN tbl_category B ON A.prod_categID = B.prod_categID ORDER BY prod_ID ASC;");

                 MySqlDataAdapter da = new MySqlDataAdapter();
                 da.SelectCommand = cmd;
                 DataTable dt = new DataTable();
                 da.Fill(dt);

                 BindingSource bSource = new BindingSource();
                 bSource.DataSource = dt;

                 dgv_prodmain.DataSource = bSource;

                 da.Update(dt);

                 //- Prod Filter Main ---------------------------------------
                 DataView dv1 = dt.DefaultView;
                 dv1.RowFilter = string.Format("prod_categname like '%{0}%'", cb_prodfilter.Text);
                 dgv_prodmain.DataSource = dv1.ToTable();
                 dgv_prodmain.ClearSelection(); dgv_prodmain.Columns[0].Width = 130;

                 //- Prod Filter View ---------------------------------------
                 DataView dv2 = dt.DefaultView;
                 dv2.RowFilter = string.Format("prod_categname like '%{0}%'", cb_prodfilter2.Text);
                 dgv_prodview.DataSource = dv2.ToTable();
                 dgv_prodview.ClearSelection(); dgv_prodview.Columns[0].Width = 130;

                 
             }
             catch
             {

             }
             finally
             {
                 cn.Close();
             }           
        }

        public void filluserfilter()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = ("SELECT CONCAT('U-', UserID), FirstName, LastName, Address, PhoneNumber, SHA1(Username), SHA1(Password), UserType FROM tbl_login;");

                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);

                BindingSource bSource = new BindingSource();
                bSource.DataSource = dt;

                dgv_userlist.DataSource = bSource;

                da.Update(dt);

                //- User Filter Main ---------------------------------------
                DataView dv = dt.DefaultView;
                dv.RowFilter = string.Format("UserType like '%{0}%'", cb_rolefilter.Text);
                dgv_userlist.DataSource = dv.ToTable();
                dgv_userlist.ClearSelection(); dgv_userlist.Columns[0].Width = 130;

                //- User Filter View ---------------------------------------
                DataView dv2 = dt.DefaultView;
                dv2.RowFilter = string.Format("UserType like '%{0}%'", cb_rolefilter2.Text);
                dgv_viewuser.DataSource = dv2.ToTable();
                dgv_viewuser.ClearSelection(); dgv_viewuser.Columns[0].Width = 130;
            }
            catch
            {

            }
            finally
            {
                cn.Close();
            }  
        }

        string btnDashboardClicked = "";

        public void dbdaterange()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                if (btnDashboardClicked == "Today")
                {                   
                    cmd.CommandText = "SELECT time_format(ordertime, '%h %p') AS Date, sum(totalAmt) AS TotalAmount, sum(discount) AS Discount FROM ordersys.tbl_ordermain Where orderdate = '" + lbl_dateOrder.Text + "' GROUP BY time_format(ordertime, '%h %p') ORDER BY ordertime;";

                    MySqlDataAdapter da = new MySqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bSource = new BindingSource();
                    bSource.DataSource = dt;
                    chart2.DataSource = bSource;

                    chart2.DataBind();

                    cmd.CommandText = "SELECT COUNT(*) AS Numoforders, time_format(ordertime, '%h %p') AS Time, orderno, orderid FROM ordersys.tbl_ordermain Where orderdate = '" + lbl_dateOrder.Text + "' GROUP BY time_format(ordertime, '%h %p') ORDER BY ordertime;";

                    MySqlDataAdapter da2 = new MySqlDataAdapter();
                    da2.SelectCommand = cmd;
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);

                    BindingSource bSource2 = new BindingSource();
                    bSource2.DataSource = dt2;
                    chart4.DataSource = bSource2;

                    chart4.DataBind();

                    cmd.CommandText = "SELECT Count(*) AS Numoforders, date_format(orderdate, '%a') AS Days FROM ordersys.tbl_ordermain Where orderdate = '" + lbl_dateOrder.Text + "' GROUP BY Days ORDER BY date_format(orderdate, '%w');";

                    MySqlDataAdapter da3 = new MySqlDataAdapter();
                    da3.SelectCommand = cmd;
                    DataTable dt3 = new DataTable();
                    da3.Fill(dt3);

                    BindingSource bSource3 = new BindingSource();
                    bSource3.DataSource = dt3;
                    chart1.DataSource = bSource3;

                    chart1.DataBind();

                    cmd.CommandText = "SELECT A.prod_name AS 'Product', A.prod_ID, SUM(C.Quantity) AS Quantity, sum(C.subtotal) AS 'Sub Total' FROM tbl_product A INNER JOIN tbl_category D ON A.prod_categID = D.prod_categID INNER JOIN tbl_orderdetails C ON A.prod_ID = C.prod_ID INNER JOIN tbl_ordermain B ON B.orderID = C.orderID Where B.orderdate = '" + lbl_dateOrder.Text + "' group by prod_name order by Quantity Desc Limit 5;";

                    MySqlDataAdapter da4 = new MySqlDataAdapter();
                    da4.SelectCommand = cmd;
                    DataTable dt4 = new DataTable();
                    da4.Fill(dt4);

                    BindingSource bSource4 = new BindingSource();
                    bSource4.DataSource = dt4;
                    chart3.DataSource = bSource4;

                    chart3.DataBind();
                } 
                else if (btnDashboardClicked == "Week")
                {
                    MessageBox.Show("w");
                }
                else if (btnDashboardClicked == "Month")
                {
                    MessageBox.Show("wd");
                }
                else if (btnDashboardClicked == "Year")
                {
                    cmd.CommandText = "SELECT date_format(orderdate, '%M/%d/%Y') AS Date, sum(totalAmt) AS TotalAmount, sum(discount) AS Discount FROM ordersys.tbl_ordermain Group by orderdate;";
                }



                
                //chart2.Series[0].XValueMember = "Date"; //key
               // chart2.Series[0].YValueMembers = "TotalAmount"; //value

               // chart2.Series[1].XValueMember = "Date"; //key
              //  chart2.Series[1].YValueMembers = "Discount"; //value

                txtlbtotalgross.Text = grosssales.ToString("₱ #,###,##0.00");
                txtlbtotalnet.Text = netsales.ToString("₱ #,###,##0.00");
               
                
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                cn.Close();
            }

        }

        string btnAuditClicked = "";

        public void auditdaterange()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                if (btnAuditClicked == "DateRange")
                {
                    cmd.CommandText = "SELECT User, Activity, Action, Date, Time_Format(Time, '%r') AS 'Time' FROM tbl_audittrail WHERE Date BETWEEN '" + dtp_auditStart.Value.ToString("yyyy-MM-dd") + "' AND '" + dtp_auditEnd.Value.ToString("yyyy-MM-dd") + "' ORDER BY auditID DESC;";
                }
                else if (btnAuditClicked == "Today")
                {
                    cmd.CommandText = "SELECT User, Activity, Action, Date, Time_Format(Time, '%r') AS 'Time' FROM tbl_audittrail WHERE Date = '" + lbl_dateOrder.Text + "' ORDER BY auditID DESC;";
                }
                else if (btnAuditClicked == "Week")
                {
                    cmd.CommandText = "SELECT User, Activity, Action, Date, Time_Format(Time, '%r') AS 'Time' FROM tbl_audittrail WHERE Week(Date) = Week('" + lbl_dateOrder.Text + "') && Month(Date) = Month('" + lbl_dateOrder.Text + "') && Year(Date) = Year('" + lbl_dateOrder.Text + "') ORDER BY auditID DESC;";
                }
                else if (btnAuditClicked == "Month")
                {
                    cmd.CommandText = "SELECT User, Activity, Action, Date, Time_Format(Time, '%r') AS 'Time' FROM tbl_audittrail WHERE Month(Date) = Month('" + lbl_dateOrder.Text + "') && Year(Date) = Year('" + lbl_dateOrder.Text + "') ORDER BY auditID DESC;";
                }
                else if (btnAuditClicked == "Year")
                {
                    cmd.CommandText = "SELECT User, Activity, Action, Date, Time_Format(Time, '%r') AS 'Time' FROM tbl_audittrail WHERE Year(Date) = Year('" + lbl_dateOrder.Text + "') ORDER BY auditID DESC;";
                }

                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);
                BindingSource bSource = new BindingSource();

                bSource.DataSource = dt;
                dgv_audit.DataSource = bSource;

                dgv_audit.Columns[0].Width = 230;

                dgv_audit.ClearSelection();
                da.Update(dt);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                cn.Close();
            }
  
        }

        string btnSalesClicked = "";

        public void salesdaterange()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                if (btnSalesClicked == "DateRange")
                {
                    cmd.CommandText = "SELECT orderno AS 'Order Number', discount AS 'Discount', totalAmt AS 'Total Amount', Time_Format(ordertime, '%r') AS 'Order Time', orderdate AS 'Order Date'  FROM tbl_ordermain WHERE orderdate BETWEEN '" + dtp_salesStart.Value.ToString("yyyy-MM-dd") + "' AND '" + dtp_salesEnd.Value.ToString("yyyy-MM-dd") + "' ORDER BY orderID DESC;";
                }
                else if (btnSalesClicked == "Today")
                {
                    cmd.CommandText = "SELECT orderno AS 'Order Number', discount AS 'Discount', totalAmt AS 'Total Amount', Time_Format(ordertime, '%r') AS 'Order Time', orderdate AS 'Order Date'  FROM tbl_ordermain WHERE orderdate = '" + lbl_dateOrder.Text + "' ORDER BY orderID DESC;";
                }
                else if (btnSalesClicked == "Week")
                {
                    cmd.CommandText = "SELECT orderno AS 'Order Number', discount AS 'Discount', totalAmt AS 'Total Amount', Time_Format(ordertime, '%r') AS 'Order Time', orderdate AS 'Order Date'  FROM tbl_ordermain WHERE Week(orderdate) = Week('" + lbl_dateOrder.Text + "') && Month(orderdate) = Month('" + lbl_dateOrder.Text + "') && Year(orderdate) = Year('" + lbl_dateOrder.Text + "') ORDER BY orderID DESC;";
                }
                else if (btnSalesClicked == "Month")
                {
                    cmd.CommandText = "SELECT orderno AS 'Order Number', discount AS 'Discount', totalAmt AS 'Total Amount', Time_Format(ordertime, '%r') AS 'Order Time', orderdate AS 'Order Date'  FROM tbl_ordermain WHERE Month(orderdate) = Month('" + lbl_dateOrder.Text + "') && Year(orderdate) = Year('" + lbl_dateOrder.Text + "') ORDER BY orderID DESC;";
                }
                else if (btnSalesClicked == "Year")
                {
                    cmd.CommandText = "SELECT orderno AS 'Order Number', discount AS 'Discount', totalAmt AS 'Total Amount', Time_Format(ordertime, '%r') AS 'Order Time', orderdate AS 'Order Date'  FROM tbl_ordermain WHERE Year(orderdate) = Year('" + lbl_dateOrder.Text + "') ORDER BY orderID DESC;";
                }

                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);
                BindingSource bSource = new BindingSource();

                bSource.DataSource = dt;
                dgv_salesmain.DataSource = bSource;

                double calculatesales = 0;

                for (int o = 0; o < dgv_salesmain.RowCount; o++)
                {
                    double totamt = calculatesales + Double.Parse(dgv_salesmain.Rows[o].Cells[2].Value.ToString());
                    netsales = totamt;

                    calculatesales = netsales;
                    lb_netsales.Text = netsales.ToString("₱ #,###,##0.00");

                    lbtxt_numberoford.Text = Convert.ToString(o + 1);
                }

                for (int o = 0; o < dgv_salesmain.RowCount; o++)
                {
                    double totamt = calculatesales + Double.Parse(dgv_salesmain.Rows[o].Cells[1].Value.ToString());
                    grosssales = totamt;

                    calculatesales = grosssales;
                    lb_grosssales.Text = grosssales.ToString("₱ #,###,##0.00");
                }

                dgv_salesmain.ClearSelection();
                da.Update(dt);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                cn.Close();
            }
        }

        public void orderhdaterange()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = "SELECT B.orderNo, A.prod_name AS 'Product Name', D.prod_categname AS 'Category', C.Quantity, C.unitprice AS 'Unit Price', C.subtotal AS 'Sub Total', Time_Format(B.ordertime, '%r') AS 'Order Time', B.orderdate AS 'Order Date' FROM tbl_product A INNER JOIN tbl_category D ON A.prod_categID = D.prod_categID INNER JOIN tbl_orderdetails C ON A.prod_ID = C.prod_ID INNER JOIN tbl_ordermain B ON B.orderID = C.orderID WHERE b.orderdate BETWEEN '" + dtp_orderhStart.Value.ToString("yyyy-MM-dd") + "' AND '" + dtp_orderhEnd.Value.ToString("yyyy-MM-dd") + "' ORDER BY recNo DESC;";

                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);
                BindingSource bSource = new BindingSource();

                bSource.DataSource = dt;
                dgv_orderhistor.DataSource = bSource;

                dgv_orderhistor.ClearSelection();
                da.Update(dt);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                cn.Close();
            }

        }

        private void addMenuItem_Click(object sender, EventArgs e)
        {
            //Switching between forms must be clean and clear. (ex. "Panel.Visible = false; at the start of the app")
            pa_prodmain.Visible = true;
            pa_order.Visible = false;
            pa_users.Visible = false;
            pa_categ.Visible = false;
            pa_cstmDscnt.Visible = false;
            pa_viewprod.Visible = false;
            pa_viewuser.Visible = false;
            pa_viewcateg.Visible = false;
            pa_audittrail.Visible = false;
            pa_backrestore.Visible = false;
            pa_orderhistory.Visible = false;
            pa_sales.Visible = false;
            pa_dashboard.Visible = false;

            pa_prodmain.Focus();
        }

        private void userMenuItem_Click(object sender, EventArgs e)
        {
            pa_users.Visible = true;
            pa_prodmain.Visible = false;            
            pa_order.Visible = false;
            pa_categ.Visible = false;
            pa_cstmDscnt.Visible = false;
            pa_viewprod.Visible = false;
            pa_viewuser.Visible = false;
            pa_viewcateg.Visible = false;
            pa_audittrail.Visible = false;
            pa_backrestore.Visible = false;
            pa_orderhistory.Visible = false;
            pa_sales.Visible = false;
            pa_dashboard.Visible = false;

            pa_users.Focus();
        }

        private void orderingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pa_order.Visible = true;
            pa_users.Visible = false;
            pa_prodmain.Visible = false;
            pa_categ.Visible = false;
            pa_cstmDscnt.Visible = false;
            pa_viewprod.Visible = false;
            pa_viewuser.Visible = false;
            pa_viewcateg.Visible = false;
            pa_audittrail.Visible = false;
            pa_backrestore.Visible = false;
            pa_orderhistory.Visible = false;
            pa_sales.Visible = false;
            pa_dashboard.Visible = false;

            pa_order.Focus();
        }

        private void categMenuItem_Click(object sender, EventArgs e)
        {

            pa_categ.Visible = true;
            pa_order.Visible = false;
            pa_users.Visible = false;
            pa_prodmain.Visible = false;
            pa_cstmDscnt.Visible = false;
            pa_viewprod.Visible = false;
            pa_viewuser.Visible = false;
            pa_viewcateg.Visible = false;
            pa_audittrail.Visible = false;
            pa_backrestore.Visible = false;
            pa_orderhistory.Visible = false;
            pa_sales.Visible = false;
            pa_dashboard.Visible = false;

            pa_categ.Focus();
        }

      
        private void viewProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pa_viewprod.Visible = true;
            pa_categ.Visible = false;
            pa_order.Visible = false;
            pa_users.Visible = false;
            pa_prodmain.Visible = false;
            pa_cstmDscnt.Visible = false;
            pa_viewuser.Visible = false;
            pa_viewcateg.Visible = false;
            pa_audittrail.Visible = false;
            pa_backrestore.Visible = false;
            pa_orderhistory.Visible = false;
            pa_sales.Visible = false;
            pa_dashboard.Visible = false;

            
            pa_viewprod.Focus(); //It focus the selected form from other forms.
            //Make a class where the function is to remove/clear/reset each form when switching it. Making it less coded and simply understable.

            
        }

        public void switchForm()
        {
          
           

        }

        private void viewUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pa_viewuser.Visible = true;
            pa_viewprod.Visible = false;
            pa_categ.Visible = false;
            pa_order.Visible = false;
            pa_users.Visible = false;
            pa_prodmain.Visible = false;
            pa_cstmDscnt.Visible = false;
            pa_viewcateg.Visible = false;
            pa_audittrail.Visible = false;
            pa_backrestore.Visible = false;
            pa_orderhistory.Visible = false;
            pa_sales.Visible = false;
            pa_dashboard.Visible = false;

            pa_viewuser.Focus();
            
        }

        private void viewCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pa_viewcateg.Visible = true;
            pa_viewuser.Visible = false;
            pa_viewprod.Visible = false;
            pa_categ.Visible = false;
            pa_order.Visible = false;
            pa_users.Visible = false;
            pa_prodmain.Visible = false;
            pa_cstmDscnt.Visible = false;
            pa_audittrail.Visible = false;
            pa_backrestore.Visible = false;
            pa_orderhistory.Visible = false;
            pa_sales.Visible = false;
            pa_dashboard.Visible = false;

            pa_viewcateg.Focus();

        }

        private void auditTrailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pa_audittrail.Visible = true;

            pa_viewcateg.Visible = false;
            pa_viewuser.Visible = false;
            pa_viewprod.Visible = false;
            pa_categ.Visible = false;
            pa_order.Visible = false;
            pa_users.Visible = false;
            pa_prodmain.Visible = false;
            pa_cstmDscnt.Visible = false;
            pa_backrestore.Visible = false;
            pa_orderhistory.Visible = false;
            pa_sales.Visible = false;
            pa_dashboard.Visible = false;

            pa_audittrail.Focus();

     
           

        }

        private void orderHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {



            pa_orderhistory.Visible = true;

                pa_audittrail.Visible = false;
                pa_viewcateg.Visible = false;
                pa_viewuser.Visible = false;
                pa_viewprod.Visible = false;
                pa_categ.Visible = false;
                pa_order.Visible = false;
                pa_users.Visible = false;
                pa_prodmain.Visible = false;
                pa_cstmDscnt.Visible = false;
                pa_backrestore.Visible = false;
                pa_sales.Visible = false;
                pa_dashboard.Visible = false;


                pa_orderhistory.Focus();

    
 

           
        }

        private void salesToolStripMenuItem_Click(object sender, EventArgs e)
        {


            pa_sales.Visible = true;

                pa_orderhistory.Visible = false;
                pa_audittrail.Visible = false;
                pa_viewcateg.Visible = false;
                pa_viewuser.Visible = false;
                pa_viewprod.Visible = false;
                pa_categ.Visible = false;
                pa_order.Visible = false;
                pa_users.Visible = false;
                pa_prodmain.Visible = false;
                pa_cstmDscnt.Visible = false;
                pa_backrestore.Visible = false;
                pa_dashboard.Visible = false;

                pa_sales.Focus();

            
        }


        private void customDiscountToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //Make something that will disable every form when custom discount button clicked.
            pa_cstmDscnt.Visible = true;
            pa_backrestore.Visible = false;
            pa_selectdisc.Visible = false;

        }

        private void btn_cstmdscExit_Click(object sender, EventArgs e)
        {
            pa_cstmDscnt.Visible = false;

        }


        private void backupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pa_backrestore.Visible = true;
            pa_cstmDscnt.Visible = false;
            pa_selectdisc.Visible = false;
        }

        private void btn_bckandres_Click(object sender, EventArgs e)
        {
            pa_backrestore.Visible = false;

        }

        private void btn_discount_Click(object sender, EventArgs e)
        {
           applyDiscount();
        }

        public void applyDiscount()
        {
            if (dgv_orderList.RowCount >= 1)
            {
                if (discountgranted == 1)
                {
                    DialogResult dialogResult = MessageBox.Show("Discount is already applied! Do you want to cancel the discount?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        btn_discount.BackColor = Color.Black;
                        discountgranted = 0; customdiscAct = "0"; seniordiscAct = "0";
                        txtb_usernameDC.Clear(); txtb_passwordDC.Clear();
                        
                        double resetTotalamt = 0;

                        for (int o = 0; o < dgv_orderList.RowCount; o++)
                        {
                            resetTotalamt += Double.Parse(dgv_orderList.Rows[o].Cells[3].Value.ToString().Replace("₱ ", ""));
                            txt_CusTotalCost.Text = resetTotalamt.ToString("₱ #,###,##0.00");
                        }

                        try
                        {
                            cn.Open();
                            cmd.Connection = cn;

                            string getlogoutdata = lb_userlvl.Text + " " + lb_name.Text + " has cancelled the discount";
                            string action = "Cancel";

                            cmd.CommandText = "INSERT INTO tbl_audittrail (User,Activity,Action,Date,Time) VALUES ('" + lb_name.Text + "','" + getlogoutdata + "', '" + action + "','" + lbl_dateOrder.Text + "','" + datetimeHMS + "') ;";
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                        finally { cn.Close(); }

                        viewAudittrail();

                        MessageBox.Show("Successfully Cancelled", "Action Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    pa_backrestore.Visible = false;
                    pa_cstmDscnt.Visible = false;
                    pa_selectdisc.Visible = true;
                }
            }
            else
            {
                MessageBox.Show("Cart is empty", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btn_exitDisc_Click(object sender, EventArgs e)
        {
            pa_order.Enabled = true; pa_selectdisc.Visible = false; 
            rb_customDisc.Checked = false; rb_senior.Checked = false;       
        }

        private void btn_dcBack_Click(object sender, EventArgs e)
        {
            pa_discountConfrm.Visible = false;
            pa_selectdisc.Visible = true;
        }

        private string customdisc, customdiscAct, seniordiscAct;
  
        private void btn_confrmDisc1_Click(object sender, EventArgs e)
        {
            if (rb_senior.Checked == true)
            {
                pa_order.Enabled = false; pa_selectdisc.Visible = false; pa_discountConfrm.Visible = true;
                seniordiscAct = "1";
               
                lb_discountshow.Text = "Discount - 20 %";
                double afterDiscount = Convert.ToDouble(txt_CusTotalCost.Text.Replace("₱ ", "")) * 0.20;
                lbl_shwDiscount.Text = string.Format("₱ {0:#,#,0.00}", afterDiscount);
                double afterGrdtotal = Convert.ToDouble(txt_CusTotalCost.Text.Replace("₱ ", "")) - afterDiscount;
                lbl_shwAFTotal.Text = string.Format("₱ {0:#,#,0.00}", afterGrdtotal);
       
            }
            else if (rb_customDisc.Checked == true)
            {
                pa_order.Enabled = false; pa_selectdisc.Visible = false; pa_discountConfrm.Visible = true;
                customdiscAct = "1";
                
                lb_discountshow.Text = "Discount - " + customdisc;
                double afterDiscount = Convert.ToDouble(txt_CusTotalCost.Text.Replace("₱ ", "")) * (Convert.ToDouble(customdisc.Replace(" %", "")) / 100);
                lbl_shwDiscount.Text = string.Format("₱ {0:#,#,0.00}", afterDiscount);
                double afterGrdtotal = Convert.ToDouble(txt_CusTotalCost.Text.Replace("₱ ", "")) - afterDiscount;
                lbl_shwAFTotal.Text = string.Format("₱ {0:#,#,0.00}", afterGrdtotal);

            }

            rb_customDisc.Checked = false;
            rb_senior.Checked = false;
        }

        private void btn_discConfrmExt_Click(object sender, EventArgs e)
        {
            pa_discountConfrm.Visible = false; pa_order.Enabled = true;
            rb_customDisc.Checked = false; rb_senior.Checked = false;        
        }

        private int discountgranted = 0;
        private void btn_confirmDC_Click(object sender, EventArgs e)
        {
  
            if (txtb_usernameDC.Text == "")
            {
                MessageBox.Show("Please enter the admin's or owner's username", "Grant Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtb_usernameDC.Focus();
                return;
            }

            if (txtb_passwordDC.Text == "")
            {
                MessageBox.Show("Please enter the admin's or owner's password", "Grant Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtb_passwordDC.Focus();
                return;
            }

            try
            {
                string username = "", userlvl = "";

                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = "SELECT COUNT(*),FirstName,UserType,LastName FROM tbl_login WHERE (Username = '" + txtb_usernameDC.Text + "' and Password = '" + txtb_passwordDC.Text + "') and (UserType = 'Owner' or UserType = 'Admin');";

                int count = int.Parse(cmd.ExecuteScalar().ToString());
                MySqlDataReader myReader;
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    if (count == 1)
                    {
                        discountgranted = 1; //-Grant Discount
                        username = myReader.GetString(1) + " " + myReader.GetString(3);
                        userlvl = myReader.GetString(2) + " Name :";

                        txt_CusTotalCost.Text = lbl_shwAFTotal.Text;

                        btn_discount.BackColor = Color.White;
                        pa_discountConfrm.Visible = false; pa_order.Enabled = true;

                        txtb_payment.Clear();
                        txtb_change.Clear();
                        _inputnumber = 0;
                    }
                    else
                    {
                        MessageBox.Show("Incorrect Username or Password / It must be an owner or admin", "Grant Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        txtb_usernameDC.Clear(); txtb_passwordDC.Clear();
                        txtb_usernameDC.Focus();
                        return;
                    }
                }

                if (count == 1)
                {
                    myReader.Close();

                    MySqlCommand cmd1 = new MySqlCommand();
                    cmd1.Connection = cn;

                    string access = lb_userlvl.Text + " " + lb_name.Text + " has been given permission to grant discount by (" + userlvl + " " + username + ")";
                    string action = "Granted";

                    cmd1.CommandText = " insert into tbl_audittrail (User,Activity,Action,Date,Time) Values  ('" + lb_name.Text + "','" + access + "', '" + action + "','" + lbl_dateOrder.Text + "','" + datetimeHMS + "') ;";

                    cmd1.ExecuteNonQuery();

                    MessageBox.Show("Discount Applied", "Grant Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cn.Dispose();
                cn.Close();
            }

            viewAudittrail();
        }

        private void logoutMenuItem_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("This will close and logout your account", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes)
            {
                loginform = new Login();
                loginform.Show();
                this.Dispose(); //Check a better way to dispose the application without making more memory load. // or maybe try "this.Close();"
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
            
        }

        private void txtb_searchorno_Enter(object sender, EventArgs e)
        {
            if (txtb_searchorno.Text == "Ex. 012345678")
            {
                txtb_searchorno.Text = "";
                txtb_searchorno.ForeColor = Color.Black;

            }
        }


        private void txtb_searchorno_Leave(object sender, EventArgs e)
        {
            string searchOrnum = txtb_searchorno.Text;

            if (txtb_searchorno.Text == "Ex. 012345678")
            {
                txtb_searchorno.Text = "Ex. 012345678";
                txtb_searchorno.ForeColor = Color.Gray;

            }
            else
            {

                if (searchOrnum.Equals(""))
                {
                    txtb_searchorno.Text = "Ex. 012345678";
                    txtb_searchorno.ForeColor = Color.Gray;

                }
                else
                {
                    txtb_searchorno.Text = searchOrnum;
                    txtb_searchorno.ForeColor = Color.Black;

                }

            }
        }

        private void paholder_srchOrdno_Click(object sender, EventArgs e)
        {
            txtb_searchorno.Focus();
        }

        private void pa_orderhistory_Leave(object sender, EventArgs e)
        {
            //Resets the form when it switches
            txtb_searchorno.Text = "Ex. 012345678";
            txtb_searchorno.ForeColor = Color.Gray;
        }

        private void paholder_textSearchpr2_Enter(object sender, EventArgs e)
        {
            gbholder_viewsrchprod.Focus();
        }

        private void paholder_textSearchpr3_Enter(object sender, EventArgs e)
        {
            txtb_searchprod.Focus();
        }

        private void pa_viewprod_Leave(object sender, EventArgs e)
        {
            //Resets the form when it switches
            txtb_searchprod.Text = "Ex. Apple, Banana, Ice Cream";
            txtb_searchprod.ForeColor = Color.Gray;
        }

        private void paholder_txtbsearcat_Click(object sender, EventArgs e)
        {
            txtb_searchCateg.Focus();
        }

        private void pa_viewcateg_Leave(object sender, EventArgs e)
        {
            //Resets the form when it switches
            txtb_searchCateg.Text = "Ex. Small, Medium, Large";
            txtb_searchCateg.ForeColor = Color.Gray;
        }

        private void paholder_txbSrcus_Click(object sender, EventArgs e)
        {
            txtb_searchUser.Focus();
        }

        private void pa_viewuser_Leave(object sender, EventArgs e)
        {
            //Resets the form when it switches
            txtb_searchUser.Text = "Ex. Allarey, Alejandro, Angelo";
            txtb_searchUser.ForeColor = Color.Gray;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_rmvCart_Click(object sender, EventArgs e)
        {
            removefrmCart();
        }

        public void removefrmCart()
        {
            pa_order.Focus();

            if (dgv_orderList.Rows.Count == 0)
            {
                MessageBox.Show("No Order's Item been Selected", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (dgv_orderList.Rows.Count >= 1 && dgv_orderList.CurrentRow.Selected == false)
            {
                MessageBox.Show("No Order's Item been Selected", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else if (dgv_orderList.RowCount >= 1 && dgv_orderList.CurrentRow.Selected == true)
            {
                if (prodstatInactive.Contains(dgv_orderList.SelectedRows[0].Cells[4].Value.ToString().Replace("P-", "")))
                {
                    try
                    {
                        cn.Open();
                        cmd.Connection = cn;

                        int RowsAffected = 0;

                        cmd.CommandText = "UPDATE tbl_product SET status = 'Inactive' WHERE prod_ID = '" + dgv_orderList.SelectedRows[0].Cells[4].Value.ToString() + "';";
                        RowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally { cn.Close(); }
                  
                    viewProductList();
                }

                //- Insert Removed Products to Undo --------------------------
                undoprodID.Push(dgv_orderList.SelectedRows[0].Cells[4].Value.ToString());
                undoprodQty.Push(Convert.ToInt16(dgv_orderList.SelectedRows[0].Cells[0].Value));
                undoprodName.Push(dgv_orderList.SelectedRows[0].Cells[1].Value.ToString());
                undoprodUprice.Push(dgv_orderList.SelectedRows[0].Cells[2].Value.ToString());
                undoprodSubprice.Push(dgv_orderList.SelectedRows[0].Cells[3].Value.ToString());

                //- Reset & Deduct the cost product that been removed from the orderlist. -----------------
                nondiscountedTotalAmt = nondiscountedTotalAmt - Convert.ToDouble(dgv_orderList.SelectedRows[0].Cells[3].Value.ToString().Replace("₱ ", ""));
                txt_CusTotalCost.Text = string.Format("₱ {0:#,#,0.00}", nondiscountedTotalAmt);

                if (discountgranted == 1)
                {
                    if (seniordiscAct == "1")
                    {
                        double afterDiscountMnual = nondiscountedTotalAmt * 0.20;
                        double afterGrdtotalMnual = nondiscountedTotalAmt - afterDiscountMnual;
                        txt_CusTotalCost.Text = String.Format("₱ {0:#,#,0.00}", afterGrdtotalMnual);
                    }
                    else if (customdiscAct == "1")
                    {
                        double afterDiscountMnual = nondiscountedTotalAmt * (Convert.ToDouble(customdisc.Replace(" %", "")) / 100);
                        double afterGrdtotalMnual = nondiscountedTotalAmt - afterDiscountMnual;
                        txt_CusTotalCost.Text = String.Format("₱ {0:#,#,0.00}", afterGrdtotalMnual);
                    }
                }

                txtb_payment.Clear(); txtb_change.Clear(); txtb_payment.Focus();               
                _inputnumber = 0;

                receiptBindingSource.RemoveCurrent(); dgv_orderList.ClearSelection();
                //-----------------------------------------------------------
            }

            //- Resets Order Form ------------------------------------------
            if (dgv_orderList.Rows.Count == 0)
            {
                txtb_qtyOrd.Clear(); txtb_qtyOrd.Visible = false;
                nondiscountedTotalAmt = 0;
                txtidprodOrd.Text = "P-0"; txtnameprodOrd.Text = "Ex. Apple, Banana, Ice Cream";
                txtupriceOrd.Text = "₱ 0.00"; txtsubtotalOrd.Text = "₱ 0.00";

                dgv_prodlist.ClearSelection(); pa_order.Focus();
            }
            //--------------------------------------------
        }

        private void btn_cncelOrdr_Click(object sender, EventArgs e)
        {
            CancelOrder();
        }

        public void CancelOrder()
        {
            pa_order.Focus();

            if (dgv_orderList.Rows.Count == 0)
            {
                MessageBox.Show("No Order Found", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (dgv_orderList.RowCount >= 1)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to cancel this order?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    if (prodstatInactive != null)
                    {
                        updProdtoInactive();
                    }

                    //- Resets Order Form ------------------------------------------
                    txtb_qtyOrd.Clear(); txtb_qtyOrd.Visible = false;
                    nondiscountedTotalAmt = 0;
                    txtidprodOrd.Text = "P-0"; txtnameprodOrd.Text = "Ex. Apple, Banana, Ice Cream";
                    txtupriceOrd.Text = "₱ 0.00"; txtsubtotalOrd.Text = "₱ 0.00";

                    dgv_orderList.Rows.Clear(); dgv_prodlist.ClearSelection();
                    
                    txt_CusTotalCost.Text = "₱ " + "0.00";
                    txtb_payment.Clear(); txtb_change.Clear();                    
                    _inputnumber = 0;
                    //------------------------------------------------
                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }

        }

        private void btn_Cmd_Click(object sender, EventArgs e)
        {

        }

      

        private void btn_prcdOrdr_Click(object sender, EventArgs e)
        {

            ProcessOrder();
            
        }

        public void ProcessOrder()
        {         
            pa_order.Focus();

            if (dgv_orderList.Rows.Count == 0)
            {
                MessageBox.Show("Cart is empty", "Invalid Order", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtb_payment.Clear();
                txtb_change.Clear();
                _inputnumber = 0;
            }

            else if (dgv_orderList.Rows.Count >= 1)
            {
                if (txtb_payment.Text == "")
                {
                    MessageBox.Show("Please enter the payment", "Invalid Payment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtb_payment.Focus();
                    return;
                }
                if (Convert.ToDouble(txt_CusTotalCost.Text.Replace("₱ ", "")) > Convert.ToDouble(txtb_payment.Text.Replace("₱ ", "")))
                {
                    MessageBox.Show("Not enough payment", "Invalid Payment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtb_payment.Clear(); _inputnumber = 0;
                    txtb_payment.Focus();
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Confirm Order?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    double discount = 0.00; int totalitems = 0;

                    try
                    {
                        if (discountgranted == 1 && seniordiscAct == "1")
                        {
                            double afterDiscount = Convert.ToDouble(string.Format("{0:#,#,#.00}", nondiscountedTotalAmt)) * 0.20;
                            discount = afterDiscount;
                        }
                        else if (discountgranted == 1 && customdiscAct == "1")
                        {
                            double afterDiscount = Convert.ToDouble(string.Format("{0:#,#,#.00}", nondiscountedTotalAmt)) * (Convert.ToDouble(customdisc.Replace(" %", "")) / 100);
                            discount = afterDiscount;
                        }
                        else
                        {
                            discount = 0.00;
                        }

                        cn.Open();
                        cmd.Connection = cn;

                        cmd.CommandText = "INSERT INTO tbl_ordermain (orderID,orderNo,orderDate,orderTime,discount,totalAmt ) VALUES (@orderID,@orderNo,@orderDate,@orderTime, @discount, @totalAmt);";

                        cmd.Parameters.AddWithValue("@orderID", orderIDMax);
                        cmd.Parameters.AddWithValue("@orderNo", lbl_orderNo.Text);
                        cmd.Parameters.AddWithValue("@orderDate", lbl_dateOrder.Text);
                        cmd.Parameters.AddWithValue("@orderTime", datetimeHMS);
                        cmd.Parameters.AddWithValue("@discount", string.Format("{0:#,#,0.00}", discount));
                        cmd.Parameters.AddWithValue("@totalAmt", Convert.ToDouble(txt_CusTotalCost.Text.Replace("₱ ", "")));

                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        cn.Close();
                    }

                    insertKitchen();
                    insertOrderDtls();

                    orderIDIncre();
                    viewSales();
                    viewOrderhistory();

                    //- Show Receipt Form -------------------------------------------
                    double vatCalculated = (nondiscountedTotalAmt / 100) * 12;
                    double totalwithvat = (nondiscountedTotalAmt + vatCalculated);

                    for (int o = 0; o < dgv_orderList.RowCount; o++)
                    {
                        totalitems += int.Parse(dgv_orderList.Rows[o].Cells[0].Value.ToString());
                    }

                    using (ReceiptForm frm = new ReceiptForm(receiptBindingSource.DataSource as List<Receipt>, string.Format("₱ {0:#,#,0.00}", discount), txt_CusTotalCost.Text, txtb_payment.Text, string.Format("{0:#,#,0.00}", txtb_change.Text), lb_dateDisplay.Text, lbl_orderNo.Text, string.Format("₱ {0:#,#,0.00}", vatCalculated), Convert.ToString(totalitems), lb_name.Text))
                    {
                        frm.ShowDialog();
                    }
                    //---------------------------------------

                    //- Resets Order Form ------------------------------------------
                    txtb_qtyOrd.Clear(); txtb_qtyOrd.Visible = false;
                    nondiscountedTotalAmt = 0;
                    txtidprodOrd.Text = "P-0"; txtnameprodOrd.Text = "Ex. Apple, Banana, Ice Cream";
                    txtupriceOrd.Text = "₱ 0.00"; txtsubtotalOrd.Text = "₱ 0.00";

                    dgv_orderList.Rows.Clear(); dgv_prodlist.ClearSelection();

                    //- Resets Discount -----------------------
                    btn_discount.BackColor = Color.Black;
                    discountgranted = 0; customdiscAct = "0"; seniordiscAct = "0";
                    txtb_usernameDC.Clear(); txtb_passwordDC.Clear();
                    //----------------------

                    txt_CusTotalCost.Text = "₱ " + "0.00";
                    txtb_payment.Clear(); txtb_change.Clear();
                    _inputnumber = 0;

                    lbl_orderNo.Text = GetRandomKey(5);

                    prodstatInactive.Clear();
                    //------------------------------------------------



                    //////////
                    //btn_ordReturn.Enabled = false;
                    // btn_ordReturn.BackgroundImage = Ordering_System.Properties.Resources.undo_arrow_gray;

                    // kd = new Kitchen_Display();
                    //kd.orderno = lbl_orderid.Text;

                    // 



                    // updateprodstatus();

                    //totalitems = 0;



                    // autoOrderIDIncre();

                    // incresalesDtlsID();

                }
                else { return; }

            }
            
        }

        public void insertKitchen()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.Parameters.Clear();

                for (int i = 0; i < dgv_orderList.Rows.Count; i++)
                {
                    cmd.CommandText = "INSERT INTO tbl_kitchendisp (orderID,prod_ID,qty ) VALUES (@orderID,@prod_ID,@qty) ;";

                    cmd.Parameters.AddWithValue("@orderID", orderIDMax);
                    cmd.Parameters.AddWithValue("@prod_ID", dgv_orderList.Rows[i].Cells[4].Value.ToString());
                    cmd.Parameters.AddWithValue("@qty", dgv_orderList.Rows[i].Cells[0].Value.ToString());

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            finally { cn.Close(); }

        }

        private void txtb_change_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_CusTotalCost_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtb_searchprod_Enter(object sender, EventArgs e)
        {
            //Making it less lenght of words so that it cannot be spam or to prevent glitch to the database since it is a an autosearch.
            //Reminder of per searchbox to set the properties of new textboxes copying it from the old ones.

            if (txtb_searchprod.Text == "Ex. Apple, Banana, Ice Cream" && txtb_searchprod.ForeColor == Color.Gray)
            {
                txtb_searchprod.Text = "";
                txtb_searchprod.ForeColor = Color.Black;

            }
        }

        private void txtb_searchprod_Leave(object sender, EventArgs e)
        {
            string searchProd = txtb_searchprod.Text;

            if (txtb_searchprod.Text == "Ex. Apple, Banana, Ice Cream" && txtb_searchprod.ForeColor == Color.Gray)
            {
                txtb_searchprod.Text = "Ex. Apple, Banana, Ice Cream";
                txtb_searchprod.ForeColor = Color.Gray;
            }
            else
            {
                if (searchProd.Equals(""))
                {
                    txtb_searchprod.Text = "Ex. Apple, Banana, Ice Cream";
                    txtb_searchprod.ForeColor = Color.Gray;
                }
                else
                {
                    txtb_searchprod.Text = searchProd;
                    txtb_searchprod.ForeColor = Color.Black;
                }
            }
        }

        private void txtb_searchprod_MouseDown(object sender, MouseEventArgs e)
        {
            int curs = txtb_searchprod.SelectionStart;
            if (txtb_searchprod.SelectionStart < txtb_searchprod.Text.Length && txtb_searchprod.Text[txtb_searchprod.Text.Length - 1] == ' ' && txtb_searchprod.SelectedText == string.Empty)
            {
                txtb_searchprod.Focus();
                txtb_searchprod.Text = txtb_searchprod.Text.TrimEnd(' ');

                txtb_searchprod.SelectionStart = curs;
            }
        }

        private void txtb_searchprod_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tbsearchprod = (TextBox)sender;
            cursorPos = tbsearchprod.SelectionStart;

            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' ' || (char)Keys.Back == e.KeyChar)
            //Allows letters, space and backspace.
            {
                if ((tbsearchprod.SelectionStart == 0) && (e.KeyChar == (char)Keys.Space))
                //Best way to remove leading spaces.
                {
                    e.Handled = true;
                }

                if ((char)Keys.Back == e.KeyChar)
                {
                    cursorPos = tbsearchprod.SelectionStart - 1;
                }

                if ((e.KeyChar == ' ') && (tbsearchprod.Text.Length > 0))
                {
                    if (tbsearchprod.Text[tbsearchprod.Text.Length - 1] == ' ' && tbsearchprod.SelectionStart == tbsearchprod.Text.Length)
                    {
                        e.Handled = true;
                    }
                }

                if (tbsearchprod.SelectionStart < tbsearchprod.Text.Length && tbsearchprod.SelectionStart > 0)
                {
                    if (tbsearchprod.Text[tbsearchprod.SelectionStart - 1] == ' ' || tbsearchprod.Text[tbsearchprod.SelectionStart] == ' ')
                    {
                        if (tbsearchprod.SelectedText == "" && e.KeyChar == ' ')
                        {
                            if (tbsearchprod.Text[tbsearchprod.Text.Length - 1] == ' ' && tbsearchprod.SelectionStart < tbsearchprod.Text.Length)
                            {
                                tbsearchprod.Focus();
                                tbsearchprod.Text = tbsearchprod.Text.TrimEnd(' ');
                                tbsearchprod.SelectionStart = cursorPos;
                            }

                            e.Handled = true;
                        }
                    }
                }
            }

            else
            {
                e.Handled = true;
            }
        }

        private void txtb_searchprod_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                int curs = txtb_searchprod.SelectionStart;
                if (txtb_searchprod.SelectionStart < txtb_searchprod.Text.Length && txtb_searchprod.Text[txtb_searchprod.Text.Length - 1] == ' ' && txtb_searchprod.SelectedText == string.Empty)
                {
                    txtb_searchprod.Focus();
                    txtb_searchprod.Text = txtb_searchprod.Text.TrimEnd(' ');

                    txtb_searchprod.SelectionStart = curs;
                }

            }
        }

        private void txtb_searchprod_TextChanged(object sender, EventArgs e)
        {
            if (txtb_searchprod.ForeColor != Color.Gray && txtb_searchprod.Text != "Ex. Apple, Banana, Ice Cream")
            {
                cb_prodfilter2.SelectedIndex = -1;

                if (txtb_searchprod.Text != "" && txtb_searchprod.Text[0] == ' ')
                {
                    txtb_searchprod.Text = txtb_searchprod.Text.TrimStart(' ');
                }

                if (txtb_searchprod.Text.Contains("  "))
                {
                    txtb_searchprod.Focus();
                    txtb_searchprod.Text = txtb_searchprod.Text.Replace("  ", " ");

                    txtb_searchprod.SelectionStart = cursorPos;

                    if (txtb_searchprod.Text[txtb_searchprod.SelectionStart - 1] != ' ' && txtb_searchprod.SelectedText == "")
                    {
                        txtb_searchprod.SelectionStart = txtb_searchprod.SelectionStart + 1;
                    }
                }

                try
                {
                    cn.Open();
                    cmd.Connection = cn;

                    cmd.CommandText = ("SELECT CONCAT('P-', A.prod_ID), A.prod_name, A.prod_desc, B.prod_categname, A.unit_price, A.status, A.prod_image FROM tbl_product A INNER JOIN tbl_category B ON A.prod_categID = B.prod_categID ORDER BY prod_ID ASC;");

                    MySqlDataAdapter da = new MySqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bSource = new BindingSource();
                    bSource.DataSource = dt;

                    dgv_prodview.DataSource = bSource;

                    da.Update(dt);

                    DataView dv = dt.DefaultView;
                    dv.RowFilter = string.Format("prod_name like '%{0}%'", txtb_searchprod.Text);
                    dgv_prodview.DataSource = dv.ToTable();

                    dgv_prodview.Columns[0].Width = 130;

                    dgv_prodview.ClearSelection();
                }
                catch
                {

                }
                finally
                {
                    cn.Close();
                }
            }
        }

        private void txtb_searchCateg_Enter(object sender, EventArgs e)
        {
            if (txtb_searchCateg.Text == "Ex. Small, Medium, Large" && txtb_searchCateg.ForeColor == Color.Gray)
            {
                txtb_searchCateg.Text = "";
                txtb_searchCateg.ForeColor = Color.Black;

            }
        }

        private void txtb_searchCateg_Leave(object sender, EventArgs e)
        {
            string searchCateg = txtb_searchCateg.Text;

            if (txtb_searchCateg.Text == "Ex. Small, Medium, Large" && txtb_searchCateg.ForeColor == Color.Gray)
            {
                txtb_searchCateg.Text = "Ex. Small, Medium, Large";
                txtb_searchCateg.ForeColor = Color.Gray;
            }
            else
            {
                if (searchCateg.Equals(""))
                {
                    txtb_searchCateg.Text = "Ex. Small, Medium, Large";
                    txtb_searchCateg.ForeColor = Color.Gray;
                }
                else
                {
                    txtb_searchCateg.Text = searchCateg;
                    txtb_searchCateg.ForeColor = Color.Black;
                }
            }
        }

        private void txtb_searchCateg_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tbsearchcateg = (TextBox)sender;
            cursorPos = tbsearchcateg.SelectionStart;

            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' ' || (char)Keys.Back == e.KeyChar)
            //Allows letters, space and backspace.
            {

                if ((txtb_searchCateg.SelectionStart == 0) && (e.KeyChar == (char)Keys.Space))
                //Best way to remove leading spaces.
                {
                    e.Handled = true;
                }

                if ((char)Keys.Back == e.KeyChar)
                {
                    cursorPos = txtb_searchCateg.SelectionStart - 1;
                }

                if ((e.KeyChar == ' ') && (tbsearchcateg.Text.Length > 0))
                {
                    if (tbsearchcateg.Text[tbsearchcateg.Text.Length - 1] == ' ' && tbsearchcateg.SelectionStart == tbsearchcateg.Text.Length)
                    {
                        e.Handled = true;
                    }
                }

                if (tbsearchcateg.SelectionStart < tbsearchcateg.Text.Length && tbsearchcateg.SelectionStart > 0)
                {
                    if (tbsearchcateg.Text[tbsearchcateg.SelectionStart - 1] == ' ' || tbsearchcateg.Text[tbsearchcateg.SelectionStart] == ' ')
                    {
                        if (txtb_searchCateg.SelectedText == "" && e.KeyChar == ' ')
                        {
                            if (txtb_searchCateg.Text[txtb_searchCateg.Text.Length - 1] == ' ' && txtb_searchCateg.SelectionStart < txtb_searchCateg.Text.Length)
                            {
                                txtb_searchCateg.Focus();
                                txtb_searchCateg.Text = txtb_searchCateg.Text.TrimEnd(' ');
                                txtb_searchCateg.SelectionStart = cursorPos;
                            }

                            e.Handled = true;
                        }
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtb_searchCateg_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {

                int curs = txtb_searchCateg.SelectionStart;
                if (txtb_searchCateg.SelectionStart < txtb_searchCateg.Text.Length && txtb_searchCateg.Text[txtb_searchCateg.Text.Length - 1] == ' ' && txtb_searchCateg.SelectedText == string.Empty)
                {
                    txtb_searchCateg.Focus();
                    txtb_searchCateg.Text = txtb_searchCateg.Text.TrimEnd(' ');
                    txtb_searchCateg.SelectionStart = curs;
                }
            }
        }

        private void txtb_searchCateg_MouseDown(object sender, MouseEventArgs e)
        {
            int curs = txtb_searchCateg.SelectionStart;
            if (txtb_searchCateg.SelectionStart < txtb_searchCateg.Text.Length && txtb_searchCateg.Text[txtb_searchCateg.Text.Length - 1] == ' ' && txtb_searchCateg.SelectedText == string.Empty)
            {
                txtb_searchCateg.Focus();
                txtb_searchCateg.Text = txtb_searchCateg.Text.TrimEnd(' ');
                txtb_searchCateg.SelectionStart = curs;
            }
        }

        private void txtb_searchCateg_TextChanged(object sender, EventArgs e)
        {
            if (txtb_searchCateg.ForeColor != Color.Gray && txtb_searchCateg.Text != "Ex. Small, Medium, Large")
            {
                if (txtb_searchCateg.Text != "" && txtb_searchCateg.Text[0] == ' ')
                {
                    txtb_searchCateg.Text = txtb_searchCateg.Text.TrimStart(' ');
                }

                if (txtb_searchCateg.Text.Contains("  "))
                {
                    txtb_searchCateg.Focus();
                    txtb_searchCateg.Text = txtb_searchCateg.Text.Replace("  ", " ");
                    txtb_searchCateg.SelectionStart = cursorPos;

                    if (txtb_searchCateg.Text[txtb_searchCateg.SelectionStart - 1] != ' ' && txtb_searchCateg.SelectedText == "")
                    {
                        txtb_searchCateg.SelectionStart = txtb_searchCateg.SelectionStart + 1;
                    }
                }

                try
                {
                    cn.Open();
                    cmd.Connection = cn;

                    cmd.CommandText = ("SELECT CONCAT('C-', prod_categID), prod_categname, Status FROM tbl_category;");

                    MySqlDataAdapter da = new MySqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bSource = new BindingSource();
                    bSource.DataSource = dt;

                    dgv_viewcateg.DataSource = bSource;

                    da.Update(dt);

                    DataView dv = dt.DefaultView;
                    dv.RowFilter = string.Format("prod_categname like '%{0}%'", txtb_searchCateg.Text);
                    dgv_viewcateg.DataSource = dv.ToTable();

                    dgv_viewcateg.ClearSelection();
                }
                catch
                {

                }
                finally
                {
                    cn.Close();
                }
            }
        }

        private void txtb_searchUser_Enter(object sender, EventArgs e)
        {
            if (txtb_searchUser.Text == "Ex. Allarey, Alejandro, Angelo" && txtb_searchUser.ForeColor == Color.Gray)
            {
                txtb_searchUser.Text = "";
                txtb_searchUser.ForeColor = Color.Black;

            }
        }

        private void txtb_searchUser_Leave(object sender, EventArgs e)
        {
            string searchUser = txtb_searchUser.Text;

            if (txtb_searchUser.Text == "Ex. Allarey, Alejandro, Angelo" && txtb_searchUser.ForeColor == Color.Gray)
            {
                txtb_searchUser.Text = "Ex. Allarey, Alejandro, Angelo";
                txtb_searchUser.ForeColor = Color.Gray;
            }
            else
            {
                if (searchUser.Equals(""))
                {
                    txtb_searchUser.Text = "Ex. Allarey, Alejandro, Angelo";
                    txtb_searchUser.ForeColor = Color.Gray;
                }
                else
                {
                    txtb_searchUser.Text = searchUser;
                    txtb_searchUser.ForeColor = Color.Black;
                }

            }
        }
        
        private void txtb_searchUser_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tbsearchuser = (TextBox)sender;
            cursorPos = tbsearchuser.SelectionStart;

            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' ' || (char)Keys.Back == e.KeyChar)
            //Allows letters, space and backspace.
            {
                if ((txtb_searchUser.SelectionStart == 0) && (e.KeyChar == (char)Keys.Space))
                //Best way to remove leading spaces.
                {
                    e.Handled = true;
                }

                if ((char)Keys.Back == e.KeyChar)
                {
                    cursorPos = txtb_searchUser.SelectionStart - 1;

                }

                if ((e.KeyChar == ' ') && (tbsearchuser.Text.Length > 0))
                {
                    if (tbsearchuser.Text[tbsearchuser.Text.Length - 1] == ' ' && tbsearchuser.SelectionStart == tbsearchuser.Text.Length)
                    {
                        e.Handled = true;
                    }
                }

                if (tbsearchuser.SelectionStart < tbsearchuser.Text.Length && tbsearchuser.SelectionStart > 0)
                {
                    if (tbsearchuser.Text[tbsearchuser.SelectionStart - 1] == ' ' || tbsearchuser.Text[tbsearchuser.SelectionStart] == ' ')
                    {
                        if (txtb_searchUser.SelectedText == "" && e.KeyChar == ' ')
                        {
                            if (txtb_searchUser.Text[txtb_searchUser.Text.Length - 1] == ' ' && txtb_searchUser.SelectionStart < txtb_searchUser.Text.Length)
                            {
                                txtb_searchUser.Focus();
                                txtb_searchUser.Text = txtb_searchUser.Text.TrimEnd(' ');
                                txtb_searchUser.SelectionStart = cursorPos;
                            }

                            e.Handled = true;
                        }
                    }
                }
            }

            else
            {
                e.Handled = true;
            }
        }

        private void txtb_searchUser_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                int curs = txtb_searchUser.SelectionStart;
                if (txtb_searchUser.SelectionStart < txtb_searchUser.Text.Length && txtb_searchUser.Text[txtb_searchUser.Text.Length - 1] == ' ' && txtb_searchUser.SelectedText == string.Empty)
                {
                    txtb_searchUser.Focus();
                    txtb_searchUser.Text = txtb_searchUser.Text.TrimEnd(' ');

                    txtb_searchUser.SelectionStart = curs;
                }

            }
        }

        private void txtb_searchUser_MouseDown(object sender, MouseEventArgs e)
        {
            int curs = txtb_searchUser.SelectionStart;
            if (txtb_searchUser.SelectionStart < txtb_searchUser.Text.Length && txtb_searchUser.Text[txtb_searchUser.Text.Length - 1] == ' ' && txtb_searchUser.SelectedText == string.Empty)
            {
                txtb_searchUser.Focus();
                txtb_searchUser.Text = txtb_searchUser.Text.TrimEnd(' ');

                txtb_searchUser.SelectionStart = curs;
            }
        }

        private void txtb_searchUser_TextChanged(object sender, EventArgs e)
        {
            if (txtb_searchUser.ForeColor != Color.Gray && txtb_searchUser.Text != "Ex. Allarey, Alejandro, Angelo")
            {
                cb_rolefilter2.SelectedIndex = -1;

                if (txtb_searchUser.Text != "" && txtb_searchUser.Text[0] == ' ')
                {
                    txtb_searchUser.Text = txtb_searchUser.Text.TrimStart(' ');
                }

                if (txtb_searchUser.Text.Contains("  "))
                {
                    txtb_searchUser.Focus();
                    txtb_searchUser.Text = txtb_searchUser.Text.Replace("  ", " ");

                    txtb_searchUser.SelectionStart = cursorPos;

                    if (txtb_searchUser.Text[txtb_searchUser.SelectionStart - 1] != ' ' && txtb_searchUser.SelectedText == "")
                    {
                        txtb_searchUser.SelectionStart = txtb_searchUser.SelectionStart + 1;
                    }
                }

                try
                {
                    cn.Open();
                    cmd.Connection = cn;

                    cmd.CommandText = ("SELECT CONCAT('U-', UserID), FirstName, LastName, Address, PhoneNumber, SHA1(Username), SHA1(Password), UserType FROM tbl_login;");

                    MySqlDataAdapter da = new MySqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bSource = new BindingSource();
                    bSource.DataSource = dt;

                    dgv_viewuser.DataSource = bSource;

                    da.Update(dt);

                    DataView dv = dt.DefaultView;
                    dv.RowFilter = string.Format("LastName like '%{0}%'", txtb_searchUser.Text);
                    dgv_viewuser.DataSource = dv.ToTable();

                    dgv_viewuser.Columns[0].Width = 130;

                    dgv_viewuser.ClearSelection();

                }
                catch
                {

                }
                finally
                {
                    cn.Close();
                }
            }
        }

        private void txtb_searchorno_TextChanged(object sender, EventArgs e)
        {
            if (txtb_searchorno.ForeColor != Color.Gray && txtb_searchorno.Text != "Ex. 012345678")
            {
                try
                {
                    cn.Open();
                    cmd.Connection = cn;

                    cmd.CommandText = "SELECT B.orderNo, A.prod_name AS 'Product Name', D.prod_categname AS 'Category', C.Quantity, C.unitprice AS 'Unit Price', C.subtotal AS 'Sub Total', Time_Format(B.ordertime, '%r') AS 'Order Time', B.orderdate AS 'Order Date' FROM tbl_product A INNER JOIN tbl_category D ON A.prod_categID = D.prod_categID INNER JOIN tbl_orderdetails C ON A.prod_ID = C.prod_ID INNER JOIN tbl_ordermain B ON B.orderID = C.orderID ORDER BY recNo DESC;";

                    MySqlDataAdapter da = new MySqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bSource = new BindingSource();
                    bSource.DataSource = dt;

                    dgv_orderhistor.DataSource = bSource;

                    da.Update(dt);

                    DataView dv = dt.DefaultView;
                    dv.RowFilter = string.Format("orderNo like '%{0}%'", txtb_searchorno.Text);
                    dgv_orderhistor.DataSource = dv.ToTable();

                    dgv_orderhistor.ClearSelection();

                }
                catch
                {

                }
                finally
                {
                    cn.Close();
                }

            }
        }

        private void txtb_searchorno_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar)) //Only numbers. (No whitespaces, letters, symbols)
            {
                
            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void btn_prodsaveimage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "png files(*.png) | *.png | jpg files (*.jpg) | *.jpg| All files(*.*) | *.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                currentImage = dialog.FileName.ToString();
                pb_prodimage.ImageLocation = currentImage;

            }
        }

        string prodmainFormAction;

        private void btn_saveprod_Click(object sender, EventArgs e)
        {
            // ---------------------------------------------------------------------------------------
            // Add Product

            if (prodmainFormAction == "Add Product")
            {
                
                //Trim at the start. This will remove space at start & end.
                txtb_prodname.Text = txtb_prodname.Text.Trim();
                txtb_proddesc.Text = txtb_proddesc.Text.Trim();

                try
                {
                    if (txtb_prodname.Text == "")
                    {
                        MessageBox.Show("Please enter the product's name", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtb_prodname.Focus();
                        return;
                    }
                    if (txtb_proddesc.Text == "")
                    {
                        MessageBox.Show("Please enter the product's description", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtb_proddesc.Focus();
                        return;
                    }
                    if (cb_categ.Text == "")
                    {
                        MessageBox.Show("Please select the product's category", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cb_categ.Focus();
                        return;
                    }
                    if (txtb_prodUprice.Text == "")
                    {
                        MessageBox.Show("Please enter the product's unit price", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtb_prodUprice.Focus();
                        return;
                    }
                    if (txtb_prodUprice.Text == "0.00" || txtb_prodUprice.Text == "0" || txtb_prodUprice.Text == "0.0")
                    {
                        MessageBox.Show("Please enter the right unit's price", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtb_prodUprice.Focus();
                        return;
                    }

                   // ---------------------------------------------------------------------------------------

                    try //Checks if the product name is alrd exists.
                    {
                        cn.Open();
                        cmd.Connection = cn;

                        cmd.CommandText = "Select count(prod_name) from tbl_product where prod_name = '" + txtb_prodname.Text + "';";

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count >= 1)
                        {
                            MessageBox.Show("Product name is already registered! Please try another one", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtb_prodname.Clear();
                            txtb_prodname.Focus();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        cn.Close(); //Force connection to close.
                    }

                    // ---------------------------------------------------------------------------------------

                    cn.Open(); //Insert product to database.
                    cmd.Connection = cn;

                    byte[] prodimage = null;

                    FileStream Stream = new FileStream(currentImage, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(Stream);
                    prodimage = br.ReadBytes((int)Stream.Length);

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new MySqlParameter("@prod_image", prodimage));

                    cmd.CommandText = "INSERT INTO tbl_product (prod_ID, prod_name, prod_desc, prod_categID, unit_price, status, prod_image) VALUES ('" + txtprodID.Text.Replace("P-", "") + "','" + txtb_prodname.Text + "', '" + txtb_proddesc.Text + "','" + cb_categ.SelectedValue + "','" + txtb_prodUprice.Text + "','" + lb_prodStat.Text + "', @prod_image) ;";

                    // It needs these if you are inserting a data to the database.
                    MySqlDataReader Read;
                    Read = cmd.ExecuteReader();
                    // -------------------------------

                    MessageBox.Show("Product saved", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception)
                {
                    MessageBox.Show("Image size is too large", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    cn.Close(); //Force connection to close.
                }

                // ---------------------------------------------------------------------------------------

                try //Insert adding data to audit trail.
                {
                    cn.Open();
                    cmd.Connection = cn;

                    string getadddata = "Inserted a new product: " + "\"" + txtb_prodname.Text + "\"";
                    string action = "Insert";

                    cmd.CommandText = "INSERT INTO tbl_audittrail (User, Activity, Action, Date, Time) VALUES ('" + lb_name.Text + "','" + getadddata + "', '" + action + "','" + lbl_dateOrder.Text + "','" + datetimeHMS + "') ;";

                    // It needs these if you are inserting a data to the database.
                    MySqlDataReader Read;
                    Read = cmd.ExecuteReader();
                    // -------------------------------
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(ex2.ToString());
                }
                finally
                {
                    cn.Close(); //Force connection to close.
                }

                // ---------------------------------------------------------------------------------------

                txtb_prodname.Clear(); txtb_proddesc.Clear(); txtb_prodUprice.Clear();
                lb_prodStat.Text = "Inactive";
                pb_prodimage.Image = null; currentImage = @"..\\Icon\\no-image-box.png";

                viewAudittrail();
                viewProductList();
                prodIDIncre();

                updCategtoActive();
                cb_categ.SelectedIndex = -1;

                // ---------------------------------------------------------------------------------------
                //Reset product search and filter

                if (txtbsearch_prodmain.Text != "Ex. Apple, Banana, Ice Cream" && txtbsearch_prodmain.ForeColor != Color.Gray)
                {
                    txtbsearch_prodmain.Text = "Ex. Apple, Banana, Ice Cream";
                    txtbsearch_prodmain.ForeColor = Color.Gray;
                }
                cb_prodfilter.SelectedIndex = -1;

            }

            // ---------------------------------------------------------------------------------------
            // Edit Product

            if (prodmainFormAction == "Edit Product")
            {
                string newproduct = "";
                string newdesc = "";
                string newcateg = "";
                string newuprice = "";

                // this will remove space at start and end
                txtb_prodname.Text = txtb_prodname.Text.Trim();
                txtb_proddesc.Text = txtb_proddesc.Text.Trim();

                try
                {

                    if (txtb_prodname.Text == "")
                    {
                        MessageBox.Show("Please enter the product's name", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtb_prodname.Focus();
                        return;
                    }
                    if (txtb_proddesc.Text == "")
                    {
                        MessageBox.Show("Please enter the product's description", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtb_proddesc.Focus();
                        return;
                    }
                    if (cb_categ.Text == "")
                    {
                        MessageBox.Show("Please select the product's category", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cb_categ.Focus();
                        return;
                    }
                    if (txtb_prodUprice.Text == "")
                    {
                        MessageBox.Show("Please enter the product's unit price", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtb_prodUprice.Focus();
                        return;
                    }

                    MemoryStream ms = new MemoryStream();
                    pb_prodimage.Image.Save(ms, pb_prodimage.Image.RawFormat);
                    byte[] images = ms.ToArray();


                    cn.Open();
                    cmd.Connection = cn;

                    int RowsAffected = 0;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new MySqlParameter("@prod_image", images));

                    cmd.CommandText = "update tbl_product set prod_name ='" + txtb_prodname.Text + "', prod_desc = '" + txtb_proddesc.Text + "', prod_categID = '" + cb_categ.SelectedValue + "', unit_price = '" + txtb_prodUprice.Text + "', status = '" + lb_prodStat.Text + "', prod_image =  @prod_image where prod_ID = '" + txtprodID.Text.Replace("P-", "") + "';";

                    RowsAffected = cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Edited", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (RowsAffected > 0)
                    {


                    }
                    else
                    {

                        MessageBox.Show("No ID Found", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());

                }
                finally
                {
                    cn.Close();
                }

                newproduct = txtb_prodname.Text;
                newdesc = txtb_proddesc.Text;
                newcateg = cb_categ.Text;
                newuprice = txtb_prodUprice.Text;

                //Checking the status of category.
                updCategtoInactive();
                updCategtoActive();
                // -----------------------

                txtb_prodname.Clear(); txtb_proddesc.Clear(); cb_categ.SelectedIndex = -1; txtb_prodUprice.Clear();
                lb_prodStat.Text = "Inactive";
                pb_prodimage.Image = null; currentImage = @"..\\Icon\\no-image-box.png";

                prodIDIncre();  
                viewProductList();

                gbhold_addmain.Enabled = true;
                cb_prodfilter.Visible = true;
                pa_addprod.Visible = false;

                // ---------------------------------------------------------------------------------------
                //Reset product search and filter

                if (txtbsearch_prodmain.Text != "Ex. Apple, Banana, Ice Cream" && txtbsearch_prodmain.ForeColor != Color.Gray)
                {
                    txtbsearch_prodmain.Text = "Ex. Apple, Banana, Ice Cream";
                    txtbsearch_prodmain.ForeColor = Color.Gray;
                }
                cb_prodfilter.SelectedIndex = -1;

                // ---------------------------------------------------------------------------------------
                //Insert updated data to audit trail.

                if (oldproduct == newproduct)
                {
                    oldproduct = "";
                    newproduct = "";
                }
                else if (oldproduct != newproduct)
                {
                    oldproduct = "Product Name: " + "\"" + oldproduct + "\""+ " to ";
                    newproduct = "\"" + newproduct + "\"" + ", ";
                }
                if (olddesc == newdesc)
                {
                    olddesc = "";
                    newdesc = "";
                }
                else if (olddesc != newdesc)
                {
                    olddesc = "Description: " + "\"" + olddesc + "\"" + " to ";
                    newdesc = "\"" + newdesc + "\"" + ", ";
                }
                if (oldcateg == newcateg)
                {
                    oldcateg = "";
                    newcateg = "";
                }
                else if (oldcateg != newcateg)
                {
                    oldcateg = "Category: " + "\"" + oldcateg + "\"" + " to ";
                    newcateg = "\"" + newcateg + "\"" + ", ";
                }
                if (olduprice == newuprice)
                {
                    olduprice = "";
                    newuprice = "";
                }
                else if (olduprice != newuprice)
                {
                    olduprice = "Unit Price: " + "\"" + olduprice + "\"" + " to ";
                    newuprice = "\"" + newuprice + "\""+ ", ";
                }

                if (newproduct != "" || newdesc != "" || newcateg != "" || newuprice != "")
                {
                    try
                    {
                        cn.Open();
                        cmd.Connection = cn;

                        string getadddata = "Edited a product: " + oldproduct + newproduct + olddesc + newdesc + oldcateg + newcateg + olduprice + newuprice;
                        string action = "Edit";

                        cmd.CommandText = "INSERT INTO tbl_audittrail (User,Activity,Action,Date,Time) VALUES ('" + lb_name.Text + "','" + getadddata + "', '" + action + "','" + lbl_dateOrder.Text + "','" + datetimeHMS + "') ;";

                        MySqlDataReader mRead;
                        mRead = cmd.ExecuteReader();

                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.ToString());
                    }

                    finally
                    {
                        cn.Close();
                    }
      
                    viewAudittrail();

                }

            }

        }

        private void btn_delprod_Click(object sender, EventArgs e)
        {
            // -------------------------------------------------------------------
            // Delete product from the database.

            if (dgv_prodmain.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select a product to delete", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (dgv_prodmain.SelectedRows[0].Cells[5].Value.ToString() == "Active")
                {

                    MessageBox.Show("Product is currently active and cannot be deleted", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {

                    cb_categ.Text = dgv_prodmain.SelectedRows[0].Cells[3].Value.ToString();
                    oldcategforstatus = cb_categ.SelectedValue.ToString();

                    try
                    {
                        DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this product?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {

                            cn.Open();
                            cmd.Connection = cn;

                            cmd.CommandText = " delete from tbl_product where prod_ID = '" + dgv_prodmain.SelectedRows[0].Cells[0].Value.ToString().Replace("P-", "") + "' ;";

                            int RowsAffected = cmd.ExecuteNonQuery();

                            if (RowsAffected == 0)
                            {

                            }
                            else if (RowsAffected > 0)
                            {
                                MessageBox.Show("Successfully Deleted", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);                          
                                gbhold_addmain.Focus();
                            }

                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            gbhold_addmain.Focus();
                            return;
                        }

                        else
                        {
                            MessageBox.Show("No ID Found", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        cn.Close();
                    }

                    // -------------------------------------------------------------------
                    // Inserting deleted data to audit trail.

                    try
                    {
                        cn.Open();
                        cmd.Connection = cn;

                        string getdeletedata = "Deleted a product: " + "\"" + dgv_prodmain.SelectedRows[0].Cells[1].Value.ToString() + "\"";
                        string action = "Delete";

                        cmd.CommandText = "INSERT INTO tbl_audittrail (User,Activity,Action,Date,Time) VALUES ('" + lb_name.Text + "','" + getdeletedata + "', '" + action + "','" + lbl_dateOrder.Text + "','" + datetimeHMS + "') ;";


                        MySqlDataReader mRead;
                        mRead = cmd.ExecuteReader();

                    }
                    catch (Exception ex2)
                    {

                        MessageBox.Show(ex2.ToString());

                    }
                    finally
                    {
                        cn.Close();
                    }

                    viewAudittrail();
                    viewProductList();

                    prodIDIncre();
                    updCategtoInactive();

                    // ---------------------------------------------------------------------------------------
                    //Reset product search and filter

                    if (txtbsearch_prodmain.Text != "Ex. Apple, Banana, Ice Cream" && txtbsearch_prodmain.ForeColor != Color.Gray)
                    {
                        txtbsearch_prodmain.Text = "Ex. Apple, Banana, Ice Cream";
                        txtbsearch_prodmain.ForeColor = Color.Gray;
                    }
                    cb_prodfilter.SelectedIndex = -1;

                }
            }
        }

        private void btn_clearprod_Click(object sender, EventArgs e)
        {
            if (txtb_prodname.Text != "" || txtb_proddesc.Text != "" || cb_categ.Text != "" || txtb_prodUprice.Text != "" || pb_prodimage.Image != null)
            {
                DialogResult diagResult = MessageBox.Show("Are you sure you want to clear all the data in the field?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (diagResult == DialogResult.Yes)
                {
                    txtb_prodname.Clear(); txtb_proddesc.Clear(); txtb_prodUprice.Clear();
                    cb_categ.SelectedIndex = -1;
                    pb_prodimage.Image = null; currentImage = @"..\\Icon\\no-image-box.png";
                }
            }

        }

        private void btn_addprod_Click(object sender, EventArgs e)
        {
            txtprodmainTitle.Text = "Add Product";
            prodmainFormAction = "Add Product";

            gbhold_addmain.Enabled = false;
            cb_prodfilter.Visible = false;

            lb_prodStat.Text = "Inactive"; lb_prodStat.ForeColor = Color.DimGray; lbl_isActive.Visible = false;
            txtb_prodname.Clear(); txtb_proddesc.Clear(); txtb_prodUprice.Clear(); txtb_prodname.Enabled = true; txtb_proddesc.Enabled = true; txtb_prodUprice.Enabled = true;
            cb_categ.SelectedIndex = -1; cb_categ.Enabled = true; txtb_cbcategholder.Visible = true;
            pb_prodimage.Image = null; currentImage = @"..\\Icon\\no-image-box.png";
            btn_saveprod.Enabled = true; btn_clearprod.Enabled = true; btn_prodsaveimage.Enabled = true;

            prodIDIncre();

            pa_addprod.Visible = true;
            txtb_prodname.Focus();
        }



        private void btn_editprod_Click(object sender, EventArgs e)
        {
            if (dgv_prodmain.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select a product to edit", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (dgv_prodmain.SelectedRows.Count == 1)
                {
                    lb_deloreditID.Text = dgv_prodmain.SelectedRows[0].Cells[0].Value.ToString();
                    txtprodID.Text = dgv_prodmain.SelectedRows[0].Cells[0].Value.ToString();
                    txtb_prodname.Text = dgv_prodmain.SelectedRows[0].Cells[1].Value.ToString();
                    txtb_proddesc.Text = dgv_prodmain.SelectedRows[0].Cells[2].Value.ToString();
                    cb_categ.Text = dgv_prodmain.SelectedRows[0].Cells[3].Value.ToString();
                    txtb_prodUprice.Text = dgv_prodmain.SelectedRows[0].Cells[4].Value.ToString();

                    lb_prodStat.Text = dgv_prodmain.SelectedRows[0].Cells[5].Value.ToString();

                    oldproduct = dgv_prodmain.SelectedRows[0].Cells[1].Value.ToString();
                    olddesc = dgv_prodmain.SelectedRows[0].Cells[2].Value.ToString();
                    oldcateg = dgv_prodmain.SelectedRows[0].Cells[3].Value.ToString();
                    olduprice = dgv_prodmain.SelectedRows[0].Cells[4].Value.ToString();

                    oldcategforstatus = cb_categ.SelectedValue.ToString();

                    byte[] images2 = (byte[])dgv_prodmain.SelectedRows[0].Cells[6].Value;
                    MemoryStream ms = new MemoryStream(images2);
                    pb_prodimage.Image = Image.FromStream(ms);

                    if (lb_prodStat.Text == "Active")
                    {
                        lb_prodStat.ForeColor = Color.DeepSkyBlue;
                        txtb_prodname.Enabled = false; txtb_proddesc.Enabled = false; cb_categ.Enabled = false; txtb_cbcategholder.Visible = false; txtb_prodUprice.Enabled = false;
                        btn_prodsaveimage.Enabled = false; btn_saveprod.Enabled = false; btn_clearprod.Enabled = false;
                        lbl_isActive.Visible = true;
                    }
                    else if (lb_prodStat.Text == "Inactive")
                    {
                        lb_prodStat.ForeColor = Color.DimGray;
                        txtb_prodname.Enabled = true; txtb_proddesc.Enabled = true; cb_categ.Enabled = true; txtb_cbcategholder.Visible = true; txtb_prodUprice.Enabled = true;
                        btn_prodsaveimage.Enabled = true; btn_saveprod.Enabled = true; btn_clearprod.Enabled = true;
                        lbl_isActive.Visible = false;
                    }
                }

                gbhold_addmain.Enabled = false;
                cb_prodfilter.Visible = false;

                txtprodmainTitle.Text = "Edit Product";
                prodmainFormAction = "Edit Product";
                pa_addprod.Visible = true;
            }

        }

        private void btn_extaddprod_Click_1(object sender, EventArgs e)
        {
            gbhold_addmain.Enabled = true;
            cb_prodfilter.Visible = true;
            pa_addprod.Visible = false;

        }

        private void dgv_prodmain_MouseClick(object sender, MouseEventArgs e)
        {
            
         
        }

        private void txtb_prodname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' ') //Can use spaces in between each words.
            {

            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void txtb_prodname_TextChanged(object sender, EventArgs e)
        {
            txtb_prodname.Text = txtb_prodname.Text.TrimStart(); //Removes space at start.

            if (txtb_prodname.Text == "")
            {
                lb_asterisk1.Visible = true;

            }
            else if (txtb_prodname.Text != "")
            {

                lb_asterisk1.Visible = false;

            }

            // - This removes the multiple spaces and starts the cursor at the end of text
            if (txtb_prodname.Text.Contains("  "))
            {

                txtb_prodname.Text = txtb_prodname.Text.Replace("  ", " ");
                txtb_prodname.SelectionStart = txtb_prodname.Text.Length;

            }
            // --------------------------------------------------------------------------
           
        }

        private void txtb_proddesc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && e.KeyChar != '\'' && e.KeyChar != '\"' && e.KeyChar != '=' && e.KeyChar != ';' && e.KeyChar != '|' && e.KeyChar != '{' && e.KeyChar != '}')
            {

            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void txtb_proddesc_TextChanged_1(object sender, EventArgs e)
        {
            txtb_proddesc.Text = txtb_proddesc.Text.TrimStart(); //Removes whitespaces at the start.

            if (txtb_proddesc.Text == "")
            {
                lb_asterisk4.Visible = true;

            }
            else if (txtb_proddesc.Text != "")
            {

                lb_asterisk4.Visible = false;

            }

            // - This removes the multiple spaces and starts the cursor at the end of text
            if (txtb_proddesc.Text.Contains("  "))
            {
                txtb_proddesc.Text = txtb_proddesc.Text.Replace("  ", " ");
                txtb_proddesc.SelectionStart = txtb_proddesc.Text.Length;
            }
            // --------------------------------------------------------------------------
        }

        private void txtb_prodUprice_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                //If unit price does not contain (.) then maxlength are 7
                if (!txtb_prodUprice.Text.Contains("."))
                {
                    txtb_prodUprice.MaxLength = 7;
                }
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains('.'))
                //If textbox has (.) then this condition will run.
            {
                //If unit price contain (.) then maxlength are 10
                txtb_prodUprice.MaxLength = 10;
            }

            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void txtb_prodUprice_TextChanged_1(object sender, EventArgs e)
        {
            //This will only accept 2 decimal after the dot.
            string word = txtb_prodUprice.Text.Trim();
            string[] wordArr = word.Split('.');
            if (wordArr.Length > 1) //No. of dots (which only sets to 1)
            {
                string afterDot = wordArr[1];
                if (afterDot.Length > 2)
                {
                    txtb_prodUprice.Text = wordArr[0] + "." + afterDot.Substring(0, 2);
                    txtb_prodUprice.SelectionStart = txtb_prodUprice.Text.Length;
                }
            }

            string[] zerodigits = { "01", "02", "03", "04", "05", "06", "07", "08", "09" }; //also set if the input number are 0.00

            foreach (string str1 in zerodigits)
            {
                if (txtb_prodUprice.Text == str1)
                {
                    txtb_prodUprice.Text = str1.Split('0')[1];
                    txtb_prodUprice.SelectionStart = txtb_prodUprice.Text.Length;
                }
                else if (txtb_prodUprice.Text == "00")
                {
                    txtb_prodUprice.Text = "0";
                    txtb_prodUprice.SelectionStart = txtb_prodUprice.Text.Length;
                }
            }

            if (txtb_prodUprice.Text == ".")
            {
                txtb_prodUprice.Text = "";
            }
            else if (txtb_prodUprice.Text == "")
            {
                lb_asterisk3.Visible = true;
            }
            else if (txtb_prodUprice.Text != "")
            {
                lb_asterisk3.Visible = false;
            }
        }

        private void cb_categ_TextChanged_1(object sender, EventArgs e)
        {
            if (cb_categ.Text == "")
            {
                lb_asterisk2.Visible = true;
            }
            else if (cb_categ.Text != "")
            {
                lb_asterisk2.Visible = false;
            }
        }

        private void pa_addprod_Paint(object sender, PaintEventArgs e)
        {
         
        }

        private void pa_brderColor_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.pa_brderColor.ClientRectangle, Color.MediumVioletRed, ButtonBorderStyle.Solid);
        }


        private void pa_prodmain_Leave(object sender, EventArgs e)
        {
            //pa_addprod.Visible = false;
        }

        private void txtbsearch_prodmain_Enter(object sender, EventArgs e)
        {
            if (txtbsearch_prodmain.Text == "Ex. Apple, Banana, Ice Cream" && txtbsearch_prodmain.ForeColor == Color.Gray)
            {
                txtbsearch_prodmain.Text = "";
                txtbsearch_prodmain.ForeColor = Color.Black;

            }
        }

        private void txtbsearch_prodmain_Leave(object sender, EventArgs e)
        {
            string searchProd = txtbsearch_prodmain.Text;

            if (txtbsearch_prodmain.Text == "Ex. Apple, Banana, Ice Cream" && txtbsearch_prodmain.ForeColor == Color.Gray)
            {
                txtbsearch_prodmain.Text = "Ex. Apple, Banana, Ice Cream";
                txtbsearch_prodmain.ForeColor = Color.Gray;
            }
            else
            {
                if (searchProd.Equals(""))
                {
                    txtbsearch_prodmain.Text = "Ex. Apple, Banana, Ice Cream";
                    txtbsearch_prodmain.ForeColor = Color.Gray;
                }
                else
                {
                    txtbsearch_prodmain.Text = searchProd;
                    txtbsearch_prodmain.ForeColor = Color.Black;
                }

            }
        }

        int cursorPos;
        
        private void txtbsearch_prodmain_MouseDown(object sender, MouseEventArgs e)
        {
            int curs = txtbsearch_prodmain.SelectionStart;
            if (txtbsearch_prodmain.SelectionStart < txtbsearch_prodmain.Text.Length && txtbsearch_prodmain.Text[txtbsearch_prodmain.Text.Length - 1] == ' ' && txtbsearch_prodmain.SelectedText == string.Empty)
            {
                txtbsearch_prodmain.Focus();
                txtbsearch_prodmain.Text = txtbsearch_prodmain.Text.TrimEnd(' ');

                txtbsearch_prodmain.SelectionStart = curs;
            }
        }

        private void txtbsearch_prodmain_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tbsearchprod = (TextBox)sender;
            cursorPos = tbsearchprod.SelectionStart;

            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' ' || (char)Keys.Back == e.KeyChar)
                //Allows letters, space and backspace.
            {

                if ((txtbsearch_prodmain.SelectionStart == 0) && (e.KeyChar == (char)Keys.Space))
                //Best way to remove leading spaces.
                {
                    e.Handled = true;
                }

                if ((char)Keys.Back == e.KeyChar)
                {
                    cursorPos = txtbsearch_prodmain.SelectionStart - 1;
                    
                }

                if ((e.KeyChar == ' ') && (tbsearchprod.Text.Length > 0))
                {
                    if (tbsearchprod.Text[tbsearchprod.Text.Length - 1] == ' ' && tbsearchprod.SelectionStart == tbsearchprod.Text.Length)
                    {
                        e.Handled = true;
                    }
                }

                if (tbsearchprod.SelectionStart < tbsearchprod.Text.Length && tbsearchprod.SelectionStart > 0)
                {
                    if (tbsearchprod.Text[tbsearchprod.SelectionStart - 1] == ' ' || tbsearchprod.Text[tbsearchprod.SelectionStart] == ' ' )
                    {
                        if (txtbsearch_prodmain.SelectedText == "" && e.KeyChar == ' ')
                        {
                            if (txtbsearch_prodmain.Text[txtbsearch_prodmain.Text.Length - 1] == ' ' && txtbsearch_prodmain.SelectionStart < txtbsearch_prodmain.Text.Length)
                            {
                                txtbsearch_prodmain.Focus();
                                txtbsearch_prodmain.Text = txtbsearch_prodmain.Text.TrimEnd(' ');
                                txtbsearch_prodmain.SelectionStart = cursorPos;
                            }

                            e.Handled = true;
                        }
                    }
                }
            }
            
            else
            {
                e.Handled = true;
            }
        }

        private void txtbsearch_prodmain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {

                int curs = txtbsearch_prodmain.SelectionStart;
                if (txtbsearch_prodmain.SelectionStart < txtbsearch_prodmain.Text.Length && txtbsearch_prodmain.Text[txtbsearch_prodmain.Text.Length - 1] == ' ' && txtbsearch_prodmain.SelectedText == string.Empty)
                {
                    txtbsearch_prodmain.Focus();
                    txtbsearch_prodmain.Text = txtbsearch_prodmain.Text.TrimEnd(' ');

                    txtbsearch_prodmain.SelectionStart = curs;

                }

            }
        }

        private void txtbsearch_prodmain_TextChanged_1(object sender, EventArgs e)
        {
            if (txtbsearch_prodmain.ForeColor != Color.Gray && txtbsearch_prodmain.Text != "Ex. Apple, Banana, Ice Cream")
            {
                cb_prodfilter.SelectedIndex = - 1;

                if (txtbsearch_prodmain.Text != "" && txtbsearch_prodmain.Text[0] == ' ')
                {
                    txtbsearch_prodmain.Text = txtbsearch_prodmain.Text.TrimStart(' ');
                }

                if (txtbsearch_prodmain.Text.Contains("  "))
                {
                    txtbsearch_prodmain.Focus();
                    txtbsearch_prodmain.Text = txtbsearch_prodmain.Text.Replace("  ", " ");

                    txtbsearch_prodmain.SelectionStart = cursorPos;

                    if (txtbsearch_prodmain.Text[txtbsearch_prodmain.SelectionStart - 1] != ' ' && txtbsearch_prodmain.SelectedText == "")
                    {
                        txtbsearch_prodmain.SelectionStart = txtbsearch_prodmain.SelectionStart + 1;
                    }
                }

                try
                {
                    cn.Open();
                    cmd.Connection = cn;

                    cmd.CommandText = ("SELECT CONCAT('P-', A.prod_ID), A.prod_name, A.prod_desc, B.prod_categname, A.unit_price, A.status, A.prod_image FROM tbl_product A INNER JOIN tbl_category B ON A.prod_categID = B.prod_categID ORDER BY prod_ID ASC;");

                    MySqlDataAdapter da = new MySqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bSource = new BindingSource();
                    bSource.DataSource = dt;

                    dgv_prodmain.DataSource = bSource;

                    da.Update(dt);

                    DataView dv = dt.DefaultView;
                    dv.RowFilter = string.Format("prod_name like '%{0}%'", txtbsearch_prodmain.Text);
                    dgv_prodmain.DataSource = dv.ToTable();

                    dgv_prodmain.Columns[0].Width = 130;

                    dgv_prodmain.ClearSelection();
                }
                catch
                {

                }
                finally
                {
                    cn.Close();
                }
            }
        }
      
        private void paholder_txtSrprd2_Enter(object sender, EventArgs e)
        {
            txtbsearch_prodmain.Focus();
        }

        private void paholder_textSearchpr_Enter(object sender, EventArgs e)
        {
            gbhold_addmain.Focus();
        }
     
        private void rtxtbsearch_prodmain_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void rtxtbsearch_prodmain_TextChanged(object sender, EventArgs e)
        {

        }

        private void rtxtbsearch_prodmain_Enter(object sender, EventArgs e)
        {

        }

        private void rtxtbsearch_prodmain_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void rtxtbsearch_prodmain_Leave(object sender, EventArgs e)
        {

        }

        private void dgv_prodview_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void paholder_textsrchuser2_Enter(object sender, EventArgs e)
        {
            txtb_searchUser.Focus();
        }

        private void paholder_textsrchuser_Enter(object sender, EventArgs e)
        {
            gbholder_viewuser.Focus();
        }

        private void paholder_textsrchcat2_Enter(object sender, EventArgs e)
        {
            txtb_searchCateg.Focus();
        }

        private void paholder_textsrchcat_Enter(object sender, EventArgs e)
        {
            gbholder_viewcateg.Focus();
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
           
        }

        string categmainFormAction;

        private void btn_savecateg_Click(object sender, EventArgs e)
        {
            // ---------------------------------------------------------------------------------------
            //Add Category

            if (categmainFormAction == "Add Category")
            {
                //Trim at the start. This will remove space at start & end.
                txtb_categname.Text = txtb_categname.Text.Trim();

                try
                {
                    if (txtb_categname.Text == "")
                    {
                        MessageBox.Show("Please enter the category's name", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtb_categname.Focus();
                        return;
                    }

                    // ---------------------------------------------------------------------------------------

                    try //Checks if the category name is alrd exists.
                    {
                        cn.Open();
                        cmd.Connection = cn;

                        cmd.CommandText = "SELECT COUNT(prod_categname) FROM tbl_category WHERE prod_categname = '" + txtb_categname.Text + "';";

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count >= 1)
                        {
                            MessageBox.Show("Category name is already registered! Please try another one", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtb_categname.Clear();
                            txtb_categname.Focus();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        cn.Close(); //Force connection to close.
                    }

                    // ---------------------------------------------------------------------------------------

                    cn.Open(); //Insert product to database.
                    cmd.Connection = cn;

                    cmd.CommandText = "INSERT INTO tbl_category (prod_categID, prod_categname, status) VALUES ('" + txtcategID.Text.Replace("C-","") + "', '" + txtb_categname.Text + "', '" + txtcategStatus.Text + "');";

                    // It needs these if you are inserting a data to the database.
                    MySqlDataReader Read;
                    Read = cmd.ExecuteReader();
                    // -------------------------------

                    MessageBox.Show("Category saved", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception ex2)
                {
                    MessageBox.Show(ex2.ToString());
                }
                finally
                {
                    cn.Close(); //Force connection to close.
                }

                // ---------------------------------------------------------------------------------------

                try //Insert adding data to audit trail.
                {
                    cn.Open();
                    cmd.Connection = cn;

                    string getadddata = "Inserted a new category: " + "\"" + txtb_categname.Text + "\"";
                    string action = "Insert";

                    cmd.CommandText = "INSERT INTO tbl_audittrail (User, Activity, Action, Date, Time) VALUES ('" + lb_name.Text + "','" + getadddata + "', '" + action + "','" + lbl_dateOrder.Text + "','" + datetimeHMS + "') ;";

                    // It needs these if you are inserting a data to the database.
                    MySqlDataReader Read;
                    Read = cmd.ExecuteReader();
                    // -------------------------------
                }
                catch (Exception ex3)
                {
                    MessageBox.Show(ex3.ToString());
                }
                finally
                {
                    cn.Close(); //Force connection to close.
                }

                // ---------------------------------------------------------------------------------------

                txtb_categname.Clear(); 
                txtcategStatus.Text = "Inactive";

                viewAudittrail();
                viewCategList();
                categIDIncre();

                fillcategcmbbox(); 
                cb_categ.SelectedIndex = -1;
                cb_prodfilter.SelectedIndex = -1;
                
            }

            // ---------------------------------------------------------------------------------------
            //Edit Category

            if (categmainFormAction == "Edit Category")
            {
                string newcategmain = "";

                //This will remove space at start and end
                txtb_categname.Text = txtb_categname.Text.Trim();

                try
                {

                    if (txtb_categname.Text == "")
                    {
                        MessageBox.Show("Please enter the category's name", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtb_categname.Focus();
                        return;
                    }

                    cn.Open();
                    cmd.Connection = cn;

                    int RowsAffected = 0;

                    cmd.CommandText = "UPDATE tbl_category SET prod_categname ='" + txtb_categname.Text + "', status = '" + txtcategStatus.Text + "' where prod_categID = '" + txtcategID.Text.Replace("C-","") + "';";

                    RowsAffected = cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Edited", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (RowsAffected > 0)
                    {

                    }
                    else
                    {
                        MessageBox.Show("No ID Found", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    cn.Close();
                }

                newcategmain = txtb_categname.Text;

                txtb_categname.Clear();
                txtcategStatus.Text = "Inactive";

                categIDIncre();
                viewCategList();

                fillcategcmbbox();
                cb_categ.SelectedIndex = -1;
                cb_prodfilter.SelectedIndex = -1;

                gbhold_categform.Enabled = true;
                paform_addcateg.Visible = false;

                // ---------------------------------------------------------------------------------------
                //Insert updated data to audit trail.


                if (oldcategmain == newcategmain)
                {
                    oldcategmain = "";
                    newcategmain = "";
                }
                else if (oldcategmain != newcategmain)
                {
                    oldcategmain = "Category: " + "\"" + oldcategmain + "\"" + " to ";
                    newcategmain = "\"" + newcategmain + "\"" + ", ";
                }


                if (newcategmain != "")
                {
                    try
                    {
                        cn.Open();
                        cmd.Connection = cn;

                        string getadddata = "Edited a category: " + oldcategmain + newcategmain;
                        string action = "Edit";

                        cmd.CommandText = "INSERT INTO tbl_audittrail (User,Activity,Action,Date,Time) VALUES ('" + lb_name.Text + "','" + getadddata + "', '" + action + "','" + lbl_dateOrder.Text + "','" + datetimeHMS + "') ;";

                        MySqlDataReader mRead;
                        mRead = cmd.ExecuteReader();

                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.ToString());
                    }

                    finally
                    {
                        cn.Close();
                    }

                    viewAudittrail();
                }

            }
        }

        private void btn_clearcateg_Click(object sender, EventArgs e)
        {
            if (txtb_categname.Text != "")
            {
                DialogResult diagResult = MessageBox.Show("Are you sure you want to clear all the data in the field?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (diagResult == DialogResult.Yes)
                {
                    txtb_categname.Clear();
                }
            }
        }

        private void btn_addcateg_Click(object sender, EventArgs e)
        {
            txtcategmainTitle.Text = "Add Category";
            categmainFormAction = "Add Category";

            gbhold_categform.Enabled = false;

            txtcategStatus.Text = "Inactive"; txtcategStatus.ForeColor = Color.DimGray; txtcateg_isActive.Visible = false;
            txtb_categname.Clear(); txtb_categname.Enabled = true;
            btn_savecateg.Enabled = true; btn_clearcateg.Enabled = true;

            categIDIncre();

            paform_addcateg.Visible = true;
            txtb_categname.Focus();
        }

        private void btn_editcateg_Click(object sender, EventArgs e)
        {
            if (dgv_categlist.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select a category to edit", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (dgv_categlist.SelectedRows.Count == 1)
                {
                    txtcategID.Text = dgv_categlist.SelectedRows[0].Cells[0].Value.ToString();
                    txtb_categname.Text = dgv_categlist.SelectedRows[0].Cells[1].Value.ToString();

                    txtcategStatus.Text = dgv_categlist.SelectedRows[0].Cells[2].Value.ToString();

                    oldcategmain = dgv_categlist.SelectedRows[0].Cells[1].Value.ToString();

                    if (txtcategStatus.Text == "Active")
                    {
                        txtcategStatus.ForeColor = Color.DeepSkyBlue;
                        txtb_categname.Enabled = false;
                        btn_savecateg.Enabled = false; btn_clearcateg.Enabled = false;
                        txtcateg_isActive.Visible = true;
                    }
                    else if (txtcategStatus.Text == "Inactive")
                    {
                        txtcategStatus.ForeColor = Color.DimGray;
                        txtb_categname.Enabled = true;
                        btn_savecateg.Enabled = true; btn_clearcateg.Enabled = true;
                        txtcateg_isActive.Visible = false;
                    }
                }

                gbhold_categform.Enabled = false;

                txtcategmainTitle.Text = "Edit Category";
                categmainFormAction = "Edit Category";
                paform_addcateg.Visible = true;
            }
        }

        private void btn_delcateg_Click(object sender, EventArgs e)
        {
            // -------------------------------------------------------------------
            // Delete category from the database.

            if (dgv_categlist.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select a category to delete", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                if (dgv_categlist.SelectedRows[0].Cells[2].Value.ToString() == "Active")
                {
                    MessageBox.Show("Category is currently active and cannot be deleted", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this category?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {
                            cn.Open();
                            cmd.Connection = cn;

                            cmd.CommandText = "DELETE FROM tbl_category WHERE prod_categID = '" + dgv_categlist.SelectedRows[0].Cells[0].Value.ToString().Replace("C-", "") + "' ;";

                            int RowsAffected = cmd.ExecuteNonQuery();

                            if (RowsAffected == 0)
                            {

                            }
                            else if (RowsAffected > 0)
                            {
                                MessageBox.Show("Successfully Deleted", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                gbhold_categform.Focus();
                            }

                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            gbhold_categform.Focus();
                            return;
                        }

                        else
                        {
                            MessageBox.Show("No ID Found", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        cn.Close();
                    }

                    // -------------------------------------------------------------------
                    // Inserting deleted data to audit trail.

                    try
                    {
                        cn.Open();
                        cmd.Connection = cn;

                        string getdeletedata = "Deleted a category: " + "\"" + dgv_categlist.SelectedRows[0].Cells[1].Value.ToString() + "\"";
                        string action = "Delete";

                        cmd.CommandText = "INSERT INTO tbl_audittrail (User,Activity,Action,Date,Time) VALUES ('" + lb_name.Text + "','" + getdeletedata + "', '" + action + "','" + lbl_dateOrder.Text + "','" + datetimeHMS + "') ;";

                        MySqlDataReader mRead;
                        mRead = cmd.ExecuteReader();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.ToString());
                    }
                    finally
                    {
                        cn.Close();
                    }

                    fillcategcmbbox(); 
                    cb_categ.SelectedIndex = -1; cb_prodfilter.SelectedIndex = -1;

                    viewCategList();
                    viewAudittrail();
                    categIDIncre();

                }
            }
        }

        private void btn_exaddcateg_Click(object sender, EventArgs e)
        {
            gbhold_categform.Enabled = true;
            paform_addcateg.Visible = false;
        }

        private void panel12_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btn_adduser_Click(object sender, EventArgs e)
        {
            txtusermainTitle.Text = "Add User"; usermainFormAction = "Add User";

            txtb_userpassword.PasswordChar = '*';
            txtb_confpassword.PasswordChar = '*';
            lb_showpass.Text = "Show";

            txtb_userfname.Clear(); txtb_userlname.Clear(); txtb_useraddr.Clear(); txtb_userphone.Clear(); txtb_username.Clear(); txtb_userpassword.Clear();
            cb_usertype.SelectedIndex = -1;
            userIDIncre();

            cb_rolefilter.Visible = false;       
            gbhold_userform.Enabled = false;
            paform_adduser.Visible = true;

        }

        private void btn_useredit_Click(object sender, EventArgs e)
        {
            if (dgv_userlist.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select a user to edit", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (dgv_userlist.SelectedRows[0].Cells[7].Value.ToString() == "Owner" && lb_userlvl.Text.Replace(" Name :", "") != "Owner")
            {
                MessageBox.Show("Unauthorized to Access", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (dgv_userlist.SelectedRows.Count == 1)
                {
                    try
                    {
                        cn.Open();
                        cmd.Connection = cn;

                        cmd.CommandText = "SELECT Username, Password FROM tbl_login WHERE UserID = '" + dgv_userlist.SelectedRows[0].Cells[0].Value.ToString().Replace("U-", "") + "';";

                        MySqlDataAdapter da = new MySqlDataAdapter();
                        da.SelectCommand = cmd;
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        olduserid = dgv_userlist.SelectedRows[0].Cells[0].Value.ToString(); 
                        txtUserID.Text = dgv_userlist.SelectedRows[0].Cells[0].Value.ToString(); 
                        txtb_userfname.Text = dgv_userlist.SelectedRows[0].Cells[1].Value.ToString();
                        txtb_userlname.Text = dgv_userlist.SelectedRows[0].Cells[2].Value.ToString();
                        txtb_useraddr.Text = dgv_userlist.SelectedRows[0].Cells[3].Value.ToString();
                        txtb_userphone.Text = dgv_userlist.SelectedRows[0].Cells[4].Value.ToString();
                        cb_usertype.Text = dgv_userlist.SelectedRows[0].Cells[7].Value.ToString();

                        oldfirstname = dgv_userlist.SelectedRows[0].Cells[1].Value.ToString();
                        oldlastname = dgv_userlist.SelectedRows[0].Cells[2].Value.ToString();
                        oldaddress = dgv_userlist.SelectedRows[0].Cells[3].Value.ToString();
                        oldphonenumber = dgv_userlist.SelectedRows[0].Cells[4].Value.ToString();
                        oldusertype = dgv_userlist.SelectedRows[0].Cells[7].Value.ToString();

                        foreach (DataRow userpass in dt.Rows)
                        {
                            txtb_username.Text = userpass["Username"].ToString();
                            oldusername = userpass["Username"].ToString();

                            txtb_userpassword.Text = userpass["Password"].ToString();
                            txtb_confpassword.Text = userpass["Password"].ToString();
                            oldpassword = userpass["Password"].ToString();
                        }

                        da.Update(dt);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        cn.Close();
                    }
                }

                txtusermainTitle.Text = "Edit User";
                usermainFormAction = "Edit User";

                txtb_userpassword.PasswordChar = '*';
                txtb_confpassword.PasswordChar = '*';
                lb_showpass.Text = "Show";

                
                cb_rolefilter.Visible = false;
                gbhold_userform.Enabled = false;
                pa_accessuser.Visible = true;
            }
        }

        private void btn_userdel_Click(object sender, EventArgs e)
        {
            //-------Delete product from the database.------------------------------------
            if (dgv_userlist.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select a user to delete", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (dgv_userlist.SelectedRows[0].Cells[7].Value.ToString() == "Owner" && lb_userlvl.Text.Replace(" Name :", "") != "Owner")
            {
                MessageBox.Show("Unauthorized to Delete", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this product?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        cn.Open();
                        cmd.Connection = cn;

                        olduserid = dgv_userlist.SelectedRows[0].Cells[0].Value.ToString();

                        cmd.CommandText = "DELETE FROM tbl_login WHERE UserID = '" + dgv_userlist.SelectedRows[0].Cells[0].Value.ToString().Replace("U-", "") + "' ;";

                        int RowsAffected = cmd.ExecuteNonQuery();

                        if (RowsAffected == 0)
                        {

                        }
                        else if (RowsAffected > 0)
                        {
                            gbhold_userform.Focus();
                        }
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        gbhold_userform.Focus(); return;
                    }
                    else { MessageBox.Show("No ID Found", "", MessageBoxButtons.OK, MessageBoxIcon.Information); }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    cn.Close();
                }

                    // ----Inserting deleted data to audit trail.----------------------------------
                    try
                    {
                        cn.Open();
                        cmd.Connection = cn;

                        string getdeletedata = "Deleted a user (" + olduserid + "): [ " + "\"" + dgv_userlist.SelectedRows[0].Cells[1].Value.ToString() + " " + dgv_userlist.SelectedRows[0].Cells[2].Value.ToString() + "\" ]";
                        string action = "Delete";

                        cmd.CommandText = "INSERT INTO tbl_audittrail (User,Activity,Action,Date,Time) VALUES ('" + lb_name.Text + "','" + getdeletedata + "', '" + action + "','" + lbl_dateOrder.Text + "','" + datetimeHMS + "') ;";

                        MySqlDataReader mRead;
                        mRead = cmd.ExecuteReader();

                        MessageBox.Show("Successfully Deleted", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.ToString());
                    }
                    finally
                    {
                        cn.Close();
                    }

                    // ---Resets product search and filter.----------------------------
                    if (txtb_usermainSearch.Text != "Ex. Allarey, Alejandro, Angelo" && txtb_usermainSearch.ForeColor != Color.Gray)
                    {
                        txtb_usermainSearch.Text = "Ex. Allarey, Alejandro, Angelo";
                        txtb_usermainSearch.ForeColor = Color.Gray;
                    }
                    cb_rolefilter.SelectedIndex = -1;

                    viewUserList(); viewAudittrail();
                    userIDIncre();
            }
        }

        private void btn_extadduser_Click(object sender, EventArgs e)
        {
            
        }

        private void btn_extadduser_Click_1(object sender, EventArgs e)
        {
            paform_adduser.Visible = false;
        }

        private void btn_appexit_Click(object sender, EventArgs e)
        {
            DialogResult diagResult = MessageBox.Show("This will exit and logout your account", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (diagResult == DialogResult.Yes)
            {
                Environment.Exit(0); //Double check if it does really exit (Check your memory or task manager for multiple application opened)
            }
            else if (diagResult == DialogResult.No)
            {


            }

            return;
        }

        private void btn_appminimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btn_appminimize_MouseEnter(object sender, EventArgs e)
        {
            btn_appminimize.ForeColor = Color.DeepSkyBlue;
        }

        private void btn_appminimize_MouseLeave(object sender, EventArgs e)
        {
            btn_appminimize.ForeColor = Color.White;
        }

        private void btn_appexit_MouseEnter(object sender, EventArgs e)
        {
            btn_appexit.ForeColor = Color.Pink;
        }

        private void btn_appexit_MouseLeave(object sender, EventArgs e)
        {
            btn_appexit.ForeColor = Color.White;
        }

        private void btncb_showallprod_Click(object sender, EventArgs e)
        {

            cb_prodfilter.DroppedDown = true;
            cb_prodfilter.Focus();

        }

        private void pb_clickprodfilter_Click(object sender, EventArgs e)
        {
            cb_prodfilter.DroppedDown = true;
            cb_prodfilter.Focus();
        }

        private void cb_prodfilter_TextChanged(object sender, EventArgs e)
        {
            if (cb_prodfilter.Text == "")
            {
                btncb_showall.Text = "All Categories";
            }
            else
            {
                if (txtbsearch_prodmain.Text != "Ex. Apple, Banana, Ice Cream" && txtbsearch_prodmain.ForeColor != Color.Gray)
                {
                    txtbsearch_prodmain.Text = "Ex. Apple, Banana, Ice Cream";
                    txtbsearch_prodmain.ForeColor = Color.Gray;
                }
               
                btncb_showall.Text = cb_prodfilter.Text;
                fillprodfilter();
            }
        }


        private void prodfilter_holder_Click(object sender, EventArgs e)
        {
            cb_prodfilter.DroppedDown = true;
            
        }
        private void prodfilter_holder_Enter(object sender, EventArgs e)
        {
            cb_prodfilter.Focus();
        }

     

        private void cb_prodfilter_Click(object sender, EventArgs e)
        {
           
        }

        private void cb_prodfilter_DropDownClosed(object sender, EventArgs e)
        {

           
        }


        private void cb_prodfilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cb_prodfilter_TextUpdate(object sender, EventArgs e)
        {
           
        }

        private void gbhold_addmain_Paint(object sender, PaintEventArgs e)
        {
         
            
        }

        private void cb_prodfilter_SelectionChangeCommitted(object sender, EventArgs e)
        {

        }

        private void cb_prodfilter_DrawItem(object sender, DrawItemEventArgs e)
        {

        }

        private void paholder_txtcategsearch_Enter(object sender, EventArgs e)
        {
            txtbsearch_categmain.Focus();
        }

        private void paholder_txtcategsearch2_Enter(object sender, EventArgs e)
        {
            gbhold_categform.Focus();
        }

        private void txtbsearch_categmain_Enter(object sender, EventArgs e)
        {

            if (txtbsearch_categmain.Text == "Ex. Small, Medium, Large" && txtbsearch_categmain.ForeColor == Color.Gray)
            {
                txtbsearch_categmain.Text = "";
                txtbsearch_categmain.ForeColor = Color.Black;

            }

        }

        private void txtbsearch_categmain_Leave(object sender, EventArgs e)
        {
            string searchCateg = txtbsearch_categmain.Text;

            if (txtbsearch_categmain.Text == "Ex. Small, Medium, Large" && txtbsearch_categmain.ForeColor == Color.Gray)
            {
                txtbsearch_categmain.Text = "Ex. Small, Medium, Large";
                txtbsearch_categmain.ForeColor = Color.Gray;

            }
            else
            {
                if (searchCateg.Equals(""))
                {
                    txtbsearch_categmain.Text = "Ex. Small, Medium, Large";
                    txtbsearch_categmain.ForeColor = Color.Gray;
                }
                else
                {
                    txtbsearch_categmain.Text = searchCateg;
                    txtbsearch_categmain.ForeColor = Color.Black;
                }
            }
        }

        private void txtbsearch_categmain_TextChanged(object sender, EventArgs e)
        {
            if (txtbsearch_categmain.ForeColor != Color.Gray && txtbsearch_categmain.Text != "Ex. Small, Medium, Large")
            {

                if (txtbsearch_categmain.Text != "" && txtbsearch_categmain.Text[0] == ' ')
                {
                    txtbsearch_categmain.Text = txtbsearch_categmain.Text.TrimStart(' ');
                }

                if (txtbsearch_categmain.Text.Contains("  "))
                {
                    txtbsearch_categmain.Focus();
                    txtbsearch_categmain.Text = txtbsearch_categmain.Text.Replace("  ", " ");
                    txtbsearch_categmain.SelectionStart = cursorPos;

                    if (txtbsearch_categmain.Text[txtbsearch_categmain.SelectionStart - 1] != ' ' && txtbsearch_categmain.SelectedText == "")
                    {
                        txtbsearch_categmain.SelectionStart = txtbsearch_categmain.SelectionStart + 1;
                    }
                }
                try
                {
                    cn.Open();
                    cmd.Connection = cn;

                    cmd.CommandText = ("SELECT CONCAT('C-', prod_categID), prod_categname, status FROM tbl_category;");

                    MySqlDataAdapter da = new MySqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bSource = new BindingSource();
                    bSource.DataSource = dt;

                    dgv_categlist.DataSource = bSource;

                    da.Update(dt);

                    DataView dv = dt.DefaultView;
                    dv.RowFilter = string.Format("prod_categname like '%{0}%'", txtbsearch_categmain.Text);
                    dgv_categlist.DataSource = dv.ToTable();

                    dgv_categlist.ClearSelection();
                }
                catch
                {

                }
                finally
                {
                    cn.Close();
                }
            }

        }

        private void txtbsearch_categmain_MouseDown(object sender, MouseEventArgs e)
        {
            int curs = txtbsearch_categmain.SelectionStart;
            if (txtbsearch_categmain.SelectionStart < txtbsearch_categmain.Text.Length && txtbsearch_categmain.Text[txtbsearch_categmain.Text.Length - 1] == ' ' && txtbsearch_categmain.SelectedText == string.Empty)
            {
                txtbsearch_categmain.Focus();
                txtbsearch_categmain.Text = txtbsearch_categmain.Text.TrimEnd(' ');
                txtbsearch_categmain.SelectionStart = curs;
            }

        }

        private void txtbsearch_categmain_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tbsearchcateg = (TextBox)sender;
            cursorPos = tbsearchcateg.SelectionStart;

            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' ' || (char)Keys.Back == e.KeyChar)
            //Allows letters, space and backspace.
            {

                if ((txtbsearch_categmain.SelectionStart == 0) && (e.KeyChar == (char)Keys.Space))
                //Best way to remove leading spaces.
                {
                    e.Handled = true;
                }

                if ((char)Keys.Back == e.KeyChar)
                {
                    cursorPos = txtbsearch_categmain.SelectionStart - 1;

                }

                if ((e.KeyChar == ' ') && (tbsearchcateg.Text.Length > 0))
                {
                    if (tbsearchcateg.Text[tbsearchcateg.Text.Length - 1] == ' ' && tbsearchcateg.SelectionStart == tbsearchcateg.Text.Length)
                    {
                        e.Handled = true;
                    }
                }

                if (tbsearchcateg.SelectionStart < tbsearchcateg.Text.Length && tbsearchcateg.SelectionStart > 0)
                {
                    if (tbsearchcateg.Text[tbsearchcateg.SelectionStart - 1] == ' ' || tbsearchcateg.Text[tbsearchcateg.SelectionStart] == ' ')
                    {
                        if (txtbsearch_categmain.SelectedText == "" && e.KeyChar == ' ')
                        {
                            if (txtbsearch_categmain.Text[txtbsearch_categmain.Text.Length - 1] == ' ' && txtbsearch_categmain.SelectionStart < txtbsearch_categmain.Text.Length)
                            {
                                txtbsearch_categmain.Focus();
                                txtbsearch_categmain.Text = txtbsearch_prodmain.Text.TrimEnd(' ');
                                txtbsearch_categmain.SelectionStart = cursorPos;
                            }

                            e.Handled = true;
                        }
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtbsearch_categmain_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {

                int curs = txtbsearch_categmain.SelectionStart;
                if (txtbsearch_categmain.SelectionStart < txtbsearch_categmain.Text.Length && txtbsearch_categmain.Text[txtbsearch_categmain.Text.Length - 1] == ' ' && txtbsearch_categmain.SelectedText == string.Empty)
                {
                    txtbsearch_categmain.Focus();
                    txtbsearch_categmain.Text = txtbsearch_categmain.Text.TrimEnd(' ');
                    txtbsearch_categmain.SelectionStart = curs;
                }

            }
        }

        private void txtb_categname_TextChanged(object sender, EventArgs e)
        {
            txtb_categname.Text = txtb_categname.Text.TrimStart(); //Removes space at start.

            if (txtb_categname.Text == "")
            {
                lb_asteriskcateg.Visible = true;

            }
            else if (txtb_categname.Text != "")
            {
                lb_asteriskcateg.Visible = false;
            }

            // - This removes the multiple spaces and starts the cursor at the end of text
            if (txtb_categname.Text.Contains("  "))
            {
                txtb_categname.Text = txtb_categname.Text.Replace("  ", " ");
                txtb_categname.SelectionStart = txtb_categname.Text.Length;
            }
            // --------------------------------------------------------------------------
        }

        private void txtb_categname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' ') //Can use spaces in between each words.
            {

            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {

            pa_dashboard.Visible = true;
            pa_sales.Visible = false;
            pa_orderhistory.Visible = false;
            pa_audittrail.Visible = false;
            pa_viewcateg.Visible = false;
            pa_viewuser.Visible = false;
            pa_viewprod.Visible = false;
            pa_categ.Visible = false;
            pa_order.Visible = false;
            pa_users.Visible = false;
            pa_prodmain.Visible = false;
            pa_cstmDscnt.Visible = false;
            pa_backrestore.Visible = false;


            pa_dashboard.Focus();
        }

        private void panel28_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.panel28.ClientRectangle, Color.MediumVioletRed, ButtonBorderStyle.Solid);
        }

        private void panel29_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.panel29.ClientRectangle, Color.MediumVioletRed, ButtonBorderStyle.Solid);
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.panel5.ClientRectangle, Color.MediumVioletRed, ButtonBorderStyle.Solid);
        }

        private void panel30_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.panel30.ClientRectangle, Color.MediumVioletRed, ButtonBorderStyle.Solid);
        }

        private void btn_extadduser_Click_2(object sender, EventArgs e)
        {
            cb_rolefilter.Visible = true;
            gbhold_userform.Enabled = true;
            paform_adduser.Visible = false;
        }

        string usermainFormAction;

        private void btn_saveuser_Click(object sender, EventArgs e)
        {
            //- This will remove space at start and end.
            txtb_userfname.Text = txtb_userfname.Text.Trim();
            txtb_userlname.Text = txtb_userlname.Text.Trim();

            if (txtb_userfname.Text == "")
            {
                MessageBox.Show("Please enter your first name", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtb_userfname.Focus();
                return;
            }

            if (txtb_userlname.Text == "")
            {
                MessageBox.Show("Please enter your last name", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtb_userlname.Focus();
                return;
            }

            if (txtb_useraddr.Text == "")
            {
                MessageBox.Show("Please enter your address", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtb_useraddr.Focus();
                return;
            }

            if (txtb_userphone.Text == "")
            {
                MessageBox.Show("Please enter your phone number", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtb_userphone.Focus();
                return;
            }

            else if (txtb_userphone.Text.Length < 11)
            {
                MessageBox.Show("Phone number must be 11 digits", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtb_userphone.Focus();
                return;
            }

            if (txtb_username.Text == "")
            {

                MessageBox.Show("Please enter your username", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtb_username.Focus();
                return;

            }
            if (txtb_userpassword.Text == "")
            {
                MessageBox.Show("Please enter your password", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtb_userpassword.Focus();
                return;

            }
            if (txtb_userpassword.Text != txtb_confpassword.Text)
            {
                MessageBox.Show("Please confirm your password", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtb_confpassword.Focus(); txtb_confpassword.Clear();
                return;
            }
            if (cb_usertype.Text == "")
            {
                MessageBox.Show("Please select your user type", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cb_usertype.Focus();
                return;
            }

            if (usermainFormAction == "Add User")
            {    
                // ----- Inserting user to database. ---------------------------------------
                try
                {
                    cn.Open();
                    cmd.Connection = cn;

                    //- Checks if the username is already exists. -----------
                    cmd.CommandText = "SELECT COUNT(Username) FROM tbl_login WHERE Username = '" + txtb_username.Text + "';";

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count >= 1)
                    {
                        MessageBox.Show("Username is already taken! Please try another one", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtb_username.Clear();
                        txtb_username.Focus();
                        return;
                    }
                    //----------------------------------------------

                    cmd.CommandText = "INSERT INTO tbl_login (userID, FirstName, LastName, Address, PhoneNumber, username, password, UserType) VALUES ('" + txtUserID.Text.Replace("U-", "") + "','" + txtb_userfname.Text + "', '" + txtb_userlname.Text + "', '" + txtb_useraddr.Text + "', '" + txtb_userphone.Text + "', '" + txtb_username.Text + "','" + txtb_userpassword.Text + "', '" + cb_usertype.Text + "');";

                    // It needs these if you are inserting a data to the database.
                    MySqlDataReader myReader;
                    myReader = cmd.ExecuteReader();
                    // -------------------------------

                    txtb_userpassword.PasswordChar = '*';
                    txtb_confpassword.PasswordChar = '*';
                    lb_showpass.Text = "Show";   

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    cn.Close();
                }

                // ---- Inserting data to Audit Trail. -----------------------------
                try
                {
                    cn.Open();
                    cmd.Connection = cn;

                    string getadddata = "Inserted a new user (" + txtUserID.Text + "): [ \"" + txtb_userfname.Text + " " + txtb_userlname.Text + "\" ]";
                    string action = "Insert";

                    cmd.CommandText = "INSERT INTO tbl_audittrail (User,Activity,Action,Date,Time) VALUES ('" + lb_name.Text + "','" + getadddata + "', '" + action + "','" + lbl_dateOrder.Text + "','" + datetimeHMS + "') ;";

                    MySqlDataReader mRead;
                    mRead = cmd.ExecuteReader();

                    MessageBox.Show("User saved", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    cn.Close();
                }
            }

            if (usermainFormAction == "Edit User")
            {
                string newfirstname, newlastname, newaddress, newphonenum, newusername, newpassword, newusertype;

                try
                {
                    cn.Open();
                    cmd.Connection = cn;

                    if (txtb_username.Text != oldusername)
                    {
                        //- Checks if the username is already exists. -----------
                        cmd.CommandText = "SELECT COUNT(Username) FROM tbl_login WHERE Username = '" + txtb_username.Text + "';";

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count >= 1)
                        {
                            MessageBox.Show("Username is already taken! Please try another one", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtb_username.Clear();
                            txtb_username.Focus();
                            return;
                        }
                        //----------------------------------------------
                    }

                    cmd.CommandText = "UPDATE tbl_login SET FirstName ='" + txtb_userfname.Text + "', LastName = '" + txtb_userlname.Text + "', Address = '" + txtb_useraddr.Text + "', PhoneNumber = '" + txtb_userphone.Text + "', Username = '" + txtb_username.Text + "', Password = '" + txtb_userpassword.Text + "', UserType = '" + cb_usertype.Text + "' WHERE UserID = '" + txtUserID.Text.Replace("U-", "") + "';";

                    int RowsAffected = 0;

                    RowsAffected = cmd.ExecuteNonQuery();

                    if (RowsAffected > 0)
                    {
                        txtb_userpassword.PasswordChar = '*';
                        txtb_confpassword.PasswordChar = '*';
                        lb_showpass.Text = "Show";
                    }
                    else
                    {
                        MessageBox.Show("No ID Found", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    cn.Close();
                }

                // -----Insert updated data to audit trail.-----------------------------------------
                newfirstname = txtb_userfname.Text; newlastname = txtb_userlname.Text;
                newaddress = txtb_useraddr.Text; newphonenum = txtb_userphone.Text;
                newusername = txtb_username.Text; newpassword = txtb_userpassword.Text;
                newusertype = cb_usertype.Text;

                string passwordchanged = "", usernamechanged = "";

                if (oldfirstname == newfirstname)
                {
                    oldfirstname = ""; newfirstname = "";
                }
                else if (oldfirstname != newfirstname)
                {
                    oldfirstname = "First Name: \"" + oldfirstname + "\"" + " to ";
                    newfirstname = "\"" + newfirstname + "\", ";
                }

                if (oldlastname == newlastname)
                {
                    oldlastname = ""; newlastname = ""; 
                }
                else if (oldlastname != newlastname)
                {
                    oldlastname = "Last Name: \"" + oldlastname + "\"" + " to ";
                    newlastname = "\"" + newlastname + "\", ";
                }

                if (oldaddress == newaddress)
                {
                    oldaddress = ""; newaddress = "";      
                }
                else if (oldaddress != newaddress)
                {
                    oldaddress = "Address: \"" + oldaddress + "\"" + " to ";
                    newaddress = "\"" + newaddress + "\", ";
                }

                if (oldphonenumber == newphonenum)
                {
                    oldphonenumber = ""; newphonenum = "";
                }
                else if (oldphonenumber != newphonenum)
                {
                    oldphonenumber = "Phone No.: \"" + oldphonenumber + "\"" + " to ";
                    newphonenum = "\"" + newphonenum + "\", ";
                }

                if (oldusername == newusername)
                {
                    oldusername = ""; newusername = "";  
                }
                else if (oldusername != newusername)
                {
                    usernamechanged = "Username Changed, ";
                }

                if (oldpassword == newpassword)
                {
                    oldpassword = ""; newpassword = "";
                }
                else if (oldpassword != newpassword)
                {
                    passwordchanged = "Password Changed, ";
                }

                if (oldusertype == newusertype)
                {
                    oldusertype = "";
                    newusertype = "";
                }
                else if (oldusertype != newusertype)
                {
                    oldusertype = "User Type: \"" + oldusertype + "\"" + " to ";
                    newusertype = "\"" + newusertype + "\"";
                }

                if (newfirstname != "" || newlastname != "" || newaddress != "" || newphonenum != "" || newusername != "" || newpassword != "" || newusertype != "")
                {
                    try
                    {
                        cn.Open();
                        cmd.Connection = cn;

                        string getadddata = "Edited a user account (" + olduserid + "): [ " + oldfirstname + newfirstname + oldlastname + newlastname + oldaddress + newaddress + oldphonenumber + newphonenum + usernamechanged + passwordchanged + oldusertype + newusertype + " ]";
                        string action = "Edit";

                        cmd.CommandText = "INSERT INTO tbl_audittrail (User,Activity,Action,Date,Time) VALUES ('" + lb_name.Text + "','" + getadddata + "', '" + action + "','" + lbl_dateOrder.Text + "','" + datetimeHMS + "') ;";

                        MySqlDataReader mRead;
                        mRead = cmd.ExecuteReader();

                        MessageBox.Show("Successfully Edited", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        cn.Close();
                    }
                }
            }

            // ---Resets & Updates the form.----------------------------
            cb_rolefilter.Visible = true; gbhold_userform.Enabled = true; paform_adduser.Visible = false;

            txtb_userfname.Clear(); txtb_userlname.Clear(); txtb_useraddr.Clear(); txtb_userphone.Clear(); txtb_username.Clear(); txtb_userpassword.Clear(); txtb_confpassword.Clear();
            cb_usertype.SelectedIndex = -1;

            viewUserList(); 
            userIDIncre();
            viewAudittrail();

            // ---Resets product search and filter.----------------------------
            if (txtb_usermainSearch.Text != "Ex. Allarey, Alejandro, Angelo" && txtb_usermainSearch.ForeColor != Color.Gray)
            {
                txtb_usermainSearch.Text = "Ex. Allarey, Alejandro, Angelo";
                txtb_usermainSearch.ForeColor = Color.Gray;
            }
            cb_rolefilter.SelectedIndex = -1;

        }

        private void btn_clearuser_Click(object sender, EventArgs e)
        {
            if (txtb_userfname.Text != "" || txtb_userlname.Text != "" || txtb_useraddr.Text != "" || txtb_userphone.Text != "" || txtb_username.Text != "" || txtb_userpassword.Text != "" || cb_usertype.Text != "")
            {
                DialogResult diagResult = MessageBox.Show("Are you sure you want to clear all the data in the field?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (diagResult == DialogResult.Yes)
                {
                    txtb_userpassword.PasswordChar = '*';
                    txtb_confpassword.PasswordChar = '*';
                    lb_showpass.Text = "Show";

                    txtb_userfname.Clear(); txtb_userlname.Clear(); txtb_useraddr.Clear(); txtb_userphone.Clear(); txtb_username.Clear(); txtb_userpassword.Clear(); txtb_confpassword.Clear();
                    cb_usertype.SelectedIndex = -1;
                }
            }
        }

        private void txtb_userfname_TextChanged(object sender, EventArgs e)
        {
            txtb_userfname.Text = txtb_userfname.Text.TrimStart(); // - Remove space at start.

            if (txtb_userfname.Text == "")
            {
                lb_userInd1.Visible = true;
            }
            else if (txtb_userfname.Text != "")
            {
                lb_userInd1.Visible = false;
            }
            // - This removes the multiple spaces and starts the cursor at the end of text
            if (txtb_userfname.Text.Contains("  "))
            {
                txtb_userfname.Text = txtb_userfname.Text.Replace("  ", " ");
                txtb_userfname.SelectionStart = txtb_userfname.Text.Length;
            }
            //
        }

        private void txtb_userfname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' ')
            {

            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void txtb_userlname_TextChanged(object sender, EventArgs e)
        {
            txtb_userlname.Text = txtb_userlname.Text.TrimStart(); // - Remove space at start.

            if (txtb_userlname.Text == "")
            {
                lb_userInd2.Visible = true;
            }
            else if (txtb_userlname.Text != "")
            {
                lb_userInd2.Visible = false;
            }
            // - This removes the multiple spaces and starts the cursor at the end of text
            if (txtb_userlname.Text.Contains("  "))
            {
                txtb_userlname.Text = txtb_userlname.Text.Replace("  ", " ");
                txtb_userlname.SelectionStart = txtb_userlname.Text.Length;
            }
            //
        }

        private void txtb_userlname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' ')
            {

            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void txtb_useraddr_TextChanged(object sender, EventArgs e)
        {
            txtb_useraddr.Text = txtb_useraddr.Text.TrimStart(); // - Remove space at start.

            if (txtb_useraddr.Text == "")
            {
                lb_userInd3.Visible = true;
            }
            else if (txtb_useraddr.Text != "")
            {
                lb_userInd3.Visible = false;
            }
            // - This removes the multiple spaces and starts the cursor at the end of text
            if (txtb_useraddr.Text.Contains("  "))
            {
                txtb_useraddr.Text = txtb_useraddr.Text.Replace("  ", " ");
                txtb_useraddr.SelectionStart = txtb_useraddr.Text.Length;
            }
            //
        }

        private void txtb_useraddr_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && e.KeyChar != '\'' && e.KeyChar != '\"' && e.KeyChar != '=' && e.KeyChar != ';' && e.KeyChar != '|' && e.KeyChar != '{' && e.KeyChar != '}' && e.KeyChar != '`')
            {

            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void txtb_userphone_TextChanged(object sender, EventArgs e)
        {
            if (txtb_userphone.Text.Length < 11)
            {
                lb_userInd5.Visible = true;
            }
            else if (txtb_userphone.Text.Length == 11)
            {
                lb_userInd5.Visible = false;
            }
        }

        private void txtb_userphone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar))
            {

            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void txtb_username_TextChanged(object sender, EventArgs e)
        {
            if (txtb_username.Text == "")
            {
                lb_userInd4.Visible = true;
            }
            else if (txtb_username.Text != "")
            {
                lb_userInd4.Visible = false;
            }
        }

        private void txtb_username_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar))
            {

            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void txtb_userpassword_TextChanged(object sender, EventArgs e)
        {
            if (txtb_userpassword.Text == "")
            {
                lb_showpass.Visible = false;
                lb_userInd7.Visible = true; txtb_confpassword.Clear();
                txtb_confpassword.Enabled = false;
            }
            else if (txtb_userpassword.Text != "")
            {
                lb_showpass.Visible = true;
                lb_userInd7.Visible = false;
                txtb_confpassword.Enabled = true;
            }

            if (txtb_userpassword.Text != txtb_confpassword.Text)
            {
                lb_userInd6.Visible = true;
            }
            else if (txtb_userpassword.Text == txtb_confpassword.Text)
            {
                lb_userInd6.Visible = false;
            }
        }

        private void txtb_confpassword_TextChanged(object sender, EventArgs e)
        {
            if (txtb_userpassword.Text == txtb_confpassword.Text)
            {
                lb_userInd6.Visible = false;

            }
            else
            {
                lb_userInd6.Visible = true;
            }
        }

        private void txtb_userpassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == ' ')
            {

            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void txtb_confpassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == ' ')
            {

            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void lb_showpass_Click(object sender, EventArgs e)
        {
            if (lb_showpass.Text == "Show")
            {
                txtb_userpassword.PasswordChar = '\0';
                txtb_confpassword.PasswordChar = '\0';
                lb_showpass.Text = "Hide";
            }
            else if (lb_showpass.Text == "Hide")
            {
                txtb_userpassword.PasswordChar = '*';
                txtb_confpassword.PasswordChar = '*';
                lb_showpass.Text = "Show";
            }
        }

        private void cb_usertype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_usertype.SelectedIndex == - 1)
            {
                lb_userInd8.Visible = true;
            }
            else if (cb_usertype.Text != "")
            {
                lb_userInd8.Visible = false;
            }
        }

        private void txtb_usermainSearch_Enter(object sender, EventArgs e)
        {
            if (txtb_usermainSearch.Text == "Ex. Allarey, Alejandro, Angelo" && txtb_usermainSearch.ForeColor == Color.Gray)
            {
                txtb_usermainSearch.Text = "";
                txtb_usermainSearch.ForeColor = Color.Black;
            }
        }

        private void txtb_usermainSearch_Leave(object sender, EventArgs e)
        {
            string searchUser = txtb_usermainSearch.Text;

            if (txtb_usermainSearch.Text == "Ex. Allarey, Alejandro, Angelo" && txtb_usermainSearch.ForeColor == Color.Gray)
            {
                txtb_usermainSearch.Text = "Ex. Allarey, Alejandro, Angelo";
                txtb_usermainSearch.ForeColor = Color.Gray;
            }
            else
            {
                if (searchUser.Equals(""))
                {
                    txtb_usermainSearch.Text = "Ex. Allarey, Alejandro, Angelo";
                    txtb_usermainSearch.ForeColor = Color.Gray;
                }
                else
                {
                    txtb_usermainSearch.Text = searchUser;
                    txtb_usermainSearch.ForeColor = Color.Black;
                }

            }
        }

        private void txtb_usermainSearch_MouseDown(object sender, MouseEventArgs e)
        {
            int curs = txtb_usermainSearch.SelectionStart;
            if (txtb_usermainSearch.SelectionStart < txtb_usermainSearch.Text.Length && txtb_usermainSearch.Text[txtb_usermainSearch.Text.Length - 1] == ' ' && txtb_usermainSearch.SelectedText == string.Empty)
            {
                txtb_usermainSearch.Focus();
                txtb_usermainSearch.Text = txtb_usermainSearch.Text.TrimEnd(' ');

                txtb_usermainSearch.SelectionStart = curs;
            }
        }

        private void txtb_usermainSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tbsearchuser = (TextBox)sender;
            cursorPos = tbsearchuser.SelectionStart;

            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' ' || (char)Keys.Back == e.KeyChar)
            //Allows letters, space and backspace.
            {

                if ((txtb_usermainSearch.SelectionStart == 0) && (e.KeyChar == (char)Keys.Space))
                //Best way to remove leading spaces.
                {
                    e.Handled = true;
                }

                if ((char)Keys.Back == e.KeyChar)
                {
                    cursorPos = txtb_usermainSearch.SelectionStart - 1;

                }

                if ((e.KeyChar == ' ') && (tbsearchuser.Text.Length > 0))
                {
                    if (tbsearchuser.Text[tbsearchuser.Text.Length - 1] == ' ' && tbsearchuser.SelectionStart == tbsearchuser.Text.Length)
                    {
                        e.Handled = true;
                    }
                }

                if (tbsearchuser.SelectionStart < tbsearchuser.Text.Length && tbsearchuser.SelectionStart > 0)
                {
                    if (tbsearchuser.Text[tbsearchuser.SelectionStart - 1] == ' ' || tbsearchuser.Text[tbsearchuser.SelectionStart] == ' ')
                    {
                        if (txtb_usermainSearch.SelectedText == "" && e.KeyChar == ' ')
                        {
                            if (txtb_usermainSearch.Text[txtb_usermainSearch.Text.Length - 1] == ' ' && txtb_usermainSearch.SelectionStart < txtb_usermainSearch.Text.Length)
                            {
                                txtb_usermainSearch.Focus();
                                txtb_usermainSearch.Text = txtb_usermainSearch.Text.TrimEnd(' ');
                                txtb_usermainSearch.SelectionStart = cursorPos;
                            }

                            e.Handled = true;
                        }
                    }
                }
            }

            else
            {
                e.Handled = true;
            }
        }

        private void txtb_usermainSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                int curs = txtb_usermainSearch.SelectionStart;
                if (txtb_usermainSearch.SelectionStart < txtb_usermainSearch.Text.Length && txtb_usermainSearch.Text[txtb_usermainSearch.Text.Length - 1] == ' ' && txtb_usermainSearch.SelectedText == string.Empty)
                {
                    txtb_usermainSearch.Focus();
                    txtb_usermainSearch.Text = txtb_usermainSearch.Text.TrimEnd(' ');

                    txtb_usermainSearch.SelectionStart = curs;
                }

            }
        }

        private void txtb_usermainSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtb_usermainSearch.ForeColor != Color.Gray && txtb_usermainSearch.Text != "Ex. Allarey, Alejandro, Angelo")
            {
                cb_rolefilter.SelectedIndex = -1;

                if (txtb_usermainSearch.Text != "" && txtb_usermainSearch.Text[0] == ' ')
                {
                    txtb_usermainSearch.Text = txtb_usermainSearch.Text.TrimStart(' ');
                }

                if (txtb_usermainSearch.Text.Contains("  "))
                {
                    txtb_usermainSearch.Focus();
                    txtb_usermainSearch.Text = txtb_usermainSearch.Text.Replace("  ", " ");

                    txtb_usermainSearch.SelectionStart = cursorPos;

                    if (txtb_usermainSearch.Text[txtb_usermainSearch.SelectionStart - 1] != ' ' && txtb_usermainSearch.SelectedText == "")
                    {
                        txtb_usermainSearch.SelectionStart = txtb_usermainSearch.SelectionStart + 1;
                    }
                }

                try
                {
                    cn.Open();
                    cmd.Connection = cn;

                    cmd.CommandText = ("SELECT CONCAT('U-', UserID), FirstName, LastName, Address, PhoneNumber, SHA1(Username), SHA1(Password), UserType FROM tbl_login;");

                    MySqlDataAdapter da = new MySqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bSource = new BindingSource();
                    bSource.DataSource = dt;

                    dgv_userlist.DataSource = bSource;

                    da.Update(dt);

                    DataView dv = dt.DefaultView;
                    dv.RowFilter = string.Format("LastName like '%{0}%'", txtb_usermainSearch.Text);
                    dgv_userlist.DataSource = dv.ToTable();

                    dgv_userlist.Columns[0].Width = 130;

                    dgv_userlist.ClearSelection();
                }
                catch
                {

                }
                finally
                {
                    cn.Close();
                }

            }
        }

        private void userfilter_holder_Click(object sender, EventArgs e)
        {
            cb_rolefilter.DroppedDown = true;
        }

        private void userfilter_holder_Enter(object sender, EventArgs e)
        {
            cb_rolefilter.Focus();
        }

        private void pb_clickuserfilter_Click(object sender, EventArgs e)
        {
            cb_rolefilter.DroppedDown = true;
            cb_rolefilter.Focus();
        }

        private void btncb_shwalluser_Click(object sender, EventArgs e)
        {
            cb_rolefilter.DroppedDown = true;
            cb_rolefilter.Focus();
        }

        private void cb_rolefilter_TextChanged(object sender, EventArgs e)
        {
            if (cb_rolefilter.Text == "")
            {
                btncb_shwalluser.Text = "All User Types";
            }
            else
            {
                if (txtb_usermainSearch.Text != "Ex. Allarey, Alejandro, Angelo" && txtb_usermainSearch.ForeColor != Color.Gray)
                {
                    txtb_usermainSearch.Text = "Ex. Allarey, Alejandro, Angelo";
                    txtb_usermainSearch.ForeColor = Color.Gray;
                }

                btncb_shwalluser.Text = cb_rolefilter.Text;
                filluserfilter();
            }
        }

        private void paholder_searchus2_Enter(object sender, EventArgs e)
        {
            txtb_usermainSearch.Focus();
        }

        private void paholder_searchus1_Enter(object sender, EventArgs e)
        {
            gbhold_userform.Focus();
        }

        private void btn_accessusExt_Click(object sender, EventArgs e)
        {
            pa_accessuser.Visible = false;
            cb_rolefilter.Visible = true;
            gbhold_userform.Enabled = true;

        }

        private void btn_accessconf_Click(object sender, EventArgs e)
        {
            if (txtb_accessUser.Text == "")
            {
                MessageBox.Show("Please enter the account's username", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtb_accessUser.Focus();
                return;
            }
            if (txtb_accessPass.Text == "")
            {
                MessageBox.Show("Please enter the account's password", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtb_accessPass.Focus();
                return;
            }

            try
            {
                string authorizeacc = "";
                
                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = "SELECT COUNT(*), FirstName, LastName, UserType, UserID FROM tbl_login WHERE (Username = '" + txtb_accessUser.Text + "' AND Password = '" + txtb_accessPass.Text + "') AND (UserID = '" + dgv_userlist.SelectedRows[0].Cells[0].Value.ToString().Replace("U-", "") + "' OR UserID = '" + lb_userid.Text.Replace("UID-", "") + "' OR Usertype = 'Owner');";
                
                int count = int.Parse(cmd.ExecuteScalar().ToString());

                MySqlDataReader reader;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (count == 1)
                    {
                        if (reader.GetString(3) == "Owner" && !lb_userlvl.Text.Contains("Owner"))
                        {
                            authorizeacc = " - Authorized by: " + reader.GetString(1) + " " + reader.GetString(2);
                        }

                        if (reader.GetString(3) == "Admin" && dgv_userlist.SelectedRows[0].Cells[7].Value.ToString() == "Cashier" || reader.GetString(3) == "Owner")
                        {
                            pa_accessuser.Visible = false;
                            paform_adduser.Visible = true;

                            txtb_accessUser.Clear();
                            txtb_accessPass.Clear();
                        }
                        else if (reader.GetString(3) == "Admin" && lb_userid.Text.Replace("UID-", "") == dgv_userlist.SelectedRows[0].Cells[0].Value.ToString().Replace("U-", "") || reader.GetString(3) == "Owner")
                        {
                            pa_accessuser.Visible = false;
                            paform_adduser.Visible = true;

                            txtb_accessUser.Clear();
                            txtb_accessPass.Clear();

                        }
                        else { count = 0; }

                    }
                   
                    if (count == 0 )
                    {
                        txtb_accessUser.Focus();
                        MessageBox.Show("Incorrect Username or Password", "Access Denied - Higher Clearance Required", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        txtb_accessUser.Clear();
                        txtb_accessPass.Clear();

                        return;
                    }

                }

                if (count == 1)
                {
                    reader.Close();

                    MySqlCommand cmd1 = new MySqlCommand();
                    cmd1.Connection = cn;

                    string date = DateTime.Now.ToString("yyyy-MM-dd");
                    string time = DateTime.Now.ToString("HH:mm:ss");

                    string access = lb_userlvl.Text + " " + lb_name.Text + " (" + lb_userid.Text.Replace("UID-", "U-") + ")" + " had access a user profile (" + dgv_userlist.SelectedRows[0].Cells[0].Value.ToString() + " | " + dgv_userlist.SelectedRows[0].Cells[1].Value.ToString() + " " + dgv_userlist.SelectedRows[0].Cells[2].Value.ToString() + " | " + dgv_userlist.SelectedRows[0].Cells[7].Value.ToString() + ")" + authorizeacc;
                    string action = "Access";

                    cmd1.CommandText = "INSERT INTO tbl_audittrail (User,Activity,Action,Date,Time) VALUES ('" + lb_name.Text + "','" + access + "', '" + action + "','" + date + "','" + time + "') ;";
                    cmd1.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cn.Dispose();
                cn.Close();
            }

            viewAudittrail();
        }

        private void txtb_accessUser_TextChanged(object sender, EventArgs e)
        {
            if (txtb_accessUser.Text == "")
            {
                lb_acAsterisk1.Visible = true;
            }
            else if (txtb_accessUser.Text != "")
            {
                lb_acAsterisk1.Visible = false;
            }
        }

        private void txtb_accessPass_TextChanged(object sender, EventArgs e)
        {
            if (txtb_accessPass.Text == "")
            {
                lb_acAsterisk2.Visible = true;
            }
            else if (txtb_accessPass.Text != "")
            {
                lb_acAsterisk2.Visible = false;
            }
        }

        private void txtb_accessUser_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar)) { }
            else { e.Handled = e.KeyChar != (char)Keys.Back; }
         
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btn_accessconf.PerformClick();
            }
        }

        private void txtb_accessPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == ' ') { }
            else { e.Handled = e.KeyChar != (char)Keys.Back; }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btn_accessconf.PerformClick();
            }
        }

        private void cb_viewuserfilt_TextChanged(object sender, EventArgs e)
        {
            if (cb_rolefilter2.Text == "")
            {
                btncb_shwalluser2.Text = "All User Types";
            }
            else
            {
                if (txtb_searchUser.Text != "Ex. Allarey, Alejandro, Angelo" && txtb_searchUser.ForeColor != Color.Gray)
                {
                    txtb_searchUser.Text = "Ex. Allarey, Alejandro, Angelo";
                    txtb_searchUser.ForeColor = Color.Gray;
                }

                btncb_shwalluser2.Text = cb_rolefilter2.Text;
                filluserfilter();
            }
        }

        private void btncb_shwalluser2_Click(object sender, EventArgs e)
        {
            cb_rolefilter2.DroppedDown = true;
            cb_rolefilter2.Focus();
        }

        private void pb_clickuserfilter2_Click(object sender, EventArgs e)
        {
            cb_rolefilter2.DroppedDown = true;
            cb_rolefilter2.Focus();
        }

        private void userfilter_holder2_Click(object sender, EventArgs e)
        {
            cb_rolefilter2.DroppedDown = true;
        }

        private void userfilter_holder2_Enter(object sender, EventArgs e)
        {
            cb_rolefilter2.Focus();
        }

        private void cb_prodfilter2_TextChanged(object sender, EventArgs e)
        {
            if (cb_prodfilter2.Text == "")
            {
                btncb_showall2.Text = "All Categories";
            }
            else
            {
                if (txtb_searchprod.Text != "Ex. Apple, Banana, Ice Cream" && txtb_searchprod.ForeColor != Color.Gray)
                {
                    txtb_searchprod.Text = "Ex. Apple, Banana, Ice Cream";
                    txtb_searchprod.ForeColor = Color.Gray;
                }

                btncb_showall2.Text = cb_prodfilter2.Text;
                fillprodfilter();
            }
        }

        private void btncb_showall2_Click(object sender, EventArgs e)
        {
            cb_prodfilter2.DroppedDown = true;
            cb_prodfilter2.Focus();
        }

        private void pb_clickprodfilter2_Click(object sender, EventArgs e)
        {
            cb_prodfilter2.DroppedDown = true;
            cb_prodfilter2.Focus();
        }

        private void prodfilter_holder2_Click(object sender, EventArgs e)
        {
            cb_prodfilter2.DroppedDown = true;
        }

        private void prodfilter_holder2_Enter(object sender, EventArgs e)
        {
            cb_prodfilter2.Focus();
        }

        private void holder_dtpauditStart_Click(object sender, EventArgs e)
        {      
            dtp_auditStart.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dtp_auditStart_ValueChanged(object sender, EventArgs e)
        {
            btnAuditClicked = "DateRange";
            holder_dtpauditStart.Text = dtp_auditStart.Text.TrimStart();
            auditdaterange();

            btn_todayAud.LinkColor = Color.DarkRed; btn_weekAud.LinkColor = Color.DarkRed; btn_monthAud.LinkColor = Color.DarkRed; btn_yearAud.LinkColor = Color.DarkRed;
        }

        private void holder_dtpauditEnd_Click(object sender, EventArgs e)
        {
            dtp_auditEnd.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dtp_auditEnd_ValueChanged(object sender, EventArgs e)
        {
            btnAuditClicked = "DateRange";
            holder_dtpauditEnd.Text = dtp_auditEnd.Text.TrimStart();
            auditdaterange();

            btn_todayAud.LinkColor = Color.DarkRed; btn_weekAud.LinkColor = Color.DarkRed; btn_monthAud.LinkColor = Color.DarkRed; btn_yearAud.LinkColor = Color.DarkRed;
        }

        private void btn_showallAud_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.dtp_auditStart.Value = dateTime;
            this.dtp_auditEnd.Value = dateTime;
            viewAudittrail();

            btn_todayAud.LinkColor = Color.DarkRed; btn_weekAud.LinkColor = Color.DarkRed; btn_monthAud.LinkColor = Color.DarkRed; btn_yearAud.LinkColor = Color.DarkRed;
        }

        private void btn_todayAud_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.dtp_auditStart.Value = dateTime;
            this.dtp_auditEnd.Value = dateTime;
            btnAuditClicked = "Today"; auditdaterange();

            btn_todayAud.LinkColor = Color.Chocolate; btn_weekAud.LinkColor = Color.DarkRed; btn_monthAud.LinkColor = Color.DarkRed; btn_yearAud.LinkColor = Color.DarkRed;
        }

        private void btn_weekAud_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.dtp_auditStart.Value = dateTime;
            this.dtp_auditEnd.Value = dateTime;
            btnAuditClicked = "Week"; auditdaterange();

            btn_todayAud.LinkColor = Color.DarkRed; btn_weekAud.LinkColor = Color.Chocolate; btn_monthAud.LinkColor = Color.DarkRed; btn_yearAud.LinkColor = Color.DarkRed;
        }

        private void btn_monthAud_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.dtp_auditStart.Value = dateTime;
            this.dtp_auditEnd.Value = dateTime;
            btnAuditClicked = "Month"; auditdaterange();

            btn_todayAud.LinkColor = Color.DarkRed; btn_weekAud.LinkColor = Color.DarkRed; btn_monthAud.LinkColor = Color.Chocolate; btn_yearAud.LinkColor = Color.DarkRed;
        }

        private void btn_yearAud_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.dtp_auditStart.Value = dateTime;
            this.dtp_auditEnd.Value = dateTime;
            btnAuditClicked = "Year"; auditdaterange();

            btn_todayAud.LinkColor = Color.DarkRed; btn_weekAud.LinkColor = Color.DarkRed; btn_monthAud.LinkColor = Color.DarkRed; btn_yearAud.LinkColor = Color.Chocolate;
        }

        private void btn_showallSales_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.dtp_salesStart.Value = dateTime;
            this.dtp_salesEnd.Value = dateTime;
            viewSales();
            txtgrossSales.Text = "Total Gross Sales"; txtnetSales.Text = "Total Net Sales";

            btn_todaySales.LinkColor = Color.DarkRed; btn_weekSales.LinkColor = Color.DarkRed; btn_monthSales.LinkColor = Color.DarkRed; btn_yearSales.LinkColor = Color.DarkRed;
        }

        private void btn_todaySales_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.dtp_salesStart.Value = dateTime;
            this.dtp_salesEnd.Value = dateTime;
            btnSalesClicked = "Today"; salesdaterange();
            txtgrossSales.Text = "Today's Gross Sales"; txtnetSales.Text = "Today's Net Sales";

            btn_todaySales.LinkColor = Color.Chocolate; btn_weekSales.LinkColor = Color.DarkRed; btn_monthSales.LinkColor = Color.DarkRed; btn_yearSales.LinkColor = Color.DarkRed;
        }

        private void btn_weekSales_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.dtp_salesStart.Value = dateTime;
            this.dtp_salesEnd.Value = dateTime;
            btnSalesClicked = "Week"; salesdaterange();
            txtgrossSales.Text = "Week's Gross Sales"; txtnetSales.Text = "Week's Net Sales";

            btn_todaySales.LinkColor = Color.DarkRed; btn_weekSales.LinkColor = Color.Chocolate; btn_monthSales.LinkColor = Color.DarkRed; btn_yearSales.LinkColor = Color.DarkRed;
        }

        private void btn_monthSales_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.dtp_salesStart.Value = dateTime;
            this.dtp_salesEnd.Value = dateTime;
            btnSalesClicked = "Month"; salesdaterange();
            txtgrossSales.Text = "Month's Gross Sales"; txtnetSales.Text = "Month's Net Sales";

            btn_todaySales.LinkColor = Color.DarkRed; btn_weekSales.LinkColor = Color.DarkRed; btn_monthSales.LinkColor = Color.Chocolate; btn_yearSales.LinkColor = Color.DarkRed;
        }

        private void btn_yearSales_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.dtp_salesStart.Value = dateTime;
            this.dtp_salesEnd.Value = dateTime;
            btnSalesClicked = "Year"; salesdaterange();
            txtgrossSales.Text = "Year's Gross Sales"; txtnetSales.Text = "Year's Net Sales";

            btn_todaySales.LinkColor = Color.DarkRed; btn_weekSales.LinkColor = Color.DarkRed; btn_monthSales.LinkColor = Color.DarkRed; btn_yearSales.LinkColor = Color.Chocolate;
        }

        private void holder_dtpsalesStart_Click(object sender, EventArgs e)
        {
            dtp_salesStart.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dtp_salesStart_ValueChanged(object sender, EventArgs e)
        {
            btnSalesClicked = "DateRange"; txtgrossSales.Text = "Custom - Gross Sales"; txtnetSales.Text = "Custom - Net Sales";
            holder_dtpsalesStart.Text = dtp_salesStart.Text.TrimStart();
            salesdaterange(); 

            if (dgv_salesmain.RowCount == 0) { lb_grosssales.Text = "₱ 0.00"; lb_netsales.Text = "₱ 0.00"; }
            btn_todaySales.LinkColor = Color.DarkRed; btn_weekSales.LinkColor = Color.DarkRed; btn_monthSales.LinkColor = Color.DarkRed; btn_yearSales.LinkColor = Color.DarkRed;
        }

        private void holder_dtpsalesEnd_Click(object sender, EventArgs e)
        {
            dtp_salesEnd.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dtp_salesEnd_ValueChanged(object sender, EventArgs e)
        {
            btnSalesClicked = "DateRange"; txtgrossSales.Text = "Custom - Gross Sales"; txtnetSales.Text = "Custom - Net Sales";
            holder_dtpsalesEnd.Text = dtp_salesEnd.Text.TrimStart();
            salesdaterange(); 

            if (dgv_salesmain.RowCount == 0) { lb_grosssales.Text = "₱ 0.00"; lb_netsales.Text = "₱ 0.00"; }
            btn_todaySales.LinkColor = Color.DarkRed; btn_weekSales.LinkColor = Color.DarkRed; btn_monthSales.LinkColor = Color.DarkRed; btn_yearSales.LinkColor = Color.DarkRed;
        }

        private void panel33_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.panel33.ClientRectangle, Color.MediumVioletRed, ButtonBorderStyle.Solid);
        }

        private void panel34_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.panel34.ClientRectangle, Color.MediumVioletRed, ButtonBorderStyle.Solid);
        }

        private void panel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel15_Paint_1(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.panel15.ClientRectangle, Color.MediumVioletRed, ButtonBorderStyle.Solid);
        }

        private void dgv_prodlist_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgv_prodlist.Rows.Count != 0)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    if (dgv_prodlist.CurrentRow.Selected == true)
                    {
                        if (txtidprodOrd.Text != dgv_prodlist.CurrentRow.Cells[0].Value.ToString())
                        {
                            txtb_qtyOrd.Clear();
                        }

                        txtidprodOrd.Text = dgv_prodlist.SelectedRows[0].Cells[0].Value.ToString();
                        txtnameprodOrd.Text = dgv_prodlist.SelectedRows[0].Cells[1].Value.ToString();

                        double orderunitprice = Convert.ToDouble(dgv_prodlist.SelectedRows[0].Cells[4].Value.ToString());
                        txtupriceOrd.Text = "₱ " + orderunitprice.ToString("#,###,##0.00");

                        txtb_qtyOrd.Visible = true;
                        txtb_qtyOrd.Focus();

                    }
                }
            }
        }

        private void txtb_qtyOrd_TextChanged(object sender, EventArgs e)
        {
            if (dgv_prodlist.Rows.Count != 0)
            {
                txtb_qtyOrd.Text = txtb_qtyOrd.Text.TrimStart('0');

                if (txtb_qtyOrd.Text != "" && dgv_prodlist.SelectedRows.Count > 0)
                {
                    double subtotal = Convert.ToInt32(txtb_qtyOrd.Text) * Convert.ToDouble(dgv_prodlist.SelectedRows[0].Cells[4].Value);
                    txtsubtotalOrd.Text = string.Format("₱ {0:#,#,#.00}", subtotal);
                }

                else if (txtb_qtyOrd.Text == "")
                {
                    txtsubtotalOrd.Text = "₱ 0.00";     
                }
            }
        }

        private void dgv_prodlist_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void dgv_prodlist_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        Stack<string> undoprodID = new Stack<string>();
        Stack<int> undoprodQty = new Stack<int>();
        Stack<string> undoprodName = new Stack<string>();
        Stack<string> undoprodUprice = new Stack<string>();
        Stack<string> undoprodSubprice = new Stack<string>();

        private double nondiscountedTotalAmt;
        private void txtb_qtyOrd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                txtb_qtyOrd.MaxLength = 2;
            }         
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }

             if (e.KeyChar == Convert.ToChar(Keys.Enter) && txtb_qtyOrd.Text != "" && dgv_prodlist.CurrentRow.Selected == true)
             {
                 txtb_payment.Focus();
                 
                 double unitprice = 0;
                 unitprice = Convert.ToDouble(dgv_prodlist.SelectedRows[0].Cells[4].Value.ToString());

                 double subtotal = Double.Parse(txtb_qtyOrd.Text.ToString()) * Double.Parse(dgv_prodlist.SelectedRows[0].Cells[4].Value.ToString());
                 nondiscountedTotalAmt = nondiscountedTotalAmt + subtotal;
                 txt_CusTotalCost.Text = Convert.ToString(String.Format("₱ {0:#,#,#.00}", nondiscountedTotalAmt));
           
                 Receipt obj = new Receipt() { ProductName = txtnameprodOrd.Text, Price = Convert.ToString((String.Format("₱ {0:#,#,#.00}", unitprice))), Quantity = Convert.ToInt32(txtb_qtyOrd.Text), SubTotal = Convert.ToString(String.Format("₱ {0:#,#,#.00}", subtotal)), ProductID = Convert.ToInt32(txtidprodOrd.Text.Replace("P-", "")) };

                 //- If product is alrd existed in the list then sum the qty together. --------------
                 bool prodalrdExists = false;
                 if (dgv_orderList.Rows.Count > 0)
                 {
                     foreach (DataGridViewRow row in dgv_orderList.Rows)
                     {
                         if (Convert.ToString(row.Cells[1].Value) == txtnameprodOrd.Text)
                         {
                             prodalrdExists = true;
                             row.Cells[0].Value = Convert.ToString(Convert.ToInt32(txtb_qtyOrd.Text) + Convert.ToInt32(row.Cells[0].Value));
                             row.Cells[3].Value = Convert.ToString(String.Format("₱ {0:#,#,#.00}", Convert.ToInt32(row.Cells[0].Value) * Convert.ToDouble(row.Cells[2].Value.ToString().Replace("₱ ", ""))));
                         }
                     }
                 }
                 if (!prodalrdExists) //- Non-Existing Product Adds to the Order List
                 {
                     receiptBindingSource.Add(obj);
                     receiptBindingSource.MoveLast();
                     
                 }
                 //----------------------------------------------------------

                 if (discountgranted == 1) //- All prod that will be selected will consider or will have discount
                 {
                     if (seniordiscAct == "1")
                     {
                         double afterDiscountMnual = nondiscountedTotalAmt * 0.20;
                         double afterGrdtotalMnual = nondiscountedTotalAmt - afterDiscountMnual; 
                         txt_CusTotalCost.Text = String.Format("₱ {0:#,#,0.00}", afterGrdtotalMnual);
                     }
                     else if (customdiscAct == "1")
                     {
                         double afterDiscountMnual = nondiscountedTotalAmt * (Convert.ToDouble(customdisc.Replace(" %", "")) / 100);
                         double afterGrdtotalMnual = nondiscountedTotalAmt - afterDiscountMnual;
                         txt_CusTotalCost.Text = String.Format("₱ {0:#,#,0.00}", afterGrdtotalMnual);
                     }
                 }

                 //- Set "Inactive Products" to Active. --------------------
                 if (dgv_prodlist.SelectedRows[0].Cells[5].Value.ToString() == "Inactive")
                 {
                     prodstatInactive.Add(dgv_prodlist.SelectedRows[0].Cells[0].Value.ToString().Replace("P-", ""));
                     updProdtoActive();
                 }
                 //--------------------------
 
                 //- Insert added products to Undo. ---------------
                 undoprodID.Push(txtidprodOrd.Text.Replace("P-", ""));
                 undoprodQty.Push(Convert.ToInt16(txtb_qtyOrd.Text));
                 undoprodSubprice.Push(txtsubtotalOrd.Text.Replace("₱ ", ""));
                 //------------------------------

                 txtb_qtyOrd.Clear();
                 txtb_qtyOrd.Visible = false;

                 txtidprodOrd.Text = "P-0"; txtnameprodOrd.Text = "Ex. Apple, Banana, Ice Cream";
                 txtupriceOrd.Text = "₱ 0.00"; txtsubtotalOrd.Text = "₱ 0.00";

                 dgv_orderList.Rows[0].Selected = false;
                 dgv_prodlist.ClearSelection();

                 txtb_payment.Clear();
                 txtb_change.Clear();
                 _inputnumber = 0;

                  
             }
        }

        private double _inputnumber = 0;
        private void txtb_payment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                if (txtb_payment.Text.Length <= 15)
                {                 
                    _inputnumber = 10 * _inputnumber + Int32.Parse(e.KeyChar.ToString());
                    ReformatOutput();
                }
            }
            else if (e.KeyChar == (char)Keys.Back)
            {
                txtb_payment.Clear(); txtb_change.Clear();
                _inputnumber = 0;
            }
        }

        private void ReformatOutput()
        {
            txtb_payment.Text = String.Format("₱ {0:#,#,0.00}", (double)_inputnumber / 100.0);
        }

        private void txtb_payment_TextChanged(object sender, EventArgs e)
        {
            if (txtb_payment.Text == "₱ 0.00") { txtb_payment.Clear(); }

            else if (txtb_payment.Text != "" && txtb_payment.Text != "₱ 0.00")
            {
                double payment;
                payment = Convert.ToDouble(string.Concat(txtb_payment.Text.Where(char.IsDigit)));
                payment = Convert.ToDouble(String.Format("{0:#,#,#.##}", payment / 100.0));

                if (txtb_payment.Text == ".")
                {

                    txtb_payment.Text = "";

                }
                if (txtb_payment.Text == "")
                {

                    txtb_change.Text = "";

                }
                else if (Convert.ToDouble(txt_CusTotalCost.Text.Replace("₱ ", "")) <= payment)
                {
                    double change = payment - Convert.ToDouble(txt_CusTotalCost.Text.Replace("₱ ", ""));
                    txtb_change.Text = "₱ " + change.ToString("#,###,##0.00");

                }
                else if (payment >= Convert.ToDouble(txt_CusTotalCost.Text.Replace("₱ ", "")))
                {
                    txtb_change.Text = "";
                }


            }
        }

        private void btn_ordReturn_Click(object sender, EventArgs e)
        {
            UndoOrder();      
        }

        private void UndoOrder()
        {
            if (dgv_orderList.Rows.Count >= 1) 
            {
                bool itemfound = false; int qtyisZero = 1;
                string undoPID = undoprodID.Pop().ToString();
                string undoQty = undoprodQty.Pop().ToString();
                string undoSubTotal = undoprodSubprice.Pop().ToString();

                foreach (DataGridViewRow row in dgv_orderList.Rows)
                {
                    if (row.Cells[4].Value.ToString().Replace("P-", "") == undoPID)
                    {
                        dgv_orderList.Rows[row.Index].Selected = true;
                        if (dgv_orderList.Rows[row.Index].Selected == true) { itemfound = true; }                   
                    }

                    if (row.Cells[4].Value.ToString().Replace("P-", "") == undoPID && itemfound)
                    {                 
                        row.Cells[0].Value = Convert.ToInt16(row.Cells[0].Value) - Convert.ToInt16(undoQty);
                        row.Cells[3].Value = String.Format("₱ {0:#,#,0.00}", Convert.ToDouble(row.Cells[0].Value) * Convert.ToDouble(row.Cells[2].Value.ToString().Replace("₱ ", "")));
                        nondiscountedTotalAmt = nondiscountedTotalAmt - Convert.ToDouble(undoSubTotal);
                        txt_CusTotalCost.Text = String.Format("₱ {0:#,#,0.00}", nondiscountedTotalAmt);

                        if (row.Cells[0].Value.Equals(0))
                        {
                           dgv_orderList.Rows.RemoveAt(row.Index);
                           qtyisZero = 0;
                        }
                    }       
                }

                //- Undo those removed products. ---------
                if (!itemfound)
                {
                    Receipt obj = new Receipt() { ProductName = undoprodName.Pop().ToString(), Price = Convert.ToString(undoprodUprice.Pop().ToString()), Quantity = Convert.ToInt32(undoQty), SubTotal = Convert.ToString(undoSubTotal), ProductID = Convert.ToInt32(undoPID) };

                    nondiscountedTotalAmt = nondiscountedTotalAmt + Convert.ToDouble(undoSubTotal.ToString().Replace("₱ ", ""));
                    txt_CusTotalCost.Text = String.Format("₱ {0:#,#,0.00}", nondiscountedTotalAmt);

                    receiptBindingSource.Add(obj);
                    receiptBindingSource.MoveLast();

                    //- Set "Inactive Products" to Active. --------------------
                    if (prodstatInactive.Contains(undoPID)) { updProdtoActive(); }
             
                }
                //----------------------------------------

                //- All Product that will be selected will consider or will have discount. ---------
                if (discountgranted == 1) 
                {
                    if (seniordiscAct == "1")
                    {
                        double afterDiscountMnual = nondiscountedTotalAmt * 0.20;
                        double afterGrdtotalMnual = nondiscountedTotalAmt - afterDiscountMnual;
                        txt_CusTotalCost.Text = String.Format("₱ {0:#,#,0.00}", afterGrdtotalMnual);
                    }
                    else if (customdiscAct == "1")
                    {
                        double afterDiscountMnual = nondiscountedTotalAmt * (Convert.ToDouble(customdisc.Replace(" %", "")) / 100);
                        double afterGrdtotalMnual = nondiscountedTotalAmt - afterDiscountMnual;
                        txt_CusTotalCost.Text = String.Format("₱ {0:#,#,0.00}", afterGrdtotalMnual);
                    }
                }
                //-------------------------------------------------

                if (prodstatInactive.Contains(undoPID) && qtyisZero == 0)
                {
                    try
                    {
                        cn.Open();
                        cmd.Connection = cn;

                        int RowsAffected = 0;

                        cmd.CommandText = "UPDATE tbl_product SET status = 'Inactive' WHERE prod_ID = '" + undoPID + "';";
                        RowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally { cn.Close(); }

                    viewProductList();
                }

                dgv_orderList.ClearSelection();
            }

            if (dgv_orderList.RowCount == 0)
            {
                nondiscountedTotalAmt = 0; //- Resets Grand Total. --
            }
        }

        private void holder_dtpOHStart_Click(object sender, EventArgs e)
        {
            dtp_orderhStart.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dtp_orderhStart_ValueChanged(object sender, EventArgs e)
        {
            holder_dtpOHStart.Text = dtp_orderhStart.Text.TrimStart();
            orderhdaterange();     
        }

        private void holder_dtpOHEnd_Click(object sender, EventArgs e)
        {
            dtp_orderhEnd.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dtp_orderhEnd_ValueChanged(object sender, EventArgs e)
        {
            holder_dtpOHEnd.Text = dtp_orderhEnd.Text.TrimStart();
            orderhdaterange();
        }

        private void kitchenDisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            kd.lb_activeuser.Text = "Kitchen Display | " + lb_userlvl.Text + " " + lb_name.Text;
            kd.Show();

            if (kd.WindowState == FormWindowState.Minimized)
            {
                kd.WindowState = FormWindowState.Normal;     
            }
           
            FormCollection fc = Application.OpenForms;

            foreach (Form frm in fc)
            {
                //iterate through
                if (frm.Name == "KitchenDP")
                {
                    frm.BringToFront();      
                }
            }
        }

        private void btn_todaydb_Click(object sender, EventArgs e)
        {
            btnDashboardClicked = "Today"; dbdaterange();

            btn_todaydb.ForeColor = Color.White;
            btn_todaydb.BackColor = Color.Chocolate;

            btn_last7db.BackColor = Color.MintCream; btn_last30db.BackColor = Color.MintCream; btn_yeardb.BackColor = Color.MintCream;
            btn_last7db.ForeColor = Color.Chocolate; btn_last30db.ForeColor = Color.Chocolate; btn_yeardb.ForeColor = Color.Chocolate;
        }

        private void btn_last7db_Click(object sender, EventArgs e)
        {
            btnDashboardClicked = "Week"; dbdaterange();

            btn_last7db.ForeColor = Color.White;
            btn_last7db.BackColor = Color.Chocolate;

            btn_todaydb.BackColor = Color.MintCream; btn_last30db.BackColor = Color.MintCream; btn_yeardb.BackColor = Color.MintCream;
            btn_todaydb.ForeColor = Color.Chocolate; btn_last30db.ForeColor = Color.Chocolate; btn_yeardb.ForeColor = Color.Chocolate;
        }

        private void btn_last30db_Click(object sender, EventArgs e)
        {
            btnDashboardClicked = "Month"; dbdaterange();

            btn_last30db.ForeColor = Color.White;
            btn_last30db.BackColor = Color.Chocolate;

            btn_last7db.BackColor = Color.MintCream; btn_todaydb.BackColor = Color.MintCream; btn_yeardb.BackColor = Color.MintCream;
            btn_last7db.ForeColor = Color.Chocolate; btn_todaydb.ForeColor = Color.Chocolate; btn_yeardb.ForeColor = Color.Chocolate;
        }

        private void btn_yeardb_Click(object sender, EventArgs e)
        {
            btnDashboardClicked = "Year"; dbdaterange();

            btn_yeardb.ForeColor = Color.White;
            btn_yeardb.BackColor = Color.Chocolate;

            btn_last7db.BackColor = Color.MintCream; btn_last30db.BackColor = Color.MintCream; btn_todaydb.BackColor = Color.MintCream;
            btn_last7db.ForeColor = Color.Chocolate; btn_last30db.ForeColor = Color.Chocolate; btn_todaydb.ForeColor = Color.Chocolate;
        }

        

    }
}
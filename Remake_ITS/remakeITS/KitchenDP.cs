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
using System.Runtime.InteropServices;

namespace remakeITS
{
    public partial class KitchenDP : Form
    {
        MySqlConnection cn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["remakeITS.Properties.Settings.ordersysConnectionString"].ConnectionString);
        MySqlCommand cmd = new MySqlCommand();

        //- Remove the new panel to not waste a ram or to prevent memory leak. ----
        Panel MPrntOrd;
        Panel SecMainPrnt;
        Panel TrdMainPrnt;
        Panel PrntOrno;
        Panel PrntTim;
        Panel PrntExt;
        Panel PrntCstmr;

        Label LbOrdno;
        Label LbTimSec;
        Label LbTimeShw;
        Label LbExt;
        Label LbCstmr;

        FlowLayoutPanel OrdFlp;
        Label LbItms;

        System.Windows.Forms.Timer timer;
        System.Windows.Forms.Timer t;

        private Dictionary<System.Windows.Forms.Timer, Label> dict = new Dictionary<System.Windows.Forms.Timer, Label>();
        private Dictionary<System.Windows.Forms.Timer, Label> dict3 = new Dictionary<System.Windows.Forms.Timer, Label>();
        private Dictionary<System.Windows.Forms.Timer, GroupBox> dict2 = new Dictionary<System.Windows.Forms.Timer, GroupBox>();

        private Dictionary<System.Windows.Forms.Timer, Panel> dictClr1 = new Dictionary<System.Windows.Forms.Timer, Panel>();
        private Dictionary<System.Windows.Forms.Timer, Panel> dictClr2 = new Dictionary<System.Windows.Forms.Timer, Panel>();
        private Dictionary<System.Windows.Forms.Timer, Panel> dictClr3 = new Dictionary<System.Windows.Forms.Timer, Panel>();
        private Dictionary<System.Windows.Forms.Timer, Panel> dictClr4 = new Dictionary<System.Windows.Forms.Timer, Panel>();

        int mnpnlNo = 1;
        string orderno; List<string> exisitingorder = new List<string>();

        //- Can drag panel to which screen you want. -------------
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        //-------------------------------

        public KitchenDP()
        {
            InitializeComponent();
        }

        private void KitchenDP_Load(object sender, EventArgs e)
        {
            KitchenDisplay();
            System.Windows.Forms.Timer timeRef = new System.Windows.Forms.Timer();
            timeRef.Interval = 500;
            timeRef.Tick += new EventHandler(timer3_Tick);
            timeRef.Start();

        }

        void timer2_Tick(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count >= 1)
            {
                t = (System.Windows.Forms.Timer)sender;

                if (dict[t].Text != "")
                {
                    dict[t].Text = Convert.ToString(Convert.ToInt32(dict[t].Text) + 1);
                    
                    int seconds = Convert.ToInt32(dict[t].Text) + 1;
                    string timeHMS = string.Format("Time : {0:D2}:{1:D2}:{2:D2}", seconds / 3600, seconds % 3600 / 60, seconds % 60);  /// make another dynamic label for converting this seconds to HH:mm:ss.

                    dict3[t].Text = timeHMS;

                    if (dict3[t].Text == "Time : 00:01:00")
                    {
                        // Can stop flicker in a panel? //typeof(GroupBox).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, Gb, new object[] { true });
                        dictClr1[t].BackColor = Color.DarkOrange;
                        dictClr2[t].BackColor = Color.DarkOrange;
                        dictClr3[t].BackColor = Color.DarkOrange;
                        dictClr4[t].BackColor = Color.DarkOrange; 
                    }
                    else if (dict3[t].Text == "Time : 00:02:00")
                    {
                        dictClr1[t].BackColor = Color.Crimson;
                        dictClr2[t].BackColor = Color.Crimson;
                        dictClr3[t].BackColor = Color.Crimson;
                        dictClr4[t].BackColor = Color.Crimson;
                    }
                }
            }
        }

        int countCtrls;
        void timer3_Tick(object sender, EventArgs e)
        {
            //- Making it a realtime db where it always check the db for changes. ----------------
            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = "Select count(distinct orderID) AS 'KD' from tbl_kitchenDisp";

                int count = Convert.ToInt32(cmd.ExecuteScalar());
              
                if (count != countCtrls) /// is it safe when i include multiple queries that runs at the same time? or put this if condition outside of this connection
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }

                    KitchenDisplay();

                    if (cn.State == ConnectionState.Closed)
                    {
                        cn.Open();
                    }
                    
                  
                }

                //- Realtime checks if the order is already done and removes it from the flowlayoutpanel. ------------
                if (flowLayoutPanel1.Controls.Count != 0)
                {
                    foreach (Control c in flowLayoutPanel1.Controls.OfType<Panel>().ToList())
                    {

                        cmd.CommandText = "select A.orderNo, B.orderID from tbl_ordermain A inner join tbl_kitchendisp B On A.orderID = B.orderID where orderNo LIKE '" + c.Name.Replace("MainPanel", "") + "' LIMIT 1;";
                        string found = Convert.ToString(cmd.ExecuteScalar());
       
                        if (found == "")
                        {
                            flowLayoutPanel1.Controls.Remove(c);
                            c.Dispose();

                            mnpnlNo = mnpnlNo - 1; countCtrls = countCtrls - 1; 
                        }

                    }
                
                }
                
                
            } 
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            finally { cn.Close(); } 
        }

        public void KitchenDisplay()
        {
            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = "select C.orderNo, A.prod_name,B.qty from tbl_product A inner join tbl_kitchendisp B on A.prod_ID = B.prod_ID inner join tbl_ordermain C on B.orderID = C.orderID group by C.orderNo order by kitchenID asc;";

                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row1 in dt.Rows)
                {
                    //- Add all non-existing (group)order number from the database. -----------------------------------
                    if (!exisitingorder.Contains(row1["orderNo"].ToString()) && mnpnlNo <= dt.Rows.Count)
                    {
                        orderno = row1["orderNo"].ToString();

                        MPrntOrd = new Panel();
                        SecMainPrnt = new Panel();
                        TrdMainPrnt = new Panel();
                        PrntOrno = new Panel();
                        PrntTim = new Panel();
                        PrntExt = new Panel();
                        PrntCstmr = new Panel();

                        LbOrdno = new Label();
                        LbTimSec = new Label();
                        LbTimeShw = new Label();
                        LbExt = new Label();
                        LbCstmr = new Label();

                        OrdFlp = new FlowLayoutPanel();

                        timer = new System.Windows.Forms.Timer();
                        timer.Tick += new EventHandler(timer2_Tick);
                        timer.Interval = 1000;

                        MPrntOrd.Name = "MainPanel" + orderno;
                        MPrntOrd.Size = new Size(294, 296);
                        MPrntOrd.BackColor = Color.Transparent;
                        MPrntOrd.Margin = new Padding(8, 8, 0, 0);
                        flowLayoutPanel1.Controls.Add(MPrntOrd);
                        //-------------------------------------
                        PrntOrno.Name = "ParentOrdNo" + mnpnlNo;
                        PrntOrno.Size = new Size(141, 31);
                        PrntOrno.Location = new Point(3, 2);
                        PrntOrno.BackColor = Color.PaleGreen;
                        PrntOrno.BorderStyle = BorderStyle.FixedSingle;
                        MPrntOrd.Controls.Add(PrntOrno);
                        //-------------------------------------
                        PrntTim.Name = "ParentTime" + mnpnlNo;
                        PrntTim.Size = new Size(126, 26);
                        PrntTim.Location = new Point(143, 7);
                        PrntTim.BackColor = Color.PaleGreen;
                        PrntTim.BorderStyle = BorderStyle.FixedSingle;
                        MPrntOrd.Controls.Add(PrntTim);
                        //-------------------------------------
                        PrntExt.Name = "ParentExt" + mnpnlNo;
                        PrntExt.Size = new Size(26, 26);
                        PrntExt.Location = new Point(268, 7);
                        PrntExt.BackColor = Color.PaleGreen;
                        PrntExt.BorderStyle = BorderStyle.FixedSingle;
                        MPrntOrd.Controls.Add(PrntExt);
                        //-------------------------------------
                        SecMainPrnt.Name = "SecondMainOrd" + mnpnlNo;
                        SecMainPrnt.Size = new Size(288, 278);
                        SecMainPrnt.Location = new Point(4, 16);
                        SecMainPrnt.BackColor = Color.PaleGreen;
                        MPrntOrd.Controls.Add(SecMainPrnt);
                        //-------------------------------------
                        TrdMainPrnt.Name = "TrdMainOrd" + mnpnlNo;
                        TrdMainPrnt.Size = new Size(282, 272);
                        TrdMainPrnt.Location = new Point(3, 3);
                        TrdMainPrnt.BackColor = Color.Black;
                        TrdMainPrnt.BorderStyle = BorderStyle.FixedSingle;
                        SecMainPrnt.Controls.Add(TrdMainPrnt);
                        //-------------------------------------
                        PrntCstmr.Name = "ParentTitle" + mnpnlNo;
                        PrntCstmr.Size = new Size(278, 28);
                        PrntCstmr.Location = new Point(1, 14);
                        PrntCstmr.BackColor = Color.Gray;
                        PrntCstmr.BorderStyle = BorderStyle.FixedSingle;
                        TrdMainPrnt.Controls.Add(PrntCstmr);
                        //-------------------------------------
                        LbOrdno.Name = "LabelOrd" + mnpnlNo;
                        LbOrdno.Size = new Size(137, 27);
                        LbOrdno.Font = new Font("Microsoft Sans Serif", 15, FontStyle.Regular);
                        LbOrdno.Text = "OR# " + orderno;
                        LbOrdno.Location = new Point(1, 1);
                        LbOrdno.BackColor = Color.Black;
                        LbOrdno.ForeColor = Color.White;
                        LbOrdno.BorderStyle = BorderStyle.FixedSingle;
                        LbOrdno.FlatStyle = FlatStyle.Flat;
                        PrntOrno.Controls.Add(LbOrdno);
                        //- Timer in Seconds ------------------
                        LbTimSec.Name = "LabelTime" + mnpnlNo;
                        LbTimSec.Visible = false;
                        LbTimSec.AutoSize = true;
                        LbTimSec.Font = new Font("Microsoft Sans Serif", 6, FontStyle.Regular);
                        LbTimSec.Text = "-1"; /// -1 so that it count up to 0 forward.
                        LbTimSec.Location = new Point(139, 52);
                        LbTimSec.ForeColor = Color.White;
                        TrdMainPrnt.Controls.Add(LbTimSec);
                        //- Timer in Format -------------------
                        LbTimeShw.Name = "LabelTimeShw" + mnpnlNo;
                        LbTimeShw.Size = new Size(122, 22);
                        LbTimeShw.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Regular);
                        LbTimeShw.Text = "Time : 00:00:00";
                        LbTimeShw.Location = new Point(1, 1);
                        LbTimeShw.ForeColor = Color.White;
                        LbTimeShw.BackColor = Color.Black;
                        LbTimeShw.BorderStyle = BorderStyle.FixedSingle;
                        LbTimeShw.FlatStyle = FlatStyle.Flat;
                        LbTimeShw.Padding = new Padding(2, 0, 0, 0);
                        PrntTim.Controls.Add(LbTimeShw);
                        //-------------------------------------
                        LbExt.Name = orderno; // I used (x button) to store the orderno, so that i can directly use to delete orders from KD/
                        LbExt.AutoSize = true;
                        LbExt.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Regular);
                        LbExt.Text = "X";
                        LbExt.Location = new Point(1, 1);
                        LbExt.ForeColor = Color.White;
                        LbExt.BackColor = Color.Black;
                        LbExt.BorderStyle = BorderStyle.FixedSingle;
                        LbExt.FlatStyle = FlatStyle.Flat;
                        LbExt.Cursor = Cursors.Hand;
                        PrntExt.Controls.Add(LbExt);
                        //-------------------------------------
                        LbCstmr.Name = "TitleCusto" + mnpnlNo;
                        LbCstmr.AutoSize = true;
                        LbCstmr.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Regular);
                        LbCstmr.Text = "Customer's Order";
                        LbCstmr.Location = new Point(72, 2);
                        LbCstmr.ForeColor = Color.White;
                        LbCstmr.FlatStyle = FlatStyle.Flat;
                        PrntCstmr.Controls.Add(LbCstmr);
                        //-------------------------------------
                        OrdFlp.Name = "OrderItemFLP" + mnpnlNo;
                        OrdFlp.AutoScroll = true;
                        OrdFlp.Size = new Size(274, 219);
                        OrdFlp.FlowDirection = FlowDirection.TopDown;
                        OrdFlp.Location = new Point(3, 48);
                        OrdFlp.WrapContents = false;
                        OrdFlp.BackColor = Color.Black;
                        TrdMainPrnt.Controls.Add(OrdFlp);

                        mnpnlNo = mnpnlNo + 1;

                        //-------------------------------------------------
                        LbExt.MouseEnter += new System.EventHandler(this.LblXHover);
                        LbExt.MouseLeave += new System.EventHandler(this.LblXLeave);
                        LbExt.Click += new System.EventHandler(this.LblXClick);

                        //----------------------------
                        dict[timer] = LbTimSec;
                        dict3[timer] = LbTimeShw;

                        dictClr1[timer] = PrntOrno;
                        dictClr2[timer] = PrntTim;
                        dictClr3[timer] = SecMainPrnt;
                        dictClr4[timer] = PrntExt;

                        timer.Start();
                        //-------------------------------------------------

                        //- New Query to show all (non-group)orderno data and put the products on each corresponding order number. ----------------- 
                        cmd.CommandText = "select C.orderNo, A.prod_name,B.qty from tbl_product A inner join tbl_kitchendisp B on A.prod_ID = B.prod_ID inner join tbl_ordermain C on B.orderID = C.orderID order by kitchenID asc;";
                        
                        MySqlDataAdapter da2 = new MySqlDataAdapter();
                        da2.SelectCommand = cmd;
                        DataTable dt2 = new DataTable();
                        da2.Fill(dt2);

                        foreach (DataRow row in dt2.Rows)
                        {
                            if (LbOrdno.Text.Replace("OR# ", "") == row["orderNo"].ToString())
                            {
                                string prodQty = "- " + row["qty"].ToString() + "x " + row["prod_name"].ToString();

                                LbItms = new Label();
                                
                                LbItms.BackColor = Color.Black;
                                LbItms.Name = "LblItems";
                                LbItms.Text = prodQty;
                                LbItms.ForeColor = Color.White;
                                LbItms.Font = new Font("Microsoft Sans Serif", 14, FontStyle.Regular);
                                LbItms.Location = new Point(3, 0);
                                LbItms.AutoSize = true;
                                LbItms.Cursor = Cursors.Hand;

                                OrdFlp.Controls.Add(LbItms);

                                LbItms.Click += new System.EventHandler(this.LbItms_Click);
                            }
                        }
                        //------------------------------------------------------------

                        //- Add all those existing orders to the list, so that it will not duplicate.--
                        exisitingorder.Add(row1["orderNo"].ToString());

                        //- Count all existing control from the flp. ---------
                        countCtrls = flowLayoutPanel1.Controls.Count;

                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            finally { cn.Close(); }
        }


        private void LblXClick(object sender, EventArgs e)
        {
            Label current = (Label)sender; 
           
            foreach (Control c in flowLayoutPanel1.Controls.OfType<Panel>().ToList())
            {
                //MessageBox.Show(c.Name);
                if (c.Text == current.Parent.Parent.ToString())
                {
                    MessageBox.Show(c.Text);
                    //  flowLayoutPanel1.Controls.Remove(c);
                    
                }

            }
            //MessageBox.Show(dict.ElementAt(0).ToString());
           // flowLayoutPanel1.Controls.Remove(current.Parent.Parent);
           // current.Parent.Parent.Dispose(); //Find a way to Dispose the parent without an error.  or just dispose it on every exit
                    
            exisitingorder.Remove(current.Name);
            //- Idle when flowlayout is empty - Resets everything. Set Timer to Stop. Clear & Dispose Timer/Dictionaries. Clear List<>.
            //- Removing order when the order is remove from the another computer.

            //KitchenDisplay();
            if (flowLayoutPanel1.Controls.Count == 0)
            {
                timer.Stop(); t.Stop(); timer.Dispose(); t.Dispose();
            }

            try
            {
                cn.Open();
                cmd.Connection = cn;
                
                //- SET SQL_SAFE_UPDATES = 0; if necessary make a query about this so that you can update or delete the data without basing the primary key to delete or update a data.
                cmd.CommandText = "DELETE kchendi.* FROM tbl_kitchendisp kchendi WHERE orderID IN (SELECT orderID FROM tbl_ordermain WHERE orderNo = '" + current.Name + "')";
                
                
                int RowsAffected = 0;
                RowsAffected = cmd.ExecuteNonQuery();
    
                if (RowsAffected > 0)
                {
                    //mnpnlNo = mnpnlNo - 1; countCtrls = countCtrls - 1;                 
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            finally { cn.Close(); }

          
              
        }

        private void LblXHover(object sender, EventArgs e)
        {
            Label current = (Label)sender;

            current.ForeColor = Color.White;
            current.BackColor = Color.Red;
        }

        private void LblXLeave(object sender, EventArgs e)
        {
            Label current = (Label)sender;

            current.ForeColor = Color.White;
            current.BackColor = Color.Black;
        }

        private void LbItms_Click(object sender, EventArgs e)
        {
            Label current = (Label)sender;

            if (current.ForeColor == Color.White)
            {
                current.ForeColor = Color.Cyan;
            }
            else if (current.ForeColor == Color.Cyan)
            {
                current.ForeColor = Color.White;
            }

        }

        private void btn_exitkd_Click(object sender, EventArgs e)
        {

        }

        private void panel15_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
          
        }

        private void btn_minimize_Click(object sender, EventArgs e)
        {
            
        }

        private void btn_maximize_Click(object sender, EventArgs e)
        {        
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void btn_minimize_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btn_exitKD_Click_1(object sender, EventArgs e)
        {
            this.Hide();
           
        }

        private void KitchenDP_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        private void btn_exitKD_MouseEnter(object sender, EventArgs e)
        {
            btn_exitKD.ForeColor = Color.Pink;
        }

        private void btn_exitKD_MouseLeave(object sender, EventArgs e)
        {
            btn_exitKD.ForeColor = Color.White;
        }

        private void btn_maximize_MouseEnter(object sender, EventArgs e)
        {
            btn_maximize.ForeColor = Color.DeepSkyBlue;
        }

        private void btn_maximize_MouseLeave(object sender, EventArgs e)
        {
            btn_maximize.ForeColor = Color.White;
        }

        private void btn_minimize_MouseEnter(object sender, EventArgs e)
        {
            btn_minimize.ForeColor = Color.DeepSkyBlue;
        }

        private void btn_minimize_MouseLeave(object sender, EventArgs e)
        {
            btn_minimize.ForeColor = Color.White;
        }

       
    }
}

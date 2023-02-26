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
using MySql.Data;

namespace remakeITS
{
    public partial class Login : Form
    {
        Main mainform;
        KitchenDP ktchdp;

        MySqlConnection con = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["remakeITS.Properties.Settings.ordersysConnectionString"].ConnectionString);
        MySqlCommand cmd = new MySqlCommand();

        string connected = System.Configuration.ConfigurationManager.ConnectionStrings["remakeITS.Properties.Settings.ordersysConnectionString"].ConnectionString;
        string[] s;

        string name;
        string userlvl;

        bool isConnected = false;
       
        string username1 = "admin";
        string password1 = "admin";

        public Login()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.ContainerControl |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.SupportsTransparentBackColor
                          , true);

            string startingsign = label3.Text;
            string startingsign2 = label8.Text;
            string startingsign3 = label10.Text;
         
            List<string> getshuffle = new List<string>();
            getshuffle.Add(startingsign);
            getshuffle.Add(startingsign2);
            getshuffle.Add(startingsign3);
   
            Random randNum = new Random();
            int aRandomPos = randNum.Next(getshuffle.Count); //Returns a non-negative random number less than the specified maximum (firstNames.Count).

            string randompicked = getshuffle[aRandomPos];

            if (randompicked == "Hello  !")
            {
                label3.Show();
                label4.Show();

                label8.Hide();
                label9.Hide();

                label10.Hide();
                label11.Hide();
                label12.Hide();
            }
            else if (randompicked == "Back Again To Work?")
            {
                label3.Hide();
                label4.Hide();

                label8.Show();
                label9.Show();

                label10.Hide();
                label11.Hide();
                label12.Hide();
            }
            else if (randompicked == "Have A")
            {
                label3.Hide();
                label4.Hide();

                label8.Hide();
                label9.Hide();

                label10.Show();
                label11.Show();
                label12.Show();
            } 
        }

        private void Login_Load(object sender, EventArgs e)
        {
   

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                cmd.Connection = con;

                cmd.CommandText = "SELECT COUNT(*),UserID,FirstName,UserType,LastName FROM tbl_login WHERE Username = '" + txtUsername.Text + "' AND Password = '" + txtPassword.Text + "'";

                int count = int.Parse(cmd.ExecuteScalar().ToString());

                MySqlDataReader reader;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (count == 1)
                    {
                        MessageBox.Show("Login Successful");

                        // - Passing the data to the main form.
                        mainform = new Main();

                        name = reader.GetString(2) + " " + reader.GetString(4);
                        userlvl = reader.GetString(3) + " Name:";

                        mainform.lb_userid.Text = "UID-" + reader.GetString(1); mainform.lb_name.Text = reader.GetString(2) + " " + reader.GetString(4);
                        mainform.lb_userlvl.Text = reader.GetString(3) + " Name :";
                        // ----------------------------------
                    }
                }

                if (count == 0) //- If login's information is incorrect.
                {
                    btnLogin.Focus();
                    MessageBox.Show("Incorrect Username or Password", "Login Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    txtUsername.Text = "Please enter your username";
                    txtUsername.ForeColor = Color.Gray;

                    txtPassword.Text = "Please enter your password";
                    txtPassword.PasswordChar = '\0';
                    txtPassword.ForeColor = Color.Gray;
                }
                else if (count == 1) //- Inserting data to Audit Trail.
                {
                    MySqlCommand cmd1 = new MySqlCommand();
                    cmd1.Connection = con;

                    string date = DateTime.Now.ToString("yyyy-MM-dd");
                    string time = DateTime.Now.ToString("HH:mm:ss");

                    string login = userlvl + " " + name + " (U-" + reader.GetString(1) + ")" + " has logged in";
                    string action = "Login";

                    reader.Close();

                    cmd1.CommandText = "INSERT INTO tbl_audittrail (User,Activity,Action,Date,Time) VALUES  ('" + name + "','" + login + "', '" + action + "','" + date + "','" + time + "') ;";
                    cmd1.ExecuteNonQuery();

                    mainform.Show();
                    this.Hide(); // or "this.Dispose();" Check a better way to dispose the application without making more memory load and passing the information to the other form.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally
            {
                con.Dispose();
                con.Close();
            }
        }

        private void btn_login_Click(object sender, EventArgs e)
        {

        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
           
        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }

        private void label3_Click(object sender, EventArgs e)
        {
           
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == ' ')
            {

            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btnLogin.PerformClick();

            }
        }

        private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar))
            {

            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btnLogin.PerformClick();

            }
        }

        private void txtUsername_Enter(object sender, EventArgs e)
        {
            if (txtUsername.Text == "Please enter your username")
            {
                txtUsername.Text = "";
                txtUsername.ForeColor = Color.Black;

            }


        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Please enter your password")
            {
                txtPassword.Text = "";
                txtPassword.ForeColor = Color.Black;
                txtPassword.PasswordChar = '*';


            }
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            string user = txtUsername.Text;

      
            if (txtUsername.Text == "Please enter your username")
            {
                txtUsername.Text = "Please enter your username";
                txtUsername.ForeColor = Color.Gray;



            }
            else
            {

                if (user.Equals(""))
                {
                    txtUsername.Text = "Please enter your username";
                    txtUsername.ForeColor = Color.Gray;


                }
                else
                {
                    txtUsername.Text = user;
                    txtUsername.ForeColor = Color.Black;


                }
                



            }

            
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            string pass = txtPassword.Text;


            if (txtPassword.Text == "Please enter your password")
            {
                txtPassword.Text = "Please enter your password";
                txtPassword.ForeColor = Color.Gray;



            }
            else
            {

                if (pass.Equals(""))
                {
                    txtPassword.PasswordChar = '\0';
                    txtPassword.Text = "Please enter your password";
                    txtPassword.ForeColor = Color.Gray;


                }
                else
                {

                    txtPassword.PasswordChar = '*';
                    txtPassword.Text = pass;         
                    txtPassword.ForeColor = Color.Black;


                }




            }
        }


        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExit_MouseHover(object sender, EventArgs e)
        {
            btnExit.BackColor = Color.DarkRed;
        }

        private void btnExit_MouseLeave_1(object sender, EventArgs e)
        {
            btnExit.BackColor = Color.Transparent;
        }

        private void btnMinimize_MouseHover(object sender, EventArgs e)
        {
            btnMinimize.BackColor = Color.LightSkyBlue;
        }

        private void btnMinimize_MouseLeave(object sender, EventArgs e)
        {
            btnMinimize.BackColor = Color.Transparent;
        }

        private void Login_KeyPress(object sender, KeyPressEventArgs e)
        {
          
        }

        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
        
        }

        private void panel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
           
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
           
        }

       
    }
}

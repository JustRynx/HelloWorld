using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace remakeITS
{
    public partial class ReceiptForm : Form
    {
        List<Receipt> _list;
        string _discount, _total, _cash, _change, _date, _orderno, _vat, _totalitems, _cashiername;

        public ReceiptForm(List<Receipt> dataSource, string discount, string total, string cash, string change, string date, string orderno, string vat, string totalitems, string cashiername)
        {
            InitializeComponent();
            _list = dataSource;
            _discount = discount;
            _total = total;
            _cash = cash;
            _change = change;
            _date = date;
            _orderno = orderno;
            _vat = vat;
            _totalitems = totalitems;
            _cashiername = cashiername;
        }

        private void ReceiptForm_Load(object sender, EventArgs e)
        {
            ReceiptBindingSource.DataSource = _list;

            Microsoft.Reporting.WinForms.ReportParameter[] para = new Microsoft.Reporting.WinForms.ReportParameter[]
            {
                new Microsoft.Reporting.WinForms.ReportParameter("discount",_discount),
                new Microsoft.Reporting.WinForms.ReportParameter("totalCost",_total),
                new Microsoft.Reporting.WinForms.ReportParameter("Cash",_cash),
                new Microsoft.Reporting.WinForms.ReportParameter("Change",_change),
                new Microsoft.Reporting.WinForms.ReportParameter("OrderDate",_date),
                new Microsoft.Reporting.WinForms.ReportParameter("CashierName",_cashiername),
                new Microsoft.Reporting.WinForms.ReportParameter("OrderNo",_orderno),
                new Microsoft.Reporting.WinForms.ReportParameter("totalItem",_totalitems),
                new Microsoft.Reporting.WinForms.ReportParameter("Vat",_vat)       
            };

            this.reportViewer1.LocalReport.SetParameters(para);
            this.reportViewer1.RefreshReport();
        }

        private void ReceiptForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void btn_receiptclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_receiptclose_MouseEnter(object sender, EventArgs e)
        {
            btn_receiptclose.ForeColor = Color.Pink;
        }

        private void btn_receiptclose_MouseLeave(object sender, EventArgs e)
        {
            btn_receiptclose.ForeColor = Color.White;
        }

      
    }
}

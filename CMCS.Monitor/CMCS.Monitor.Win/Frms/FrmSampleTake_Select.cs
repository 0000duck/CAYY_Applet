using CMCS.Common.DAO;
using CMCS.Common.Entities.iEAA;
using DevComponents.DotNetBar.SuperGrid;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CMCS.Monitor.Win.Frms
{
    public partial class FrmSampleTake_Select : DevComponents.DotNetBar.Metro.MetroForm
    {
        /// <summary>
        /// ѡ�е�ʵ��
        /// </summary>
        public CodeContent Output;

        /// <summary>
        /// �������
        /// </summary>
        string sqlWhere;

        public FrmSampleTake_Select(string sqlWhere)
        {
            InitializeComponent();

            this.sqlWhere = sqlWhere;

            Search(string.Empty);
        }

        private void FrmSupplier_Select_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Output = null;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (superGridControl1.PrimaryGrid.Rows.Count > 0) superGridControl1.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                Return();
            }
            else
            {
                Search(txtInput.Text.Trim());
            }
        }

        void Search(string input)
        {
            List<CodeContent> list = CommonDAO.GetInstance().GetCodeContentByKind(this.sqlWhere);
            superGridControl1.PrimaryGrid.DataSource = list;
        }

        void Return()
        {
            GridRow gridRow = superGridControl1.PrimaryGrid.ActiveRow as GridRow;
            if (gridRow == null) return;

            this.Output = (gridRow.DataItem as CodeContent);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void superGridControl1_BeginEdit(object sender, DevComponents.DotNetBar.SuperGrid.GridEditEventArgs e)
        {
            // ȡ���༭ģʽ
            e.Cancel = true;
        }

        /// <summary>
        /// �����к�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superGridControl_GetRowHeaderText(object sender, DevComponents.DotNetBar.SuperGrid.GridGetRowHeaderTextEventArgs e)
        {
            e.Text = (e.GridRow.RowIndex + 1).ToString();
        }

        private void superGridControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) Return();
        }

        private void superGridControl1_CellDoubleClick(object sender, GridCellDoubleClickEventArgs e)
        {
            Return();
        }
    }
}
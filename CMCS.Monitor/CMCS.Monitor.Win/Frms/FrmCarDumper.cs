using CMCS.CarTransport.BeltSampler.Core;
using CMCS.Common.DAO;
using CMCS.Common.Utilities;
using DevComponents.DotNetBar.SuperGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace CMCS.CarTransport.BeltSampler.Frms
{
	public partial class FrmCarDumper : DevComponents.DotNetBar.Metro.MetroForm
	{
		/// <summary>
		/// ����Ψһ��ʶ��
		/// </summary>
		public static string UniqueKey = "FrmCarDumper";

		public FrmCarDumper()
		{
			InitializeComponent();
		}

		#region Vars

		CommonDAO commonDAO = CommonDAO.GetInstance();
		RTxtOutputer rTxtOutputer;
		

		#endregion

		/// <summary>
		/// �����ʼ��
		/// </summary>
		private void InitForm()
		{
			superGridControl1.PrimaryGrid.AutoGenerateColumns = false;
			superGridControl1.PrimaryGrid.AutoGenerateColumns = false;
			////��SuperGridControl�¼� gclmSetSampler
			//GridButtonXEditControl btnSetSampler = superGridControl1.PrimaryGrid.Columns["gclmSetSampler"].EditControl as GridButtonXEditControl;
			//btnSetSampler.Click += btnSetSampler_Click;


		}

		private void FrmCarSampler_Load(object sender, EventArgs e)
		{
			
		}

		private void FrmCarSampler_Shown(object sender, EventArgs e)
		{
			InitForm();

			//BindRCSampling(superGridControl1);
			
		}

		private void FrmCarSampler_FormClosing(object sender, FormClosingEventArgs e)
		{

		}

		#region ����ҵ��
		

		private void timer2_Tick(object sender, EventArgs e)
		{
			timer2.Stop();
			// 2��ִ��һ��
			timer2.Interval = 10000;

			try
			{
				BindG2(superGridControl1);
				BindG4(superGridControl2);
				lblG2WFC.Text= "δ������"+ Count2() + " ��";
				lblG4WFC.Text = "δ������" + Count4() + " ��";
			}
			catch (Exception ex)
			{
				Log4Neter.Error("timer2_Tick", ex);
			}
			finally
			{
				timer2.Start();
			}
		}

		#endregion


		#region ����

		private void superGridControl_BeginEdit(object sender, DevComponents.DotNetBar.SuperGrid.GridEditEventArgs e)
		{
			// ȡ������༭
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

		/// <summary>
		/// Invoke��װ
		/// </summary>
		/// <param name="action"></param>
		public void InvokeEx(Action action)
		{
			if (this.IsDisposed || !this.IsHandleCreated) return;

			this.Invoke(action);
		}

	
		#endregion

		/// <summary>
		/// �󶨷�������Ϣ
		/// </summary>
		/// <param name="superGridControl"></param>
		/// <param name="machineCode">�豸����</param>
		private void BindG2(SuperGridControl superGridControl)
		{
			string sql = string.Format(@"select t2.batch,
									  t5.ordernum,t5.trainnumber,t5.carmodel,t5.PASSTIME,case when t6.isdischarged=1 then '�ѷ���' else 'δ����' end IsFC,t1.GROSSQTY,t1.SKINQTY,t1.SUTTLEQTY
									  from cmcstbtraincarriagepass t5 
									  left join fultbtransport t1 on t1.pkid=t5.id
									  left join fultbinfactorybatch t2 on t1.infactorybatchid=t2.id
									  inner join cmcstbtransportposition t6 on t5.id=t6.transportid 
									  where t5.machinecode = '2' and t5.direction='����' and t5.passtime>sysdate-1
									  order by t5.passtime desc,t5.ordernum
										");
			DataTable list = commonDAO.SelfDber.ExecuteDataTable(sql);
			superGridControl.PrimaryGrid.DataSource = list;
		}
		/// <summary>
		/// �󶨷�������Ϣ
		/// </summary>
		/// <param name="superGridControl"></param>
		/// <param name="machineCode">�豸����</param>
		private void BindG4(SuperGridControl superGridControl)
		{
			string sql = string.Format(@"select t2.batch,
									  t5.ordernum,t5.trainnumber,t5.carmodel,t5.PASSTIME,case when t6.isdischarged=1 then '�ѷ���' else 'δ����' end IsFC,t1.GROSSQTY,t1.SKINQTY,t1.SUTTLEQTY
									  from cmcstbtraincarriagepass t5 
									  left join fultbtransport t1 on t1.pkid=t5.id
									  left join fultbinfactorybatch t2 on t1.infactorybatchid=t2.id
									  inner join cmcstbtransportposition t6 on t5.id=t6.transportid 
									  where t5.machinecode = '3' and t5.direction='����' and t5.passtime>sysdate-1
									  order by t5.passtime desc,t5.ordernum
										");
			DataTable list = commonDAO.SelfDber.ExecuteDataTable(sql);
			superGridControl.PrimaryGrid.DataSource = list;
		}

		/// <summary>
		/// ����δ������
		/// </summary>
		/// <returns></returns>
		private int Count2()
		{
			int result = 0;
			string sql = string.Format(@"select sum(case when t6.isdischarged=1 then 0 else 1 end) cn
									  from cmcstbtraincarriagepass t5 
									  left join fultbtransport t1 on t1.pkid=t5.id
									  left join fultbinfactorybatch t2 on t1.infactorybatchid=t2.id
									  inner join cmcstbtransportposition t6 on t5.id=t6.transportid 
									  where t5.machinecode = '2' and t5.direction='����' and t5.passtime>sysdate-1
									  order by t5.passtime desc,t5.ordernum
										");
			DataTable list = commonDAO.SelfDber.ExecuteDataTable(sql);
			if (list != null && list.Rows.Count > 0)
			{
				result = Convert.ToInt32(list.Rows[0]["cn"]);
			}

			return result;
		}

		/// <summary>
		/// ����δ������
		/// </summary>
		/// <returns></returns>
		private int Count4()
		{
			int result = 0;
			string sql = string.Format(@"select sum(case when t6.isdischarged=1 then 0 else 1 end) cn
									  from cmcstbtraincarriagepass t5 
									  left join fultbtransport t1 on t1.pkid=t5.id
									  left join fultbinfactorybatch t2 on t1.infactorybatchid=t2.id
									  inner join cmcstbtransportposition t6 on t5.id=t6.transportid 
									  where t5.machinecode = '3' and t5.direction='����' and t5.passtime>sysdate-1
									  order by t5.passtime desc,t5.ordernum
										");
			DataTable list = commonDAO.SelfDber.ExecuteDataTable(sql);
			if (list != null && list.Rows.Count > 0)
			{
				result = Convert.ToInt32(list.Rows[0]["cn"]);
			}

			return result;
		}
	}
}

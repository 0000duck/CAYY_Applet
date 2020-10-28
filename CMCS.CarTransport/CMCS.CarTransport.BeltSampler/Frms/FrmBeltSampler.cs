using CMCS.CarTransport.BeltSampler.Core;
using CMCS.CarTransport.BeltSampler.Enums;
using CMCS.CarTransport.BeltSampler.Frms.Sys;
using CMCS.CarTransport.DAO;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common.Entities.Fuel;
using CMCS.Common.Entities.Inf;
using CMCS.Common.Enums;
using CMCS.Common.Utilities;
using CMCS.Common.Views;
using CMCS.Forms.UserControls;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.SuperGrid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CMCS.CarTransport.BeltSampler.Frms
{
	public partial class FrmBeltSampler : DevComponents.DotNetBar.Metro.MetroForm
	{
		/// <summary>
		/// ����Ψһ��ʶ��
		/// </summary>
		public static string UniqueKey = "FrmBeltSampler";

		public FrmBeltSampler()
		{
			InitializeComponent();
		}

		#region Vars

		CarTransportDAO carTransportDAO = CarTransportDAO.GetInstance();
		BeltSamplerDAO beltSamplerDAO = BeltSamplerDAO.GetInstance();
		CommonDAO commonDAO = CommonDAO.GetInstance();
		RTxtOutputer rTxtOutputer;
		/// <summary>
		/// ��������
		/// </summary>
		//VoiceSpeaker voiceSpeaker = new VoiceSpeaker();

		eFlowFlag currentFlowFlag = eFlowFlag.���ͼƻ�;
		/// <summary>
		/// ��ǰҵ�����̱�ʶ
		/// </summary>
		public eFlowFlag CurrentFlowFlag
		{
			get { return currentFlowFlag; }
			set
			{
				currentFlowFlag = value;
				panCurrentCarNumber.Text = value.ToString();
			}
		}

		/// <summary>
		/// ���������� Ĭ��#1������
		/// </summary>
		string[] sampleMachineCodes = new string[] { GlobalVars.MachineCode_RCPDCYJ_1 };
		string[] jYGMachineCodes = new string[] { "#1�볧Ƥ��������������", "#2�볧Ƥ��������������" };
		/// <summary>
		/// ��ǰѡ�е�Ƥ���������豸
		/// </summary>
		CmcsCMEquipment currentSampleMachine;
		/// <summary>
		/// ��ǰѡ��Ĳ������豸
		/// </summary>
		public CmcsCMEquipment CurrentSampleMachine
		{
			get { return currentSampleMachine; }
			set
			{
				currentSampleMachine = value;
				if (value != null)
				{
					lblCurrSamplerName.Text = value.EquipmentName.Contains("#1") ? "AƤ��������" : "BƤ��������";
				}
			}
		}

		/// <summary>
		/// �������豸����
		/// </summary>
		public string JYGMachineCode = "#1�볧Ƥ��������������";

		View_RCSampling currentRCSampling;
		/// <summary>
		/// ��ǰ������
		/// </summary>
		public View_RCSampling CurrentRCSampling
		{
			get { return currentRCSampling; }
			set
			{
				currentRCSampling = value;
				if (value != null)
				{
					lblBatch.Text = value.Batch;
					lblFactarriveDate.Text = value.FactarriveDate.ToString("yyyy-MM-dd");
					lblSupplierName.Text = value.SupplierName;
					lblMineName.Text = value.MineName;
				}
				else
				{
					lblBatch.Text = "####";
					lblFactarriveDate.Text = "####";
					lblSupplierName.Text = "####";
					lblMineName.Text = "####";
				}
			}
		}

		InfBeltSampleCmd currentSampleCMD;
		/// <summary>
		/// ��ǰ��������
		/// </summary>
		public InfBeltSampleCmd CurrentSampleCMD
		{
			get { return currentSampleCMD; }
			set { currentSampleCMD = value; }
		}

		eEquInfGatherType currentGatherType = eEquInfGatherType.��жʽ;
		/// <summary>
		/// ��ǰж����ʽ Ĭ�ϵ�жʽ
		/// </summary>
		public eEquInfGatherType CurrentGatherType
		{
			get { return currentGatherType; }
			set
			{
				currentGatherType = value;
				lblGatherType.Text = value.ToString();
			}
		}

		//eEquInfCmdResultCode currentCmdResultCode = eEquInfCmdResultCode.Ĭ��;
		///// <summary>
		///// ��ǰ����ִ�н�� 
		///// </summary>
		//public eEquInfCmdResultCode CurrentCmdResultCode
		//{
		//	get { return currentCmdResultCode; }
		//	set
		//	{
		//		currentCmdResultCode = value;

		//		lblResult.Text = currentCmdResultCode.ToString();
		//	}
		//}

		eEquInfCmdResultCode currentJYGCmdResultCode = eEquInfCmdResultCode.Ĭ��;
		/// <summary>
		/// �����޵�ǰ����ִ�н�� 
		/// </summary>
		public eEquInfCmdResultCode CurrentJYGCmdResultCode
		{
			get { return currentJYGCmdResultCode; }
			set
			{
				currentJYGCmdResultCode = value;

				lblResultJYG.Text = currentJYGCmdResultCode.ToString();
			}
		}

		eEquInfSamplerSystemStatus currentSystemStatus = eEquInfSamplerSystemStatus.��������;
		/// <summary>
		/// ��ǰ������ϵͳ״̬
		/// </summary>
		public eEquInfSamplerSystemStatus CurrentSystemStatus
		{
			get { return currentSystemStatus; }
			set
			{
				currentSystemStatus = value;
				lblSampleState.Text = value.ToString();
			}
		}

		/// <summary>
		/// ����������Ƿ�ִ��
		/// </summary>
		bool IsResultSample = false;

		/// <summary>
		/// ����������Ƿ�ִ��
		/// </summary>
		bool IsResultJYG = false;
		#endregion

		/// <summary>
		/// �����ʼ��
		/// </summary>
		private void InitForm()
		{
			superGridControl1.PrimaryGrid.AutoGenerateColumns = false;
			superGridControl2.PrimaryGrid.AutoGenerateColumns = false;
			//��SuperGridControl�¼� gclmSetSampler
			GridButtonXEditControl btnSetSampler = superGridControl2.PrimaryGrid.Columns["gclmSetSampler"].EditControl as GridButtonXEditControl;
			btnSetSampler.Click += btnSetSampler_Click;

			// �������豸���룬����������һһ��Ӧ
			sampleMachineCodes = CommonDAO.GetInstance().GetCommonAppletConfigString("�볧�������豸����").Split('|');

			// ���ó���Զ�̿�������
			commonDAO.ResetAppRemoteControlCmd(CommonAppConfig.GetInstance().AppIdentifier);
		}

		private void FrmCarSampler_Load(object sender, EventArgs e)
		{
			this.rTxtOutputer = new RTxtOutputer(rtxtOutput);
		}

		private void FrmCarSampler_Shown(object sender, EventArgs e)
		{
			InitForm();

			CreateSamplerButton();
			CreateEquStatus();
			BindRCSampling(superGridControl2);
			// ������һ����ť
			if (lypanSamplerButton.Controls.Count > 0) (lypanSamplerButton.Controls[0] as RadioButton).Checked = true;
			SetGatherType();
		}

		private void FrmCarSampler_FormClosing(object sender, FormClosingEventArgs e)
		{

		}

		#region ����ҵ��
		/// <summary>
		/// ����������ʶ������
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Stop();
			timer1.Interval = 2000;

			try
			{
				switch (this.CurrentFlowFlag)
				{
					case eFlowFlag.�ȴ�ִ��:

						if (!IsResultSample)
						{
							CurrentCmdResultCode = beltSamplerDAO.GetSampleCmdResult(CurrentSampleCMD.Id);
							if (CurrentCmdResultCode == eEquInfCmdResultCode.�ɹ�)
							{
								this.rTxtOutputer.Output("��������ִ�гɹ�", eOutputType.Success);
							}
							else if (CurrentCmdResultCode == eEquInfCmdResultCode.ʧ��)
							{
								this.rTxtOutputer.Output("��������ִ��ʧ��", eOutputType.Warn);
								List<InfEquInfHitch> list_Hitch = commonDAO.GetWarnEquInfHitch(DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(1), this.CurrentSampleMachine.EquipmentCode);
								foreach (InfEquInfHitch item in list_Hitch)
								{
									this.rTxtOutputer.Output("������:" + item.HitchDescribe, eOutputType.Error);
								}
							}
							IsResultSample = CurrentCmdResultCode != eEquInfCmdResultCode.Ĭ��;
						}

						break;
					case eFlowFlag.ִ�����:
						ResetBuyFuel();
						break;
				}
			}
			catch (Exception ex)
			{
				Log4Neter.Error("timer1_Tick", ex);
			}
			finally
			{
				timer1.Start();
			}

			timer1.Start();
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			timer2.Stop();
			// 2��ִ��һ��
			timer2.Interval = 2000;

			try
			{
				RefreshEquStatus();
				BindBeltSampleBarrel(superGridControl1, this.JYGMachineCode);
				BindRCSampling(superGridControl2);

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

		/// <summary>
		/// ��ʼ����
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnStartSampler_Click(object sender, EventArgs e)
		{
			if (this.CurrentFlowFlag == eFlowFlag.�ȴ�ִ��)
			{ MessageBoxEx.Show("�ȴ���ǰ����ִ�����"); return; }
			if (CurrentRCSampling == null) { MessageBoxEx.Show("�������õ�ǰ������"); return; }

			string lczt = CommonDAO.GetInstance().GetSignalDataValue("�볧ת����", eSignalDataName.ת���ߴ�����.ToString());
			if (lczt != "����")
			{
				{ MessageBoxEx.Show("�豸ת���ߴ�����δ����Ϊ������"); return; }
			}

			string hgfs = CommonDAO.GetInstance().GetSignalDataValue(this.JYGMachineCode, eSignalDataName.���޷�ʽ.ToString());
			if (hgfs != "�����")
			{
				{ MessageBoxEx.Show("�豸���޷�ʽδ����Ϊ�������"); return; }
			}

			//string systemStatus = CommonDAO.GetInstance().GetSignalDataValue(this.JYGMachineCode, eSignalDataName.�豸״̬.ToString());
			//if (systemStatus != eEquInfSamplerSystemStatus.��������.ToString())
			//{
			//	{ MessageBoxEx.Show("������δ����"); return; }
			//}

			if (!SendSamplingCMD(true)) { MessageBoxEx.Show("���������ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

			MessageBoxEx.Show("����ͳɹ����ȴ�ִ��");
			timer1.Enabled = true;
			this.CurrentFlowFlag = eFlowFlag.�ȴ�ִ��;
		}

		/// <summary>
		/// ���õ�ǰ������
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSetSampler_Click(object sender, EventArgs e)
		{
			GridRow gridRow = (superGridControl2.PrimaryGrid.ActiveRow as GridRow);
			if (gridRow == null) return;

			if (MessageBoxEx.Show("�Ƿ����øü�¼Ϊ��ǰ������", "������ʾ", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
				CurrentRCSampling = gridRow.DataItem as View_RCSampling;
		}
		#endregion

		#region �볧ú����ҵ��
		/// <summary>
		/// �����볧ú�����¼
		/// </summary>
		/// <returns></returns>
		bool SendSamplingPlan()
		{
			InfBeltSamplePlan oldBeltSamplePlan = Dbers.GetInstance().SelfDber.Entity<InfBeltSamplePlan>("where InFactoryBatchId=:InFactoryBatchId and SampleCode=:SampleCode", new { InFactoryBatchId = this.CurrentRCSampling.BatchId, SampleCode = this.CurrentRCSampling.SampleCode });
			if (oldBeltSamplePlan == null)
			{
				return Dbers.GetInstance().SelfDber.Insert<InfBeltSamplePlan>(new InfBeltSamplePlan
				{
					DataFlag = 0,
					InterfaceType = this.CurrentSampleMachine.InterfaceType,
					InFactoryBatchId = this.CurrentRCSampling.BatchId,
					SampleCode = this.CurrentRCSampling.SampleCode,
					FuelKindName = this.CurrentRCSampling.FuelName,
					CarCount = 0,
					Mt = 0,
					TicketWeight = 0,
					GatherType = CurrentGatherType.ToString(),
					SampleType = "Ƥ������"
				}) > 0;
			}
			else
			{
				oldBeltSamplePlan.DataFlag = 0;
				oldBeltSamplePlan.FuelKindName = this.CurrentRCSampling.FuelName;
				oldBeltSamplePlan.CarCount = 0;
				oldBeltSamplePlan.Mt = 0;
				oldBeltSamplePlan.TicketWeight = 0;
				oldBeltSamplePlan.GatherType = CurrentGatherType.ToString();

				return Dbers.GetInstance().SelfDber.Update(oldBeltSamplePlan) > 0;
			}
		}

		/// <summary>
		/// ���Ϳ�ʼ��������
		/// </summary>
		/// <returns></returns>
		bool SendSamplingCMD(bool isStart)
		{
			string barrelNumber = string.Empty;
			if (beltSamplerDAO.CheckIsUnFinishCmd(this.JYGMachineCode, this.CurrentRCSampling.SampleCode, 5))
			{
				this.rTxtOutputer.Output("�ò��������5��������δִ�е�������Ժ��ٷ�������", eOutputType.Warn);
				return false;
			}
			InfCYSampleCMD SampleCmd_Old = Dbers.GetInstance().SelfDber.Entity<InfCYSampleCMD>("where MachineCode=:MachineCode and SampleCode=:SampleCode order by CreationTime", new { MachineCode = this.JYGMachineCode, SampleCode = this.CurrentRCSampling.SampleCode });
			if (SampleCmd_Old != null)
			{
				if (!beltSamplerDAO.CheckBarrelNumberIsFull(SampleCmd_Old.BarrelNumber.ToString(), SampleCmd_Old.MachineCode))
				{
					this.rTxtOutputer.Output("��Ͱ������������", eOutputType.Warn);
					return false;
				}
				else
					barrelNumber = SampleCmd_Old.BarrelNumber.ToString();
			}
			else
				barrelNumber = beltSamplerDAO.GetBarrelNumberBySampelCode(this.CurrentRCSampling.SampleCode, this.JYGMachineCode);
			if (string.IsNullOrEmpty(barrelNumber))
			{
				this.rTxtOutputer.Output("����Ͱ�ɷ���", eOutputType.Warn);
				return false;
			}
			this.CurrentJYGCMD = new InfCYSampleCMD();
			this.CurrentJYGCMD.MachineCode = this.JYGMachineCode;
			this.CurrentJYGCMD.SampleCode = this.CurrentRCSampling.SampleCode;
			this.CurrentJYGCMD.BarrelNumber = Convert.ToInt32(barrelNumber);
			this.CurrentJYGCMD.ResultCode = eEquInfCmdResultCode.Ĭ��.ToString();
			this.CurrentJYGCMD.DataFlag = 0;
			this.CurrentJYGCMD.CmdCode = "1";
			// ���ͼ�������
			if (commonDAO.SelfDber.Insert<InfCYSampleCMD>(CurrentJYGCMD) > 0)
			{
				//this.rTxtOutputer.Output("��������ͳɹ�,���޺�:" + barrelNumber);
				this.rTxtOutputer.Output(MachineCodeToShow(this.JYGMachineCode) + "��������ͳɹ�");
				//д��ʵʱ�ź�
				commonDAO.SetSignalDataValue(this.JYGMachineCode, "��������", this.CurrentRCSampling.SampleCode);
				commonDAO.SetSignalDataValue(this.JYGMachineCode, "��ȡ��ʼʱ��", DateTime.Now.ToShortTimeString());
				CmcsInFactoryBatch batch = Dbers.GetInstance().SelfDber.Entity<CmcsInFactoryBatch>("where Id=:Id", new { Id = this.CurrentRCSampling.BatchId });
				if (batch != null)
				{
					commonDAO.SetSignalDataValue(this.JYGMachineCode, "����", batch.TicketQty.ToString());
					commonDAO.SetSignalDataValue(this.JYGMachineCode, "��ú����", batch.TransportNumber.ToString());
				}

				return true;
			}
			else
			{
				//this.rTxtOutputer.Output("���������ʧ��,���޺�:" + barrelNumber);
				this.rTxtOutputer.Output(MachineCodeToShow(this.JYGMachineCode) + "���������ʧ��");
				return false;
			}

			//CurrentSampleCMD = new InfBeltSampleCmd
			//{
			//	DataFlag = 0,
			//	InterfaceType = this.CurrentSampleMachine.InterfaceType,
			//	MachineCode = this.CurrentSampleMachine.EquipmentCode,
			//	ResultCode = eEquInfCmdResultCode.Ĭ��.ToString(),
			//	SampleCode = this.CurrentRCSampling.SampleCode,
			//	//BarrelNumber =Convert.ToInt32(barrelNumber),
			//	CmdCode = (isStart == true ? eEquInfSamplerCmd.��ʼ����.ToString() : eEquInfSamplerCmd.��������.ToString())
			//};
			//if (Dbers.GetInstance().SelfDber.Insert<InfBeltSampleCmd>(CurrentSampleCMD) > 0)
			//{
			//	this.rTxtOutputer.Output("��������ͳɹ�");
			//	return true;
			//}
			//return false;
		}

		/// <summary>
		/// �����볧ú�����¼
		/// </summary>
		void ResetBuyFuel()
		{
			this.CurrentFlowFlag = eFlowFlag.ѡ��ƻ�;
			//this.CurrentCmdResultCode = eEquInfCmdResultCode.Ĭ��;
			this.CurrentJYGCmdResultCode = eEquInfCmdResultCode.Ĭ��;
			this.CurrentSampleCMD = null;
			this.CurrentRCSampling = null;
			IsResultJYG = false;
			IsResultSample = false;
		}

		/// <summary>
		/// ����
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnReset_Click(object sender, EventArgs e)
		{
			ResetBuyFuel();
		}

		#endregion

		#region �ź�ҵ��
		/// <summary>
		/// ����Ƥ����������������״̬
		/// </summary>
		private void CreateEquStatus()
		{
			flpanEquState.SuspendLayout();

			foreach (string cMEquipmentCode in sampleMachineCodes)
			{
				LabelX lblMachineName = new LabelX()
				{
					Text = cMEquipmentCode.Contains("#1") ? "AƤ��������" : "BƤ��������",
					AutoSize = true,
					Anchor = AnchorStyles.Left,
					Font = new Font("Segoe UI", 14.25f, FontStyle.Bold)
				};

				flpanEquState.Controls.Add(lblMachineName);

				LabelX uCtrlSignalLight = new LabelX()
				{
					Tag = cMEquipmentCode,
					AutoSize = true,
					Anchor = AnchorStyles.Left,
					Font = new Font("Segoe UI", 14.25f, FontStyle.Bold),
					Padding = new System.Windows.Forms.Padding(10, 0, 0, 0)
				};
				SetSystemStatusToolTip(uCtrlSignalLight);
				flpanEquState.Controls.Add(uCtrlSignalLight);
			}

			foreach (string cMEquipmentCode in jYGMachineCodes)
			{
				LabelX lblMachineName = new LabelX()
				{
					Text = cMEquipmentCode.Contains("#1") ? "A�볧Ƥ��������������" : "B�볧Ƥ��������������",
					AutoSize = true,
					Anchor = AnchorStyles.Left,
					Font = new Font("Segoe UI", 14.25f, FontStyle.Bold)
				};

				flpanEquState.Controls.Add(lblMachineName);

				LabelX uCtrlSignalLight = new LabelX()
				{
					Tag = cMEquipmentCode,
					AutoSize = true,
					Anchor = AnchorStyles.Left,
					Font = new Font("Segoe UI", 14.25f, FontStyle.Bold),
					Padding = new System.Windows.Forms.Padding(10, 0, 0, 0)
				};
				SetSystemStatusToolTip(uCtrlSignalLight);
				flpanEquState.Controls.Add(uCtrlSignalLight);
			}

			flpanEquState.ResumeLayout();
			if (this.flpanEquState.Controls.Count == 0)
				MessageBoxEx.Show("Ƥ��������������������δ���ã�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		/// <summary>
		/// ����Ƥ��������״̬
		/// </summary>
		private void RefreshEquStatus()
		{
			foreach (LabelX uCtrlSignalLight in flpanEquState.Controls.OfType<LabelX>())
			{
				if (uCtrlSignalLight.Tag == null) continue;

				string machineCode = uCtrlSignalLight.Tag.ToString();
				if (string.IsNullOrEmpty(machineCode)) continue;
				if (machineCode.Contains("������"))
				{
					string systemStatus = CommonDAO.GetInstance().GetSignalDataValue(machineCode, eSignalDataName.ϵͳ����.ToString());
					uCtrlSignalLight.Text = systemStatus;
					if (systemStatus == "����")
					{
						uCtrlSignalLight.BackColor = EquipmentStatusColors.BeReady;
					}
					else
					{
						uCtrlSignalLight.BackColor = EquipmentStatusColors.Breakdown;
					}
				}
				else
				{
					string systemStatus = CommonDAO.GetInstance().GetSignalDataValue(machineCode, eSignalDataName.�豸״̬.ToString());
					uCtrlSignalLight.Text = systemStatus;
					if (systemStatus == eEquInfSamplerSystemStatus.��������.ToString())
						uCtrlSignalLight.BackColor = EquipmentStatusColors.BeReady;
					else if (systemStatus == eEquInfSamplerSystemStatus.��������.ToString() || systemStatus == eEquInfSamplerSystemStatus.����ж��.ToString())
						uCtrlSignalLight.BackColor = EquipmentStatusColors.Working;
					else if (systemStatus == eEquInfSamplerSystemStatus.��������.ToString())
						uCtrlSignalLight.BackColor = EquipmentStatusColors.Breakdown;
					else if (systemStatus == eEquInfSamplerSystemStatus.��Ͱ����.ToString())
						uCtrlSignalLight.BackColor = EquipmentStatusColors.Full;
					else if (systemStatus == eEquInfSamplerSystemStatus.ϵͳֹͣ.ToString())
						uCtrlSignalLight.BackColor = EquipmentStatusColors.Forbidden;

					eEquInfSamplerSystemStatus status;
					//��ǰѡ��Ĳ�����״̬
					if (machineCode == CurrentSampleMachine.EquipmentCode)
						if (Enum.TryParse(systemStatus, out status))
							CurrentSystemStatus = status;
				}

			}
		}

		/// <summary>
		/// ����ToolTip��ʾ
		/// </summary>
		private void SetSystemStatusToolTip(Control control)
		{
			this.toolTip1.SetToolTip(control, "<��ɫ> ��������\r\n<��ɫ> ��������\r\n<��ɫ> ��������\r\n<��ɫ> ϵͳֹͣ");
		}

		private void SetGatherType()
		{
			eEquInfGatherType GatherType;
			if (currentSampleMachine.EquipmentCode == "#1Ƥ��������")
			{
				if (Enum.TryParse(CommonDAO.GetInstance().GetAppletConfigString("��������", "#1Ƥ��������������ʽ"), out GatherType))
					CurrentGatherType = GatherType;
			}
			else
				if (Enum.TryParse(CommonDAO.GetInstance().GetAppletConfigString("��������", "#2Ƥ��������������ʽ"), out GatherType))
				CurrentGatherType = GatherType;
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

		/// <summary>
		/// ���ɲ�����ѡ��
		/// </summary>
		private void CreateSamplerButton()
		{
			foreach (string machineCode in sampleMachineCodes)
			{
				CmcsCMEquipment Equipment = CommonDAO.GetInstance().GetCMEquipmentByMachineCode(machineCode);
				RadioButton rbtnSampler = new RadioButton();
				rbtnSampler.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
				rbtnSampler.Text = Equipment.EquipmentName.Contains("#1") ? "AƤ��������" : "BƤ��������";
				rbtnSampler.Tag = Equipment;
				rbtnSampler.AutoSize = true;
				rbtnSampler.Padding = new System.Windows.Forms.Padding(10, 0, 0, 10);
				rbtnSampler.CheckedChanged += new EventHandler(rbtnSampler_CheckedChanged);

				lypanSamplerButton.Controls.Add(rbtnSampler);
			}
		}

		void rbtnSampler_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton rbtnSampler = sender as RadioButton;
			this.CurrentSampleMachine = rbtnSampler.Tag as CmcsCMEquipment;
			JYGMachineCode = this.CurrentSampleMachine.EquipmentCode.Contains("1") ? "#1�볧Ƥ��������������" : "#2�볧Ƥ��������������";
			BindBeltSampleBarrel(superGridControl1, this.JYGMachineCode);
		}

		#endregion

		/// <summary>
		/// �󶨼�������Ϣ
		/// </summary>
		/// <param name="superGridControl"></param>
		/// <param name="machineCode">�豸����</param>
		private void BindBeltSampleBarrel(SuperGridControl superGridControl, string machineCode)
		{
			IList<InfEquInfSampleBarrel> list = CommonDAO.GetInstance().GetEquInfSampleBarrels(machineCode);
			superGridControl.PrimaryGrid.DataSource = list;
		}

		private void BindRCSampling(SuperGridControl superGridControl)
		{
			List<View_RCSampling> list = beltSamplerDAO.GetViewRCSampling(string.Format("where BatchType='��' and SamplingWay!='�˹�����' and to_char(SamplingDate,'yyyy-MM-dd hh24:mm:ss')>='{0}' order by SamplingDate desc", DateTime.Now.AddDays(-3).Date.ToString("yyyy-MM-dd")));
			superGridControl.PrimaryGrid.DataSource = list;
		}

		/// <summary>
		/// ת��Ϊ��ʾ����
		/// </summary>
		/// <param name="machineCode"></param>
		/// <returns></returns>
		public string MachineCodeToShow(string machineCode)
		{

			if (machineCode.Contains("#1"))
			{
				return "A�볧Ƥ��������������";
			}
			else
			{
				return "B�볧Ƥ��������������";
			}

		}
	}
}

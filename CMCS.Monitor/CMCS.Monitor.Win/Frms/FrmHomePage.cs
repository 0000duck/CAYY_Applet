using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Enums;
using CMCS.Monitor.Win.Core;
using CMCS.Monitor.Win.Html;
using CMCS.Monitor.Win.UserControls;
using DevComponents.DotNetBar;
using Xilium.CefGlue;
using Xilium.CefGlue.WindowsForms;
using CMCS.Monitor.Win.Utilities;
using CMCS.Monitor.Win.CefGlue;
using CMCS.Common.Entities.Fuel;

namespace CMCS.Monitor.Win.Frms
{
	public partial class FrmHomePage : DevComponents.DotNetBar.Metro.MetroForm
	{
		/// <summary>
		/// ����Ψһ��ʶ��
		/// </summary>
		public static string UniqueKey = "FrmHomePage";

		CommonDAO commonDAO = CommonDAO.GetInstance();
		MonitorCommon monitorCommon = MonitorCommon.GetInstance();

		CefWebBrowserEx cefWebBrowser = new CefWebBrowserEx();

		public FrmHomePage()
		{
			InitializeComponent();
		}

		/// <summary>
		/// �����ʼ��
		/// </summary>
		private void FormInit()
		{
#if DEBUG
			gboxTest.Visible = true;
#else
            gboxTest.Visible = false;
#endif
			cefWebBrowser.StartUrl = SelfVars.Url_HomePage;
			cefWebBrowser.Dock = DockStyle.Fill;
			cefWebBrowser.WebClient = new HomePageCefWebClient(cefWebBrowser);
			cefWebBrowser.LoadEnd += new EventHandler<LoadEndEventArgs>(cefWebBrowser_LoadEnd);
			panWebBrower.Controls.Add(cefWebBrowser);
		}

		void cefWebBrowser_LoadEnd(object sender, LoadEndEventArgs e)
		{
			timer1.Enabled = true;

			RequestData();
		}

		private void FrmHomePage_Load(object sender, EventArgs e)
		{
			FormInit();
		}

		/// <summary>
		/// ���� - ˢ��ҳ��
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRefresh_Click(object sender, EventArgs e)
		{
			cefWebBrowser.Browser.Reload();
		}

		/// <summary>
		/// ���� - ����ˢ��
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRequestData_Click(object sender, EventArgs e)
		{
			RequestData();
		}

		/// <summary>
		/// ��������
		/// </summary>
		void RequestData()
		{
			string value = string.Empty, machineCode = string.Empty;
			List<HtmlDataItem> datas = new List<HtmlDataItem>();

			datas.Clear();
			datas.Add(new HtmlDataItem("���볧����", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, eSignalDataName.���볧����.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("�𳵷�������", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, eSignalDataName.�𳵷�������.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("�𳵳�������", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, eSignalDataName.�𳵳�������.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("#1�������ѷ�����", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_TrunOver_1, eSignalDataName.�ѷ�����.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("#2�������ѷ�����", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_TrunOver_2, eSignalDataName.�ѷ�����.ToString()), eHtmlDataItemType.svg_text));
			string CarNumber_1 = commonDAO.GetSignalDataValue(GlobalVars.MachineCode_TrunOver_1, eSignalDataName.��ǰ����.ToString());
			string CarNumber_2 = commonDAO.GetSignalDataValue(GlobalVars.MachineCode_TrunOver_2, eSignalDataName.��ǰ����.ToString());
			datas.Add(new HtmlDataItem("#1��������ǰ����", CarNumber_1, eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("#2��������ǰ����", CarNumber_2, eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("#1������", monitorCommon.ConvertBooleanToColor(string.IsNullOrEmpty(commonDAO.GetSignalDataValue(GlobalVars.MachineCode_TrunOver_1, eSignalDataName.��ǰ����.ToString())) ? "0" : "1"), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("#2������", monitorCommon.ConvertBooleanToColor(string.IsNullOrEmpty(commonDAO.GetSignalDataValue(GlobalVars.MachineCode_TrunOver_2, eSignalDataName.��ǰ����.ToString())) ? "0" : "1"), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("#4����ʶ��", monitorCommon.ConvertBooleanToColor(string.IsNullOrEmpty(commonDAO.GetSignalDataValue(GlobalVars.MachineCode_TrunOver_1, eSignalDataName.��ǰ����.ToString())) ? "0" : "1"), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("#5����ʶ��", monitorCommon.ConvertBooleanToColor(string.IsNullOrEmpty(commonDAO.GetSignalDataValue(GlobalVars.MachineCode_TrunOver_2, eSignalDataName.��ǰ����.ToString())) ? "0" : "1"), eHtmlDataItemType.svg_color));

			datas.Add(new HtmlDataItem("������", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, eSignalDataName.������.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("����ת�˳���", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, eSignalDataName.����ת�˳���.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("�����볧����", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, eSignalDataName.�����볧����.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("�����������س���", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, eSignalDataName.�����������س���.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("������Ƥ����", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, eSignalDataName.������Ƥ����.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("������������", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, eSignalDataName.������������.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("����������", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, eSignalDataName.����������.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("������", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, eSignalDataName.������.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("��������", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, eSignalDataName.��������.ToString()), eHtmlDataItemType.svg_text));

			datas.Add(new HtmlDataItem("������_��������", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "������_��������"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("������_��������", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "������_��������"), eHtmlDataItemType.svg_text));

			#region ���� ���ƻ�
			if (!string.IsNullOrEmpty(CarNumber_1))
			{
				BindBatch(CarNumber_1, "#2", datas);
			}
			if (!string.IsNullOrEmpty(CarNumber_1))
			{
				BindBatch(CarNumber_2, "#4", datas);
			}
			#endregion

			#region ����������

			machineCode = GlobalVars.MachineCode_QC_JxSampler_1;
			datas.Add(new HtmlDataItem("����_1�Ų���_ϵͳ", monitorCommon.ConvertMachineStatusToColor(commonDAO.GetSignalDataValue(GlobalVars.MachineCode_QCJXCYJ_1, eSignalDataName.ϵͳ.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("����_1�Ų���_����", commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��ǰ����.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("����_1�Ų���_��բ1", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��բ1����.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("����_1�Ų���_��բ2", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��բ2����.ToString())), eHtmlDataItemType.svg_color));

			machineCode = GlobalVars.MachineCode_QC_JxSampler_2;
			datas.Add(new HtmlDataItem("����_2�Ų���_ϵͳ", monitorCommon.ConvertMachineStatusToColor(commonDAO.GetSignalDataValue(GlobalVars.MachineCode_QCJXCYJ_2, eSignalDataName.ϵͳ.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("����_2�Ų���_����", commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��ǰ����.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("����_2�Ų���_��բ1", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��բ1����.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("����_2�Ų���_��բ2", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��բ2����.ToString())), eHtmlDataItemType.svg_color));

			#endregion

			#region ������

			machineCode = GlobalVars.MachineCode_QC_Weighter_1;
			datas.Add(new HtmlDataItem("#1��ϵͳ", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.ϵͳ.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("#1����ǰ����", commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��ǰ����.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("#1����ǰ����", commonDAO.GetSignalDataValue(machineCode, eSignalDataName.�ذ��Ǳ�_ʵʱ����.ToString() + "t"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("����_1�ź�_��բ1", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��բ1����.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("����_1�ź�_��բ2", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��բ2����.ToString())), eHtmlDataItemType.svg_color));
			AddDataItemBySignal(datas, machineCode, "����_1�ź�_���̵�");

			machineCode = GlobalVars.MachineCode_QC_Weighter_2;
			datas.Add(new HtmlDataItem("#2��ϵͳ", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.ϵͳ.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("#2����ǰ����", commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��ǰ����.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("#2����ǰ����", commonDAO.GetSignalDataValue(machineCode, eSignalDataName.�ذ��Ǳ�_ʵʱ����.ToString() + "t"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("����_2�ź�_��բ1", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��բ1����.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("����_2�ź�_��բ2", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��բ2����.ToString())), eHtmlDataItemType.svg_color));
			AddDataItemBySignal(datas, machineCode, "����_2�ź�_���̵�");

			machineCode = GlobalVars.MachineCode_QC_Weighter_3;
			datas.Add(new HtmlDataItem("#3��ϵͳ", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.ϵͳ.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("#3����ǰ����", commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��ǰ����.ToString()), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("#3����ǰ����", commonDAO.GetSignalDataValue(machineCode, eSignalDataName.�ذ��Ǳ�_ʵʱ����.ToString() + "t"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("����_3�ź�_��բ1", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��բ1����.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("����_3�ź�_��բ2", monitorCommon.ConvertBooleanToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.��բ2����.ToString())), eHtmlDataItemType.svg_color));
			AddDataItemBySignal(datas, machineCode, "����_3�ź�_���̵�");

			#endregion

			datas.Add(new HtmlDataItem("�Ž�_�����ҽ�", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "�Ž�_�����ҽ�"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("�Ž�_�����ҽ�", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "�Ž�_�����ҽ�"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("�Ž�_�����ҽ�", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "�Ž�_�����ҽ�"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("�Ž�_�칫¥��", commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "�Ž�_�칫¥��"), eHtmlDataItemType.svg_text));

			// ��Ӹ���...

			// ���͵�ҳ��
			cefWebBrowser.Browser.GetMainFrame().ExecuteJavaScript("requestData(" + Newtonsoft.Json.JsonConvert.SerializeObject(datas) + ");", "", 0);
		}

		public void BindBatch(string carnumber, string flag, List<HtmlDataItem> datas)
		{
			DataTable data = commonDAO.SelfDber.ExecuteDataTable(string.Format(@"select t.infactorybatchid,a.batch,b.samplecode,c.makecode,d.assaycode,a.fueltype,a.transportnumber,a.fuelkindname,a.minename,a.ticketqty,a.suttleqty 
																					from fultbtransport t inner join fultbinfactorybatch a on t.infactorybatchid=a.id inner join cmcstbrcsampling b on a.id=b.infactorybatchid inner join 
																					cmcstbmake c on c.samplingid=b.id inner join cmcstbassay d on d.makeid=c.id where trunc(a.factarrivedate)=trunc(sysdate) and t.transportno='{0}' and b.samplingtype!='�˹�����'", carnumber));
			if (data != null && data.Rows.Count > 0)
			{
				datas.Add(new HtmlDataItem(flag + "���α��", data.Rows[0]["batch"].ToString(), eHtmlDataItemType.svg_text));
				datas.Add(new HtmlDataItem(flag + "��������", data.Rows[0]["samplecode"].ToString(), eHtmlDataItemType.svg_text));
				datas.Add(new HtmlDataItem(flag + "��������", data.Rows[0]["makecode"].ToString(), eHtmlDataItemType.svg_text));
				datas.Add(new HtmlDataItem(flag + "�������", data.Rows[0]["assaycode"].ToString(), eHtmlDataItemType.svg_text));
				datas.Add(new HtmlDataItem(flag + "��ú��ʽ", data.Rows[0]["fueltype"].ToString(), eHtmlDataItemType.svg_text));
				datas.Add(new HtmlDataItem(flag + "��ú����", data.Rows[0]["transportnumber"].ToString(), eHtmlDataItemType.svg_text));
				datas.Add(new HtmlDataItem(flag + "ú��", data.Rows[0]["fuelkindname"].ToString(), eHtmlDataItemType.svg_text));
				datas.Add(new HtmlDataItem(flag + "���", data.Rows[0]["minename"].ToString(), eHtmlDataItemType.svg_text));
				datas.Add(new HtmlDataItem(flag + "����", data.Rows[0]["ticketqty"].ToString(), eHtmlDataItemType.svg_text));
				//datas.Add(new HtmlDataItem(flag + "����", data.Rows[0]["suttleqty"].ToString(), eHtmlDataItemType.svg_text));
				IList<CmcsTransport> list = commonDAO.SelfDber.Entities<CmcsTransport>("where InFactoryBatchId=:InFactoryBatchId", new { InFactoryBatchId = data.Rows[0]["infactorybatchid"] });
				if (list != null)
				{
					datas.Add(new HtmlDataItem(flag + "ë��", list.Sum(a => a.GrossQty).ToString(), eHtmlDataItemType.svg_text));
					datas.Add(new HtmlDataItem(flag + "Ƥ��", list.Sum(a => a.SkinQty).ToString(), eHtmlDataItemType.svg_text));
					datas.Add(new HtmlDataItem(flag + "����", list.Sum(a => a.SuttleQty).ToString(), eHtmlDataItemType.svg_text));
				}
			}

		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			// ���治�ɼ�ʱ��ֹͣ��������
			if (!this.Visible) return;

			RequestData();
		}

		/// <summary>
		/// ��Ӻ��̵ƿ����ź�
		/// </summary>
		/// <param name="datas"></param>
		/// <param name="machineCode"></param>
		/// <param name="signalValue"></param>
		private void AddDataItemBySignal(List<HtmlDataItem> datas, string machineCode, string signalValue)
		{
			if (commonDAO.GetSignalDataValue(machineCode, eSignalDataName.�źŵ�1.ToString()) == "1")
			{
				//���
				datas.Add(new HtmlDataItem(signalValue + "_��", "#FF0000", eHtmlDataItemType.svg_color));
				datas.Add(new HtmlDataItem(signalValue + "_��", "#CCCCCC", eHtmlDataItemType.svg_color));
			}
			else
			{
				//�̵�
				datas.Add(new HtmlDataItem(signalValue + "_��", "#CCCCCC", eHtmlDataItemType.svg_color));
				datas.Add(new HtmlDataItem(signalValue + "_��", "#00FF00", eHtmlDataItemType.svg_color));
			}
		}

	}

	public class HomePageCefWebClient : CefWebClient
	{
		CefWebBrowser cefWebBrowser;

		public HomePageCefWebClient(CefWebBrowser cefWebBrowser)
			: base(cefWebBrowser)
		{
			this.cefWebBrowser = cefWebBrowser;
		}

		protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
		{
			if (message.Name == "OpenTruckWeighter")
				SelfVars.MainFrameForm.OpenTruckWeighter();
			else if (message.Name == "TruckWeighterChangeSelected")
				SelfVars.TruckWeighterForm.CurrentMachineCode = MonitorCommon.GetInstance().GetTruckWeighterMachineCodeBySelected(message.Arguments.GetString(0));
			else if (message.Name == "CarSamplerChangeSelected")
				SelfVars.CarSamplerForm.CurrentMachineCode = MonitorCommon.GetInstance().GetCarSamplerMachineCodeBySelected(message.Arguments.GetString(0));
			else if (message.Name == "TrainSamplerChangeSelected")
				SelfVars.TrainSamplerForm.CurrentMachineCode = message.Arguments.GetString(0);

			return true;
		}

		protected override CefContextMenuHandler GetContextMenuHandler()
		{
			return new CefMenuHandler();
		}
	}
}
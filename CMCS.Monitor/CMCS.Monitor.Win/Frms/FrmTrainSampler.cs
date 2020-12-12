using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Enums;
using CMCS.Monitor.Win.Core;
using CMCS.Monitor.Win.Html;
using CMCS.Monitor.Win.UserControls;
using CMCS.Monitor.Win.Utilities;
using DevComponents.DotNetBar;
using Xilium.CefGlue.WindowsForms;

namespace CMCS.Monitor.Win.Frms
{
	public partial class FrmTrainSampler : DevComponents.DotNetBar.Metro.MetroForm
	{
		/// <summary>
		/// ����Ψһ��ʶ��
		/// </summary>
		public static string UniqueKey = "FrmTrainSampler";

		CommonDAO commonDAO = CommonDAO.GetInstance();
		MonitorCommon monitorCommon = MonitorCommon.GetInstance();

		CefWebBrowserEx cefWebBrowser = new CefWebBrowserEx();

		string currentMachineCode = GlobalVars.MachineCode_HCJXCYJ_1;
		/// <summary>
		/// ��ǰѡ�е��豸
		/// </summary>
		public string CurrentMachineCode
		{
			get { return currentMachineCode; }
			set { currentMachineCode = value; }
		}

		public FrmTrainSampler()
		{
			InitializeComponent();
		}

		private void FrmTruckWeighter_Load(object sender, EventArgs e)
		{
			FormInit();
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

			cefWebBrowser.StartUrl = SelfVars.Url_TrainSampler;
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

			machineCode = this.CurrentMachineCode;

			datas.Add(new HtmlDataItem("������_��ǰ����", machineCode, eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("#1������״̬", monitorCommon.ConvertMachineStatusToColor(commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HCJXCYJ_1, eSignalDataName.�豸״̬.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("#2������״̬", monitorCommon.ConvertMachineStatusToColor(commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HCJXCYJ_2, eSignalDataName.�豸״̬.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("#3������״̬", monitorCommon.ConvertMachineStatusToColor(commonDAO.GetSignalDataValue(GlobalVars.MachineCode_HCJXCYJ_3, eSignalDataName.�豸״̬.ToString())), eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("��ǰ�豸״̬", monitorCommon.ConvertMachineStatusToColor(commonDAO.GetSignalDataValue(machineCode, eSignalDataName.�豸״̬.ToString())), eHtmlDataItemType.svg_color));

			datas.Add(new HtmlDataItem("������", commonDAO.GetSignalDataValue(machineCode, "������"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("����", commonDAO.GetSignalDataValue(machineCode, "����"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("��ʼʱ��", commonDAO.GetSignalDataValue(machineCode, "��ʼʱ��"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("��ú����", commonDAO.GetSignalDataValue(machineCode, "��ú����"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("��������", commonDAO.GetSignalDataValue(machineCode, "��������"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("��ǰ����", commonDAO.GetSignalDataValue(machineCode, "��ǰ����"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("2������ǣ��", commonDAO.GetSignalDataValue(machineCode, "2������ǣ��"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("4������ǣ��", commonDAO.GetSignalDataValue(machineCode, "4������ǣ��"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("�϶�", monitorCommon.ConvertStatusToColor(commonDAO.GetSignalDataValue(machineCode, "�϶�")), eHtmlDataItemType.svg_color));
			string point = commonDAO.GetSignalDataValue(machineCode, "ʵʱ����");
			if (!string.IsNullOrEmpty(point))
			{
				string[] points = point.Split(',');
				if (points.Length == 3)
				{
					datas.Add(new HtmlDataItem("������", points[0], eHtmlDataItemType.svg_text));
					datas.Add(new HtmlDataItem("С������", points[1], eHtmlDataItemType.svg_text));
					datas.Add(new HtmlDataItem("��������", points[2], eHtmlDataItemType.svg_text));
				}
			}
			// ��Ӹ���...

			// ���͵�ҳ��
			cefWebBrowser.Browser.GetMainFrame().ExecuteJavaScript("requestData(" + Newtonsoft.Json.JsonConvert.SerializeObject(datas) + ");", "", 0);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			// ���治�ɼ�ʱ��ֹͣ��������
			if (!this.Visible) return;

			RequestData();
		}

		private void buttonX1_Click(object sender, EventArgs e)
		{
			// ���͵�ҳ��
			cefWebBrowser.Browser.GetMainFrame().ExecuteJavaScript("test1();", "", 0);
		}

	}
}
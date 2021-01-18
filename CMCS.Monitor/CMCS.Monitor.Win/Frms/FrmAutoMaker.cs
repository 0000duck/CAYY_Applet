﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Entities;
using CMCS.Common.Entities.Inf;
using CMCS.Monitor.DAO;
using CMCS.Monitor.Win.Core;
using CMCS.Monitor.Win.Html;
using DevComponents.DotNetBar.Metro;
using Xilium.CefGlue.WindowsForms;
using CMCS.Common.Enums;
using CMCS.Common.Entities.Fuel;

namespace CMCS.Monitor.Win.Frms
{
	public partial class FrmAutoMaker : MetroForm
	{
		/// <summary>
		/// 窗体唯一标识符
		/// </summary>
		public static string UniqueKey = "FrmAutoMakerCSKY";
		public string[] strSignal = new string[] { "湿煤破碎机", "链式缩分驱动器运行信号", "对辊破碎机", "3mm一级缩分器驱动器运行信号", "制样机_3mm缩分1",
			"制样机_3mm缩分2","制样机_干燥","制样机_3mm缩分3","制样机_02mm破碎","制样机_02mm缩分","制样机_6mm缩分3","制样机_6mm弃料","制样机_弃料清洗样",
			"制样机_鼓风机","制样机_一体机"};

		CefWebBrowser cefWebBrowser = new CefWebBrowser();

		public FrmAutoMaker()
		{
			InitializeComponent();
		}

		/// <summary>
		/// 窗体初始化
		/// </summary>
		private void FormInit()
		{
#if DEBUG
			gboxTest.Visible = true;
#else
            gboxTest.Visible = false; 
#endif
			cefWebBrowser.StartUrl = SelfVars.Url_AutoMaker;
			cefWebBrowser.Dock = DockStyle.Fill;
			cefWebBrowser.LoadEnd += new EventHandler<LoadEndEventArgs>(cefWebBrowser_LoadEnd);
			panWebBrower.Controls.Add(cefWebBrowser);
		}

		void cefWebBrowser_LoadEnd(object sender, LoadEndEventArgs e)
		{
			timer1.Enabled = true;
		}

		private void FrmAutoMakerCSKY_Load(object sender, EventArgs e)
		{
			FormInit();
		}
		/// <summary>
		/// 请求数据
		/// </summary>
		void RequestData()
		{
			CommonDAO commonDAO = CommonDAO.GetInstance();
			AutoMakerDAO automakerDAO = AutoMakerDAO.GetInstance();

			string value = string.Empty, machineCode = string.Empty;
			List<HtmlDataItem> datas = new List<HtmlDataItem>();
			List<InfEquInfHitch> equInfHitchs = new List<InfEquInfHitch>();

			#region 全自动制样机

			datas.Clear();
			machineCode = GlobalVars.MachineCode_QZDZYJ_1;

			//制样信息
			string makeCode = commonDAO.GetSignalDataValue(machineCode, "制样编码");
			datas.Add(new HtmlDataItem("制样机_制样编码", makeCode, eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("制样机_开始时间", commonDAO.GetSignalDataValue(machineCode, "开始时间"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("制样机_煤种", commonDAO.GetSignalDataValue(machineCode, "煤种"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("制样机_水分", commonDAO.GetSignalDataValue(machineCode, "水分"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("制样机_粒度", commonDAO.GetSignalDataValue(machineCode, "粒度"), eHtmlDataItemType.svg_text));

			value = commonDAO.GetSignalDataValue(machineCode, eSignalDataName.系统.ToString());
			if ("|就绪待机|".Contains("|" + value + "|"))
				datas.Add(new HtmlDataItem("制样机_系统", "#00c000", eHtmlDataItemType.svg_color));
			else if ("|正在运行|正在卸样|".Contains("|" + value + "|"))
				datas.Add(new HtmlDataItem("制样机_系统", "#ff0000", eHtmlDataItemType.svg_color));
			else if ("|发生故障|".Contains("|" + value + "|"))
				datas.Add(new HtmlDataItem("制样机_系统", "#ffff00", eHtmlDataItemType.svg_color));
			else
				datas.Add(new HtmlDataItem("制样机_系统", "#c0c0c0", eHtmlDataItemType.svg_color));

			//信号状态
			//string keys = string.Empty;
			//foreach (string item in strSignal)
			//{
			//	if (commonDAO.GetSignalDataValue(machineCode, item) == "1")
			//	{
			//		keys += item;
			//		datas.Add(new HtmlDataItem(item, "Red", eHtmlDataItemType.svg_color));
			//	}
			//	else
			//		datas.Add(new HtmlDataItem(item + "_Line", "#6d6e71", eHtmlDataItemType.svg_color));
			//}

			//datas.Add(new HtmlDataItem("Keys", keys, eHtmlDataItemType.svg_scroll));

			///信号接入
			datas.Add(new HtmlDataItem("湿煤破碎电机", commonDAO.GetSignalDataValue(machineCode, "湿煤破碎机") == "1" ? "#00ff00" : "#ff0000", eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("链式缩分器", commonDAO.GetSignalDataValue(machineCode, "链式缩分驱动器运行信号") == "1" ? "#00ff00" : "#ff0000", eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("对辊破碎", commonDAO.GetSignalDataValue(machineCode, "对辊破碎机") == "1" ? "#00ff00" : "#ff0000", eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("3mm一级圆盘缩分器", commonDAO.GetSignalDataValue(machineCode, "3mm一级缩分器驱动器运行信号") == "1" ? "#00ff00" : "#ff0000", eHtmlDataItemType.svg_color));
			//datas.Add(new HtmlDataItem("弃料真空上料机", commonDAO.GetSignalDataValue(machineCode, "弃料真空上料机") == "1" ? "#00ff00" : "#ff0000", eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("筛分破碎", commonDAO.GetSignalDataValue(machineCode, "3mm筛分破碎机正转") == "1" ? "#00ff00" : "#ff0000", eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("3mm二级圆盘缩分器", commonDAO.GetSignalDataValue(machineCode, "3mm二级缩分器驱动器运行信号") == "1" ? "#00ff00" : "#ff0000", eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("粉碎机", commonDAO.GetSignalDataValue(machineCode, "0_2mm制粉机变频器运行信号") == "1" ? "#00ff00" : "#ff0000", eHtmlDataItemType.svg_color));
			datas.Add(new HtmlDataItem("真空上料机", commonDAO.GetSignalDataValue(machineCode, "粉碎单元真空上料机") == "1" ? "#00ff00" : "#ff0000", eHtmlDataItemType.svg_color));

			//datas.Add(new HtmlDataItem("煤样编码", commonDAO.GetSignalDataValue(machineCode, "煤样编码"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("原煤制样重量", commonDAO.GetSignalDataValue(machineCode, "原煤制样重量")+" Kg", eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("在线测水状态", commonDAO.GetSignalDataValue(machineCode, "在线测水状态"), eHtmlDataItemType.svg_text));

			datas.Add(new HtmlDataItem("左侧干燥机转速", commonDAO.GetSignalDataValue(machineCode, "左侧干燥机转速")+ " r/min", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("右侧干燥机转速", commonDAO.GetSignalDataValue(machineCode, "右侧干燥机转速")+ " r/min", eHtmlDataItemType.svg_text));

			//datas.Add(new HtmlDataItem("全水样有瓶", commonDAO.GetSignalDataValue(machineCode, "全水样有瓶"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("存查样有瓶", commonDAO.GetSignalDataValue(machineCode, "存查样有瓶"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("分析样有瓶", commonDAO.GetSignalDataValue(machineCode, "分析样有瓶"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("存查样有瓶", commonDAO.GetSignalDataValue(machineCode, "存查样有瓶"), eHtmlDataItemType.svg_text));

			//datas.Add(new HtmlDataItem("全水样重", commonDAO.GetSignalDataValue(machineCode, "全水样重")+" g", eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("3mm煤样重", commonDAO.GetSignalDataValue(machineCode, "3mm煤样重")+" g", eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("干燥煤样重", commonDAO.GetSignalDataValue(machineCode, "干燥煤样重")+ " g", eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("存查样重", commonDAO.GetSignalDataValue(machineCode, "存查样重")+" g", eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("分析样重", commonDAO.GetSignalDataValue(machineCode, "分析样重")+" g", eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("存查样重", commonDAO.GetSignalDataValue(machineCode, "存查样重")+" g", eHtmlDataItemType.svg_text));

			//datas.Add(new HtmlDataItem("6mm制样", commonDAO.GetSignalDataValue(machineCode, "6mm制样"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("3mm制样", commonDAO.GetSignalDataValue(machineCode, "3mm制样"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("3mm缩分", commonDAO.GetSignalDataValue(machineCode, "3mm缩分"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("干燥布料", commonDAO.GetSignalDataValue(machineCode, "干燥布料"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("干燥出料", commonDAO.GetSignalDataValue(machineCode, "干燥出料"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("粉碎制样", commonDAO.GetSignalDataValue(machineCode, "粉碎制样"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("6mm瓶装机", commonDAO.GetSignalDataValue(machineCode, "6mm瓶装机"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("3mm瓶装机", commonDAO.GetSignalDataValue(machineCode, "3mm瓶装机"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("弃料流程", commonDAO.GetSignalDataValue(machineCode, "弃料流程"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("6mm制样倒计时", commonDAO.GetSignalDataValue(machineCode, "6mm制样倒计时")+" 秒", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("3mm制样倒计时", commonDAO.GetSignalDataValue(machineCode, "3mm制样倒计时")+" 秒", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("左侧烘干倒计时", commonDAO.GetSignalDataValue(machineCode, "左侧烘干倒计时")+" 秒", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("右侧烘干倒计时", commonDAO.GetSignalDataValue(machineCode, "右侧烘干倒计时")+" 秒", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("粉碎制样总计时", commonDAO.GetSignalDataValue(machineCode, "粉碎总计时")+" 秒", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("粉碎负压", commonDAO.GetSignalDataValue(machineCode, "粉碎单元真空上料机负压值")+" kpa", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("弃料负压", commonDAO.GetSignalDataValue(machineCode, "弃料收集仓负压值")+" kpa", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("主气路正压", commonDAO.GetSignalDataValue(machineCode, "主气路正压值")+" kpa", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("粉碎单元正压", commonDAO.GetSignalDataValue(machineCode, "粉碎单元正压值")+" kpa", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("原煤样称（实时）", commonDAO.GetSignalDataValue(machineCode, "原煤称实时重量")+" Kg", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("3mm分析样称（实时）", commonDAO.GetSignalDataValue(machineCode, "3mm分析样称实时重量")+"g", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("3mm干燥样称（实时）", commonDAO.GetSignalDataValue(machineCode, "3mm干燥样称实时重量"+" g"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("6mm瓶装机称（实时）", commonDAO.GetSignalDataValue(machineCode, "6mm瓶装机称（实时）"), eHtmlDataItemType.svg_text));
			//datas.Add(new HtmlDataItem("3mm瓶装机称（实时）", commonDAO.GetSignalDataValue(machineCode, "3mm瓶装机称（实时）"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("弃料样称（实时）", commonDAO.GetSignalDataValue(machineCode, "弃料称实时重量")+" Kg", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("左侧干燥箱温度", commonDAO.GetSignalDataValue(machineCode, "左侧干燥箱温度")+" ℃", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("右侧干燥箱温度", commonDAO.GetSignalDataValue(machineCode, "右侧干燥箱温度")+" ℃", eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("粉碎机电流", commonDAO.GetSignalDataValue(machineCode, "粉碎电机电流")+" A", eHtmlDataItemType.svg_text));


			datas.Add(new HtmlDataItem("煤样编码", commonDAO.GetSignalDataValue(machineCode, "原煤煤样编码"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("6mm瓶装机煤样编码", commonDAO.GetSignalDataValue(machineCode, "6mm瓶装机煤样编码"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("3mm弃料一级皮带煤样编码", commonDAO.GetSignalDataValue(machineCode, "3mm弃料一级皮带煤样编码"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("干燥箱1煤样编码", commonDAO.GetSignalDataValue(machineCode, "干燥箱1煤样编码"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("3mm煤样编码", commonDAO.GetSignalDataValue(machineCode, "3mm煤样编码"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("干燥箱2煤样编码", commonDAO.GetSignalDataValue(machineCode, "干燥箱2煤样编码"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("3mm瓶装机煤样编码", commonDAO.GetSignalDataValue(machineCode, "原煤煤样编码"), eHtmlDataItemType.svg_text));
			datas.Add(new HtmlDataItem("煤样编码", commonDAO.GetSignalDataValue(machineCode, "3mm瓶装机煤样编码"), eHtmlDataItemType.svg_text));

			datas.Add(new HtmlDataItem("煤样编码", commonDAO.GetSignalDataValue(machineCode, "原煤煤样编码"), eHtmlDataItemType.svg_text));
			#endregion

			// 发送到页面
			cefWebBrowser.Browser.GetMainFrame().ExecuteJavaScript("requestData(" + Newtonsoft.Json.JsonConvert.SerializeObject(datas) + ");", "", 0);


			//出样信息
			List<InfMakerRecord> listMakerRecord = automakerDAO.GetMakerRecordByMakeCode(makeCode);
			List<object> listRes = new List<object>();
			foreach (InfMakerRecord item in listMakerRecord)
			{
				//获取样瓶传输状态
				string Status = automakerDAO.GetMakerRecordStatusByBarrelCode(item.BarrelCode);
				var makerRecord = new
				{
					EndTime = item.EndTime.ToString("yyyy-MM-dd HH:mm"),
					YPType = item.YPType,
					BarrelCode = item.BarrelCode,
					YPWeight = item.YPWeight,
					Status = Status
				};
				listRes.Add(makerRecord);
			}
			cefWebBrowser.Browser.GetMainFrame().ExecuteJavaScript("LoadSampleInfo(" + Newtonsoft.Json.JsonConvert.SerializeObject(listRes) + ");", "", 0);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			// 界面不可见时，停止发送数据
			if (!this.Visible) return;

			RequestData();
		}

		/// <summary>
		/// 刷新页面
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRefresh_Click(object sender, EventArgs e)
		{
			cefWebBrowser.Browser.Reload();
		}

		private void btnRequestData_Click(object sender, EventArgs e)
		{
			RequestData();
		}

		private void buttonX1_Click(object sender, EventArgs e)
		{
			// 发送到页面
			cefWebBrowser.Browser.GetMainFrame().ExecuteJavaScript("testColor();", "", 0);
		}

		/// <summary>
		/// 开始制样
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnStartMake_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtMakeCode.Text))
			{
				MessageBox.Show("请输入制样码", "提示");
				return;
			}

			CmcsRCMake rcMake = CommonDAO.GetInstance().SelfDber.Entity<CmcsRCMake>("where MakeCode=:MakeCode", new { MakeCode = txtMakeCode.Text });
			if (rcMake == null)
			{
				MessageBox.Show("未找到制样记录", "提示");
				return;
			}

			string currentMessage = string.Empty;
			InfMakerControlCmd makerControlCmd = new InfMakerControlCmd();
			makerControlCmd.InterfaceType = GlobalVars.InterfaceType_QZDZYJ;
			makerControlCmd.MachineCode = GlobalVars.MachineCode_QZDZYJ_1;
			makerControlCmd.MakeCode = rcMake.MakeCode;
			makerControlCmd.ResultCode = eEquInfCmdResultCode.默认.ToString();
			makerControlCmd.CmdCode = eEquInfMakerCmd.开始制样.ToString();
			makerControlCmd.SyncFlag = 0;
			if (Dbers.GetInstance().SelfDber.Insert<InfMakerControlCmd>(makerControlCmd) > 0)
			{
				MessageBox.Show("命令发送成功", "提示");
				return;
			}
		}

		private void btnDownMake_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtMakeCode.Text))
			{
				MessageBox.Show("请输入制样码", "提示");
				return;
			}

			CmcsRCMake rcMake = CommonDAO.GetInstance().SelfDber.Entity<CmcsRCMake>("where MakeCode=:MakeCode", new { MakeCode = txtMakeCode.Text });
			if (rcMake == null)
			{
				MessageBox.Show("未找到制样记录", "提示");
				return;
			}

			string currentMessage = string.Empty;
			InfMakerControlCmd makerControlCmd = new InfMakerControlCmd();
			makerControlCmd.InterfaceType = GlobalVars.InterfaceType_QZDZYJ;
			makerControlCmd.MachineCode = GlobalVars.MachineCode_QZDZYJ_1;
			makerControlCmd.MakeCode = rcMake.MakeCode;
			makerControlCmd.ResultCode = eEquInfCmdResultCode.默认.ToString();
			makerControlCmd.CmdCode = eEquInfMakerCmd.停止制样.ToString();
			makerControlCmd.SyncFlag = 0;
			if (Dbers.GetInstance().SelfDber.Insert<InfMakerControlCmd>(makerControlCmd) > 0)
			{
				MessageBox.Show("命令发送成功", "提示");
				return;
			}
		}

		private void btnKeepMake_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtMakeCode.Text))
			{
				MessageBox.Show("请输入制样码", "提示");
				return;
			}

			CmcsRCMake rcMake = CommonDAO.GetInstance().SelfDber.Entity<CmcsRCMake>("where MakeCode=:MakeCode", new { MakeCode = txtMakeCode.Text });
			if (rcMake == null)
			{
				MessageBox.Show("未找到制样记录", "提示");
				return;
			}

			string currentMessage = string.Empty;
			InfMakerControlCmd makerControlCmd = new InfMakerControlCmd();
			makerControlCmd.InterfaceType = GlobalVars.InterfaceType_QZDZYJ;
			makerControlCmd.MachineCode = GlobalVars.MachineCode_QZDZYJ_1;
			makerControlCmd.MakeCode = rcMake.MakeCode;
			makerControlCmd.ResultCode = eEquInfCmdResultCode.默认.ToString();
			makerControlCmd.CmdCode = eEquInfMakerCmd.继续制样.ToString();
			makerControlCmd.SyncFlag = 0;
			if (Dbers.GetInstance().SelfDber.Insert<InfMakerControlCmd>(makerControlCmd) > 0)
			{
				MessageBox.Show("命令发送成功", "提示");
				return;
			}
		}
	}
}

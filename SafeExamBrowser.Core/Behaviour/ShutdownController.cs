﻿/*
 * Copyright (c) 2017 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SafeExamBrowser.Contracts.Behaviour;
using SafeExamBrowser.Contracts.Configuration;
using SafeExamBrowser.Contracts.I18n;
using SafeExamBrowser.Contracts.Logging;
using SafeExamBrowser.Contracts.Monitoring;
using SafeExamBrowser.Contracts.UserInterface;

namespace SafeExamBrowser.Core.Behaviour
{
	public class ShutdownController : IShutdownController
	{
		private ILogger logger;
		private IMessageBox messageBox;
		private IProcessMonitor processMonitor;
		private ISettings settings;
		private ISplashScreen splashScreen;
		private IText text;
		private IUiElementFactory uiFactory;
		private IWorkingArea workingArea;

		private IEnumerable<Action> ShutdownOperations
		{
			get
			{
				yield return StopProcessMonitoring;
				yield return RestoreWorkingArea;
				yield return FinalizeApplicationLog;
			}
		}

		public ShutdownController(
			ILogger logger,
			IMessageBox messageBox,
			IProcessMonitor processMonitor,
			ISettings settings,
			IText text,
			IUiElementFactory uiFactory,
			IWorkingArea workingArea)
		{
			this.logger = logger;
			this.messageBox = messageBox;
			this.processMonitor = processMonitor;
			this.settings = settings;
			this.text = text;
			this.uiFactory = uiFactory;
			this.workingArea = workingArea;
		}

		public void FinalizeApplication()
		{
			try
			{
				InitializeSplashScreen();

				foreach (var operation in ShutdownOperations)
				{
					operation();
					splashScreen.UpdateProgress();

					// TODO: Remove!
					Thread.Sleep(250);
				}
			}
			catch (Exception e)
			{
				logger.Error($"Failed to finalize application!", e);
				messageBox.Show(text.Get(Key.MessageBox_ShutdownError), text.Get(Key.MessageBox_ShutdownErrorTitle), icon: MessageBoxIcon.Error);
			}
		}

		private void InitializeSplashScreen()
		{
			splashScreen = uiFactory.CreateSplashScreen(settings, text);
			splashScreen.SetMaxProgress(ShutdownOperations.Count());
			splashScreen.UpdateText(Key.SplashScreen_ShutdownProcedure);
			splashScreen.InvokeShow();
			logger.Info("--- Initiating shutdown procedure ---");
		}

		private void StopProcessMonitoring()
		{
			logger.Info("--- Stopping process monitoring ---");
			splashScreen.UpdateText(Key.SplashScreen_StopProcessMonitoring);

			// TODO

			processMonitor.StopMonitoringExplorer();
		}

		private void RestoreWorkingArea()
		{
			logger.Info("--- Restoring working area ---");
			splashScreen.UpdateText(Key.SplashScreen_RestoreWorkingArea);

			// TODO

			workingArea.Reset();

			splashScreen.UpdateText(Key.SplashScreen_WaitExplorerStartup, true);
			processMonitor.StartExplorerShell();
		}

		private void FinalizeApplicationLog()
		{
			logger.Info("--- Application successfully finalized! ---");
			logger.Log($"{Environment.NewLine}# Application terminated at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
		}
	}
}
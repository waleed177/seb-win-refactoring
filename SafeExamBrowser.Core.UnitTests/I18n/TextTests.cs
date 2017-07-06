﻿/*
 * Copyright (c) 2017 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SafeExamBrowser.Core.I18n;

namespace SafeExamBrowser.Core.UnitTests
{
	[TestClass]
	public class TextTests
	{
		[TestMethod]
		public void MustNeverBeNull()
		{
			Assert.IsNotNull(Text.Instance);
		}

		[TestMethod]
		public void MustNeverReturnNull()
		{
			var text = Text.Instance.Get((Key) (-1));

			Assert.IsNotNull(text);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MustNotAllowBeingNull()
		{
			Text.Instance = null;
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MustNotAllowNullResource()
		{
			new Text(null);
		}
	}
}
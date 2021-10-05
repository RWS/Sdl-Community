﻿using System.Collections.Generic;
using Moq;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.OmitZero
{
	public class OmitZeroSourceAndTarget
	{

		/// <summary>
		/// Helper method for settings
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public List<ErrorReporting> SourceOmitCheckedTargetUnchecked(string source, string target)
		{
			//source settings
			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
			numberVerifierSettings.Setup(d => d.SourceOmitLeadingZero).Returns(true);
			numberVerifierSettings.Setup(d => d.SourceDecimalPeriod).Returns(true);

			//target settings 
			numberVerifierSettings.Setup(t => t.TargetOmitLeadingZero).Returns(false);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			return errorMessage;
		}

		public List<ErrorReporting> SourceOmitUncheckedTargetChecked(string source, string target)
		{
			//source settings
			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
			numberVerifierSettings.Setup(d => d.SourceOmitLeadingZero).Returns(false);
			numberVerifierSettings.Setup(d => d.SourceDecimalPeriod).Returns(true);

			numberVerifierSettings.Setup(t => t.TargetOmitLeadingZero).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			return errorMessage;
		}

		public List<ErrorReporting> SourceOmitUncheckedTargetUnchecked(string source, string target)
		{
			//source settings
			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
			numberVerifierSettings.Setup(d => d.SourceOmitLeadingZero).Returns(false);
			numberVerifierSettings.Setup(d => d.SourceDecimalPeriod).Returns(true);

			numberVerifierSettings.Setup(t => t.TargetOmitLeadingZero).Returns(false);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			return errorMessage;
		}

		public List<ErrorReporting> SourceOmitCheckedTargetChecked(string source, string target)
		{
			//source settings
			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
			numberVerifierSettings.Setup(d => d.SourceOmitLeadingZero).Returns(true);
			numberVerifierSettings.Setup(d => d.SourceDecimalPeriod).Returns(true);

			numberVerifierSettings.Setup(t => t.TargetOmitLeadingZero).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			return errorMessage;
		}

		/// <summary>
		/// Source: omit zero true
		/// Target: omit zero false
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData(".55 another number 0.55 negative -.58 another -0.58", "0.55 number 0.55 negative -0.58 nr -0.58")]
		public void SourceOmitCheckedTargetUncheckedWithError(string source, string target)
		{
			var errorMessage = SourceOmitCheckedTargetUnchecked(source, target);
			Assert.True(errorMessage.Count == 0);
		}

		/// <summary>
		/// Source: omit zero true
		/// Target: omit zero false
		/// Number modified/unlocalised
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData(".55 number 0.55", ".55 omit .55")]
		public void SourceOmitCheckedTargetUncheckedError(string source, string target)
		{
			var errorMessage = SourceOmitCheckedTargetUnchecked(source, target);
			Assert.Equal(errorMessage[0]?.ErrorMessage, PluginResources.Error_NumbersNotIdentical);
		}

		[Theory]
		[InlineData("-.55", "-.55")]
		public void SourceOmitCheckedTargetUncheckedNegativeNumbersNoError(string source, string target)
		{
			var errorMessage = SourceOmitUncheckedTargetUnchecked(source, target);
			Assert.True(errorMessage.Count == 0);
		}

		/// <summary>
		/// Source: omit zero false
		/// Target: omit zero true
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData("0.55 another number 0.55 neg -0.44", ".55 number 0.55 a −.44")]
		public void SourceOmitUncheckedTargetCheckedWithErrors(string source, string target)
		{
			var errorMessage = SourceOmitUncheckedTargetChecked(source, target);
			Assert.True(errorMessage.Count == 0);
		}

		/// <summary>
		/// Source: omit zero false
		/// Target: omit zero true
		/// Number modified/unlocalised.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData(".55 number .55", "0.55 omit .55")]
		public void SourceOmitUncheckedTargetCheckedError(string source, string target)
		{
			var errorMessage = SourceOmitUncheckedTargetChecked(source, target);
			Assert.Equal(PluginResources.Error_NumbersNotIdentical, errorMessage[0].ErrorMessage);
		}

		/// <summary>
		/// Source: omit zero false
		/// Target: omit zero false
		/// No error
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData("0.55", "0.55")]
		public void SourceOmitUncheckedTargetUncheckedNoError(string source, string target)
		{
			var errorMessage = SourceOmitUncheckedTargetUnchecked(source, target);
			Assert.True(errorMessage.Count == 0);
		}

		/// <summary>
		/// Source: omit zero true
		/// Target: omit zero true
		/// Number modified/unlocalised.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData(".55 test 0.55 a 0.55 b -.55", ".55 another test .55 number 0.55 c −0.55")]
		public void SourceOmitCheckedTargetCheckedWithError(string source, string target)
		{
			var errorMessage = SourceOmitCheckedTargetChecked(source, target);
			//Assert.Equal(PluginResources.Error_NumbersNotIdentical, errorMessage[0]?.ErrorMessage);
			Assert.True(errorMessage.Count == 0);
		}

		/// <summary>
		/// Only decimal numbers which start with 0 can be written in short form 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData("1.55", ".55")]
		public void CheckError(string source, string target)
		{
			var errorMessage = SourceOmitCheckedTargetChecked(source, target);
			Assert.Equal(PluginResources.Error_NumbersNotIdentical, errorMessage[0]?.ErrorMessage);
		}
	}
}
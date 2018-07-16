using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Dialogs
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            Text = PluginResources.About_Qualitivity_for_SDL_Trados_Studio_2017;
            labelProductNameAndVersion.Text = string.Format(PluginResources.Qualitivity___Version__0_, AssemblyVersion);
            label1.Text = PluginResources.Qualitivity;            
            labelCopyright.Text = AssemblyCopyright;
            labelCompanyName.Text = AssemblyCompany;
            textBoxDescription.Text = _copyRight;
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }


		private readonly string _copyRight = "License agreement\r\n"
		+ "\r\n"
		+ "SDL AppStore End User Terms and Conditions" + "\r\n" + "\r\n"

		+ "1. Your Relationship with SDL" + "\r\n"
		+ "1.1 Your use of SDL AppStore, including its web pages, products, software (including software made available for download) and services(together the “Services”), is subject to the terms of a legal agreement between you and SDL plc of New Globe House, Vanwall Business Park, Vanwall Road, Maidenhead, Berkshire, SL6 4UB, UNITED KINGDOM(“SDL”)." + "\r\n"
		+ "1.2 Unless otherwise agreed in writing between you and SDL, this agreement includes, at a minimum, the terms and conditions set out in this document and Terms and Conditions for the use of any SDL websites including but not limited to: sdl.com; appstore.sdl.com, sdltrados.com, community.sdl.com; languagecloud.sdl.com('SDL Websites')." + "\r\n"
		+ "1.3 Where additional terms and conditions apply to a Service, these will be accessible for you to read either within, or through your use of, that Service." + "\r\n"
		+ "1.4 Taken together, these Terms and Conditions for the Use of SDL AppStore, the Terms and Conditions for the Use of the SDL Websites and any additional terms and conditions that apply to a Service are called the “Terms”. In the event that there is a conflict between these, they will prevail in that order." + "\r\n"
		+ "1.5 Sometimes a member of the group of companies of which SDL is the parent (an “SDL Company”) may provide Services to you on behalf of SDL, and you accept that they may do so under the same Terms.You accept that each SDL Company shall be third party beneficiary to the Terms and that they shall be entitled to directly enforce, and rely upon, any provision of the Terms which confers a benefit on them.Other than this, no other person or company shall be third party beneficiaries to the Terms." + "\r\n"
		+ "1.6 SDL may make changes to these Terms from time-to-time by mail or e-mail or by posting replacements or changes to the site at which this document is located, and you accept that if you use the Services after the date on which the Terms have been replaced or changed SDL will treat your use as acceptance of the updated Terms." + "\r\n" + "\r\n"


		+ "2. Accepting these Terms" + "\r\n"
		+ "2.1 In order to use the Services, you must accept the Terms.You may not use the Services if you do not accept the Terms." + "\r\n"
		+ "2.2 By clicking to accept or to agree to the Terms (where this option is made available to you by SDL in the interface for a Service) or by actually using the Services you accept the Terms, and you understand and agree that SDL will treat your use of the Services as acceptance of the Terms from the point of clicking or of use." + "\r\n" + "\r\n"


		+ "3. Your Use of the Services" + "\r\n"
		+ "3.1 You agree to use the Services only for purposes that are permitted by the Terms and as permitted by applicable law, regulation and generally accepted practice or guideline in the relevant jurisdictions (including any laws regarding the export of data and/or software)." + "\r\n"
		+ "3.2 You agree not to reproduce, duplicate, copy, sell, trade or resell the Services for any purpose or to access(or to attempt to access) any of the Services by any means other than through the interface that is provided by SDL, unless you have been specifically allowed to do so in a separate agreement with SDL.You specifically agree not to access(or to attempt to access) any of the Services through any automated means, including use of scripts or web crawlers." + "\r\n"
		+ "3.3 You agree that you are responsible for maintaining the confidentiality of any password associated with any account that you use to access the Services and you agree that you will be solely responsible to SDL for all activities that occur under your account." + "\r\n"
		+ "3.4 Copyright and other intellectual property laws protect the Services provided to you, and you agree to abide by and maintain all notices, license information, and restrictions contained therein." + "\r\n"
		+ "3.5 You may not decompile, reverse engineer, dissemble, attempt to derive the source code of any software or security components of the Services (except as and only to the extent any foregoing restriction is prohibited by applicable law or to the extent as may be permitted by any licensing terms accompanying the foregoing)." + "\r\n"
		+ "3.6 You may not use the Services to violate, tamper with, or circumvent the security of any computer network, software, passwords, encryption codes, technological protection measures, or to otherwise engage in any kind of illegal activity, or to enable others to do so." + "\r\n" + "\r\n"

		+ "4. External Links and Third-Party Content" + "\r\n"
		+ "4.1 Sometimes through your use of the Services you may use a service or purchase goods or access information(such as data files, written text, computer software or audio files) which are provided by another person or company('Third-Party Content'), and the Services may include hyperlinks to such Third-Party Content." + "\r\n"
		+ "4.2 You accept that Third-Party Content is the sole responsibility of the company or person from whom it originated, that SDL may have no control over such Third-Party Content and does not endorse this. You acknowledge and agree that SDL is not liable for any loss or damage which may be incurred by you as a result of the availability or non-availability of Third-Party Content, or as a result of any reliance placed by you on the completeness, accuracy or existence of Third-party Content. You understand that by using the Services you may be exposed to Third-Party Content that you may find offensive, indecent or objectionable and this, in this respect, you use the Services at your own risk." + "\r\n" + "\r\n"

		+ "5. SDL Support and Technical Services" + "\r\n"
		+ "As a registered user of SDL AppStore, you have access to the AppStore developer community, which may offer discretionary support from both developers and SDL depending on application." + "\r\n"
		+ "5.1  SDL Community Developers:  Certain applications you access from the AppStore may have been developed by SDL Community Developers.SDL may, but is under no obligation to, provide discretionary support for such applications, available from time to time, either free-of-charge or for a separate fee.You agree that all use of such services will be in accordance with SDL’s usage policies for such services, which are subject to change from time to time, with or without prior notice to you." + "\r\n"
		+ "i.You agree that SDL shall not be liable to you or any third party for any medication or cessation of such services." + "\r\n"
		+ "ii.You agree that when requesting and receiving support and technical services you will not provide SDL with any information that is confidential to you or any third party." + "\r\n"
		+ "iii.SDL reserves the right to reject a request for support and technical services at any time and for any reason." + "\r\n"
		+ "iv.You shall be solely responsible for any restoration of lost or altered files, data, programs or other materials provided." + "\r\n"
		+ "5.2  SDL Development Community; Third Party Developers:  Certain applications you access from the AppStore may have been developed by third party SDL Developers.  SDL is not responsible for any such support, which is the sole responsibility of the applicable Developer, and disclaims any and all liability related to your use of such applications and support." + "\r\n" + "\r\n"

		+ "6. Confidentiality" + "\r\n"
		+ "6.1 “Confidential Information” means any information, in whatever form or medium, of SDL or its affiliates furnished or otherwise made available to you in connection with your registration within the SDL AppStore together with all analyses, compilations, reports, memoranda, notes and other written or electronic materials which contain, reflect or are based, in whole or in part, upon such information either directly or indirectly in written, oral, or electronic form.For purposes of this Agreement, with respect to SDL, “affiliate” means any entity is directly or indirectly controlled by SDL." + "\r\n"
		+ "6.2 During and after the term of this Agreement, you agree that you will: (a) hold in strict confidence and not disclose any Confidential Information to any third party except as expressly authorized by SDL in writing, (b) use any Confidential Information only to the extent necessary in connection with the authorized purposes for which they were disclosed to you and not otherwise for your own or any third party’s gain or benefit, (c) restrict access to Confidential Information to those employees, representatives or agents of you or your employer who have a 'need to know' in connection with the authorized purpose for which it was disclosed to you; and (d) not reverse engineer, decompile or disassemble any software disclosed to you by SDL, except as expressly permitted by and for the purposes of this Agreement." + "\r\n"
		+ "6.3 If you are required to disclose Confidential Information pursuant to any applicable law, regulation, court order or document discovery request, then you agree to give SDL prompt written notice of such requirement." + "\r\n"
		+ "6.4 You shall not make any copies of the Confidential Information unless it is deemed necessary for the authorized purpose for which it was disclosed to you. All copies shall be designated as “proprietary” or “confidential”. You shall reproduce SDL's proprietary rights notices on any such approved copies, in the same manner in which such notices were set forth in or on the original." + "\r\n"
		+ "6.5 You acknowledge and agree that any unauthorized disclosure or other violation, or threatened violation of this Agreement by you may cause irreparable damage to SDL.Accordingly, SDL will be entitled to seek an injunction prohibiting you from any such disclosure, attempted disclosure, violation or threatened violation without the necessity of proving damages or furnishing a bond or other security, in addition to any other available remedies. " + "\r\n" + "\r\n"

		+ "7. Exclusion of Warranties and Limitation of Liabilities" + "\r\n"
		+ "7.1 THE SOFTWARE AND/OR SERVICE PROVIDED UNDER THE SDL APPSTORE IS PROVIDED ON AN AS¬ IS BASIS. SDL DOES NOT ENDORSE THE SOFTWARE AND/OR SERVICE AVAILABLE ON THIS SITE. ALL WARRANTIES, CONDITIONS OR OTHER TERMS CONCERNING THE SOFTWARE AND/OR SERVICE, WHETHER EXPRESS OR IMPLIED BY STATUTE, COMMON LAW OR OTHERWISE (INCLUDING THOSE RELATING TO SATISFACTORY QUALITY, NON-INFRINGEMENT AND FITNESS FOR PURPOSES) ARE EXCLUDED. " + "\r\n"
		+ "7.2 IN NO EVENT WILL SDL (OR ANY OF ITS AFFILIATES) BE LIABLE FOR ANY DAMAGES, IN CONTRACT, TORT (INCLUDING NEGLIGENCE OR BREACH OF STATUTORY DUTY), OR OTHERWISE FOR LOSS OF DATA, LOST PROFITS, COSTS OF COVER OR OTHER SPECIAL, INCIDENTAL, CONSEQUENTIAL, ACTUAL, GENERAL, OR INDIRECT DAMAGES ARISING FROM THE USE OF THE SOFTWARE AND/OR SERVICE, HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY THIS LIMITATION WILL APPLY EVEN IF SDL HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. THE PARTIES ACKNOWLEDGE THAT THIS IS A REASONABLE ALLOCATION OF RISK. LICENSOR'S TOTAL AGGREGATE LIABILITY UNDER THIS AGREEMENT SHALL IN NO EVENT EXCEED THE AMOUNT PAID DURING THE PRECEDING TWELVE (12) MONTHS FOR THE APPLICABLE SOFTWARE OR SERVICE." + "\r\n"
		+ "7.3 NOTHING IN THIS AGREEMENT SHALL EXCLUDE OR LIMIT EITHER PARTY'S LIABILITY TO THE OTHER FOR (I) DEATH OR PERSONAL INJURY CAUSED BY ITS NEGLIGENCE OR (II) ANY OTHER LIABILITY TO THE EXTENT THAT IT CANNOT BE EXCLUDED OR LIMITED AS A MATIER OF LAW." + "\r\n" + "\r\n"

		+ "8. Termination" + "\r\n"
		+ "8.1 SDL may terminate its agreement with you at any time for any reason including, without limitation: " + "\r\n"
		+ "i. you have breached any provision of the Terms (or have acted in manner which shows that you are unable to or do not intend to comply with the provisions of the Terms);" + "\r\n"
		+ "ii. SDL is required to do so by law (for example, where the provision of the Services to you is, or becomes, unlawful);" + "\r\n"
		+ "iii. SDL intends to withdraw provision of the Services from users of a profile to which you belong, or for no reason. " + "\r\n"
		+ "8.2 Sections 10 (Exclusion of Warranties and Limitation of Liabilities) and 13 (Law and Jurisdiction) and any other provisions which are expressly stated to or by their nature must survive termination of the agreement between you and SDL shall survive." + "\r\n" + "\r\n"

		+ "9. Amendment; Communication." + "\r\n"
		+ "9.1 SDL reserves the right, at its discretion, to modify the Terms, including any rules and policies at any time. You will be responsible for reviewing and becoming familiar with any such modifications (including new terms, updates, revisions, supplements, modifications, and additional rules, policies, terms and conditions) ('Additional Terms') communicated to you by SDL." + "\r\n"
		+ "9.2 All Additional Terms are hereby incorporated into this agreement by this reference and your continued use of the SDL AppStore will indicate your acceptance of any Additional Terms.In addition, SDL may send communications to you from time to time including, but not be limited to, marketing materials, technical information, and updates and/or changes regarding your participation as a registered user of the SDL AppStore.By agreeing to these Terms, you consent that SDL may provide you with such communications." + "\r\n" + "\r\n"

		+ "10. Law and Jurisdiction" + "\r\n"
		+ "These Terms and the relationship between you and SDL under these Terms shall be governed by and shall be interpreted in accordance with the laws of England and Wales and you and SDL submit to the exclusive jurisdiction of the English Courts in relation to all matters arising therefrom. You agree, however, that SDL shall be allowed to apply for injunctive remedies in any jurisdiction." + "\r\n"
		+ "October 2017" + "\r\n";




		#region Assembly Attribute Accessors

	public string GetPostEditCompareProductVersion()
        {
            var productVersion = string.Empty;

            var fileVersionInfo = PostEditCompareVersionInfo(); 
            return fileVersionInfo!=null ? fileVersionInfo.ProductVersion : productVersion;
        }

        public FileVersionInfo PostEditCompareVersionInfo()
        {
            FileVersionInfo versionInfo =null;
            
            var sPath = Path.Combine(Application.StartupPath, "PostEdit.Compare.exe");
            if (File.Exists(sPath))
                versionInfo = FileVersionInfo.GetVersionInfo(sPath);
            
            return versionInfo;
        }

        public string AssemblyTitle
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length <= 0)
                    return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                return titleAttribute.Title != "" ? titleAttribute.Title : Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        #endregion

  
        private void linkLabel_www_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://codificare.net/tools/qualitivity/");
        }
    }
}

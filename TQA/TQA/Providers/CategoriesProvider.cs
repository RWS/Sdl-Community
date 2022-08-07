using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.TQA.Model;
using Sdl.Core.Settings;

namespace Sdl.Community.TQA.Providers
{
	public class CategoriesProvider
	{
		private Dictionary<Guid, string> TQS_J2450_Categories => new Dictionary<Guid, string>
		{
			{new Guid("c6be61c6-8796-4280-8c11-5f2450572e98"),"Wrong Term (WT)"}, //WT (is Abbreviation)
			{new Guid("28fe5b65-c6c1-4a35-9ed5-8b63353ce654"),"Wrong Meaning (WM)"},//WM
			{new Guid("19743cbc-ca35-49bf-8070-c79323081760"),"Omission (OM)"},//OM
			{new Guid("6065ab78-cf2f-4eb0-8913-803492f5e01e"),"Structural Error (SE)"},//SE
			{new Guid("8777c0ac-8ab7-42b5-bb7e-b14254e8d0c3"),"Misspelling (SP)"},//SP
			{new Guid("b6944581-16b7-4f5a-8f69-e4ccd51aef05"),"Punctuation (PE)"},
			{new Guid("9f4ba59d-7183-4cdb-85ab-cc32ffb69a62"),"Style (ST)"},//ST
			{new Guid("b73f3f31-47f8-4079-8b67-1ac7a07440c6"),"Miscellaneous (ME)"},//ME
			{new Guid("c5d92dba-975e-4036-b36b-92114159e20d"),"Legacy Data Error"},//NS1-Unspecified 1
			{new Guid("8163333e-a059-4bc9-a082-a663131bf4ed"),"Source Text Error"},//NS2- Unspecified 2
			{new Guid("9fe180e2-4e7d-4e78-acf2-fab0dfe64106"),"Formatting / non-linguistic"}//NS3-Unspecified 3
		};

		private Dictionary<Guid, string> TQS_MQM_Categories => new Dictionary<Guid, string>
		{
			{new Guid("c6be61c6-8796-4280-8c11-5f2450572e98"),"Terminology"},
			{new Guid("28fe5b65-c6c1-4a35-9ed5-8b63353ce654"),"Accuracy"},
			{new Guid("19743cbc-ca35-49bf-8070-c79323081760"),"Linguistic conventions"},
			{new Guid("6065ab78-cf2f-4eb0-8913-803492f5e01e"),"Style"},
			{new Guid("8777c0ac-8ab7-42b5-bb7e-b14254e8d0c3"),"Locale conventions"},
			{new Guid("b6944581-16b7-4f5a-8f69-e4ccd51aef05"),"Audience appropriateness"},
			{new Guid("9f4ba59d-7183-4cdb-85ab-cc32ffb69a62"),"Design and markup"},
			{new Guid("b73f3f31-47f8-4079-8b67-1ac7a07440c6"),"Other"}
		};

		private Dictionary<Guid, Dictionary<Guid, string>> TQS_MQM_SubCategories =>
		new Dictionary<Guid, Dictionary<Guid, string>>
		{
			//Terminology -c6be61c6-8796-4280-8c11-5f2450572e98
			{ new Guid("c6be61c6-8796-4280-8c11-5f2450572e98"), new Dictionary<Guid, string>
			  {
				{new Guid("16b3dfaf-d217-4013-b3e4-da7937a884ed"),"Inconsistent with terminology resource"},
				{new Guid("0a985ce6-11b6-40e1-aa9c-e633a864c8b1"),"Inconsistent use of terminology"},
				{new Guid("90cf17ea-cc5b-4e4f-b49b-2b15920517c6"),"Wrong term"},
				{new Guid("f6f3fccd-089b-447c-844c-10ec449a00e9"),"Other"}
			  }
			 },
			//Accuracy -28fe5b65-c6c1-4a35-9ed5-8b63353ce654
			{ new Guid("28fe5b65-c6c1-4a35-9ed5-8b63353ce654"), new Dictionary<Guid, string>
			  {
				{new Guid("89312671-b97d-415c-a1b0-f476d44167f5"),"Mistranslation"},
				{new Guid("6949c70d-af7a-4bf4-84dc-aec72ceb615e"),"Over-translation"},
				{new Guid("8c10b3ce-7e40-44c2-a2ff-d7cc311d38fa"),"Under-translation"},
				{new Guid("48ba347e-485d-44a2-bd89-96719386b779"),"Addition"},
				{new Guid("16613163-d7dd-4482-a3b0-9d2d1897040d"),"Omission"},
				{new Guid("81f79c80-919d-4f9f-8ffd-12564c3c22da"),"Do not translate (DNT)"},
				{new Guid("5e4ad536-333c-47cd-baf7-7d13f56015a0"),"Untranslated"},
				{new Guid("d2a8e029-8585-40eb-8a6a-cb37caebe1ad"),"Other"}
			  }
			 },
			//Linguistic conventions -19743cbc-ca35-49bf-8070-c79323081760
			{ new Guid("19743cbc-ca35-49bf-8070-c79323081760"), new Dictionary<Guid, string>
			  {
				{new Guid("182dd5c1-6e9e-4b63-ae98-fd97d073d8e0"),"Grammar"},
				{new Guid("bd757980-acdd-497f-a3d6-502739316f56"),"Punctuation"},
				{new Guid("21d3b45b-d9f2-4e5a-ac9a-2c944beb0f40"),"Spelling"},
				{new Guid("2a3cb4ee-4287-40e3-9134-abe51be8c011"),"Unintelligible"},
				{new Guid("d73755fa-6603-4670-8b3f-53a602177ff6"),"Character encoding"},
				{new Guid("e30caabd-cbd5-4265-8c0b-c9fbb96c680a"),"Other"}
			  }
			 },
			//Style - 6065ab78-cf2f-4eb0-8913-803492f5e01e
			{ new Guid("6065ab78-cf2f-4eb0-8913-803492f5e01e"), new Dictionary<Guid, string>
			  {
				{new Guid("beb45730-b5d2-47cb-b609-48db84e1b794"),"Organizational style"},
				{new Guid("eb653bd4-6c87-4302-bb46-cedd5d8995cf"),"Third-party style"},
				{new Guid("4af1ce81-40ee-49aa-8980-6222c3fda0c6"),"Inconsistent with external reference"},
				{new Guid("40e17604-b9a8-4837-9175-fa3643e7cbf8"),"Register"},
				{new Guid("9c46a50c-463b-42f2-967e-7810a7f1d631"),"Awkward style"},
				{new Guid("71fe09f3-a091-4f7b-b804-89ab511a8b84"),"Unidiomatic style"},
				{new Guid("2eb92cca-e9f3-43e8-9105-2c28041c84a5"),"Inconsistent style"},
				{new Guid("a6a2ca78-3cdc-411c-b514-d4676ae11de4"),"Other"}
			  }
			 },
			//Locale conventions - 8777c0ac-8ab7-42b5-bb7e-b14254e8d0c3
			{ new Guid("8777c0ac-8ab7-42b5-bb7e-b14254e8d0c3"), new Dictionary<Guid, string>
			  {
				{new Guid("23405059-ed3b-486d-9946-013b5a5d5b88"),"Number format"},
				{new Guid("bf0d750f-2603-4843-970c-c02b5a77aa25"),"Currency format"},
				{new Guid("cb3d6505-9739-4c43-a873-adee8ab01ee4"),"Measurement format"},
				{new Guid("a8f954b9-df19-4a3b-a0dd-12abf9446ca8"),"Time format"},
				{new Guid("5bb33939-658a-4352-a6c1-526631b924bb"),"Date format"},
				{new Guid("315ca844-b6cc-472e-b7b2-a0da8b4ce0a8"),"Address format"},
				{new Guid("1db9279d-6788-461e-89eb-32447d322e6b"),"Telephone format"},
				{new Guid("65eacba9-57f4-41c9-bae2-9f8244524f64"),"Shortcut key"},
				{new Guid("afcfb999-2d5d-4230-b796-19fae2195246"),"Other"}
			  }
			 },
			//Audience appropriateness -b6944581-16b7-4f5a-8f69-e4ccd51aef05
			{ new Guid("b6944581-16b7-4f5a-8f69-e4ccd51aef05"), new Dictionary<Guid, string>
			  {
				{new Guid("329f3e7c-8632-4f8d-a89b-78ca3b8dda2b"),"Culture-specific reference"},
				{new Guid("ef42b80e-32df-46ca-bab8-aba26a3308b0"),"Other"}
			  }
			 },

			//Design and markup-9f4ba59d-7183-4cdb-85ab-cc32ffb69a62
			{ new Guid("9f4ba59d-7183-4cdb-85ab-cc32ffb69a62"), new Dictionary<Guid, string>
			  {
				{new Guid("aa4cb0d9-dde8-4a1e-9594-9c7583fb3a04"),"Character formatting"},
				{new Guid("573c27e8-7c21-43be-910e-72f20cff0fcf"),"Layout"},
				{new Guid("3d1248e1-574b-4b85-988b-59f3918f5a45"),"Markup tag"},
				{new Guid("f4efdfe7-7c2c-4127-835c-8b420bea7f0b"),"Truncation/text expansion"},
				{new Guid("2db4f2f1-d051-48aa-97bf-ac111fd7e400"),"Missing text"},
				{new Guid("accf8bf0-dfee-479b-bccd-7ba8ce4f0811"),"Link/cross-reference"},
				{new Guid("92d0d14e-0198-4341-b7ce-4aa0144f4d0e"),"Other"}
			  }
			 },
			//Other - b73f3f31-47f8-4079-8b67-1ac7a07440c6
			{ new Guid("b73f3f31-47f8-4079-8b67-1ac7a07440c6"), new Dictionary<Guid, string>
			  {
				{new Guid("582d71b8-5d65-4a64-9175-6a79c69f73f9"),"Legacy data"},
				{new Guid("cd3e59d0-bf53-466d-98e6-d9a5ccdb4e55"),"Source text data"},
				{new Guid("5ec74d77-1fd2-4b22-acdd-e6e06135daff"),"Other"}
			  }
			 }
		};

		public TQAProfileType GetTQAProfileType(List<Guid> categoriesIds)
		{
			if (categoriesIds.All(catId => TQS_J2450_Categories.ContainsKey(catId)) &&
				TQS_J2450_Categories.Keys.All(catKey => categoriesIds.Contains(catKey)))
			{
				return TQAProfileType.tqsJ2450;
			}

			if (categoriesIds.All(catId => TQS_MQM_Categories.ContainsKey(catId)) &&
				TQS_MQM_Categories.Keys.All(catKey => categoriesIds.Contains(catKey)))
			{
				return TQAProfileType.tqsMQM;
			}

			throw new ArgumentException(string.Format(PluginResources.UnknownCategoriesInReportTemplate, string.Join(",", categoriesIds)));
		}

		public TQAProfileType GetTQAProfileType(ISettingsBundle settingsBundle)
		{
			return GetTQAProfileType(GetAssessmentCategories(settingsBundle));
		}

		public TQAProfileType GetTQAProfileType(AssessmentCategories tqaProjectCategories)
		{
			if (tqaProjectCategories == null)
			{
				return TQAProfileType.tqsEmpty;
			}

			var tqaCategNames = tqaProjectCategories.Select(category => category.Name).ToList();

			if (!tqaCategNames.Any())
			{
				return TQAProfileType.tqsEmpty;
			}

			if (AllTQACategoriesAreCompatible(TQS_J2450_Categories, tqaCategNames))
			{
				return TQAProfileType.tqsJ2450;
			}

			if (AllMQMCategoriesAndSubcategoriesAreCompatible(tqaProjectCategories, tqaCategNames))
			{
				return TQAProfileType.tqsMQM;
			}

			return TQAProfileType.tqsOther;
		}

		public AssessmentCategories GetAssessmentCategories(ISettingsBundle settingsBundle)
		{
			var settingsGroup = settingsBundle.GetSettingsGroup("TranslationQualityAssessmentSettings");
			if (settingsGroup.ContainsSetting("AssessmentCategories"))
			{
				return settingsGroup.GetSetting<AssessmentCategories>("AssessmentCategories");
			}

			return null;
		}

		private bool AllMQMCategoriesAndSubcategoriesAreCompatible(AssessmentCategories tqaProjectCategories, List<string> categoriesNames)
		{
			//checks if the categories from the current TQA profile are the same (perfect match) with TQS_MQM_Categories-fixed/hardcoded MQM TQAStandard. 
			if (!AllTQACategoriesAreCompatible(TQS_MQM_Categories, categoriesNames))
			{
				return false;
			}

			//for each MQM Profile category checks  if their subcategories are matching the TQS_MQMCategory Standard
			foreach (var category in tqaProjectCategories)//Categories from MQM imported profile
			{
				if (!AllSubcategoriesOfTQSProfileArePresentInTQS_MQMCategory(category.Id, category.SubCategories))
				{
					return false;
				}
			}

			//checks if the categories and their subcategories from TQS_MQM_Categories(fixed MQM TQAStandard) are compatible with the categories and subcategories from the current TQA profile.
			//In the profile imported may have fewer like the TQS_MQM_SubCategories
			foreach (var categoryId in TQS_MQM_Categories.Keys)
			{
				var mqmProfileSubcategories = tqaProjectCategories.Where(categ => categ.Id == categoryId).First().SubCategories;
				if (!AllTQA_MQMSubcategoryArePresentInTQSProfile(categoryId, mqmProfileSubcategories))
				{
					return false;
				}
			}

			return true;
		}

		private bool AllTQACategoriesAreCompatible(Dictionary<Guid, string> TQSCategories, List<string> taProfileCategoriesNames)
		{   //cross checks for categories full match between TQA Project Imported Profile vs TQA Standard profile (J2450 or MQM)
			return taProfileCategoriesNames.All(catName => TQSCategories.ContainsValue(catName))
				&& TQSCategories.Values.All(cat => taProfileCategoriesNames.Contains(cat));
		}
		private bool AllSubcategoriesOfTQSProfileArePresentInTQS_MQMCategory(Guid mqmProfileCategoryId, SubCategories mqmProfileSubcategories)
		{
			//get all subcategories names from the current TQS_MQM_Category
			var subcategsNamesInMQMStandardCategory = GetMQMStandardSubcategoriesNamesFor(mqmProfileCategoryId);
			//verify if each subcategory form imported MQM Profile is part of the MQM Standard
			return mqmProfileSubcategories.Select(item => item.Name).ToList().All(subcat => subcategsNamesInMQMStandardCategory.Contains(subcat));
		}

		private bool AllTQA_MQMSubcategoryArePresentInTQSProfile(Guid tqs_MQMCategoryId, SubCategories mqmProfileSubcategories)
		{   //get all subcategories names from the current TQS_MQM_Category
			var subcategsNamesInMQMStandardCategory = GetMQMStandardSubcategoriesNamesFor(tqs_MQMCategoryId);
			return subcategsNamesInMQMStandardCategory.All(subcat => mqmProfileSubcategories.Select(item => item.Name).ToList().Contains(subcat));
		}

		private List<string> GetMQMStandardSubcategoriesNamesFor(Guid tqs_MQMCategoryId)
		{
			//get all subcategories of TQS_MQM_Category from TQA MQM Standard (Standard means the subcategories that are in MQM-Standard Profile)
			var subcategsInMQMStandardCategory = TQS_MQM_SubCategories.Where(subcateglist => subcateglist.Key == tqs_MQMCategoryId).Select(subcategs => subcategs.Value).First();
			//return all subcategories names from TQA MQM Standard values of category specified by tqs_MQMCategoryId
			return subcategsInMQMStandardCategory.Values.ToList();
		}
	}

}


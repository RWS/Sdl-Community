using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Model.TM;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using FieldValue = Sdl.Community.SdlTmAnonymizer.Model.FieldDefinitions.FieldValue;

namespace Sdl.Community.SdlTmAnonymizer.TM.SqliteTM
{
	public class Query
	{
		private readonly SQLiteConnection _connection;
		private readonly SerializerService _serializerService;
		private readonly SegmentService _segmentService;
		private readonly DateTime _minDate = new DateTime(1900, 01, 01, 0, 0, 0, DateTimeKind.Utc);

		public Query(string databasePath, string password, SerializerService serializerService, SegmentService segmentService)
		{
			_serializerService = serializerService;
			_segmentService = segmentService;
			_connection = new SQLiteConnection(GetConnectionString(databasePath, password));
			_connection.Open();
		}

		public void OpenConnection()
		{
			if (_connection.State == ConnectionState.Closed)
			{
				_connection.Open();
			}
		}

		public void CloseConnection()
		{
			_connection?.Close();
		}

		/// <summary>
		/// Get translation memories
		/// </summary>
		/// <returns>A list of translation memories</returns>
		public List<TranslationMemory> GeTranslationMemories()
		{
			var values = new List<TranslationMemory>();

			var tableName = "translation_memories";
			var sqlQuery = "SELECT " +
							"id, " + //0
							"guid, " + //1
							"name, " + //2
							"source_language, " + //3
							"target_language, " + //4
							"copyright, " + //5
							"description, " + //6
							"creation_user, " + //7
							"creation_date, " + //8
							"tucount " + //9
							"FROM " + tableName + " ";

			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				var rdrSelect = cmdQuery.ExecuteReader();
				try
				{
					if (rdrSelect.HasRows)
					{
						while (rdrSelect.Read())
						{
							values.Add(CreateTranslationMemory(rdrSelect));
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error querying " + tableName + "!  " + ex.Message);
				}
				finally
				{
					if (rdrSelect != null)
						if (!rdrSelect.IsClosed)
							rdrSelect.Close();
				}
			}

			return values;
		}

		/// <summary>
		/// Get TM attributes
		/// </summary>
		/// <param name="ids">A list of TM IDs</param>
		/// <returns>A list of TM attributes</returns>
		public List<TmAttribute> GetTmAttributes(List<int> ids)
		{
			var values = new List<TmAttribute>();

			var tableName = "attributes";
			var sqlQuery = "SELECT " +
							"id, " + //0
							"guid, " + //1
							"name, " + //2
							"type, " + //3
							"tm_id " + //4
							"FROM " + tableName + " " +
							"WHERE tm_id IN (" + GetIdsString(ids) + ")";

			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				var rdrSelect = cmdQuery.ExecuteReader();
				try
				{
					if (rdrSelect.HasRows)
					{
						while (rdrSelect.Read())
						{
							values.Add(CreateAttribute(rdrSelect));
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error querying " + tableName + "!  " + ex.Message);
				}
				finally
				{
					if (rdrSelect != null)
						if (!rdrSelect.IsClosed)
							rdrSelect.Close();
				}
			}

			return values;
		}

		/// <summary>
		/// Get string attributes
		/// </summary>
		/// <param name="ids">A list of TM attribute IDs</param>
		/// <returns>A list of string attributes</returns>
		public Dictionary<int, List<StringAttribute>> GetStringAttributes(List<int> ids)
		{
			var values = new Dictionary<int, List<StringAttribute>>();

			var tableName = "string_attributes";
			var sqlQuery = "SELECT " +
						   "translation_unit_id, " + //0
						   "attribute_id, " + //1
						   "value " + //2
						   "FROM " + tableName + " " +
						   "WHERE attribute_id IN (" + GetIdsString(ids) + ")";

			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				var rdrSelect = cmdQuery.ExecuteReader();
				try
				{
					if (rdrSelect.HasRows)
					{
						while (rdrSelect.Read())
						{
							var item = CreateStringAttribute(rdrSelect);

							if (values.ContainsKey(item.TranslationUnitId))
							{
								values[item.TranslationUnitId].Add(item);
							}
							else
							{
								values.Add(item.TranslationUnitId, new List<StringAttribute> { item });
							}
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error querying " + tableName + "!  " + ex.Message);
				}
				finally
				{
					if (rdrSelect != null)
						if (!rdrSelect.IsClosed)
							rdrSelect.Close();
				}
			}

			return values;
		}

		/// <summary>
		/// Get numberic attributes
		/// </summary>
		/// <param name="ids">A list of TM attribute IDs</param>
		/// <returns>A list of numeric attributes</returns>
		public Dictionary<int, List<NumericAttribute>> GetNumericAttributes(List<int> ids)
		{
			var values = new Dictionary<int, List<NumericAttribute>>();

			var tableName = "numeric_attributes";
			var sqlQuery = "SELECT " +
						   "translation_unit_id, " + //0
						   "attribute_id, " + //1
						   "value " + //2
						   "FROM " + tableName + " " +
						   "WHERE attribute_id IN (" + GetIdsString(ids) + ")";

			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				var rdrSelect = cmdQuery.ExecuteReader();
				try
				{
					if (rdrSelect.HasRows)
					{
						while (rdrSelect.Read())
						{
							var item = CreateNumericAttribute(rdrSelect);

							if (values.ContainsKey(item.TranslationUnitId))
							{
								values[item.TranslationUnitId].Add(item);
							}
							else
							{
								values.Add(item.TranslationUnitId, new List<NumericAttribute> { item });
							}
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error querying " + tableName + "!  " + ex.Message);
				}
				finally
				{
					if (rdrSelect != null)
						if (!rdrSelect.IsClosed)
							rdrSelect.Close();
				}
			}

			return values;
		}

		/// <summary>
		/// Get date attributes
		/// </summary>
		/// <param name="ids">A list of TM attribute IDs</param>
		/// <returns>A list of date attributes</returns>
		public Dictionary<int, List<DateAttribute>> GetDateAttributes(List<int> ids)
		{
			var values = new Dictionary<int, List<DateAttribute>>();

			var tableName = "date_attributes";
			var sqlQuery = "SELECT " +
						   "translation_unit_id, " + //0
						   "attribute_id, " + //1
						   "value " + //2
						   "FROM " + tableName + " " +
						   "WHERE attribute_id IN (" + GetIdsString(ids) + ")";

			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				var rdrSelect = cmdQuery.ExecuteReader();
				try
				{
					if (rdrSelect.HasRows)
					{
						while (rdrSelect.Read())
						{
							var item = CreateDateAttribute(rdrSelect);

							if (values.ContainsKey(item.TranslationUnitId))
							{
								values[item.TranslationUnitId].Add(item);
							}
							else
							{
								values.Add(item.TranslationUnitId, new List<DateAttribute> { item });
							}
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error querying " + tableName + "!  " + ex.Message);
				}
				finally
				{
					if (rdrSelect != null)
						if (!rdrSelect.IsClosed)
							rdrSelect.Close();
				}
			}

			return values;
		}

		/// <summary>
		/// Get picklist values
		/// </summary>
		/// <param name="ids">A list of TM attribute IDs</param>
		/// <returns>A list of picklist values</returns>
		public List<PickListValue> GetPickListValues(List<int> ids)
		{
			var values = new List<PickListValue>();

			var tableName = "picklist_values";
			var sqlQuery = "SELECT " +
						   "id, " + //0
						   "guid, " + //1
						   "attribute_id, " + //2
						   "value " + //3
						   "FROM " + tableName + " " +
						   "WHERE attribute_id IN (" + GetIdsString(ids) + ")";

			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				var rdrSelect = cmdQuery.ExecuteReader();
				try
				{
					if (rdrSelect.HasRows)
					{
						while (rdrSelect.Read())
						{
							values.Add(CreatePickListValue(rdrSelect));
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error querying " + tableName + "!  " + ex.Message);
				}
				finally
				{
					if (rdrSelect != null)
						if (!rdrSelect.IsClosed)
							rdrSelect.Close();
				}
			}

			return values;
		}

		/// <summary>
		/// Get picklist attributes
		/// </summary>
		/// <param name="ids">A list of picklist value IDs</param>
		/// <returns>A list of picklist attributes</returns>
		public Dictionary<int, List<PickListAttribute>> GetPickListAttributes(List<int> ids)
		{
			var values = new Dictionary<int, List<PickListAttribute>>();

			var tableName = "picklist_attributes";
			var sqlQuery = "SELECT " +
						   "translation_unit_id, " + //0
						   "picklist_value_id " + //1
						   "FROM " + tableName + " " +
						   "WHERE picklist_value_id IN (" + GetIdsString(ids) + ")";

			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				var rdrSelect = cmdQuery.ExecuteReader();
				try
				{
					if (rdrSelect.HasRows)
					{
						while (rdrSelect.Read())
						{
							var item = CreatePickListAttribute(rdrSelect);

							if (values.ContainsKey(item.TranslationUnitId))
							{
								values[item.TranslationUnitId].Add(item);
							}
							else
							{
								values.Add(item.TranslationUnitId, new List<PickListAttribute> { item });
							}
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error querying " + tableName + "!  " + ex.Message);
				}
				finally
				{
					if (rdrSelect != null)
						if (!rdrSelect.IsClosed)
							rdrSelect.Close();
				}
			}

			return values;
		}

		/// <summary>
		/// Get translation units
		/// </summary>
		/// <param name="context"></param>
		/// <param name="ids">A list of TM IDs</param>
		/// <returns>A list of Translation Units</returns>
		public List<TmTranslationUnit> GetTranslationUnits(ProgressDialogContext context, List<int> ids)
		{
			var values = new List<TmTranslationUnit>();

			decimal iTotalUnits = GetTranslationUnitsCount(ids);

			var tableName = "translation_units";
			var sqlQuery = "SELECT " +
						   "id, " + //0
						   "guid, " + //1
						   "source_segment, " + //2
						   "target_segment, " + //3
						   "creation_date, " + //4
						   "creation_user, " + //5
						   "change_date, " + //6
						   "change_user, " + //7
						   "last_used_date, " + //8
						   "last_used_user, " + //9
						   "usage_counter " + //10
						   "FROM " + tableName + " " +
						   "WHERE translation_memory_id IN (" + GetIdsString(ids) + ")";

			var serializer = new XmlSerializer(typeof(Segment), new[]{
				typeof(Tag),
				typeof(Text)
			});

			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				decimal iCurrent = 0;
				var rdrSelect = cmdQuery.ExecuteReader();
				try
				{
					if (rdrSelect.HasRows)
					{
						while (rdrSelect.Read())
						{
							iCurrent++;

							if (iCurrent % 1000 == 0)
							{
								var progress = iCurrent++ / iTotalUnits * 100;
								context?.Report(Convert.ToInt32(progress),
									"Reading: " + iCurrent + " of " + iTotalUnits + " Translation Units");
							}

							values.Add(CreateTranslationUnit(rdrSelect, serializer));
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error querying " + tableName + "!  " + ex.Message);
				}
				finally
				{
					if (rdrSelect != null)
						if (!rdrSelect.IsClosed)
							rdrSelect.Close();
				}
			}

			context?.Report(Convert.ToInt32(values.Count), "Reading: done!");

			AddCustomFields(context, ids, values);

			return values;
		}

		public void UpdateSystemFields(ProgressDialogContext context, List<TmTranslationUnit> units)
		{
			decimal iTotalUnits = units.Count;

			var tableName = "translation_units";
			var sqlQuery = "UPDATE " + tableName + " SET " +
						   "creation_date = @creation_date, " +
						   "creation_user = @creation_user, " +
						   "change_date = @change_date, " +
						   "change_user = @change_user, " +
						   "last_used_date = @last_used_date, " +
						   "last_used_user = @last_used_user, " +
						   "usage_counter = @usage_counter " +
						   "WHERE id = @id";

			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				decimal iCurrent = 0;
				foreach (var unit in units)
				{
					iCurrent++;

					if (iCurrent % 100 == 0)
					{
						var progress = iCurrent++ / iTotalUnits * 100;
						context?.Report(Convert.ToInt32(progress),
							"Updating: " + iCurrent + " of " + iTotalUnits + " Translation Units");
					}

					cmdQuery.Parameters.Add("@id", DbType.Int32).Value = unit.ResourceId.Id;
					cmdQuery.Parameters.Add("@creation_date", DbType.String).Value = NormalizeToString(unit.SystemFields.ChangeDate);
					cmdQuery.Parameters.Add("@creation_user", DbType.String).Value = unit.SystemFields.ChangeUser;
					cmdQuery.Parameters.Add("@change_date", DbType.String).Value = NormalizeToString(unit.SystemFields.ChangeDate);
					cmdQuery.Parameters.Add("@change_user", DbType.String).Value = unit.SystemFields.ChangeUser;
					cmdQuery.Parameters.Add("@last_used_date", DbType.String).Value = NormalizeToString(unit.SystemFields.UseDate);
					cmdQuery.Parameters.Add("@last_used_user", DbType.String).Value = unit.SystemFields.UseUser;
					cmdQuery.Parameters.Add("@usage_counter", DbType.Int32).Value = unit.SystemFields.UseCount;

					//TODO log result; -1: error; >=0: number of records updated
					var result = cmdQuery.ExecuteNonQuery();
				}
			}
		}

		public void UpdateTranslationUnitContent(ProgressDialogContext context, List<TmTranslationUnit> units)
		{
			decimal iTotalUnits = units.Count;

			var tableName = "translation_units";
			var sqlQuery = "UPDATE " + tableName + " SET " +
						   "source_segment = @source_segment, " +
						   "target_segment = @target_segment " +
						   "WHERE id = @id";

			var serializer = new XmlSerializer(typeof(Segment), new[]{
				typeof(Tag),
				typeof(Text)
			});

			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				decimal iCurrent = 0;
				foreach (var unit in units)
				{
					iCurrent++;

					if (iCurrent % 100 == 0)
					{
						var progress = iCurrent++ / iTotalUnits * 100;
						context?.Report(Convert.ToInt32(progress),
							"Updating: " + iCurrent + " of " + iTotalUnits + " Translation Units");
					}

					var sourceXml = _serializerService.Serialize(_segmentService.BuildSegment(
						new CultureInfo(unit.SourceSegment.Language), unit.SourceSegment.Elements), serializer);

					var targetXml = _serializerService.Serialize(_segmentService.BuildSegment(
						new CultureInfo(unit.TargetSegment.Language), unit.TargetSegment.Elements), serializer);

					cmdQuery.Parameters.Add("@id", DbType.Int32).Value = unit.ResourceId.Id;
					cmdQuery.Parameters.Add("@source_segment", DbType.String).Value = sourceXml;
					cmdQuery.Parameters.Add("@target_segment", DbType.String).Value = targetXml;

					//TODO log result; -1: error; >=0: number of records updated
					var result = cmdQuery.ExecuteNonQuery();
				}
			}
		}

		public int GetTranslationUnitsCount(List<int> ids)
		{
			var value = 0;

			var idsString = string.Empty;
			foreach (var id in ids)
			{
				idsString += (idsString != string.Empty ? "," : string.Empty) + id;
			}

			var tableName = "translation_units";
			var sqlQuery = "SELECT COUNT(*) as record_count " +
						   "FROM " + tableName + " " +
						   "WHERE translation_memory_id IN (" + idsString + ")";


			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				var rdrSelect = cmdQuery.ExecuteReader();
				try
				{
					if (rdrSelect.HasRows)
					{
						rdrSelect.Read();

						value = Convert.ToInt32(rdrSelect["record_count"]);
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error querying " + tableName + "!  " + ex.Message);
				}
				finally
				{
					if (rdrSelect != null)
						if (!rdrSelect.IsClosed)
							rdrSelect.Close();
				}
			}

			return value;
		}

		private static string GetConnectionString(string databasePath, string password)
		{
			if (!string.IsNullOrEmpty(password))
			{
				return "Data Source=\"" + databasePath + "\";Version=3;New=False;Password" + password;
			}

			return "Data Source=\"" + databasePath + "\";Version=3;New=False";
		}

		private static TranslationMemory CreateTranslationMemory(IDataRecord rdrSelect)
		{
			var value = new TranslationMemory
			{
				Id = Convert.ToInt32(rdrSelect["id"]), //0
				Guid = rdrSelect.GetGuid(1).ToString(), //1
				Name = rdrSelect["name"].ToString().Trim(), //2
				SourceLangauge = rdrSelect["source_language"].ToString(), //3
				TargetLanguage = rdrSelect["target_language"].ToString(), //4
				Copyright = rdrSelect["copyright"].ToString(), //5
				Description = rdrSelect["description"].ToString(), //6
				CreationUser = rdrSelect["creation_user"].ToString(), //7
				CreationDate = rdrSelect.GetDateTime(8), //8
				TuCount = Convert.ToInt32(rdrSelect["tucount"]) //9
			};
			return value;
		}

		private static TmAttribute CreateAttribute(IDataRecord rdrSelect)
		{
			var value = new TmAttribute
			{
				Id = Convert.ToInt32(rdrSelect["id"]), //0
				Guid = rdrSelect.GetGuid(1).ToString(), //1
				Name = rdrSelect["name"].ToString().Trim(), //2
				Type = Convert.ToInt32(rdrSelect["type"]), //3
				TmId = Convert.ToInt32(rdrSelect["tm_id"]) //4
			};
			return value;
		}

		private static StringAttribute CreateStringAttribute(IDataRecord rdrSelect)
		{
			var value = new StringAttribute
			{
				TranslationUnitId = Convert.ToInt32(rdrSelect["translation_unit_id"]), //0																
				AttributeId = Convert.ToInt32(rdrSelect["attribute_id"]), //1
				Value = rdrSelect["value"].ToString().Trim() //2
			};
			return value;
		}

		private static NumericAttribute CreateNumericAttribute(IDataRecord rdrSelect)
		{
			var value = new NumericAttribute
			{
				TranslationUnitId = Convert.ToInt32(rdrSelect["translation_unit_id"]), //0																
				AttributeId = Convert.ToInt32(rdrSelect["attribute_id"]), //1
				Value = Convert.ToInt32(rdrSelect["value"]) //2
			};
			return value;
		}

		private static DateAttribute CreateDateAttribute(IDataRecord rdrSelect)
		{
			var value = new DateAttribute
			{
				TranslationUnitId = Convert.ToInt32(rdrSelect["translation_unit_id"]), //0																
				AttributeId = Convert.ToInt32(rdrSelect["attribute_id"]), //1
				Value = rdrSelect.GetDateTime(2) //2
			};
			return value;
		}

		private static PickListValue CreatePickListValue(IDataRecord rdrSelect)
		{
			var value = new PickListValue
			{
				Id = Convert.ToInt32(rdrSelect["id"]), //0
				Guid = rdrSelect.GetGuid(1).ToString(), //1
				AttributeId = Convert.ToInt32(rdrSelect["attribute_id"]), //2
				Value = rdrSelect["value"].ToString() //3
			};
			return value;
		}

		private static PickListAttribute CreatePickListAttribute(IDataRecord rdrSelect)
		{
			var value = new PickListAttribute
			{
				TranslationUnitId = Convert.ToInt32(rdrSelect["translation_unit_id"]), //0																
				PickListValueId = Convert.ToInt32(rdrSelect["picklist_value_id"]) //1
			};
			return value;
		}

		private TmTranslationUnit CreateTranslationUnit(IDataRecord rdrSelect, XmlSerializer serializer)
		{
			var value = new TmTranslationUnit
			{
				ResourceId = new PersistentObjectToken(Convert.ToInt32(rdrSelect["id"]), rdrSelect.GetGuid(1)),
				SourceSegment = GetTmSegment(rdrSelect["source_segment"].ToString(), serializer), //2
				TargetSegment = GetTmSegment(rdrSelect["target_segment"].ToString(), serializer), //3
				SystemFields = new SystemFields
				{
					CreationDate = rdrSelect.GetDateTime(4), //4
					CreationUser = rdrSelect["creation_user"].ToString(), //5
					ChangeDate = rdrSelect.GetDateTime(6), //6
					ChangeUser = rdrSelect["change_user"].ToString(), //7
					UseDate = rdrSelect.GetDateTime(8), //8
					UseUser = rdrSelect["last_used_user"].ToString(), //9
					UseCount = Convert.ToInt32(rdrSelect["usage_counter"]) //10
				},
				FieldValues = new List<FieldValue>()
			};

			return value;
		}

		private void AddCustomFields(ProgressDialogContext context, List<int> ids, IReadOnlyCollection<TmTranslationUnit> values)
		{
			var attributeIds = GetTmAttributes(ids);

			context?.Report(0, "Analyzing custom fields data...");

			var stringAttributes = GetStringAttributes(attributeIds.Select(a => a.Id).ToList());
			var numericAttributes = GetNumericAttributes(attributeIds.Select(a => a.Id).ToList());
			var dateAttributes = GetDateAttributes(attributeIds.Select(a => a.Id).ToList());

			var pickListValues = GetPickListValues(attributeIds.Select(a => a.Id).ToList());
			var pickListAttributes = GetPickListAttributes(pickListValues.Select(a => a.Id).ToList());

			decimal iCurrent = 0;
			decimal iTotalUnits = values.Count;
			foreach (var unit in values)
			{
				iCurrent++;
				if (iCurrent % 5000 == 0)
				{
					var progress = iCurrent / iTotalUnits * 100;
					context?.Report(Convert.ToInt32(progress),
						"Analyzing: " + iCurrent + " of " + iTotalUnits + " Translation Units");
				}

				var id = unit.ResourceId.Id;

				var stringItems = stringAttributes.ContainsKey(id) ? stringAttributes[id] : null;
				var numericItems = numericAttributes.ContainsKey(id) ? numericAttributes[id] : null;
				var dateItems = dateAttributes.ContainsKey(id) ? dateAttributes[id] : null;
				var pickListItems = pickListAttributes.ContainsKey(id) ? pickListAttributes[id] : null;

				if (stringItems != null)
				{
					foreach (var item in stringItems)
					{
						var attribute = attributeIds.FirstOrDefault(a => a.Id == item.AttributeId);

						if (attribute != null)
						{
							if (attribute.Type == 1)
							{
								var singleStringFieldValue = new Model.FieldDefinitions.SingleStringFieldValue
								{
									Name = attribute.Name,
									Value = item.Value,
									ValueType = FieldValueType.SingleString
								};
								unit.FieldValues.Add(singleStringFieldValue);
							}
							else if (attribute.Type == 2)
							{
								var multipleStringFieldValue = unit.FieldValues.FirstOrDefault(a => a.Name == attribute.Name && (int)a.ValueType == 2);
								if (multipleStringFieldValue == null)
								{
									multipleStringFieldValue = new Model.FieldDefinitions.MultipleStringFieldValue
									{
										Name = attribute.Name,
										Values = new HashSet<string>(new List<string> { item.Value }),
										ValueType = FieldValueType.MultipleString
									};
									unit.FieldValues.Add(multipleStringFieldValue);
								}
								else
								{
									multipleStringFieldValue.Add(item.Value);
								}
							}
						}
					}
				}

				if (numericItems != null)
				{
					foreach (var item in numericItems)
					{
						var attribute = attributeIds.FirstOrDefault(a => a.Id == item.AttributeId);

						if (attribute != null)
						{
							var singleStringFieldValue = new Model.FieldDefinitions.IntFieldValue
							{
								Name = attribute.Name,
								Value = item.Value,
								ValueType = FieldValueType.Integer
							};

							unit.FieldValues.Add(singleStringFieldValue);
						}
					}
				}

				if (dateItems != null)
				{
					foreach (var item in dateItems)
					{
						var attribute = attributeIds.FirstOrDefault(a => a.Id == item.AttributeId);

						if (attribute != null)
						{
							var singleStringFieldValue = new Model.FieldDefinitions.DateTimeFieldValue
							{
								Name = attribute.Name,
								Value = item.Value,
								ValueType = FieldValueType.DateTime
							};

							unit.FieldValues.Add(singleStringFieldValue);
						}
					}
				}

				if (pickListItems != null)
				{
					foreach (var item in pickListItems)
					{
						var pickListValue = pickListValues.FirstOrDefault(a => a.Id == item.PickListValueId);
						var attribute = attributeIds.FirstOrDefault(a => a.Id == pickListValue?.AttributeId);

						if (attribute != null && pickListValue != null)
						{
							if (attribute.Type == 4)
							{
								var singlePicklistFieldValue = unit.FieldValues.FirstOrDefault(a => a.Name == attribute.Name && (int)a.ValueType == 4);
								if (singlePicklistFieldValue == null)
								{
									singlePicklistFieldValue = new Model.FieldDefinitions.SinglePicklistFieldValue
									{
										Name = attribute.Name,
										Value = new PicklistItem(pickListValue.Value) { ID = pickListValue.Id },
										ValueType = FieldValueType.SinglePicklist
									};
								}

								unit.FieldValues.Add(singlePicklistFieldValue);
							}
							else if (attribute.Type == 5)
							{
								var multiplePicklistFieldValue = unit.FieldValues.FirstOrDefault(a => a.Name == attribute.Name && (int)a.ValueType == 5);
								if (multiplePicklistFieldValue == null)
								{
									multiplePicklistFieldValue = new Model.FieldDefinitions.MultiplePicklistFieldValue
									{
										Name = attribute.Name,
										Values = new List<PicklistItem>
										{
											new PicklistItem(pickListValue.Value) { ID = pickListValue.Id }
										},
										ValueType = FieldValueType.MultiplePicklist
									};

									unit.FieldValues.Add(multiplePicklistFieldValue);
								}
								else
								{
									((Model.FieldDefinitions.MultiplePicklistFieldValue)multiplePicklistFieldValue).Add(
										new PicklistItem(pickListValue.Value) { ID = pickListValue.Id });
								}
							}
						}
					}
				}
			}

			context?.Report(Convert.ToInt32(values.Count), "Analyzing: done!");
		}

		private TmSegment GetTmSegment(string content, XmlSerializer serializer)
		{
			var segment = _serializerService.Deserialize<Segment>(content, serializer);
			if (segment != null)
			{
				return new TmSegment
				{
					Elements = segment.Elements,
					Language = segment.Culture.Name
				};
			}

			return null;
		}

		private static string GetIdsString(IEnumerable<int> ids)
		{
			return ids.Aggregate(string.Empty, (current, id) => current + ((current != string.Empty ? "," : string.Empty) + id));
		}

		private string NormalizeToString(DateTime val)
		{
			if (val < _minDate)
			{
				val = _minDate;
			}

			return DateTime.SpecifyKind(
				new DateTime(val.Year, val.Month, val.Day, val.Hour, val.Minute, val.Second),
				DateTimeKind.Utc).ToString("yyyy-MM-dd HH:mm:ss");
		}
	}
}

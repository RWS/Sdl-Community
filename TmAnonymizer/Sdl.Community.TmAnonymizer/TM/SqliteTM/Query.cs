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
		/// Get Translation Units
		/// </summary>
		/// <param name="context">Progress dialog context</param>
		/// <param name="ids">A list of Translation Memory Ids</param>
		/// <returns>A list of Translation Unit</returns>
		public List<TmTranslationUnit> GetTranslationUnits(ProgressDialogContext context, List<int> ids)
		{
			if (ids == null || ids.Count == 0)
			{
				return null;
			}

			var values = new List<TmTranslationUnit>();

			decimal iTotalUnits = GetTranslationUnitsCount(ids);

			var tableName = "translation_units";
			var sqlQuery = "SELECT " +
						   "id, " + //0
						   "guid, " + //1
						   "translation_memory_id, " + //2
						   "source_segment, " + //3
						   "target_segment, " + //4
						   "creation_date, " + //5
						   "creation_user, " + //6
						   "change_date, " + //7
						   "change_user, " + //8
						   "last_used_date, " + //9
						   "last_used_user, " + //10
						   "usage_counter " + //11
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

		/// <summary>
		/// Update System Fields
		/// </summary>
		/// <param name="context">Progress dialog context</param>
		/// <param name="units">Translation Units with updated System Field data</param>
		public void UpdateSystemFields(ProgressDialogContext context, List<TmTranslationUnit> units)
		{
			if (units == null || units.Count == 0)
			{
				return;
			}

			decimal iTotalUnits = units.Count;

			var tableName = "translation_units";
			var sqlQuery = "UPDATE " + tableName + " SET " +
						   "creation_user = @creation_user, " +
						   "change_user = @change_user, " +
						   "last_used_user = @last_used_user " +
						   "WHERE id = @id";

			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
				cmdQuery.Parameters.Add(new SQLiteParameter("@creation_user", DbType.String));
				cmdQuery.Parameters.Add(new SQLiteParameter("@change_user", DbType.String));
				cmdQuery.Parameters.Add(new SQLiteParameter("@last_used_user", DbType.String));

				using (var transaction = _connection.BeginTransaction())
				{
					decimal iCurrent = 0;
					foreach (var unit in units)
					{
						iCurrent++;

						if (iCurrent % 1000 == 0)
						{
							if (context != null && context.CheckCancellationPending())
							{
								break;
							}

							var progress = iCurrent++ / iTotalUnits * 100;
							context?.Report(Convert.ToInt32(progress),
								"Updating: " + iCurrent + " of " + iTotalUnits + " Translation Units");
						}

						cmdQuery.Parameters["@id"].Value = unit.ResourceId.Id;
						cmdQuery.Parameters["@creation_user"].Value = unit.SystemFields.CreationUser;
						cmdQuery.Parameters["@change_user"].Value = unit.SystemFields.ChangeUser;
						cmdQuery.Parameters["@last_used_user"].Value = unit.SystemFields.UseUser;

						//TODO log result; -1: error; >=0: number of records updated
						var result = cmdQuery.ExecuteNonQuery();
					}

					transaction.Commit();
				}
			}
		}

		/// <summary>
		/// Update Custom Fields
		/// </summary>
		/// <param name="context">Progress dialog context</param>
		/// <param name="units">Translation Units with updated Custom Field data</param>
		public void UpdateCustomFields(ProgressDialogContext context, List<TmTranslationUnit> units)
		{
			if (units == null || units.Count == 0)
			{
				return;
			}

			var attributeIds = GetTmAttributes(units.Select(a => a.TmId).Distinct().ToList());

			var singleStringFieldValues = new List<Model.FieldDefinitions.SingleStringFieldValue>();
			var multpleStringFieldValues = new List<Model.FieldDefinitions.MultipleStringFieldValue>();
			var intFieldValues = new List<Model.FieldDefinitions.IntFieldValue>();
			var dateFieldValues = new List<Model.FieldDefinitions.DateTimeFieldValue>();

			foreach (var unit in units)
			{
				foreach (var value in unit.FieldValues)
				{
					if (value.ValueType == FieldValueType.SingleString &&
						value is Model.FieldDefinitions.SingleStringFieldValue singleStringFieldValue)
					{
						var item = singleStringFieldValues.FirstOrDefault(
							a => a.Name == singleStringFieldValue.Name &&
								 a.Value == singleStringFieldValue.Value &&
								 a.PreviousValue == singleStringFieldValue.PreviousValue);

						if (item == null && singleStringFieldValue.PreviousValue != null)
						{
							singleStringFieldValues.Add(singleStringFieldValue);
						}
					}

					if (value.ValueType == FieldValueType.MultipleString &&
						value is Model.FieldDefinitions.MultipleStringFieldValue multipleStringFieldValue)
					{
						var item = multpleStringFieldValues.FirstOrDefault(
							a => a.Name == multipleStringFieldValue.Name &&
								 a.Values == multipleStringFieldValue.Values &&
								 a.PreviousValues == multipleStringFieldValue.PreviousValues);

						if (item == null && multipleStringFieldValue.PreviousValues != null)
						{
							multpleStringFieldValues.Add(multipleStringFieldValue);
						}
					}

					if (value.ValueType == FieldValueType.Integer &&
						value is Model.FieldDefinitions.IntFieldValue intFieldValue)
					{
						var item = intFieldValues.FirstOrDefault(
							a => a.Name == intFieldValue.Name &&
								 a.Value == intFieldValue.Value &&
								 a.PreviousValue == intFieldValue.PreviousValue);

						if (item == null && intFieldValue.PreviousValue != null)
						{
							intFieldValues.Add(intFieldValue);
						}
					}

					if (value.ValueType == FieldValueType.DateTime &&
						value is Model.FieldDefinitions.DateTimeFieldValue dateFieldValue)
					{
						var item = dateFieldValues.FirstOrDefault(
							a => a.Name == dateFieldValue.Name &&
								 a.Value == dateFieldValue.Value &&
								 a.PreviousValue == dateFieldValue.PreviousValue);

						if (item == null && dateFieldValue.PreviousValue != null)
						{
							dateFieldValues.Add(dateFieldValue);
						}
					}
				}
			}

			//TODO: add logic to update picklists; currently the picklists are updated via the TM API for convinience...

			UpdateCustomFields(context, FieldValueType.SingleString, singleStringFieldValues, attributeIds);
			UpdateCustomFields(context, FieldValueType.MultipleString, multpleStringFieldValues, attributeIds);
			UpdateCustomFields(context, FieldValueType.Integer, intFieldValues, attributeIds);
			UpdateCustomFields(context, FieldValueType.DateTime, dateFieldValues, attributeIds);
		}

		/// <summary>
		/// Update Translation Unit content
		/// </summary>
		/// <param name="context">Progress dialog context</param>
		/// <param name="units">Translation Units with updated source and/or target data</param>
		public void UpdateTranslationUnitContent(ProgressDialogContext context, List<TmTranslationUnit> units)
		{
			if (units == null || units.Count == 0)
			{
				return;
			}

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
				cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
				cmdQuery.Parameters.Add(new SQLiteParameter("@source_segment", DbType.String));
				cmdQuery.Parameters.Add(new SQLiteParameter("@target_segment", DbType.String));

				using (var transaction = _connection.BeginTransaction())
				{
					decimal iCurrent = 0;
					foreach (var unit in units)
					{
						iCurrent++;

						if (iCurrent % 1000 == 0)
						{
							if (context != null && context.CheckCancellationPending())
							{
								break;
							}

							var progress = iCurrent++ / iTotalUnits * 100;
							context?.Report(Convert.ToInt32(progress),
								"Updating: " + iCurrent + " of " + iTotalUnits + " Translation Units");
						}

						var sourceXml = _serializerService.Serialize(_segmentService.BuildSegment(
							new CultureInfo(unit.SourceSegment.Language), unit.SourceSegment.Elements), serializer);

						var targetXml = _serializerService.Serialize(_segmentService.BuildSegment(
							new CultureInfo(unit.TargetSegment.Language), unit.TargetSegment.Elements), serializer);

						cmdQuery.Parameters["@id"].Value = unit.ResourceId.Id;
						cmdQuery.Parameters["@source_segment"].Value = sourceXml;
						cmdQuery.Parameters["@target_segment"].Value = targetXml;

						//TODO log result; -1: error; >=0: number of records updated
						var result = cmdQuery.ExecuteNonQuery();
					}

					transaction.Commit();
				}
			}
		}

		/// <summary>
		/// Get Translation Unit count
		/// </summary>
		/// <param name="ids">A list of Translation Memory Ids</param>
		/// <returns>Translation Unit count</returns>
		public int GetTranslationUnitsCount(List<int> ids)
		{
			if (ids == null || ids.Count == 0)
			{
				return 0;
			}

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
				TmId = Convert.ToInt32(rdrSelect["translation_memory_id"]), //2
				SourceSegment = GetTmSegment(rdrSelect["source_segment"].ToString(), serializer), //3
				TargetSegment = GetTmSegment(rdrSelect["target_segment"].ToString(), serializer), //4
				SystemFields = new SystemFields
				{
					CreationDate = rdrSelect.GetDateTime(5), //5
					CreationUser = rdrSelect["creation_user"].ToString(), //6
					ChangeDate = rdrSelect.GetDateTime(7), //7
					ChangeUser = rdrSelect["change_user"].ToString(), //8
					UseDate = rdrSelect.GetDateTime(9), //9
					UseUser = rdrSelect["last_used_user"].ToString(), //10
					UseCount = Convert.ToInt32(rdrSelect["usage_counter"]) //11
				},
				FieldValues = new List<Model.FieldDefinitions.FieldValue>()
			};

			return value;
		}

		private void AddCustomFields(ProgressDialogContext context, List<int> ids, IReadOnlyCollection<TmTranslationUnit> values)
		{
			if (values == null || values.Count == 0)
			{
				return;
			}

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
										Values = new HashSet<string>(new List<string>
										{
											item.Value
										}),
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
										Value = new Model.FieldDefinitions.PicklistItem(pickListValue.Value)
										{
											ID = pickListValue.Id
										},
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
										Values = new List<Model.FieldDefinitions.PicklistItem>
										{
											new Model.FieldDefinitions.PicklistItem(pickListValue.Value)
											{
												ID = pickListValue.Id
											}
										},
										ValueType = FieldValueType.MultiplePicklist
									};

									unit.FieldValues.Add(multiplePicklistFieldValue);
								}
								else
								{
									((Model.FieldDefinitions.MultiplePicklistFieldValue)multiplePicklistFieldValue).Add(
										new Model.FieldDefinitions.PicklistItem(pickListValue.Value)
										{
											ID = pickListValue.Id
										});
								}
							}
						}
					}
				}
			}

			context?.Report(Convert.ToInt32(values.Count), "Analyzing: done!");
		}

		private void UpdateCustomFields(ProgressDialogContext context, FieldValueType fieldValueType, IReadOnlyCollection<Model.FieldDefinitions.FieldValue> fieldValues, List<TmAttribute> attributeIds)
		{
			if (fieldValues == null || fieldValues.Count == 0)
			{
				return;
			}

			decimal iTotalUnits = fieldValues.Count;

			var tableName = string.Empty;
			switch (fieldValueType)
			{
				case FieldValueType.SingleString:
				case FieldValueType.MultipleString:
					tableName = "string_attributes";
					break;
				case FieldValueType.DateTime:
					tableName = "date_attributes";
					break;
				case FieldValueType.Integer:
					tableName = "numeric_attributes";
					break;
			}

			var sqlQuery = "UPDATE " + tableName + " SET " +
						   "value = @value " +
						   "WHERE attribute_id = @attribute_id AND value = @previous_value";

			using (var cmdQuery = new SQLiteCommand(sqlQuery, _connection))
			{
				cmdQuery.Parameters.Add(new SQLiteParameter("@attribute_id", DbType.Int32));

				switch (fieldValueType)
				{
					case FieldValueType.SingleString:
					case FieldValueType.MultipleString:
						cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.String));
						cmdQuery.Parameters.Add(new SQLiteParameter("@previous_value", DbType.String));
						break;
					case FieldValueType.DateTime:
						cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.DateTime));
						cmdQuery.Parameters.Add(new SQLiteParameter("@previous_value", DbType.DateTime));
						break;
					case FieldValueType.Integer:
						cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.Int32));
						cmdQuery.Parameters.Add(new SQLiteParameter("@previous_value", DbType.Int32));
						break;
				}

				using (var transaction = _connection.BeginTransaction())
				{
					decimal iCurrent = 0;
					foreach (var field in fieldValues)
					{
						iCurrent++;

						if (iCurrent % 1000 == 0)
						{
							if (context != null && context.CheckCancellationPending())
							{
								break;
							}

							var progress = iCurrent++ / iTotalUnits * 100;
							context?.Report(Convert.ToInt32(progress),
								"Updating: " + iCurrent + " of " + iTotalUnits + " '" + field.ValueType + "' values");
						}

						var attributes = attributeIds.Where(a => a.Type == (int)field.ValueType && a.Name == field.Name).ToList();

						if (attributes.Count > 0)
						{
							foreach (var attribute in attributes)
							{
								var result = 0;
								switch (fieldValueType)
								{
									case FieldValueType.SingleString:
										var singleStringFieldValue = field as Model.FieldDefinitions.DateTimeFieldValue;
										if (singleStringFieldValue?.PreviousValue != null && singleStringFieldValue.Value != singleStringFieldValue.PreviousValue)
										{
											result = UpdateCustomFields(cmdQuery, attribute, singleStringFieldValue);
										}

										break;
									case FieldValueType.MultipleString:
										if (field is Model.FieldDefinitions.MultipleStringFieldValue multipleStringFieldValue)
										{
											for (var i = 0; i < multipleStringFieldValue.Values.Count; i++)
											{
												var multipleStringFieldValues = multipleStringFieldValue.Values.ToList();
												var multipleStringFieldPreviousValues = multipleStringFieldValue.PreviousValues.ToList();
												if (multipleStringFieldPreviousValues[i] != null && multipleStringFieldValues[i] != multipleStringFieldPreviousValues[i])
												{
													var stringFieldValue = new Model.FieldDefinitions.SingleStringFieldValue
													{
														Name = multipleStringFieldValue.Name,
														Value = multipleStringFieldValues[i],
														PreviousValue = multipleStringFieldPreviousValues[i]
													};
													result = UpdateCustomFields(cmdQuery, attribute, stringFieldValue);
												}
											}
										}
										break;
									case FieldValueType.DateTime:
										var dateTimeFieldValue = field as Model.FieldDefinitions.DateTimeFieldValue;
										if (dateTimeFieldValue?.PreviousValue != null && dateTimeFieldValue.Value != dateTimeFieldValue.PreviousValue)
										{
											result = UpdateCustomFields(cmdQuery, attribute, dateTimeFieldValue);
										}
										break;
									case FieldValueType.Integer:
										var intFieldValue = field as Model.FieldDefinitions.IntFieldValue;
										if (intFieldValue?.PreviousValue != null && intFieldValue.Value != intFieldValue.PreviousValue)
										{
											result = UpdateCustomFields(cmdQuery, attribute, intFieldValue);
										}
										break;
								}

								//TODO log result; -1: error; >=0: number of records updated
								if (result > 0)
								{

								}
							}
						}
					}

					transaction.Commit();
				}
			}
		}

		private static int UpdateCustomFields(SQLiteCommand cmdQuery, TmAttribute attribute, Model.FieldDefinitions.SingleStringFieldValue field)
		{
			cmdQuery.Parameters["@attribute_id"].Value = attribute.Id;
			cmdQuery.Parameters["@value"].Value = field.Value;
			cmdQuery.Parameters["@previous_value"].Value = field.PreviousValue;

			//TODO log result; -1: error; >=0: number of records updated
			var result = cmdQuery.ExecuteNonQuery();

			field.PreviousValue = null;

			return result;
		}

		private static int UpdateCustomFields(SQLiteCommand cmdQuery, TmAttribute attribute, Model.FieldDefinitions.IntFieldValue field)
		{
			cmdQuery.Parameters["@attribute_id"].Value = attribute.Id;
			cmdQuery.Parameters["@value"].Value = field.Value;
			cmdQuery.Parameters["@previous_value"].Value = field.PreviousValue;

			//TODO log result; -1: error; >=0: number of records updated
			var result = cmdQuery.ExecuteNonQuery();

			field.PreviousValue = null;

			return result;
		}

		private static int UpdateCustomFields(SQLiteCommand cmdQuery, TmAttribute attribute, Model.FieldDefinitions.DateTimeFieldValue field)
		{
			cmdQuery.Parameters["@attribute_id"].Value = attribute.Id;
			cmdQuery.Parameters["@value"].Value = field.Value;
			cmdQuery.Parameters["@previous_value"].Value = field.PreviousValue;

			//TODO log result; -1: error; >=0: number of records updated
			var result = cmdQuery.ExecuteNonQuery();

			field.PreviousValue = null;

			return result;
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.QuickTags
{
	[Serializable]
	public class MarkupDataContainer : IAbstractMarkupDataContainer, ICloneable
	{
		#region Data members

		//NOTE: should NOT be serialized, nor used in Equals() and GetHashCode() implementations!
		[NonSerialized]
		int _uniqueId = 0;

		protected List<IAbstractMarkupData> _Content = new List<IAbstractMarkupData>();
		protected internal IAbstractMarkupDataContainer _PublicContainer;


		#endregion Data members
		#region Construction


		public MarkupDataContainer()
		{
			_PublicContainer = this;
		}


		/// <summary>
		/// Constructor used when this object is used as a data member providing
		/// the implementation of a public IAbstractMarkupDataContainer interface.
		/// 
		/// The hosting class should pass "this" as the public container.
		/// That ensures that locations and Parent properties for items in the
		/// collection is set correctly.
		/// </summary>
		/// <param name="publicContainer"></param>
		public MarkupDataContainer(IAbstractMarkupDataContainer publicContainer)
		{
			_PublicContainer = publicContainer;
		}


		/// <summary>
		/// Creates a clone of the other collection, where each
		/// item in the other collection is cloned (i.e. a deep copy).
		/// </summary>
		/// <param name="other"></param>
		public MarkupDataContainer(MarkupDataContainer other)
		{
			_PublicContainer = this;

			foreach (IAbstractMarkupData item in other)
			{
				Add((IAbstractMarkupData)item.Clone());
			}

		}


		#endregion Construction
		#region overrides


		/// <summary>
		/// <c>true</c> if the content of the container is equal
		/// to the content of the other container, item by item.
		/// 
		/// Explicitly does not compare the PublicContainer property,
		/// as that would not make much sense.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			//       
			// See the full list of guidelines at
			//   http://go.microsoft.com/fwlink/?LinkID=85237  
			// and also the guidance for operator== at
			//   http://go.microsoft.com/fwlink/?LinkId=85238
			//

			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			MarkupDataContainer other = (MarkupDataContainer)obj;
			if ((_Content == null) != (other._Content == null))
			{
				return false;
			}
			if (_Content != null)
			{
				// NOTE: calling Equals on the collection will not compare each of the list members,
				// we need to implement that ourselves...
				if (_Content.Count != other._Content.Count)
				{
					return false;
				}

				for (int i = 0; i < _Content.Count; ++i)
				{
					IAbstractMarkupData thisItem = _Content[i];
					IAbstractMarkupData otherItem = other._Content[i];
					if ((thisItem == null) != (otherItem == null))
					{
						return false;
					}
					if (thisItem != null && !thisItem.Equals(otherItem))
					{
						return false;
					}
				}
			}

			// everything checks out
			return true;
		}


		// override object.GetHashCode
		public override int GetHashCode()
		{
			int contentHash = 0;
			if (_Content != null)
			{
				contentHash ^= _Content.Count;
				foreach (var item in _Content)
				{
					if (item != null)
					{
						contentHash ^= item.GetHashCode();
					}
				}
			}
			return contentHash;
		}


		// overridden to provide useful information visible in the debugger
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (IAbstractMarkupData item in _Content)
			{
				sb.Append(item.ToString());
			}
			return sb.ToString();
		}


		#endregion overrides


		/// <summary>
		/// This property should be set to the object that exposes the IAbstractMarkupDataContainer interface.
		/// It is used for locations and when setting the Parent property of items in the collection.
		/// </summary>
		public IAbstractMarkupDataContainer PublicContainer
		{
			get
			{
				return _PublicContainer;
			}
			set
			{
				if (_PublicContainer == value)
				{
					// no change
					return;
				}

				_PublicContainer = value;

				// update Parent property for all items in the container by moving all items to the public container.
				// (the items will then still end up in this container if things are set up properly!)
				// NOTE: We cannot call MoveAllItemsTo() since that uses Insert to add items to the container,
				// which won't work in our particular case, as the items go back into the very same container...
				IAbstractMarkupData[] items = _Content.ToArray();
				_Content.Clear();
				foreach (IAbstractMarkupData item in items)
				{
					item.Parent = null;
					_PublicContainer.Add(item);
				}
			}
		}


		/// <summary>
		/// Only derived classes that really know what they are doing may manipulate the list directly.
		/// </summary>
		//protected List<IAbstractMarkupData> Content
		//{
		//    get
		//    {
		//        return _Content;
		//    }
		//}


		#region IAbstractMarkupDataContainer Members


		public IEnumerable<IAbstractMarkupData> AllSubItems
		{
			get
			{
				foreach (IAbstractMarkupData node in _Content)
				{
					yield return node;

					IAbstractMarkupDataContainer container = node as IAbstractMarkupDataContainer;
					if (container != null)
					{
						// iterate over all nodes in the sub-container (recursively)
						foreach (IAbstractMarkupData subNode in container.AllSubItems)
						{
							yield return subNode;
						}
					}
				}
			}
		}


		public virtual bool CanBeSplit
		{
			get
			{
				return true;
			}
		}


		public virtual IAbstractMarkupDataContainer Split(int splitBeforeItemIndex)
		{
			if (splitBeforeItemIndex < 0 || splitBeforeItemIndex > _Content.Count)
			{
				throw new ArgumentOutOfRangeException("splitBeforeItemIndex");
			}

			// create second half
			MarkupDataContainer secondHalf = new MarkupDataContainer();

			// move everything from the split point into the second half
			MoveItemsTo(secondHalf, splitBeforeItemIndex, _Content.Count - splitBeforeItemIndex);

			return secondHalf;
		}


		public virtual IEnumerable<Location> Locations
		{
			get
			{
				for (int i = 0; i < _Content.Count; i++)
				{
					yield return new Location(new LevelLocation(_PublicContainer, i));

					IAbstractMarkupDataContainer subContainer = _Content[i] as IAbstractMarkupDataContainer;
					if (subContainer != null)
					{
						// iterate over items in the container
						foreach (Location location in subContainer.Locations)
						{
							location.Levels.Insert(0, new LevelLocation(_PublicContainer, i));
							yield return location;
						}
					}
				}

				// the position after the last item:
				yield return new Location(new LevelLocation(_PublicContainer, _Content.Count));
			}
		}


		public virtual IEnumerable<Location> GetLocationsFrom(Location startingFrom)
		{
			if (startingFrom == null)
			{
				throw new ArgumentNullException("startingFrom");
			}
			if (!startingFrom.IsValid)
			{
				throw new ArgumentException(StringResources.MarkupDataContainer_InvalidStartLocationError);
			}

			Location startLocation = (Location)startingFrom.Clone();

			// find this collection inside the location level hierarchy
			while (startLocation.Levels.Count > 0 && startLocation.Levels[0].Parent != _PublicContainer)
			{
				startLocation.Levels.RemoveAt(0);
			}
			if (startLocation.Depth == 0)
			{
				throw new ArgumentOutOfRangeException("startingFrom", StringResources.MarkupDataContainer_WrongCollectionError);
			}
			Debug.Assert(startLocation.Levels[0].Parent == _PublicContainer);

			// we have a valid start location in this collection
			for (int i = startLocation.Levels[0].Index; i < _Content.Count; i++)
			{
				if (startLocation.Depth == 1)
				{
					yield return new Location(new LevelLocation(_PublicContainer, i));
					continue;
				}

				IAbstractMarkupDataContainer subContainer = _Content[i] as IAbstractMarkupDataContainer;
				if (subContainer != null)
				{
					// iterate over items in the container
					foreach (Location location in subContainer.GetLocationsFrom(startLocation))
					{
						location.Levels.Insert(0, new LevelLocation(_PublicContainer, i));
						yield return location;
					}
				}
			}

			// the position after the last item:
			yield return new Location(new LevelLocation(_PublicContainer, _Content.Count));
		}


		public virtual Location Find(Predicate<Location> match)
		{
			foreach (Location loc in Locations)
			{
				if (match(loc))
				{
					return loc;
				}
			}

			// not found 
			return null;
		}


		public virtual Location Find(Location startAt, Predicate<Location> match)
		{
			foreach (Location loc in GetLocationsFrom(startAt))
			{
				if (match(loc))
				{
					return loc;
				}
			}

			// not found 
			return null;
		}


		public void MoveAllItemsTo(IAbstractMarkupDataContainer destinationContainer)
		{
			MoveAllItemsTo(destinationContainer, destinationContainer.Count);
		}


		public void MoveItemsTo(IAbstractMarkupDataContainer destinationContainer, int startIndex, int count)
		{
			MoveItemsTo(destinationContainer, destinationContainer.Count, startIndex, count);
		}


		public IAbstractMarkupData Find(Predicate<IAbstractMarkupData> match)
		{
			foreach (IAbstractMarkupData item in AllSubItems)
			{
				if (match(item))
				{
					return item;
				}
			}
			// not found
			return null;
		}


		public void MoveAllItemsTo(IAbstractMarkupDataContainer destinationContainer, int insertAtIndex)
		{
			// validate parameters
			if (destinationContainer == null)
			{
				throw new ArgumentNullException("container");
			}
			if (insertAtIndex < 0 || insertAtIndex > destinationContainer.Count)
			{
				throw new ArgumentOutOfRangeException("insertAtIndex");
			}

			IAbstractMarkupData[] items = _Content.ToArray();
			_Content.Clear();
			foreach (IAbstractMarkupData item in items)
			{
				item.Parent = null;
				// NOTE: must increment the destination index, otherwise we 
				// will inadvertedly be reversing the order!
				destinationContainer.Insert(insertAtIndex++, item);
			}
		}


		public void MoveItemsTo(IAbstractMarkupDataContainer destinationContainer, int destinationIndex,
			int startIndex, int count)
		{
			// validate parameters
			if (destinationContainer == null)
			{
				throw new ArgumentNullException("destinationContainer");
			}
			if (count == 0)
			{
				// allowed special case - nothing to do
				return;
			}
			if (startIndex < 0 || startIndex >= _Content.Count)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (count < 0 || count > _Content.Count - startIndex)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (destinationIndex < 0 || destinationIndex > destinationContainer.Count)
			{
				throw new ArgumentOutOfRangeException("destinationIndex");
			}

			for (int i = 0; i < count; i++)
			{
				IAbstractMarkupData item = _Content[startIndex];
				RemoveAt(startIndex);
				// NOTE: must increment the destination index, otherwise we 
				// will inadvertedly be reversing the order!
				destinationContainer.Insert(destinationIndex++, item);
			}
		}


		public void ForEachSubItem(Action<IAbstractMarkupData> action)
		{
			foreach (IAbstractMarkupData item in AllSubItems)
			{
				action(item);
			}
		}


		#endregion
		#region IList<IAbstractMarkupData> Members


		public virtual int IndexOf(IAbstractMarkupData item)
		{
			// NOTE: calling _Content.IndexOf(item) may yield incorrect behaviour here,
			// as it may result in calls to overloaded Object.Equals() implementations,
			// which may equate two different objects that have the same properties.
			//return _Content.IndexOf(item);
			int i = 0;
			foreach (IAbstractMarkupData contentItem in _Content)
			{
				if (item == contentItem)
				{
					return i;
				}
				++i;
			}
			// not found
			return -1;
		}


		public virtual void Insert(int index, IAbstractMarkupData item)
		{
			if (item.Parent != null)
			{
				throw new FileTypeSupportException(StringResources.MarkupDataContainer_AlreadyInCollectionError);
			}
			_Content.Insert(index, item);
			item.Parent = _PublicContainer;
		}


		public virtual void RemoveAt(int index)
		{
			CheckIndexValue(index);
			IAbstractMarkupData item = _Content[index];
			_Content.RemoveAt(index);
			if (item != null)
			{
				item.Parent = null;
			}
		}


		public virtual IAbstractMarkupData this[int index]
		{
			get
			{
				CheckIndexValue(index);
				return _Content[index];
			}
			set
			{
				RemoveAt(index);
				Insert(index, value);
			}
		}

		private void CheckIndexValue(int index)
		{
			if (index < 0 || index >= _Content.Count)
			{
				throw new ArgumentOutOfRangeException("index", String.Format(StringResources.MarkupDataContainer_IndexRangeError,
					index, _Content.Count));
			}
		}


		#endregion
		#region ICollection<IAbstractMarkupData> Members


		public virtual void Add(IAbstractMarkupData item)
		{
			if (item.Parent != null)
			{
				throw new FileTypeSupportException(StringResources.MarkupDataContainer_AlreadyInCollectionError);
			}
			_Content.Add(item);
			item.Parent = _PublicContainer;
		}


		public virtual void Clear()
		{
			IAbstractMarkupData[] items = _Content.ToArray();
			_Content.Clear();
			foreach (IAbstractMarkupData item in items)
			{
				item.Parent = null;
			}
		}


		public virtual bool Contains(IAbstractMarkupData item)
		{
			// NOTE: the default container Contains implementation will call Object.Equals(),
			// which may not result in correct behavior, as two different instances that
			// share the same properties can still be considered equal, but not the same.
			//return _Content.Contains(item);
			foreach (IAbstractMarkupData contentItem in _Content)
			{
				if (contentItem == item)
				{
					return true;
				}
			}
			return false;
		}


		public virtual void CopyTo(IAbstractMarkupData[] array, int arrayIndex)
		{
			_Content.CopyTo(array, arrayIndex);
		}


		public virtual int Count
		{
			get
			{
				return _Content.Count;
			}
		}


		/// <summary>
		/// This property is intentionally not made public, as it would just be confusing.
		/// </summary>
		bool ICollection<IAbstractMarkupData>.IsReadOnly
		{
			get
			{
				return ((ICollection<IAbstractMarkupData>)_Content).IsReadOnly;
			}
		}


		public virtual bool Remove(IAbstractMarkupData item)
		{
			// NOTE: The default Remove() implementation may result in calls to Object.Equals(),
			// which may in fact yield the wrong result here, since two equal objects may not be the same.
			// For that reason we do this through indicies instead.	
			int index = IndexOf(item);
			if (index != -1)
			{
				RemoveAt(index);
				return true;
			}
			return false;
		}


		#endregion
		#region IEnumerable<IAbstractMarkupData> Members


		public virtual IEnumerator<IAbstractMarkupData> GetEnumerator()
		{
			foreach (IAbstractMarkupData item in _Content)
			{
				yield return item;
			}
		}


		#endregion
		#region IEnumerable Members


		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}


		#endregion
		#region ICloneable Members


		public virtual object Clone()
		{
			return new MarkupDataContainer(this);
		}


		#endregion

		#region ISupportsUniqueId Members
		[XmlIgnore]
		int ISupportsUniqueId.UniqueId
		{
			get
			{
				return _uniqueId;
			}
			set
			{
				_uniqueId = value;
			}
		}
		#endregion
	}
}

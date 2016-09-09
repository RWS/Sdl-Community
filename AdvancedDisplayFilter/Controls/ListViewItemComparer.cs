using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.Toolkit.Integration.DisplayFilter;
using Sdl.Community.Toolkit.FileType;

namespace Sdl.Community.AdvancedDisplayFilter.Controls
{
    public class ListViewItemComparer : IComparer
    {
        /// <summary>
        /// Compare the listview items; returns a predefined order
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            var viewItemX = (ListViewItem)x;
            var viewItemY = (ListViewItem)y;

            var groupX = GetGroupPosition(viewItemX.Group);
            var groupY = GetGroupPosition(viewItemY.Group);

            var itemX = GetItemPosition(viewItemX);
            var itemY = GetItemPosition(viewItemY);

            try
            {
                return groupX == groupY
                    ? (itemX > itemY ? 1 : 0)
                    : (groupX > groupY ? 1 : 0);
            }
            catch (Exception ex)
            {

                Trace.WriteLine(ex.Message);
            }

            return 0;

        }


        #region  |  Helpers  |

        /// <summary>
        /// Get the position of the listview group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private int GetGroupPosition(ListViewGroup group)
        {
            try
            {
                for (var i = 0; i < _listViewGroupsSorted.Count; i++)
                {
                    if (_listViewGroupsSorted[i] == group)
                        return i;
                }
            }
            catch (Exception ex)
            {

                Trace.WriteLine(ex.Message);
            }

            return 0;
        }

        /// <summary>
        /// Get the position of the enumeration
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static int GetItemPosition(ListViewItem item)
        {
            try
            {
                if (item.Tag == null)
                    return 0;

                if (item.Tag.GetType() == typeof(DisplayFilterSettings.ConfirmationLevel))
                    return (int)(DisplayFilterSettings.ConfirmationLevel)item.Tag;
                else if (item.Tag.GetType() == typeof(OriginType))
                    return (int)(OriginType)item.Tag;
                else if (item.Tag.GetType() == typeof(DisplayFilterSettings.RepetitionType))
                    return (int)(DisplayFilterSettings.RepetitionType)item.Tag;
                else if (item.Tag.GetType() == typeof(DisplayFilterSettings.SegmentReviewType))
                    return (int)(DisplayFilterSettings.SegmentReviewType)item.Tag;
                else if (item.Tag.GetType() == typeof(DisplayFilterSettings.SegmentLockingType))
                    return (int)(DisplayFilterSettings.SegmentLockingType)item.Tag;
                else if (item.Tag.GetType() == typeof(DisplayFilterSettings.SegmentContentType))
                    return (int)(DisplayFilterSettings.SegmentContentType)item.Tag;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            return 0;
        }


        /// <summary>
        /// predefined order for the listview groups
        /// </summary>
        private readonly List<ListViewGroup> _listViewGroupsSorted = new List<ListViewGroup>
        {
            DisplayFilterControl.GroupGeneralAvailable,
            DisplayFilterControl.GroupStatusAvailable,
            DisplayFilterControl.GroupOriginAvailable,
            DisplayFilterControl.GroupPreviousOriginAvailable,
            DisplayFilterControl.GroupRepetitionTypesAvailable,
            DisplayFilterControl.GroupReviewTypesAvailable,
            DisplayFilterControl.GroupLockingTypesAvailable,
            DisplayFilterControl.GroupContentTypesAvailable
        };
        #endregion
    }
}

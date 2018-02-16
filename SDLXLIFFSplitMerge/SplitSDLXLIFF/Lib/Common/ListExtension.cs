// <copyright file="ListExtension.cs" company="SDL International">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Oleksandr Tkachenko</author>
// <email>otkachenko@sdl.com</email>
// <date>2010-06-10</date>
// <summary>ListExtension</summary>

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for List
    /// </summary>
    internal static class ListExtension
    {
        /// <summary>
        /// Removes last elements from List
        /// </summary>
        /// <typeparam name="T">Type of elements in the list</typeparam>
        /// <param name="list">List to remove last element from</param>
        public static void RemoveLast<T>(this List<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

        /// <summary>
        /// Getting the last element in list
        /// </summary>
        /// <typeparam name="T">Type of elements in the list</typeparam>
        /// <param name="list">List to get last element from</param>
        /// <returns>Returns last element in list. If the list is empty returns default(T)</returns>
        public static T Last<T>(this List<T> list)
        {
            if (list.Count > 0)
            {
                return list[list.Count - 1];
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Checking if the list is equal to this list
        /// </summary>
        /// <typeparam name="T">Type of elements in the list</typeparam>
        /// <param name="list">List which is checking for equality</param>
        /// <param name="listToCompare">List with which is comparing this list</param>
        /// <returns>Returns true if the comparing list is equal to this list. Returns false if lists are different</returns>
        public static bool IsEqual<T>(this List<T> list, List<T> listToCompare)
        {
            if (list.Count != listToCompare.Count)
            {
                return false;
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].Equals(listToCompare[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Moves a range of items to the given position.
        /// </summary>
        /// <typeparam name="T">A type of items within the collection</typeparam>
        /// <param name="list">A collection of items</param>
        /// <param name="indexStart">A zero-based index indicating the start position of the range</param>
        /// <param name="indexEnd">A zero-based index indicating the end position of the range</param>
        /// <param name="targetPosition">A zero-based index indicating the target position where items should be moved</param>
        public static void MoveItems<T>(this List<T> list, int indexStart, int indexEnd, int targetPosition)
        {
            // if items are already there where they should be
            if (targetPosition == indexStart)
            {
                return;
            }

            // validate the arguments
            if (indexStart > indexEnd ||
                indexStart < 0 ||
                indexEnd >= list.Count ||
                targetPosition < 0 || targetPosition > list.Count ||
                (targetPosition >= indexStart && targetPosition <= indexEnd))
            {
                throw new ArgumentException("Invalid argument values.");
            }

            // move items
            List<T> itemsToMove = list.GetRange(indexStart, indexEnd - indexStart + 1);
            list.RemoveRange(indexStart, indexEnd - indexStart + 1);

            int position = targetPosition < indexStart
                ? targetPosition
                : targetPosition - itemsToMove.Count;

            list.InsertRange(position, itemsToMove);
        }
    }
}
